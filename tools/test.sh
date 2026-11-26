#!/usr/bin/env bash
# Native tests of the pure game package, plus type-checks of both packages for wasm.
set -euo pipefail
cd "$(dirname "$0")/.."
python3 tools/gen_fonts.py >/dev/null
git diff --quiet -- odin/game/font_data.odin || echo "note: odin/game/font_data.odin differs from the committed copy"
odin test odin/game -collection:los=odin -o:speed -define:ODIN_TEST_THREADS=1 -vet-shadowing -vet-unused
odin check odin/game -collection:los=odin -target:js_wasm32 -no-entry-point -vet-shadowing -vet-unused
odin check odin/platform/web -collection:los=odin -target:js_wasm32 -vet-shadowing -vet-unused
