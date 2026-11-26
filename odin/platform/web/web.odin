// The browser layer: storage, entropy, the frame step and the exports the page calls. All game logic is in package game.
#+build js
package main

import "base:runtime"
import "los:game"

foreign import host "env"
@(default_calling_convention = "contextless")
foreign host {
	// Returns the byte length of the stored value, or -1 when the key is missing.
	storage_len :: proc(key: [^]u8, key_len: i32) -> i32 ---
	storage_read :: proc(key: [^]u8, key_len: i32, buf: [^]u8, cap: i32) ---
	storage_write :: proc(key: [^]u8, key_len: i32, value: [^]u8, value_len: i32) ---
	// A random number in [0, 1) from the page.
	entropy :: proc() -> f64 ---
}

state: game.Game

web_get :: proc(key: string) -> (string, bool) {
	k := key
	n := storage_len(raw_data(k), i32(len(k)))
	if n < 0 {
		return "", false
	}
	buf := make([]u8, n, context.temp_allocator)
	storage_read(raw_data(k), i32(len(k)), raw_data(buf), n)
	return string(buf), true
}

web_set :: proc(key: string, value: string) {
	k, v := key, value
	storage_write(raw_data(k), i32(len(k)), raw_data(v), i32(len(v)))
}

main :: proc() {
	seed := u64(entropy() * 9007199254740992.0) ~ u64(entropy() * 9007199254740992.0) << 21
	game.game_init(&state, seed, game.Storage{get = web_get, set = web_set})
}

// odin.js calls this every animation frame with the elapsed seconds.
@(export)
step :: proc(dt: f64, c: runtime.Context) -> bool {
	context = c
	game.game_frame(&state, min(dt, 0.25))
	return true
}

@(export)
platform_frame_ptr :: proc "c" () -> rawptr {
	return &state.screen.pixels
}

@(export)
platform_frame_width :: proc "c" () -> i32 {
	return game.VIEW_WIDTH
}

@(export)
platform_frame_height :: proc "c" () -> i32 {
	return game.VIEW_HEIGHT
}

// command: the game.Command enum: 0 up, 1 right, 2 down, 3 left, 4 Ok, 5 Back, 6 G, 7 I, 8 F, 9 R, 10 M, 11 Y, 12 N.
@(export)
platform_command :: proc "c" (command: i32, down: i32) {
	context = runtime.default_context()
	if command < 0 || command >= i32(len(game.Command)) {
		return
	}
	game.game_command(&state, game.Command(command), down != 0)
}

@(export)
platform_sfx_count :: proc "c" () -> i32 {
	return i32(state.out.sfx_count)
}

@(export)
platform_sfx_at :: proc "c" (i: i32) -> i32 {
	if i < 0 || int(i) >= state.out.sfx_count {
		return 0
	}
	return i32(state.out.sfx[i])
}

@(export)
platform_sfx_clear :: proc "c" () {
	state.out.sfx_count = 0
}

// True once after "Full Screen" was chosen; the page must act on it inside the key or tap handler.
@(export)
platform_fullscreen_requested :: proc "c" () -> i32 {
	requested := state.fullscreen_request
	state.fullscreen_request = false
	return 1 if requested else 0
}
