package game

import "core:strconv"
import "core:strings"

// The original saved its SQLite database to a file name typed at a prompt ("Save Game..." and "Load...").
// The browser has one slot instead: the whole World as one line of integers in localStorage. world_load
// checks every value before it accepts a save.

SAVE_MAGIC :: "LOS1"

world_save :: proc(w: ^World) -> string {
	b := strings.builder_make(context.temp_allocator)
	strings.write_string(&b, SAVE_MAGIC)
	put :: proc(b: ^strings.Builder, v: int) {
		strings.write_byte(b, ' ')
		strings.write_int(b, v)
	}
	strings.write_byte(&b, ' ')
	strings.write_u64(&b, w.rng.state)
	put(&b, int(w.facing))
	put(&b, w.player)
	put(&b, w.next_order)
	for l in w.locations {
		bits := 0
		for open, d in l.door {
			if open {
				bits |= 1 << uint(d)
			}
		}
		put(&b, bits)
		put(&b, 1 if l.visited else 0)
	}
	put(&b, len(w.characters))
	for c in w.characters {
		put(&b, int(c.type))
		put(&b, c.location)
		put(&b, c.wounds)
		put(&b, 1 if c.alive else 0)
	}
	put(&b, len(w.items))
	for item in w.items {
		put(&b, int(item.type))
		put(&b, item.holder)
		put(&b, item.order)
		put(&b, item.depletion)
	}
	return strings.to_string(b)
}

Save_Reader :: struct {
	rest: string,
	ok:   bool,
}

read_token :: proc(r: ^Save_Reader) -> string {
	r.rest = strings.trim_left_space(r.rest)
	end := strings.index_byte(r.rest, ' ')
	if end < 0 {
		end = len(r.rest)
	}
	token := r.rest[:end]
	r.rest = r.rest[end:]
	if len(token) == 0 {
		r.ok = false
	}
	return token
}

read_int :: proc(r: ^Save_Reader, lo, hi: int) -> int {
	v, parsed := strconv.parse_int(read_token(r), 10)
	if !parsed || v < lo || v > hi {
		r.ok = false
		return lo
	}
	return v
}

MAX_SAVED_THINGS :: 10_000

// Replaces *w with the saved world and returns true, or leaves *w untouched and returns false.
world_load :: proc(w: ^World, text: string) -> bool {
	r := Save_Reader{rest = text, ok = true}
	if read_token(&r) != SAVE_MAGIC || !r.ok {
		return false
	}
	loaded: World
	state, state_ok := strconv.parse_u64(read_token(&r), 10)
	loaded.rng.state = state
	r.ok &&= state_ok
	loaded.facing = Direction(read_int(&r, 0, 3))
	loaded.player = read_int(&r, 0, MAX_SAVED_THINGS)
	loaded.next_order = read_int(&r, 0, max(int))
	for &l in loaded.locations {
		bits := read_int(&r, 0, 15)
		for d in Direction {
			l.door[d] = bits & (1 << uint(d)) != 0
		}
		l.visited = read_int(&r, 0, 1) == 1
	}
	character_count := read_int(&r, 1, MAX_SAVED_THINGS)
	for _ in 0 ..< character_count {
		if !r.ok {
			break
		}
		c: Character
		c.type = Character_Type(read_int(&r, 0, len(Character_Type) - 1))
		c.location = read_int(&r, 0, LOCATION_COUNT - 1)
		c.wounds = read_int(&r, 0, CHARACTERS[c.type].max_health)
		c.alive = read_int(&r, 0, 1) == 1
		append(&loaded.characters, c)
	}
	item_count := read_int(&r, 0, MAX_SAVED_THINGS)
	for _ in 0 ..< item_count {
		if !r.ok {
			break
		}
		item: Item
		item.type = Item_Type(read_int(&r, 0, len(Item_Type) - 1))
		item.holder = read_int(&r, GONE, LOCATION_COUNT - 1)
		item.order = read_int(&r, 0, max(int))
		item.depletion = read_int(&r, 0, 1_000_000)
		append(&loaded.items, item)
	}
	r.ok &&= len(strings.trim_space(r.rest)) == 0
	r.ok &&= loaded.player < len(loaded.characters) && loaded.characters[loaded.player].type == .Larrikin
	r.ok &&= loaded.player < len(loaded.characters) && loaded.characters[loaded.player].alive
	// doors come in pairs and never lead out of the maze
	for l, i in loaded.locations {
		for open, d in l.door {
			if !open {
				continue
			}
			n, inside := neighbor(i, d)
			r.ok &&= inside && loaded.locations[n].door[OPPOSITE[d]]
		}
	}
	if !r.ok {
		world_destroy(&loaded)
		return false
	}
	world_destroy(w)
	w^ = loaded
	return true
}
