#!/usr/bin/env bash
set -euo pipefail

MI="modinfo.json"

modid=$(jq -r '.modid' "$MI")
version=$(jq -r '.version' "$MI")

archive="${modid}-${version}.zip"

zip -r "$archive" assets/ modinfo.json modicon.png
