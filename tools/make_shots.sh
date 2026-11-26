#!/usr/bin/env bash
# Renders the real game (native test harness, same framebuffer code as the wasm build) to assets/screenshots/*.png at 3x.
set -euo pipefail
cd "$(dirname "$0")/.."
tmp="$(mktemp -d)"
trap 'rm -rf "$tmp"' EXIT
odin test odin/game -collection:los=odin -o:speed -define:ODIN_TEST_THREADS=1 -define:DUMP_DIR="$tmp" -define:ODIN_TEST_NAMES=game.dump_screens >/dev/null 2>&1
mkdir -p assets/screenshots
python3 - "$tmp" <<'PY'
import sys, glob, os
from PIL import Image
for f in sorted(glob.glob(sys.argv[1] + "/*.ppm")):
    im = Image.open(f)
    im = im.resize((im.width * 3, im.height * 3), Image.NEAREST)
    out = os.path.join("assets", "screenshots", os.path.basename(f)[:-4] + ".png")
    im.save(out)
    print("wrote", out)
PY
