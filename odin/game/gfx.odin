package game

import "core:strings"

// The original drew into a Windows console with Spectre.Console: a 112 by 30 character terminal where a
// Canvas pixel is two characters wide (so it looks square). Here the screen is a 448 by 240 framebuffer:
// the 56 by 28 view canvas is 8 by 8 blocks (448 by 224), and text is a 74 by 30 grid of 6 by 8 DINOBYTE
// glyphs. Colours are the Windows Terminal palette the jam screenshots were taken in, and xterm's for the
// 256-colour names Spectre used (DarkGreen, LightSlateGrey and so on).

VIEW_WIDTH :: 448
VIEW_HEIGHT :: 240
BLOCK :: 8 // one Spectre canvas pixel
TEXT_COLUMNS :: 74
TEXT_ROWS :: 30
TEXT_LEFT :: (VIEW_WIDTH - TEXT_COLUMNS * FONT_WIDTH) / 2

rgb :: proc "contextless" (r, g, b: u32) -> u32 {
	return 0xFF000000 | b << 16 | g << 8 | r
}

COLOR_BACKGROUND := rgb(0x0C, 0x0C, 0x0C) // the terminal's black
COLOR_TEXT := rgb(0xCC, 0xCC, 0xCC) // the terminal's default foreground

Named_Color :: struct {
	name:  string,
	color: u32,
}

NAMED_COLORS := [?]Named_Color {
	{"black", rgb(0x0C, 0x0C, 0x0C)},
	{"white", rgb(0xF2, 0xF2, 0xF2)},
	{"red", rgb(0xE7, 0x48, 0x56)},
	{"green", rgb(0x13, 0xA1, 0x0E)},
	{"lime", rgb(0x16, 0xC6, 0x0C)},
	{"olive", rgb(0xC1, 0x9C, 0x00)},
	{"aqua", rgb(0x61, 0xD6, 0xD6)},
	{"gray", rgb(0x76, 0x76, 0x76)},
	{"grey", rgb(0x76, 0x76, 0x76)},
	{"blue", rgb(0x3B, 0x78, 0xFF)},
	{"darkgreen", rgb(0x00, 0x5F, 0x00)},
	{"lightslategrey", rgb(0x87, 0x87, 0xAF)},
	{"darkslategray1", rgb(0x87, 0xFF, 0xFF)},
	{"sandybrown", rgb(0xFF, 0xAF, 0x5F)},
	{"rosybrown", rgb(0xAF, 0x87, 0x87)},
	{"default", rgb(0xCC, 0xCC, 0xCC)},
}

color_named :: proc(name: string) -> u32 {
	for n in NAMED_COLORS {
		if strings.equal_fold(n.name, name) {
			return n.color
		}
	}
	return COLOR_TEXT
}

Screen :: struct {
	pixels: [VIEW_WIDTH * VIEW_HEIGHT]u32,
}

clear_screen :: proc(s: ^Screen) {
	for &p in s.pixels {
		p = COLOR_BACKGROUND
	}
}

fill_rect :: proc(s: ^Screen, x, y, w, h: int, color: u32) {
	for yy in max(y, 0) ..< min(y + h, VIEW_HEIGHT) {
		for xx in max(x, 0) ..< min(x + w, VIEW_WIDTH) {
			s.pixels[yy * VIEW_WIDTH + xx] = color
		}
	}
}

// One Spectre canvas pixel at canvas coordinates, with the canvas placed at (left, top) in screen pixels.
set_block :: proc(s: ^Screen, left, top, column, row: int, color: u32, size := BLOCK) {
	fill_rect(s, left + column * size, top + row * size, size, size, color)
}

draw_glyph :: proc(s: ^Screen, x, y: int, ch: u8, color: u32, scale := 1) {
	c := int(ch)
	if c < FONT_FIRST || c >= FONT_FIRST + len(FONT) {
		c = '?'
	}
	glyph := FONT[c - FONT_FIRST]
	for gy in 0 ..< FONT_HEIGHT {
		bits := glyph[gy]
		for gx in 0 ..< FONT_WIDTH {
			if bits & (1 << uint(gx)) != 0 {
				fill_rect(s, x + gx * scale, y + gy * scale, scale, scale, color)
			}
		}
	}
}

// Plain text at a pixel position.
draw_text :: proc(s: ^Screen, x, y: int, text: string, color: u32, scale := 1) {
	for i in 0 ..< len(text) {
		draw_glyph(s, x + i * FONT_WIDTH * scale, y, text[i], color, scale)
	}
}

// Text with Spectre markup ("[aqua]H:[/][red]5/5[/]") at a cell of the text grid. "[[" is a literal "[".
// Returns the column after the last character.
print_markup :: proc(s: ^Screen, column, row: int, text: string, base := COLOR_TEXT) -> int {
	stack: [8]u32
	depth := 0
	color := base
	col := column
	i := 0
	for i < len(text) {
		ch := text[i]
		if ch == '[' {
			if i + 1 < len(text) && text[i + 1] == '[' {
				draw_glyph(s, TEXT_LEFT + col * FONT_WIDTH, row * FONT_HEIGHT, '[', color)
				col += 1
				i += 2
				continue
			}
			end := strings.index_byte(text[i:], ']')
			if end > 0 {
				tag := text[i + 1:i + end]
				if tag == "/" {
					if depth > 0 {
						depth -= 1
						color = stack[depth]
					} else {
						color = base
					}
				} else {
					if depth < len(stack) {
						stack[depth] = color
						depth += 1
					}
					color = color_named(tag)
				}
				i += end + 1
				continue
			}
		}
		if col < TEXT_COLUMNS {
			draw_glyph(s, TEXT_LEFT + col * FONT_WIDTH, row * FONT_HEIGHT, ch, color)
		}
		col += 1
		i += 1
	}
	return col
}

// How many characters the markup shows.
markup_length :: proc(text: string) -> int {
	n := 0
	i := 0
	for i < len(text) {
		if text[i] == '[' {
			if i + 1 < len(text) && text[i + 1] == '[' {
				n += 1
				i += 2
				continue
			}
			if end := strings.index_byte(text[i:], ']'); end > 0 {
				i += end + 1
				continue
			}
		}
		n += 1
		i += 1
	}
	return n
}

// ExplorationState.DrawImage: "X" cells in the solid colour, others in the empty colour (or left alone).
draw_image :: proc(s: ^Screen, left, top, column, row: int, image: []string, solid: u32, empty: Maybe(u32), size := BLOCK) {
	for line, y in image {
		for x in 0 ..< len(line) {
			if line[x] == 'X' {
				set_block(s, left, top, column + x, row + y, solid, size)
			} else if e, ok := empty.?; ok {
				set_block(s, left, top, column + x, row + y, e, size)
			}
		}
	}
}

// CommonUI.DoubleImage: every cell twice as wide and twice as tall, drawn straight from the 12 by 12 art.
draw_image_doubled :: proc(s: ^Screen, left, top, column, row: int, image: []string, solid: u32) {
	for line, y in image {
		for x in 0 ..< len(line) {
			if line[x] == 'X' {
				for dy in 0 ..< 2 {
					for dx in 0 ..< 2 {
						set_block(s, left, top, column + x * 2 + dx, row + y * 2 + dy, solid)
					}
				}
			}
		}
	}
}
