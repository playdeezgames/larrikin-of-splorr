#+build !js
package game

import "core:fmt"
import "core:os"
import "core:strings"
import "core:testing"

DUMP_DIR :: #config(DUMP_DIR, "")

@(test)
maze_is_a_spanning_tree :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 50 {
		w: World
		defer world_destroy(&w)
		world_new(&w, seed)
		testing.expect_value(t, open_door_count(&w), LOCATION_COUNT - 1)
		seen: [LOCATION_COUNT]bool
		queue := make([dynamic]int, context.temp_allocator)
		append(&queue, 0)
		seen[0] = true
		for len(queue) > 0 {
			here := pop_front(&queue)
			for d in Direction {
				if !w.locations[here].door[d] {
					continue
				}
				there, inside := neighbor(here, d)
				testing.expect(t, inside, "a door leads out of the maze")
				testing.expect(t, w.locations[there].door[OPPOSITE[d]], "a door is one-sided")
				if inside && !seen[there] {
					seen[there] = true
					append(&queue, there)
				}
			}
		}
		for s in seen {
			testing.expect(t, s, "a room cannot be reached")
		}
	}
}

@(test)
new_world_matches_the_original_spawns :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 100 {
		w: World
		defer world_destroy(&w)
		world_new(&w, seed)
		counts: [Item_Type]int
		for item in w.items {
			counts[item.type] += 1
		}
		testing.expect_value(t, counts[.Potion], 50)
		testing.expect_value(t, counts[.Compass], 1)
		testing.expect_value(t, counts[.Macguffin], 1)
		testing.expect(t, counts[.Dagger] >= 3 && counts[.Dagger] <= 12)
		testing.expect(t, counts[.Shield] >= 2 && counts[.Shield] <= 8)
		testing.expect(t, counts[.Short_Sword] >= 2 && counts[.Short_Sword] <= 8)
		testing.expect(t, counts[.Long_Sword] >= 1 && counts[.Long_Sword] <= 4)
		enemies: [Character_Type]int
		for c in w.characters {
			enemies[c.type] += 1
		}
		testing.expect_value(t, enemies[.Larrikin], 1)
		testing.expect(t, enemies[.Chaos_Warrior] >= 3 && enemies[.Chaos_Warrior] <= 18)
		testing.expect(t, enemies[.Orc] >= 16 && enemies[.Orc] <= 96)
		testing.expect(t, enemies[.Goblin] >= 12 && enemies[.Goblin] <= 72)
		testing.expect(t, enemies[.Skeleton] >= 12 && enemies[.Skeleton] <= 72)
		testing.expect_value(t, w.player, len(w.characters) - 1)
		testing.expect_value(t, player_location(&w), 0)
		testing.expect_value(t, w.facing, Direction.North)
		testing.expect(t, w.locations[0].visited)
		testing.expect_value(t, health(player_character(&w)), 5)
	}
}

@(test)
one_in_three_dice :: proc(t: ^testing.T) {
	r: Rng
	rng_seed(&r, 7)
	ones := 0
	for _ in 0 ..< 30000 {
		v := rng_dice(&r, "1d3/3")
		testing.expect(t, v == 0 || v == 1)
		ones += v
	}
	testing.expect(t, ones > 9000 && ones < 11000, "1d3/3 should be 1 a third of the time")
	for _ in 0 ..< 1000 {
		v := rng_dice(&r, "1d6/6+1d6/6+1d6/6+1d6/6")
		testing.expect(t, v >= 0 && v <= 4)
		h := rng_dice(&r, "2D3")
		testing.expect(t, h >= 2 && h <= 6)
	}
}

@(test)
potions_heal_up_to_the_maximum :: proc(t: ^testing.T) {
	w: World
	defer world_destroy(&w)
	world_new(&w, 3)
	me := player_character(&w)
	set_health(me, 1)
	potion := -1
	for item, i in w.items {
		if item.type == .Potion {
			potion = i
			break
		}
	}
	take_item(&w, potion)
	o: Out
	player_drink(&w, potion, &o)
	testing.expect(t, health(me) >= 3 && health(me) <= 5)
	testing.expect_value(t, w.items[potion].holder, GONE)
	testing.expect_value(t, o.sfx[0], Sfx.Health_Up)
	set_health(me, 99)
	testing.expect_value(t, health(me), 5)
	set_health(me, -3)
	testing.expect(t, is_dead(me))
}

@(test)
best_weapon_wears_out :: proc(t: ^testing.T) {
	w: World
	defer world_destroy(&w)
	world_new(&w, 4)
	dagger, long := -1, -1
	for item, i in w.items {
		if item.type == .Dagger && dagger < 0 {
			dagger = i
		}
		if item.type == .Long_Sword && long < 0 {
			long = i
		}
	}
	take_item(&w, dagger)
	testing.expect_value(t, player_weapon(&w), dagger)
	take_item(&w, long)
	testing.expect_value(t, player_weapon(&w), long)
	testing.expect(t, !wear(&w, long, 29))
	testing.expect(t, wear(&w, long, 1))
}

@(test)
save_round_trip :: proc(t: ^testing.T) {
	w: World
	defer world_destroy(&w)
	world_new(&w, 11)
	take_item(&w, 5)
	w.characters[3].alive = false
	player_character(&w).wounds = 2
	text := strings.clone(world_save(&w), context.temp_allocator)
	loaded: World
	defer world_destroy(&loaded)
	testing.expect(t, world_load(&loaded, text))
	testing.expect_value(t, world_save(&loaded), text)
	testing.expect(t, !world_load(&loaded, "LOS1 1 2 3"))
	testing.expect(t, !world_load(&loaded, "nonsense"))
	testing.expect(t, !world_load(&loaded, strings.concatenate({text, " 9"}, context.temp_allocator)))
	testing.expect_value(t, world_save(&loaded), text)
}

// ---- driving the real screens ----

Bot_Result :: enum {
	Won,
	Died,
	Stuck,
}

press :: proc(g: ^Game, c: Command) {
	game_command(g, c, true)
	game_command(g, c, false)
}

// The first step towards the nearest room not yet visited.
step_towards_unvisited :: proc(w: ^World) -> (Direction, bool) {
	from: [LOCATION_COUNT]int
	first: [LOCATION_COUNT]Direction
	for &f in from {
		f = -1
	}
	start := player_location(w)
	from[start] = start
	queue := make([dynamic]int, context.temp_allocator)
	append(&queue, start)
	for len(queue) > 0 {
		here := pop_front(&queue)
		if !w.locations[here].visited {
			return first[here], true
		}
		for d in Direction {
			if !w.locations[here].door[d] {
				continue
			}
			there, _ := neighbor(here, d)
			if from[there] < 0 {
				from[there] = here
				first[there] = d if here == start else first[here]
				append(&queue, there)
			}
		}
	}
	return .North, false
}

play_bot :: proc(seed: u64) -> (Bot_Result, int) {
	g := new(Game)
	defer free(g)
	defer game_destroy(g)
	game_init(g, seed, {})
	press(g, .Ok) // welcome
	press(g, .Ok) // Embark!
	turns := 0
	for steps := 0; steps < 200_000; steps += 1 {
		w := &g.world
		#partial switch g.state {
		case .Win:
			return .Won, turns
		case .Dead:
			return .Died, turns
		case .Messages:
			press(g, .Ok)
		case .Ground:
			press(g, .Down)
			press(g, .Ok)
		case .Fight:
			turns += 1
			enemies := len(enemies_at(w, player_location(w)))
			if health(player_character(w)) <= 2 && can_use_item(w) {
				for _ in 0 ..< enemies {
					press(g, .Down)
				}
				press(g, .Ok) // Use Item...
				press(g, .Down)
				press(g, .Ok) // the first potion
			} else {
				press(g, .Ok) // the first enemy
			}
		case .Explore:
			here := player_location(w)
			if has_enemies(w, here) {
				press(g, .Fight)
			} else if has_items(w, here) {
				press(g, .Ground)
			} else if health(player_character(w)) < 5 && can_use_item(w) {
				press(g, .Inventory)
				// find a potion in the list (choice 0 is Never Mind)
				for i, at in items_held(w, INVENTORY) {
					if w.items[i].type == .Potion {
						for _ in 0 ..= at {
							press(g, .Down)
						}
						break
					}
				}
				press(g, .Ok)
				if g.state == .Item_Menu {
					press(g, .Down)
					press(g, .Down)
					press(g, .Ok) // Drink
				}
				if g.state == .Inventory {
					press(g, .Back)
				}
			} else {
				d, found := step_towards_unvisited(w)
				if !found {
					return .Stuck, turns
				}
				for w.facing != d {
					press(g, .Right)
				}
				press(g, .Up)
			}
		case:
			return .Stuck, turns
		}
		free_all(context.temp_allocator)
	}
	return .Stuck, turns
}

@(test)
the_bot_can_finish_and_never_gets_stuck :: proc(t: ^testing.T) {
	results: [Bot_Result]int
	for seed in u64(1) ..= 300 {
		r, _ := play_bot(seed)
		results[r] += 1
	}
	fmt.printfln("bot: %d won, %d died, %d stuck", results[.Won], results[.Died], results[.Stuck])
	testing.expect_value(t, results[.Stuck], 0)
	testing.expect(t, results[.Won] > 0, "nobody ever wins")
	testing.expect(t, results[.Died] > 0, "nobody ever dies")
}

@(test)
every_screen_draws :: proc(t: ^testing.T) {
	g := new(Game)
	defer free(g)
	defer game_destroy(g)
	game_init(g, 99, {})
	for s in State {
		if s == .Explore || s == .Map || s == .Fight || s == .Ground || s == .Inventory || s == .Item_Menu || s == .Use_Item || s == .Game_Menu || s == .Confirm_Abandon {
			continue
		}
		g.state = s
		game_frame(g, 0.016)
	}
	press(g, .Ok)
	press(g, .Ok)
	testing.expect_value(t, g.state, State.Explore)
	for &c in g.world.characters {
		if c.type != .Larrikin && c.location == player_location(&g.world) {
			c.alive = false
		}
	}
	game_frame(g, 0.016)
	press(g, .Map)
	testing.expect_value(t, g.state, State.Map)
	game_frame(g, 0.016)
	press(g, .Ok)
	press(g, .Back)
	testing.expect_value(t, g.state, State.Game_Menu)
	press(g, .Down)
	press(g, .Down)
	press(g, .Ok)
	testing.expect_value(t, g.state, State.Confirm_Abandon)
	game_frame(g, 0.016)
	press(g, .Yes)
	testing.expect_value(t, g.state, State.Main_Menu)
}

// ---- screenshots for the itch page (tools/make_shots.sh) ----

dump_ppm :: proc(g: ^Game, name: string) {
	game_frame(g, 0.016)
	b := strings.builder_make(context.temp_allocator)
	fmt.sbprintf(&b, "P6\n%d %d\n255\n", VIEW_WIDTH, VIEW_HEIGHT)
	for p in g.screen.pixels {
		strings.write_byte(&b, u8(p))
		strings.write_byte(&b, u8(p >> 8))
		strings.write_byte(&b, u8(p >> 16))
	}
	path := fmt.tprintf("%s/%s.ppm", DUMP_DIR, name)
	_ = os.write_entire_file(path, b.buf[:])
}

@(test)
dump_screens :: proc(t: ^testing.T) {
	when DUMP_DIR == "" {
		return
	}
	g := new(Game)
	defer free(g)
	defer game_destroy(g)
	game_init(g, 2022, {})
	dump_ppm(g, "01-title")
	press(g, .Ok)
	press(g, .Ok)
	w := &g.world
	// walk until a room with an enemy, then one with a chest, for the pictures
	shot_enemy, shot_chest, shot_fight, shot_map := false, false, false, false
	for steps := 0; steps < 5000 && !(shot_enemy && shot_chest && shot_fight && shot_map); steps += 1 {
		#partial switch g.state {
		case .Explore:
			here := player_location(w)
			if has_enemies(w, here) {
				if !shot_enemy {
					dump_ppm(g, "02-enemy")
					shot_enemy = true
				}
				press(g, .Fight)
			} else if has_items(w, here) {
				if !shot_chest {
					dump_ppm(g, "03-chest")
					shot_chest = true
				}
				press(g, .Ground)
			} else {
				if !shot_map && steps > 1500 {
					press(g, .Map)
					dump_ppm(g, "05-map")
					shot_map = true
					press(g, .Ok)
				}
				d, found := step_towards_unvisited(w)
				if !found {
					break
				}
				for w.facing != d {
					press(g, .Right)
				}
				press(g, .Up)
			}
		case .Fight:
			if !shot_fight && len(enemies_at(w, player_location(w))) > 0 {
				dump_ppm(g, "04-fight")
				press(g, .Ok)
				dump_ppm(g, "06-blows")
				shot_fight = true
			} else {
				press(g, .Ok)
			}
		case .Ground:
			press(g, .Down)
			press(g, .Ok)
		case .Messages:
			press(g, .Ok)
		case .Dead, .Win:
			press(g, .Ok)
			press(g, .Ok)
		case:
			press(g, .Ok)
		}
		// keep the larrikin alive for the pictures
		if g.has_world {
			player_character(w).wounds = 0
		}
		free_all(context.temp_allocator)
	}
	g.state = .Dead
	dump_ppm(g, "07-dead")
	_ = t
}
