package game

import "core:fmt"

// The screens of the original (LarrikinOfSPLORR/LarrikinOfSPLORR: Program, MainMenu, InPlayMenu and the
// InPlay states). The original was a loop of blocking Spectre.Console prompts; here each prompt is a
// state, commands arrive from the page one at a time, and the current state draws every frame. Nothing
// here depends on the browser, so all of it runs natively under test.

Command :: enum u8 {
	Up,
	Right,
	Down,
	Left,
	Ok, // Enter or Space: choose in a prompt
	Back, // Escape
	Ground, // G
	Inventory, // I
	Fight, // F
	Run, // R
	Map, // M
	Yes, // Y in the confirm prompt
	No, // N in the confirm prompt
}

State :: enum u8 {
	Welcome,
	Main_Menu,
	About,
	Load_Failed,
	Explore,
	Game_Menu,
	Confirm_Abandon,
	Ground,
	Inventory,
	Item_Menu,
	Fight,
	Use_Item,
	Messages,
	Map,
	Dead,
	Win,
}

// What the Ok on the message screen leads to.
After :: enum u8 {
	Explore,
	Defend, // the enemies' turn, added to the same screen, then another Ok
	Fight_Or_Dead,
}

Storage :: struct {
	get: proc(key: string) -> (string, bool), // the returned string may live in the temp allocator
	set: proc(key: string, value: string),
}

SAVE_KEY :: "los:save"

Game :: struct {
	world:              World,
	has_world:          bool,
	state:              State,
	storage:            Storage,
	rng:                Rng, // seeds new worlds; the world has its own
	screen:             Screen,
	out:                Out, // the message screen's lines, and the sounds for the page
	cursor:             int, // the highlighted choice of the current prompt
	item:               int, // the item the item menu is about
	after:              After,
	saved:              bool, // "Save Game..." was just chosen
	fullscreen_request: bool,
}

game_init :: proc(g: ^Game, seed: u64, storage: Storage) {
	g^ = {}
	g.storage = storage
	rng_seed(&g.rng, seed)
	enter(g, .Welcome)
}

game_destroy :: proc(g: ^Game) {
	world_destroy(&g.world)
}

game_frame :: proc(g: ^Game, dt: f64) {
	_ = dt
	game_render(g)
	free_all(context.temp_allocator)
}

enter :: proc(g: ^Game, s: State) {
	g.state = s
	g.cursor = 0
	#partial switch s {
	case .Welcome:
		play(&g.out, .Title)
	case .Dead:
		play(&g.out, .Death)
	case .Win:
		play(&g.out, .Win)
	case .Game_Menu:
		g.saved = false
	}
}

// ---- prompts (Spectre SelectionPrompt: a title, choices, the highlighted one in blue behind "> ") ----

Choice_Kind :: enum u8 {
	Ok,
	Embark,
	Load,
	About,
	Full_Screen,
	Continue,
	Save,
	Abandon,
	No,
	Yes,
	Never_Mind,
	Item,
	Drop,
	Drink,
	Enemy,
	Use_Item,
	Run,
}

Choice :: struct {
	label: string,
	kind:  Choice_Kind,
	arg:   int,
}

PAGE_SIZE :: 10 // Spectre's default

prompt_title :: proc(g: ^Game) -> string {
	w := &g.world
	#partial switch g.state {
	case .Main_Menu:
		return "[olive]Main Menu:[/]"
	case .Game_Menu:
		return "[olive]Game Menu:[/]"
	case .Ground:
		return "[olive]On The Ground:[/]"
	case .Inventory:
		return "[olive]In Yer Inventory:[/]"
	case .Item_Menu:
		return fmt.tprintf("[olive]What would you like to do with %s:[/]", item_name(w, g.item))
	case .Fight:
		return "[olive]Attack Target:[/]"
	case .Use_Item:
		return "[olive]Use What?[/]"
	}
	return ""
}

prompt_choices :: proc(g: ^Game) -> [dynamic]Choice {
	w := &g.world
	c := make([dynamic]Choice, context.temp_allocator)
	#partial switch g.state {
	case .Welcome, .About, .Load_Failed, .Messages, .Dead, .Win:
		append(&c, Choice{"Ok", .Ok, 0})
	case .Main_Menu:
		append(&c, Choice{"Embark!", .Embark, 0})
		append(&c, Choice{"Load...", .Load, 0})
		append(&c, Choice{"About...", .About, 0})
		append(&c, Choice{"Full Screen", .Full_Screen, 0})
	case .Game_Menu:
		append(&c, Choice{"Continue", .Continue, 0})
		append(&c, Choice{"Save Game...", .Save, 0})
		append(&c, Choice{"Abandon Game", .Abandon, 0})
	case .Confirm_Abandon:
		append(&c, Choice{"No", .No, 0})
		append(&c, Choice{"Yes", .Yes, 0})
	case .Ground:
		append(&c, Choice{"Never Mind", .Never_Mind, 0})
		for i in items_held(w, player_location(w)) {
			append(&c, Choice{item_name(w, i), .Item, i})
		}
	case .Inventory:
		append(&c, Choice{"Never Mind", .Never_Mind, 0})
		for i in items_held(w, INVENTORY) {
			append(&c, Choice{item_name(w, i), .Item, i})
		}
	case .Item_Menu:
		append(&c, Choice{"Never Mind", .Never_Mind, 0})
		append(&c, Choice{"Drop", .Drop, 0})
		if w.items[g.item].type == .Potion {
			append(&c, Choice{"Drink", .Drink, 0})
		}
	case .Fight:
		for e in enemies_at(w, player_location(w)) {
			foe := &w.characters[e]
			label := fmt.tprintf("%s(H:[red]%d/%d[/])", character_name(w, e), health(foe), max_health(foe))
			append(&c, Choice{label, .Enemy, e})
		}
		if can_use_item(w) {
			append(&c, Choice{"Use Item...", .Use_Item, 0})
		}
		append(&c, Choice{"RUN!", .Run, 0})
	case .Use_Item:
		append(&c, Choice{"Never Mind", .Never_Mind, 0})
		for i in items_held(w, INVENTORY) {
			if w.items[i].type == .Potion {
				append(&c, Choice{item_name(w, i), .Item, i})
			}
		}
	}
	return c
}

// ---- input ----

game_command :: proc(g: ^Game, command: Command, down: bool) {
	if !down {
		return
	}
	switch g.state {
	case .Explore:
		explore_command(g, command)
	case .Map:
		enter(g, .Explore) // Console.ReadKey: any key
	case .Welcome, .Main_Menu, .About, .Load_Failed, .Game_Menu, .Confirm_Abandon, .Ground, .Inventory, .Item_Menu, .Fight, .Use_Item, .Messages, .Dead, .Win:
		prompt_command(g, command)
	}
	in_play := g.state != .Welcome && g.state != .Main_Menu && g.state != .About && g.state != .Load_Failed && g.state != .Win && g.state != .Dead
	// InPlayMenu checks for the macguffin before every turn
	if in_play && g.has_world && has_won(&g.world) {
		g.has_world = false
		enter(g, .Win)
	}
}

explore_command :: proc(g: ^Game, command: Command) {
	w := &g.world
	here := player_location(w)
	enemies := has_enemies(w, here)
	#partial switch command {
	case .Left:
		explore_turn_left(w)
	case .Right:
		explore_turn_right(w)
	case .Up:
		explore_ahead(w, &g.out)
	case .Ground:
		if has_items(w, here) && !enemies {
			enter(g, .Ground)
		}
	case .Inventory:
		if has_items(w, INVENTORY) && !enemies {
			enter(g, .Inventory)
		}
	case .Fight:
		if enemies {
			enter(g, .Fight)
		}
	case .Run:
		if enemies {
			run_away(g)
		}
	case .Map:
		if !enemies {
			enter(g, .Map)
		}
	case .Back:
		enter(g, .Game_Menu)
	}
}

prompt_command :: proc(g: ^Game, command: Command) {
	choices := prompt_choices(g)
	n := len(choices)
	#partial switch command {
	case .Up:
		g.cursor = max(g.cursor - 1, 0)
	case .Down:
		g.cursor = min(g.cursor + 1, n - 1)
	case .Ok:
		if n > 0 {
			choose(g, choices[clamp(g.cursor, 0, n - 1)])
		}
	case .Yes:
		if g.state == .Confirm_Abandon {
			choose(g, Choice{"", .Yes, 0})
		}
	case .No:
		if g.state == .Confirm_Abandon {
			choose(g, Choice{"", .No, 0})
		}
	case .Back:
		// The original's prompts ignored Escape; here it picks the way out where there is one.
		for c in choices {
			if c.kind == .Never_Mind || c.kind == .Continue || c.kind == .No {
				choose(g, c)
				break
			}
		}
	}
}

choose :: proc(g: ^Game, c: Choice) {
	w := &g.world
	g.cursor = 0 // a new prompt every time, as in the original
	switch c.kind {
	case .Ok:
		ok_pressed(g)
	case .Embark:
		seed := rng_next(&g.rng)
		world_new(w, seed)
		g.has_world = true
		enter(g, .Explore)
	case .Load:
		load_game(g)
	case .About:
		enter(g, .About)
	case .Full_Screen:
		g.fullscreen_request = true
	case .Continue:
		enter(g, .Explore)
	case .Save:
		if g.storage.set != nil {
			g.storage.set(SAVE_KEY, world_save(w))
		}
		g.saved = true
	case .Abandon:
		enter(g, .Confirm_Abandon)
	case .No:
		enter(g, .Game_Menu)
	case .Yes:
		g.has_world = false
		enter(g, .Main_Menu)
	case .Never_Mind:
		#partial switch g.state {
		case .Item_Menu:
			enter(g, .Inventory)
		case .Use_Item:
			enter(g, .Fight)
		case:
			enter(g, .Explore)
		}
	case .Item:
		#partial switch g.state {
		case .Ground:
			take_item(w, c.arg)
			if !has_items(w, player_location(w)) {
				enter(g, .Explore)
			}
		case .Inventory:
			g.item = c.arg
			enter(g, .Item_Menu)
		case .Use_Item:
			out_clear(&g.out)
			say(&g.out, "You use the item!")
			player_drink(w, c.arg, &g.out)
			defend(g)
		}
	case .Drop:
		drop_item(w, g.item)
		after_item_menu(g)
	case .Drink:
		player_drink(w, g.item, &g.out)
		after_item_menu(g)
	case .Enemy:
		out_clear(&g.out)
		result := player_attack(w, c.arg, &g.out)
		if result == .Killed {
			show_messages(g, .Defend if has_enemies(w, player_location(w)) else .Explore)
		} else {
			defend(g)
		}
	case .Use_Item:
		enter(g, .Use_Item)
	case .Run:
		run_away(g)
	}
}

// ItemMenu returns to the inventory, or to exploring once it is empty.
after_item_menu :: proc(g: ^Game) {
	enter(g, .Inventory if has_items(&g.world, INVENTORY) else .Explore)
}

run_away :: proc(g: ^Game) {
	out_clear(&g.out)
	if player_run(&g.world, &g.out) {
		show_messages(g, .Explore)
	} else {
		defend(g)
	}
}

// DefendState: the enemies' turn goes on the same screen, then Ok.
defend :: proc(g: ^Game) {
	enemies_attack(&g.world, &g.out)
	show_messages(g, .Fight_Or_Dead)
}

show_messages :: proc(g: ^Game, after: After) {
	g.after = after
	enter(g, .Messages)
}

ok_pressed :: proc(g: ^Game) {
	#partial switch g.state {
	case .Welcome, .About, .Load_Failed, .Dead, .Win:
		enter(g, .Main_Menu)
	case .Messages:
		switch g.after {
		case .Explore:
			enter(g, .Explore)
		case .Defend:
			defend(g)
		case .Fight_Or_Dead:
			if is_dead(player_character(&g.world)) {
				g.has_world = false
				enter(g, .Dead)
			} else {
				enter(g, .Fight)
			}
		}
	}
}

// Load...: the one browser save slot. A loaded game opens on the game menu, where it was saved from.
load_game :: proc(g: ^Game) {
	if g.storage.get != nil {
		if text, found := g.storage.get(SAVE_KEY); found && world_load(&g.world, text) {
			g.has_world = true
			enter(g, .Game_Menu)
			return
		}
	}
	enter(g, .Load_Failed)
}

// ---- drawing ----

game_render :: proc(g: ^Game) {
	s := &g.screen
	clear_screen(s)
	w := &g.world
	row := 0
	switch g.state {
	case .Welcome:
		draw_big_centered(s, 8, "Larrikin")
		draw_big_centered(s, 40, "of")
		draw_big_centered(s, 72, "SPLORR!!")
		print_markup(s, 0, 14, "[gray]A Production of TheGrumpyGameDev[/]")
		print_markup(s, 0, 15, "[gray]For Dungeon Crawler Jam 2022[/]")
		print_markup(s, 0, 16, "[gray]...With \"help\" from his \"friends\"[/]")
		draw_prompt(s, g, 17)
	case .Main_Menu:
		draw_prompt(s, g, 0)
	case .About:
		print_markup(s, 0, 0, "About Larrikin of SPLORR!!:")
		print_markup(s, 0, 1, "Made by TheGrumpyGameDev for Dungeon Crawler Jam 2022")
		print_markup(s, 0, 2, "With graphics adapted from https://vurmux.itch.io/urizen-onebit-tilesets")
		print_markup(s, 0, 3, "Grim reaper adapted from https://game-icons.net/1x1/lorc/grim-reaper.html")
		print_markup(s, 0, 4, "[gray]Rebuilt in Odin and WebAssembly in 2026 by an AI agent (Claude Code).[/]")
		print_markup(s, 0, 5, "[gray]Font: DINOBYTE by mby, https://mby.itch.io/dinobyte, CC BY 4.0[/]")
		draw_prompt(s, g, 6)
	case .Load_Failed:
		print_markup(s, 0, 0, "[red]Loading didn't work. I blame Azure.[/]")
		draw_prompt(s, g, 1)
	case .Explore:
		draw_explore(s, g)
	case .Game_Menu:
		if g.saved {
			print_markup(s, 0, 0, "[gray]Saved in this browser.[/]")
			row = 1
		}
		draw_prompt(s, g, row)
	case .Confirm_Abandon:
		print_markup(s, 0, 0, "[red]Are you sure you want to abandon the game?[/] [blue][[y/n][/] [green](n)[/]")
		draw_prompt(s, g, 1)
	case .Ground, .Inventory, .Item_Menu, .Use_Item:
		draw_prompt(s, g, 0)
	case .Fight:
		me := player_character(w)
		print_markup(s, 0, 0, fmt.tprintf("Yer Health:[red]%d/%d[/]", health(me), max_health(me)))
		draw_prompt(s, g, 1)
	case .Messages:
		// a terminal that scrolls: the newest lines, then the Ok prompt
		room := TEXT_ROWS - 2
		first := max(g.out.count - room, 0)
		for i in first ..< g.out.count {
			print_markup(s, 0, i - first, out_line(&g.out, i))
		}
		draw_prompt(s, g, g.out.count - first)
	case .Map:
		draw_map(s, w)
	case .Dead:
		for line, i in REAPER {
			print_markup(s, 0, i, line)
		}
		print_markup(s, 0, len(REAPER), "[red]Yer dead.[/]")
		draw_prompt(s, g, len(REAPER) + 1)
	case .Win:
		draw_big_centered(s, 40, "You Win!")
		draw_prompt(s, g, 10)
	}
}

// FigletText, centered, in lime. The font is DINOBYTE at four times its size rather than a figlet font.
draw_big_centered :: proc(s: ^Screen, y: int, text: string) {
	scale := 4
	width := len(text) * FONT_WIDTH * scale
	draw_text(s, (VIEW_WIDTH - width) / 2, y, text, color_named("lime"), scale)
}

draw_prompt :: proc(s: ^Screen, g: ^Game, row: int) {
	title := prompt_title(g)
	r := row
	if len(title) > 0 {
		print_markup(s, 0, r, title)
	}
	r += 1 // the title line (blank when there is none), as Spectre leaves it
	choices := prompt_choices(g)
	n := len(choices)
	if n == 0 {
		return
	}
	cursor := clamp(g.cursor, 0, n - 1)
	top := 0
	if n > PAGE_SIZE {
		top = clamp(cursor - PAGE_SIZE / 2, 0, n - PAGE_SIZE)
	}
	for i in top ..< min(top + PAGE_SIZE, n) {
		if i == cursor {
			print_markup(s, 0, r, "[blue]>[/]")
			print_markup(s, 2, r, choices[i].label, color_named("blue"))
		} else {
			print_markup(s, 2, r, choices[i].label)
		}
		r += 1
	}
	if n > PAGE_SIZE {
		print_markup(s, 0, r, "[grey](Move up and down to reveal more choices)[/]")
	}
}

LEFT_COLUMN :: 0
AHEAD_COLUMN :: 14
RIGHT_COLUMN :: 42

draw_explore :: proc(s: ^Screen, g: ^Game) {
	w := &g.world
	here := player_location(w)
	l := &w.locations[here]
	white, black := color_named("white"), color_named("black")
	left, ahead, right := turn_left(w.facing), w.facing, turn_right(w.facing)
	draw_image(s, 0, 0, LEFT_COLUMN, 0, LEFT_DOOR[:] if l.door[left] else LEFT_WALL[:], white, black)
	draw_image(s, 0, 0, AHEAD_COLUMN, 0, AHEAD_DOOR[:] if l.door[ahead] else AHEAD_WALL[:], white, black)
	draw_image(s, 0, 0, RIGHT_COLUMN, 0, RIGHT_DOOR[:] if l.door[right] else RIGHT_WALL[:], white, black)
	enemies := enemies_at(w, here)
	if len(enemies) > 0 {
		t := w.characters[enemies[0]].type
		art: []string
		color: u32
		#partial switch t {
		case .Goblin:
			art, color = GOBLIN_ART[:], color_named("green")
		case .Orc:
			art, color = ORC_ART[:], color_named("darkgreen")
		case .Skeleton:
			art, color = SKELETON_ART[:], color_named("lightslategrey")
		case .Chaos_Warrior:
			art, color = CHAOS_WARRIOR_ART[:], color_named("darkslategray1")
		}
		draw_image_doubled(s, 0, 0, 16, 2, art, color)
	} else if has_items(w, here) {
		draw_image(s, 0, 0, 22, 14, CHEST_BACKGROUND[:], color_named("sandybrown"), nil)
		draw_image(s, 0, 0, 22, 14, CHEST_FOREGROUND[:], color_named("rosybrown"), nil)
	}
	me := player_character(w)
	status := fmt.tprintf("[aqua]H:[/][red]%d/%d[/]", health(me), max_health(me))
	status = fmt.tprintf("%s [gray]|[/] [aqua]Facing:[/]%s", status, DIRECTION_NAMES[w.facing] if has_compass(w) else "?")
	if len(enemies) > 0 {
		status = fmt.tprintf("%s [gray]|[/] [aqua]Enemies:[/]%d", status, len(enemies))
	}
	print_markup(s, 0, 28, status)
	keys := "[aqua]<Esc>[/]-menu"
	if len(enemies) == 0 {
		keys = fmt.tprintf("%s [gray]|[/] [aqua]Arrows[/]-move", keys)
		if has_items(w, here) {
			keys = fmt.tprintf("%s [gray]|[/] [aqua]G[/]-ground", keys)
		}
		if has_items(w, INVENTORY) {
			keys = fmt.tprintf("%s [gray]|[/] [aqua]I[/]-inventory", keys)
		}
		keys = fmt.tprintf("%s [gray]|[/] [aqua]M[/]-map", keys)
	} else {
		keys = fmt.tprintf("%s [gray]|[/] [aqua]F[/]-fight [gray]|[/] [aqua]R[/]-run", keys)
	}
	print_markup(s, 0, 29, keys)
}

MAP_SIZE :: MAZE_COLUMNS * 2 + 1

// MapState: a 29 by 29 canvas. Each visited room is a black (or, where the larrikin stands, green) pixel in
// a white ring, with the ring opened where a door is. Rooms not yet visited are not drawn.
draw_map :: proc(s: ^Screen, w: ^World) {
	left := (VIEW_WIDTH - MAP_SIZE * BLOCK) / 2
	top := (VIEW_HEIGHT - MAP_SIZE * BLOCK) / 2
	white, black, green := color_named("white"), color_named("black"), color_named("green")
	here := player_location(w)
	for l, i in w.locations {
		if !l.visited {
			continue
		}
		column := location_column(i) * 2 + 1
		row := location_row(i) * 2 + 1
		for dy in -1 ..= 1 {
			for dx in -1 ..= 1 {
				set_block(s, left, top, column + dx, row + dy, white)
			}
		}
		set_block(s, left, top, column, row, green if i == here else black)
		for d in Direction {
			if l.door[d] {
				set_block(s, left, top, column + DELTA[d].x, row + DELTA[d].y, black)
			}
		}
	}
}
