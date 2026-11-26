#!/usr/bin/env bash
# Prepares the itch.io build: tests, a size-optimised web build, a zip. Pushes ONLY with --push.
# Never commits, never deletes anything but its own zip. Usage: tools/ship.sh [--push]
set -euo pipefail
cd "$(dirname "$0")/.."
# The page: https://thegrumpygamedev.itch.io/larrikin-of-splorr  (its old windows channel is untouched by this; they come off the page in the dashboard on ship day)
TARGET="thegrumpygamedev/larrikin-of-splorr:html"
ZIP="build/larrikin-of-splorr-html5.zip"

tools/test.sh
tools/build.sh --size
rm -f "$ZIP"
(cd build/web && zip -q -9 -r "../$(basename "$ZIP")" .)
echo "zip: $ZIP ($(du -h "$ZIP" | cut -f1)); wasm $(du -h build/web/game.wasm | cut -f1)"
unzip -l "$ZIP" | tail -20

if [ "${1:-}" = "--push" ]; then
	if [ -n "$(git status --porcelain)" ]; then
		echo "note: the working tree has uncommitted changes; what is pushed is the working tree, not a commit"
	fi
	butler push "$ZIP" "$TARGET"
	echo "pushed to $TARGET. It goes live after itch.io processes it; check: butler status $TARGET"
else
	echo "not pushed (run tools/ship.sh --push when you want it live; the target is $TARGET)"
fi
