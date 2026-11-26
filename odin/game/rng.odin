package game

import "core:strconv"
import "core:strings"

// SplitMix64. Self-contained so native tests and the wasm build draw identical sequences.
Rng :: struct {
	state: u64,
}

rng_seed :: proc(r: ^Rng, seed: u64) {
	r.state = seed
}

rng_next :: proc(r: ^Rng) -> u64 {
	r.state += 0x9E3779B97F4A7C15
	z := r.state
	z = (z ~ (z >> 30)) * 0xBF58476D1CE4E5B9
	z = (z ~ (z >> 27)) * 0x94D049BB133111EB
	return z ~ (z >> 31)
}

// Uniform in [0, n). Returns 0 when n <= 0, but still consumes a draw like Random.Next(n) does.
rng_below :: proc(r: ^Rng, n: int) -> int {
	v := rng_next(r)
	if n <= 0 {
		return 0
	}
	return int(v % u64(n))
}

// Inclusive on both ends (RNG.FromRange in the VB original).
rng_range :: proc(r: ^Rng, minimum, maximum: int) -> int {
	return rng_below(r, maximum - minimum + 1) + minimum
}

// RNG.FromGenerator(MakeBooleanGenerator(false_weight, true_weight)): True is listed first.
rng_chance :: proc(r: ^Rng, false_weight, true_weight: int) -> bool {
	return rng_below(r, false_weight + true_weight) < true_weight
}

rng_xdy :: proc(r: ^Rng, die_count, die_size: int) -> int {
	total := 0
	for _ in 0 ..< die_count {
		total += rng_range(r, 1, die_size)
	}
	return total
}

// Dice text such as "2d2+-2d1" or "1d6/6": "+" separated sets, each "XdY" with an optional "*n" or "/n".
// Integer division truncates toward zero, as the original's "\" does. Malformed text rolls 0 for that set.
rng_dice :: proc(r: ^Rng, text: string) -> int {
	if len(strings.trim_space(text)) == 0 {
		return 0
	}
	tally := 0
	rest := text
	for part in strings.split_iterator(&rest, "+") {
		set := part
		multiplier, divisor := 1, 1
		if star := strings.index_byte(set, '*'); star >= 0 {
			multiplier, _ = strconv.parse_int(set[star + 1:])
			set = set[:star]
		} else if slash := strings.index_byte(set, '/'); slash >= 0 {
			divisor, _ = strconv.parse_int(set[slash + 1:])
			set = set[:slash]
		}
		d := strings.index_any(set, "dD")
		if d < 0 || divisor == 0 {
			continue
		}
		die_count, _ := strconv.parse_int(set[:d])
		die_size, _ := strconv.parse_int(set[d + 1:])
		rolled := -rng_xdy(r, -die_count, die_size) if die_count < 0 else rng_xdy(r, die_count, die_size)
		tally += (rolled * multiplier) / divisor
	}
	return tally
}
