"""
Wrap a raw window screenshot in the same chrome as the existing
flag-icons-demo.png: light-grey canvas, rounded window corners, soft
drop shadow. One-shot — run on demand after a fresh capture.

Usage: python add-preview-chrome.py <raw.png> <out.png>
"""
import sys
from PIL import Image, ImageDraw, ImageFilter


def round_corners(im: Image.Image, radius: int) -> Image.Image:
    """Apply an antialiased rounded-rect alpha mask to an RGBA image."""
    w, h = im.size
    # Render the mask at 4x and downsample for crisp edges.
    scale = 4
    mask = Image.new("L", (w * scale, h * scale), 0)
    ImageDraw.Draw(mask).rounded_rectangle(
        (0, 0, w * scale - 1, h * scale - 1),
        radius=radius * scale,
        fill=255,
    )
    mask = mask.resize((w, h), Image.LANCZOS)
    out = im.convert("RGBA")
    out.putalpha(mask)
    return out


def make_shadow(size: tuple[int, int], radius: int, blur: int,
                opacity: int, offset_y: int) -> Image.Image:
    """Soft drop-shadow layer sized to receive the rounded window."""
    w, h = size
    pad = blur * 2
    shadow = Image.new("RGBA", (w + pad * 2, h + pad * 2 + offset_y), (0, 0, 0, 0))
    ImageDraw.Draw(shadow).rounded_rectangle(
        (pad, pad + offset_y, pad + w, pad + h + offset_y),
        radius=radius,
        fill=(0, 0, 0, opacity),
    )
    return shadow.filter(ImageFilter.GaussianBlur(blur))


def main(src_path: str, out_path: str) -> None:
    window = Image.open(src_path).convert("RGBA")
    w, h = window.size

    corner = 12
    side_pad = 56     # canvas padding (l/r/top)
    bottom_pad = 80   # extra bottom room so the shadow tail breathes
    bg = (245, 245, 245, 255)  # matches the soft light-grey in the existing preview

    rounded = round_corners(window, corner)
    shadow = make_shadow(
        (w, h),
        radius=corner,
        blur=24,        # gaussian sigma
        opacity=80,     # ~31% alpha
        offset_y=14,    # subtle downward cast
    )

    canvas_w = w + side_pad * 2
    canvas_h = h + side_pad + bottom_pad
    canvas = Image.new("RGBA", (canvas_w, canvas_h), bg)

    # Shadow first (centered, offset down)
    sx = (canvas_w - shadow.width) // 2
    sy = side_pad - (shadow.height - h) // 2
    canvas.alpha_composite(shadow, (sx, sy))

    # Then the window on top
    canvas.alpha_composite(rounded, (side_pad, side_pad))

    canvas.convert("RGB").save(out_path, "PNG", optimize=True)
    print(f"Wrote {out_path}: {canvas.width}x{canvas.height}")


if __name__ == "__main__":
    main(sys.argv[1], sys.argv[2])
