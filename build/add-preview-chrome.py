"""
Wrap a raw window screenshot in the project's preview chrome:
delicate drop shadow + light-grey canvas + crisply-rounded window corners.

The tricky bit is that Win+Shift+S / Snipping Tool / PrintWindow all
capture the window WITH its DWM-rendered window edge baked in — a 5-7 px
near-black border around the perimeter, plus rounded-corner anti-alias
pixels blended against whatever was behind the window on the screen at
capture time. Both bleed through onto a different-coloured canvas as
the "black outline" / "halo" artefact the old preview had.

Fix: auto-detect the dark-border thickness on each side independently,
crop it off, then apply our own clean rounded-rect mask supersampled 8x
for crisp anti-aliasing on a solid background. Per-side detection
handles Windows 11's asymmetric border (typically thicker on bottom-
right where the DWM shadow-anchor band sits) without hand-tuning.

Usage: python add-preview-chrome.py <raw.png> <out.png>
"""
import sys
from PIL import Image, ImageDraw, ImageFilter

CORNER_RADIUS = 10       # final rounded-corner radius in canvas pixels
SUPERSAMPLE = 8          # mask is rendered SUPERSAMPLEx and downsampled
CANVAS_BG = (0, 0, 0, 0)  # transparent — the README's GitHub background shows through
SIDE_PAD = 56            # canvas padding on left/right/top of the window
BOTTOM_PAD = 80          # extra bottom room so the shadow tail breathes
# Shadow values tuned for "delicate" — softer fall-off + lower peak alpha
# so the window appears to float without the shadow drawing attention.
SHADOW_BLUR = 32         # gaussian sigma — larger = softer / more diffuse
SHADOW_OPACITY = 45      # ~18% alpha at the darkest point of the shadow
SHADOW_OFFSET_Y = 10     # subtle downward cast

# Auto-detect knobs.
# 175 picks up everything from the heavy ~35-RGB DWM border on some captures
# down to the thin ~120-160-RGB grey-edge pixels on others, while leaving
# typical content (title bar gradient ~205-232, client area ~240+) above
# the line. Dark text glyphs inside the window are below it too but they
# don't appear at the depths we scan (text is always inset by padding).
DARK_THRESHOLD = 175
MAX_DETECT_DEPTH = 40    # don't crop more than this many px per side, ever
SAMPLE_COUNT = 60        # how many positions to probe along each edge
EDGE_MARGIN_PCT = 0.10   # skip the outer N% of each edge to avoid corners
CONSENSUS_FRACTION = 0.50  # trim a row/column only while >50% of samples are
                           # still dark at that depth — finds the *consistent*
                           # border thickness without getting fooled by a few
                           # positions that have a deeper dark zone. 50% is
                           # the right cutoff for the MAUI captures the
                           # README uses, where Win+Shift+S includes some
                           # solid-black padding around the actual window
                           # (15-30 px on right/bottom depending on the
                           # capture viewport) and the title-bar close
                           # buttons are hidden under the overlay popup
                           # anyway — so an aggressive trim removes the
                           # whole black zone without eating anything the
                           # user wanted to see.


def detect_border(im: Image.Image, side: str) -> int:
    """Return the dark-border thickness on `side` in pixels.

    Samples SAMPLE_COUNT positions evenly spaced along the edge (skipping
    the corners via EDGE_MARGIN_PCT) and walks inward one row/column at a
    time. Trims while a strict majority of samples (>CONSENSUS_FRACTION)
    are still dark at that depth; stops as soon as the majority flips to
    light. The returned consensus depth becomes the per-side trim amount.

    Aggressive on purpose: an overlay popup that extends into the dark-
    padding zone (e.g. a dropdown whose right border sits inside the
    captured window-edge band) will get its overhanging columns clipped.
    That's the right trade-off for the README's MAUI captures, where the
    title-bar buttons hidden under the overlay popup aren't visible
    anyway. Re-capture without the dropdown open if you need pristine
    overlay rendering.
    """
    if side not in ('top', 'bottom', 'left', 'right'):
        raise ValueError(f"detect_border: unsupported side {side!r}")

    W, H = im.size
    px = im.load()

    if side in ('top', 'bottom'):
        span = W
        scan_max = H
        get = (
            (lambda pos, d: px[pos, d])
            if side == 'top'
            else (lambda pos, d: px[pos, H - 1 - d])
        )
    else:
        span = H
        scan_max = W
        get = (
            (lambda pos, d: px[d, pos])
            if side == 'left'
            else (lambda pos, d: px[W - 1 - d, pos])
        )

    margin = int(span * EDGE_MARGIN_PCT)
    step = max(1, (span - 2 * margin) // max(1, SAMPLE_COUNT - 1))
    positions = [margin + i * step for i in range(SAMPLE_COUNT) if margin + i * step < span]
    if not positions:
        return 0

    # Consensus pass — trim while the majority of sampled positions are
    # still dark. No safety cap: priority is removing the black frame,
    # even if an overlay popup that extends into the dark zone gets
    # clipped at its right border (the user can re-capture with the
    # dropdown closed if they want the popup pristine).
    # Clamp the scan depth to the image's perpendicular dimension so we
    # never read out of bounds on pathologically small captures.
    depth_limit = min(MAX_DETECT_DEPTH, scan_max)
    threshold_count = len(positions) * CONSENSUS_FRACTION
    trim = 0
    for d in range(depth_limit):
        dark_at_d = sum(
            1 for pos in positions
            if all(c < DARK_THRESHOLD for c in get(pos, d)[:3])
        )
        if dark_at_d > threshold_count:
            trim = d + 1
        else:
            break
    return trim


def round_corners(im: Image.Image, radius: int) -> Image.Image:
    """Apply a supersampled rounded-rect alpha mask. PIL's rounded_rectangle
    draws at the target resolution with limited anti-aliasing — rendering at
    SUPERSAMPLEx then Lanczos-downscaling gives sub-pixel edge quality."""
    w, h = im.size
    big = (w * SUPERSAMPLE, h * SUPERSAMPLE)
    mask = Image.new("L", big, 0)
    ImageDraw.Draw(mask).rounded_rectangle(
        (0, 0, big[0] - 1, big[1] - 1),
        radius=radius * SUPERSAMPLE,
        fill=255,
    )
    mask = mask.resize((w, h), Image.LANCZOS)
    out = im.convert("RGBA")
    out.putalpha(mask)
    return out


def make_shadow(size: tuple[int, int], radius: int) -> Image.Image:
    """Soft drop-shadow layer sized to receive the rounded window."""
    w, h = size
    pad = SHADOW_BLUR * 2
    shadow = Image.new("RGBA", (w + pad * 2, h + pad * 2 + SHADOW_OFFSET_Y), (0, 0, 0, 0))
    ImageDraw.Draw(shadow).rounded_rectangle(
        (pad, pad + SHADOW_OFFSET_Y, pad + w, pad + h + SHADOW_OFFSET_Y),
        radius=radius,
        fill=(0, 0, 0, SHADOW_OPACITY),
    )
    return shadow.filter(ImageFilter.GaussianBlur(SHADOW_BLUR))


def main(src_path: str, out_path: str) -> None:
    raw = Image.open(src_path).convert("RGB")
    rw, rh = raw.size

    tt = detect_border(raw, 'top')
    tb = detect_border(raw, 'bottom')
    tl = detect_border(raw, 'left')
    tr = detect_border(raw, 'right')

    # Guard against degenerate trim combos on tiny / pathological captures
    # that would otherwise produce an empty or inverted crop rectangle.
    left, top, right, bottom = tl, tt, rw - tr, rh - tb
    if right <= left or bottom <= top:
        raise ValueError(
            f"auto-trim L{tl}/T{tt}/R{tr}/B{tb} would yield an empty crop "
            f"of source {rw}x{rh}"
        )
    window = raw.crop((left, top, right, bottom))
    w, h = window.size

    rounded = round_corners(window, CORNER_RADIUS)
    shadow = make_shadow((w, h), CORNER_RADIUS)

    canvas_w = w + SIDE_PAD * 2
    canvas_h = h + SIDE_PAD + BOTTOM_PAD
    canvas = Image.new("RGBA", (canvas_w, canvas_h), CANVAS_BG)

    sx = (canvas_w - shadow.width) // 2
    sy = SIDE_PAD - (shadow.height - h) // 2
    canvas.alpha_composite(shadow, (sx, sy))
    canvas.alpha_composite(rounded, (SIDE_PAD, SIDE_PAD))

    # Save with the alpha channel intact so the transparent canvas + soft
    # shadow falloff blend cleanly onto whatever background the README
    # renders against (GitHub light/dark, nuget.org, etc.).
    canvas.save(out_path, "PNG", optimize=True)
    print(f"Wrote {out_path}: {canvas.width}x{canvas.height} "
          f"(auto-trim L{tl}/T{tt}/R{tr}/B{tb}, {SUPERSAMPLE}x mask supersample, "
          f"radius {CORNER_RADIUS}px)")


if __name__ == "__main__":
    main(sys.argv[1], sys.argv[2])
