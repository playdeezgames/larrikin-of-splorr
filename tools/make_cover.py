#!/usr/bin/env python3
"""Draws assets/cover.png (630x500) with the game's own DINOBYTE font and a real frame
(assets/screenshots/02-enemy.png, made by tools/make_shots.sh). Run make_shots.sh first."""
import os
from PIL import Image

root = os.path.join(os.path.dirname(__file__), "..")
font = Image.open(os.path.join(root, "assets", "fonts", "dbyte_1x.png")).convert("RGBA")
W, H = 630, 500
img = Image.new("RGB", (W, H), (12, 12, 12))
px = img.load()


def text(s, x, y, scale, color):
    for ch in s:
        code = ord(ch)
        sx, sy = code % 16 * 6, code // 16 * 8
        for gy in range(8):
            for gx in range(6):
                if font.getpixel((sx + gx, sy + gy))[3] > 0:
                    for dy in range(scale):
                        for dx in range(scale):
                            X, Y = x + gx * scale + dx, y + gy * scale + dy
                            if 0 <= X < W and 0 <= Y < H:
                                px[X, Y] = color
        x += 6 * scale


def centered(s, y, scale, color):
    text(s, (W - len(s) * 6 * scale) // 2, y, scale, color)


LIME = (22, 198, 12)
GRAY = (118, 118, 118)
centered("Larrikin of SPLORR!!", 24, 4, LIME)
shot = Image.open(os.path.join(root, "assets", "screenshots", "02-enemy.png")).convert("RGB")
frame = shot.resize((560, 300), Image.NEAREST)
img.paste(frame, ((W - frame.width) // 2, 80))
centered("Find the Macguffin. Fight things.", 404, 3, GRAY)
centered("Dungeon Crawler Jam 2022", 440, 3, GRAY)
out = os.path.join(root, "assets", "cover.png")
img.save(out)
print("wrote", out)
