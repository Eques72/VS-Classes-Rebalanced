#!/usr/bin/env bash

set -euo pipefail

PROJECT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
RELEASE_DIR="$PROJECT_DIR/Releases"
MODS_DIR="$HOME/.var/app/at.vintagestory.VintageStory/config/VintagestoryData/Mods"

latest_release="$(printf '%s\n' "$RELEASE_DIR"/allrebalanced_*.zip | sort -V | tail -n 1)"

if [[ ! -f "$latest_release" ]]; then
    printf 'No release zip found in %s\n' "$RELEASE_DIR" >&2
    exit 1
fi

mkdir -p -- "$MODS_DIR"
cp -f -- "$latest_release" "$MODS_DIR/"

exec /usr/bin/flatpak run \
    --branch=stable \
    --arch=x86_64 \
    --command=vintagestory \
    at.vintagestory.VintageStory
