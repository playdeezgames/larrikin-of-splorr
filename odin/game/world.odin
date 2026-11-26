package game

import "core:fmt"

// The rules of Larrikin of SPLORR!! (Dungeon Crawler Jam 2022), ported from the VB.NET original in
// LarrikinOfSPLORR/LOS.Game. The original kept everything in an in-memory SQLite database; here the
// world is plain arrays. Characters and items keep their creation order, as the database's row ids did,
// because the screens list them in that order (the first enemy in a room is the one drawn).

MAZE_COLUMNS :: 14
MAZE_ROWS :: 14
LOCATION_COUNT :: MAZE_COLUMNS * MAZE_ROWS

Direction :: enum u8 {
	North,
	East,
	South,
	West,
}

DIRECTION_NAMES := [Direction]string {
	.North = "N",
	.East  = "E",
	.South = "S",
	.West  = "W",
}

DELTA := [Direction][2]int {
	.North = {0, -1},
	.East  = {1, 0},
	.South = {0, 1},
	.West  = {-1, 0},
}

OPPOSITE := [Direction]Direction {
	.North = .South,
	.East  = .West,
	.South = .North,
	.West  = .East,
}

turn_right :: proc(d: Direction) -> Direction {
	return Direction((int(d) + 1) % 4)
}

turn_left :: proc(d: Direction) -> Direction {
	return Direction((int(d) + 3) % 4)
}

Location :: struct {
	door:    [Direction]bool, // true = open (TransitionState.Open), false = wall
	visited: bool,
}

Character_Type :: enum u8 {
	Larrikin,
	Chaos_Warrior,
	Orc,
	Goblin,
	Skeleton,
}

Character_Descriptor :: struct {
	name:          string,
	max_health:    int,
	attack_dice:   string,
	defend_dice:   string,
	spawn_dice:    string,
}

CHARACTERS := [Character_Type]Character_Descriptor {
	.Larrikin      = {"larrikin", 5, "1d3/3", "1d3/3+1d3/3", "0d1"},
	.Chaos_Warrior = {"chaos warrior", 3, "1d3/3+1d3/3+1d3/3+1d3/3", "1d6/6+1d6/6+1d6/6+1d6/6", "3d6"},
	.Orc           = {"orc", 1, "1d3/3+1d3/3+1d3/3", "1d6/6+1d6/6", "16d6"},
	.Goblin        = {"goblin", 1, "1d3/3+1d3/3", "1d6/6", "12d6"},
	.Skeleton      = {"skeleton", 1, "1d3/3+1d3/3", "1d6/6+1d6/6", "12d6"},
}

// AllEnemyTypes in the original, which is also the spawn order.
ENEMY_TYPES := [?]Character_Type{.Chaos_Warrior, .Orc, .Goblin, .Skeleton}

Item_Type :: enum u8 {
	Potion,
	Compass,
	Macguffin,
	Dagger,
	Short_Sword,
	Long_Sword,
	Shield,
}

Item_Descriptor :: struct {
	name:        string,
	spawn_dice:  string,
	heal_dice:   string, // only potions can be drunk
	attack_dice: string, // weapons
	defend_dice: string, // the shield
	max_damage:  int, // picks the best weapon
	durability:  int, // depletion at or above this breaks it; 0 = never used up
}

ITEMS := [Item_Type]Item_Descriptor {
	.Potion      = {"potion", "50D1", "2D3", "", "", 0, 0},
	.Compass     = {"compass", "1d1", "", "", "", 0, 0},
	.Macguffin   = {"macguffin", "1d1", "", "", "", 0, 0},
	.Dagger      = {"dagger", "3d4", "", "1D2/2", "", 1, 10},
	.Short_Sword = {"shortsword", "2d4", "", "1D2/2+1D2/2", "", 2, 20},
	.Long_Sword  = {"longsword", "1d4", "", "1D2/2+1D2/2+1D2/2", "", 3, 30},
	.Shield      = {"shield", "2d4", "", "", "1D3/3", 0, 10},
}

// AllItemTypes in the original, which is also the spawn order.
ITEM_SPAWN_ORDER := [?]Item_Type{.Potion, .Compass, .Macguffin, .Dagger, .Shield, .Short_Sword, .Long_Sword}

is_weapon :: proc(t: Item_Type) -> bool {
	return t == .Dagger || t == .Short_Sword || t == .Long_Sword
}

INVENTORY :: -1 // an item's holder when the player carries it
GONE :: -2 // drunk or broken

Character :: struct {
	type:     Character_Type,
	location: int,
	wounds:   int,
	alive:    bool,
}

Item :: struct {
	type:      Item_Type,
	holder:    int, // a location index, INVENTORY or GONE
	order:     int, // when it last changed hands; lists are sorted by it, as the original's REPLACE INTO did
	depletion: int,
}

World :: struct {
	rng:        Rng,
	locations:  [LOCATION_COUNT]Location,
	characters: [dynamic]Character,
	items:      [dynamic]Item,
	player:     int,
	facing:     Direction,
	next_order: int,
}

// What a rules call has to say: lines (Spectre-style markup) for the message screen, and sounds.
MAX_LINES :: 256
LINE_CAP :: 96

Sfx :: enum u8 {
	None,
	Title,
	Player_Hit,
	Player_Miss,
	Enemy_Hit,
	Enemy_Miss,
	Bump_Wall,
	Health_Up,
	Death,
	Run_Away,
	Kill_Enemy,
	Win,
}

Out :: struct {
	lines:     [MAX_LINES][LINE_CAP]u8,
	lens:      [MAX_LINES]int,
	count:     int,
	sfx:       [64]Sfx,
	sfx_count: int,
}

say :: proc(o: ^Out, format: string, args: ..any) {
	if o == nil {
		return
	}
	if o.count == MAX_LINES {
		// Keep the newest lines, like a terminal that scrolls.
		for i in 1 ..< MAX_LINES {
			o.lines[i - 1] = o.lines[i]
			o.lens[i - 1] = o.lens[i]
		}
		o.count -= 1
	}
	s := fmt.bprintf(o.lines[o.count][:], format, ..args)
	o.lens[o.count] = len(s)
	o.count += 1
}

out_line :: proc(o: ^Out, i: int) -> string {
	return string(o.lines[i][:o.lens[i]])
}

out_clear :: proc(o: ^Out) {
	o.count = 0
}

play :: proc(o: ^Out, s: Sfx) {
	if o == nil || o.sfx_count == len(o.sfx) {
		return
	}
	o.sfx[o.sfx_count] = s
	o.sfx_count += 1
}

location_index :: proc(column, row: int) -> int {
	return column + row * MAZE_COLUMNS
}

location_column :: proc(index: int) -> int {
	return index % MAZE_COLUMNS
}

location_row :: proc(index: int) -> int {
	return index / MAZE_COLUMNS
}

neighbor :: proc(index: int, d: Direction) -> (int, bool) {
	c := location_column(index) + DELTA[d].x
	r := location_row(index) + DELTA[d].y
	if c < 0 || r < 0 || c >= MAZE_COLUMNS || r >= MAZE_ROWS {
		return -1, false
	}
	return location_index(c, r), true
}

world_destroy :: proc(w: ^World) {
	delete(w.characters)
	delete(w.items)
	w.characters = nil
	w.items = nil
}

// Game.NewGame: the maze, items, enemies, then the larrikin at the top left corner facing north.
world_new :: proc(w: ^World, seed: u64) {
	world_destroy(w)
	w^ = {}
	rng_seed(&w.rng, seed)
	generate_maze(w)
	for t in ITEM_SPAWN_ORDER {
		for _ in 0 ..< rng_dice(&w.rng, ITEMS[t].spawn_dice) {
			location := rng_below(&w.rng, LOCATION_COUNT)
			append(&w.items, Item{type = t, holder = location, order = w.next_order})
			w.next_order += 1
		}
	}
	for t in ENEMY_TYPES {
		for _ in 0 ..< rng_dice(&w.rng, CHARACTERS[t].spawn_dice) {
			location := rng_below(&w.rng, LOCATION_COUNT)
			append(&w.characters, Character{type = t, location = location, alive = true})
		}
	}
	start := location_index(0, 0)
	w.player = len(w.characters)
	append(&w.characters, Character{type = .Larrikin, location = start, alive = true})
	w.facing = .North
	w.locations[start].visited = true
}

// Maze.Generate: randomised Prim's. Start from a random cell; take a random frontier cell and open a door
// to a random neighbour already inside, until the frontier is empty. Every room is reachable by one path.
generate_maze :: proc(w: ^World) {
	inside: [LOCATION_COUNT]bool
	on_frontier: [LOCATION_COUNT]bool
	frontier := make([dynamic]int, 0, LOCATION_COUNT, context.temp_allocator)
	add_neighbors :: proc(cell: int, inside, on_frontier: ^[LOCATION_COUNT]bool, frontier: ^[dynamic]int) {
		for d in Direction {
			if n, ok := neighbor(cell, d); ok && !inside[n] && !on_frontier[n] {
				on_frontier[n] = true
				append(frontier, n)
			}
		}
	}
	start := location_index(rng_range(&w.rng, 0, MAZE_COLUMNS - 1), rng_range(&w.rng, 0, MAZE_ROWS - 1))
	inside[start] = true
	add_neighbors(start, &inside, &on_frontier, &frontier)
	for len(frontier) > 0 {
		pick := rng_below(&w.rng, len(frontier))
		cell := frontier[pick]
		choices: [4]Direction
		count := 0
		for d in Direction {
			if n, ok := neighbor(cell, d); ok && inside[n] {
				choices[count] = d
				count += 1
			}
		}
		d := choices[rng_below(&w.rng, count)]
		n, _ := neighbor(cell, d)
		w.locations[cell].door[d] = true
		w.locations[n].door[OPPOSITE[d]] = true
		inside[cell] = true
		ordered_remove(&frontier, pick)
		on_frontier[cell] = false
		add_neighbors(cell, &inside, &on_frontier, &frontier)
	}
}

player_character :: proc(w: ^World) -> ^Character {
	return &w.characters[w.player]
}

player_location :: proc(w: ^World) -> int {
	return w.characters[w.player].location
}

max_health :: proc(c: ^Character) -> int {
	return CHARACTERS[c.type].max_health
}

health :: proc(c: ^Character) -> int {
	return max_health(c) - c.wounds
}

// Character.Health's setter clamps to 0..maximum.
set_health :: proc(c: ^Character, value: int) {
	v := clamp(value, 0, max_health(c))
	c.wounds = max_health(c) - v
}

is_dead :: proc(c: ^Character) -> bool {
	return health(c) == 0
}

// Living enemies in a room, oldest first.
enemies_at :: proc(w: ^World, location: int, allocator := context.temp_allocator) -> [dynamic]int {
	result := make([dynamic]int, allocator)
	for c, i in w.characters {
		if c.alive && c.type != .Larrikin && c.location == location {
			append(&result, i)
		}
	}
	return result
}

has_enemies :: proc(w: ^World, location: int) -> bool {
	for c in w.characters {
		if c.alive && c.type != .Larrikin && c.location == location {
			return true
		}
	}
	return false
}

// Items with this holder (a room or INVENTORY), in the order they arrived there.
items_held :: proc(w: ^World, holder: int, allocator := context.temp_allocator) -> [dynamic]int {
	result := make([dynamic]int, allocator)
	for item, i in w.items {
		if item.holder == holder {
			append(&result, i)
		}
	}
	for i in 1 ..< len(result) {
		for j := i; j > 0 && w.items[result[j]].order < w.items[result[j - 1]].order; j -= 1 {
			result[j], result[j - 1] = result[j - 1], result[j]
		}
	}
	return result
}

has_items :: proc(w: ^World, holder: int) -> bool {
	for item in w.items {
		if item.holder == holder {
			return true
		}
	}
	return false
}

carries :: proc(w: ^World, t: Item_Type) -> bool {
	for item in w.items {
		if item.holder == INVENTORY && item.type == t {
			return true
		}
	}
	return false
}

has_compass :: proc(w: ^World) -> bool {
	return carries(w, .Compass)
}

has_won :: proc(w: ^World) -> bool {
	return carries(w, .Macguffin)
}

can_use_item :: proc(w: ^World) -> bool {
	return carries(w, .Potion)
}

move_item :: proc(w: ^World, item, holder: int) {
	w.items[item].holder = holder
	w.items[item].order = w.next_order
	w.next_order += 1
}

take_item :: proc(w: ^World, item: int) {
	move_item(w, item, INVENTORY)
}

drop_item :: proc(w: ^World, item: int) {
	move_item(w, item, player_location(w))
}

// The weapon with the highest maximum damage; the first carried among equals (a stable OrderByDescending).
player_weapon :: proc(w: ^World) -> int {
	best := -1
	for i in items_held(w, INVENTORY) {
		t := w.items[i].type
		if is_weapon(t) && (best < 0 || ITEMS[t].max_damage > ITEMS[w.items[best].type].max_damage) {
			best = i
		}
	}
	return best
}

player_shield :: proc(w: ^World) -> int {
	for i in items_held(w, INVENTORY) {
		if w.items[i].type == .Shield {
			return i
		}
	}
	return -1
}

// Item.DecreaseDurability: true when the item is used up.
wear :: proc(w: ^World, item, amount: int) -> bool {
	w.items[item].depletion += amount
	return w.items[item].depletion >= ITEMS[w.items[item].type].durability
}

destroy_item :: proc(w: ^World, item: int) {
	w.items[item].holder = GONE
}

// PlayerCharacter.Move: through an open door into the next room, which goes on the map.
player_move :: proc(w: ^World) -> bool {
	here := player_location(w)
	if !w.locations[here].door[w.facing] {
		return false
	}
	there, _ := neighbor(here, w.facing)
	w.characters[w.player].location = there
	w.locations[there].visited = true
	return true
}

// ExplorationState's arrow keys. Nothing happens while enemies are in the room.
explore_turn_left :: proc(w: ^World) {
	if !has_enemies(w, player_location(w)) {
		w.facing = turn_left(w.facing)
	}
}

explore_turn_right :: proc(w: ^World) {
	if !has_enemies(w, player_location(w)) {
		w.facing = turn_right(w.facing)
	}
}

explore_ahead :: proc(w: ^World, o: ^Out) {
	if !has_enemies(w, player_location(w)) {
		if !player_move(w) {
			play(o, .Bump_Wall)
		}
	}
}

Attack_Result :: enum u8 {
	Missed,
	Hit,
	Killed,
}

// FightMenu.HandleAttack: roll the attack (the weapon wears by the roll and may break), the enemy defends,
// the difference is damage. The caller moves on to the enemies' turn (or, after a kill, to an Ok first).
player_attack :: proc(w: ^World, enemy: int, o: ^Out) -> Attack_Result {
	weapon := player_weapon(w)
	attack := rng_dice(&w.rng, CHARACTERS[.Larrikin].attack_dice)
	if weapon >= 0 {
		attack += rng_dice(&w.rng, ITEMS[w.items[weapon].type].attack_dice)
	}
	say(o, "You roll an attack of %d!", attack)
	if weapon >= 0 && wear(w, weapon, attack) {
		say(o, "[red]You broke yer %s![/]", ITEMS[w.items[weapon].type].name)
		destroy_item(w, weapon)
	}
	foe := &w.characters[enemy]
	name := CHARACTERS[foe.type].name
	defend := rng_dice(&w.rng, CHARACTERS[foe.type].defend_dice)
	say(o, "%s rolls a defense of %d!", name, defend)
	if attack > defend {
		damage := attack - defend
		say(o, "You hit for %d damage!", damage)
		set_health(foe, health(foe) - damage)
		if is_dead(foe) {
			say(o, "You kill %s!", name)
			foe.alive = false // Character.Destroy; enemies carry nothing
			play(o, .Kill_Enemy)
			return .Killed
		}
		play(o, .Player_Hit)
		return .Hit
	}
	say(o, "You miss!")
	play(o, .Player_Miss)
	return .Missed
}

// DefendState: every enemy in the room attacks in turn; the shield wears by each attack roll.
enemies_attack :: proc(w: ^World, o: ^Out) {
	me := player_character(w)
	for enemy in enemies_at(w, player_location(w)) {
		say(o, "-----------------------------------")
		if is_dead(me) {
			continue
		}
		foe := &w.characters[enemy]
		name := CHARACTERS[foe.type].name
		attack := rng_dice(&w.rng, CHARACTERS[foe.type].attack_dice)
		say(o, "%s rolls an attack of %d!", name, attack)
		defend := rng_dice(&w.rng, CHARACTERS[.Larrikin].defend_dice)
		shield := player_shield(w)
		if shield >= 0 {
			defend += rng_dice(&w.rng, ITEMS[.Shield].defend_dice)
		}
		say(o, "You roll a defense of %d!", defend)
		if shield >= 0 && wear(w, shield, attack) {
			say(o, "[red]Yer %s broke![/]", ITEMS[.Shield].name)
			destroy_item(w, shield)
		}
		if attack > defend {
			damage := attack - defend
			say(o, "%s hits you for %d!", name, damage)
			set_health(me, health(me) - damage)
			if is_dead(me) {
				say(o, "Yer dead!")
			} else {
				play(o, .Enemy_Hit)
			}
		} else {
			say(o, "%s misses!", name)
			play(o, .Enemy_Miss)
		}
	}
}

// FightMenu.HandleRun: face a random way and try to go through it. True when it worked.
player_run :: proc(w: ^World, o: ^Out) -> bool {
	w.facing = Direction(rng_below(&w.rng, 4))
	if player_move(w) {
		say(o, "You manage to run away!")
		play(o, .Run_Away)
		return true
	}
	say(o, "You fail to run away!")
	play(o, .Bump_Wall)
	return false
}

// Character.Drink: heal 2d3 (up to the maximum) and the potion is gone.
player_drink :: proc(w: ^World, item: int, o: ^Out) {
	me := player_character(w)
	set_health(me, health(me) + rng_dice(&w.rng, ITEMS[w.items[item].type].heal_dice))
	play(o, .Health_Up)
	destroy_item(w, item)
}

item_name :: proc(w: ^World, item: int) -> string {
	return ITEMS[w.items[item].type].name
}

character_name :: proc(w: ^World, c: int) -> string {
	return CHARACTERS[w.characters[c].type].name
}

// For tests and the save checker.
open_door_count :: proc(w: ^World) -> int {
	n := 0
	for l in w.locations {
		for open in l.door {
			if open {
				n += 1
			}
		}
	}
	return n / 2
}

