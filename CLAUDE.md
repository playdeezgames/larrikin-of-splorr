# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## State of the repo

"Larrikin of SPLORR!!" (Dungeon Crawler Jam 2022) was a VB.NET terminal game on Spectre.Console. **It now has an Odin / `js_wasm32` port in `odin/`, `web/` and `tools/`, which is the live code** (ported November 26, 2026). `LarrikinOfSPLORR/` is the original VB.NET, kept as the **reference** for the rules and the look; do not extend it. `published/` is the old committed Windows build, `ss/` the 2022 screenshots, `urizen/` the tileset the pictures were adapted from; leave them. Do not describe the game's deliberate design (no overheal protection, attacks that mostly roll 0, Run's random facing) as a problem. `QUIRKS.md` lists what was kept and what changed.

The user's knowledge base is the BOK at `/home/yermom/git/bok-of-splorr/splorr` (read `Home.md` first; this game's note is `Games/Larrikin of SPLORR!!.md`). Relevant notes: `Tech/Odin wasm recipe.md`, `Tech/Release standard.md`, `Tech/Shipping to itch.io.md`, `Tech/GitHub Pages.md`, `Tech/LootLocker.md`, `Tech/DINOBYTE font.md`, `Gotchas.md`. The port follows the Kordanor's Catacomb port (`/home/yermom/git/KordanorsCatacomb`), the sequel.

## Commands

```bash
tools/test.sh               # regenerates font_data.odin, native tests (odin/game), odin check of both packages for js_wasm32
tools/build.sh [--size]     # build/web (game.wasm, odin.js, index.html, lootlocker.js); --size for shipping
python3 -m http.server -d build/web 8000   # wasm cannot load from file://
tools/make_shots.sh         # assets/screenshots/*.png from the real renderer (native, 3x)
python3 tools/make_cover.py # assets/cover.png (630x500) from DINOBYTE and 02-enemy.png
python3 tools/gen_fonts.py  # assets/fonts/dbyte_1x.png -> odin/game/font_data.odin (generated; do not hand-edit)
tools/pages.sh              # shipping build copied to docs/ for GitHub Pages (main, /docs); never commits or pushes
tools/ship.sh [--push]      # test, size build, zip; pushes to itch ONLY with --push. shippit.sh is a one-line wrapper
```

Packages are imported with `-collection:los=odin`. The scripts pass `-vet-shadowing -vet-unused`.

**Never `git push`, run `tools/ship.sh --push`, `shippit.sh --push` or `butler push` unless the user explicitly says so**; "push" always means `git push`, "ship" always means itch.io. Ships go through Coxswain (`ship_today` first; one a day). The GitHub repo was archived from 2022 until November 26, 2026, when it was unarchived and Pages enabled (`main`, `/docs`): https://playdeezgames.github.io/larrikin-of-splorr/

## Architecture of the Odin port

- `odin/game` (package `game`, no platform imports, native-testable): `world.odin` (the 14 by 14 Prim's maze, the character and item tables, spawning, attack, defend, run, drink, the Out buffer of message lines and sounds), `save.odin` (the world as one line of integers, validated on load), `rng.odin` (SplitMix64 and the dice-string roller), `gfx.odin` + `font_data.odin` (448 by 240 framebuffer, the terminal palette, DINOBYTE text with Spectre-style `[color]...[/]` markup, block-pixel images), `images.odin` (the original's ASCII pictures and the reaper, generated once from the VB source), `ui.odin` (one state per Spectre prompt; every screen). Tests are `#+build !js`; `the_bot_can_finish_and_never_gets_stuck` plays 300 runs through the real screens.
- `odin/platform/web` (`#+build js`): `step`, `platform_*` exports, imports for localStorage and entropy. `web/index.html` blits the frame, maps keys and the touch pad to `platform_command` (the `Command` enum order), plays the original's beep tunes as square waves (`TUNES` order must match `Sfx` in `world.odin`), and holds the footer (credits, back links, a hidden LootLocker feedback link: `LL_KEY` is empty until the user makes a LootLocker game).

## Shipping (itch.io)

Page `thegrumpygamedev/larrikin-of-splorr` (public, jam entry, Windows `windows` channel, name-your-own-price; its Content section says "No generative AI was used", which must change to the AI disclosure when the port ships). The browser build goes to a new `html` channel; on ship day the old download comes off in the dashboard and the embed is "Click to launch in fullscreen". Release layout: `ITCH_DESCRIPTION.md`, `assets/cover.png`, `assets/screenshots/`, `devlog/YYYYMMDD/devlog.md`, `tools/ship.sh`, `shippit.sh`.
