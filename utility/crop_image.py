#!/usr/bin/env python3
"""Crop a 480x480 image to its centered 480x320 portion."""

import argparse
import os
from pathlib import Path

from PIL import Image


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("image", type=Path, help="path to the 480x480 PNG image")
    args = parser.parse_args()

    if not os.path.isfile(args.image):
        print(f"Warning: file not found.")        
        return

    with Image.open(args.image) as image:
        if image.size != (480, 480):
            print(f"Warning: expected a 480x480 image, got {image.width}x{image.height}.")
            return

        cropped = image.crop((0, 80, 480, 400))
        output_path = args.image.with_name(f"{args.image.stem}320.png")
        cropped.save(output_path)
        print(f"Saved {output_path}")


if __name__ == "__main__":
    main()
