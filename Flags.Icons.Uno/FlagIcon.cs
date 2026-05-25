using System;
using System.Collections.Concurrent;
using System.IO;
using Flags.Icons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Flags.Icons.Uno {
    /// <summary>
    /// Uno Platform control that renders a single flag SVG from one of the 4 bundled sources.
    /// Set exactly one of <see cref="Twemoji"/>, <see cref="Circle"/>, <see cref="Square"/>,
    /// <see cref="Lipis"/> — assigning to one of them clears the others. SVGs are loaded through
    /// <see cref="SvgImageSource"/> so they scale crisply at any size.
    /// </summary>
    public sealed class FlagIcon : ContentControl {
        public static readonly DependencyProperty TwemojiProperty = DependencyProperty.Register(
            nameof(Twemoji), typeof(TwemojiFlag), typeof(FlagIcon),
            new PropertyMetadata(TwemojiFlag.None, (d, _) => ((FlagIcon)d).OnKindChanged(FlagSource.Twemoji)));

        public static readonly DependencyProperty CircleProperty = DependencyProperty.Register(
            nameof(Circle), typeof(CircleFlag), typeof(FlagIcon),
            new PropertyMetadata(CircleFlag.None, (d, _) => ((FlagIcon)d).OnKindChanged(FlagSource.Circle)));

        public static readonly DependencyProperty SquareProperty = DependencyProperty.Register(
            nameof(Square), typeof(SquareFlag), typeof(FlagIcon),
            new PropertyMetadata(SquareFlag.None, (d, _) => ((FlagIcon)d).OnKindChanged(FlagSource.Square)));

        public static readonly DependencyProperty LipisProperty = DependencyProperty.Register(
            nameof(Lipis), typeof(LipisFlag), typeof(FlagIcon),
            new PropertyMetadata(LipisFlag.None, (d, _) => ((FlagIcon)d).OnKindChanged(FlagSource.Lipis)));

        private static readonly DependencyProperty SourcePropertyInternal = DependencyProperty.Register(
            nameof(Source), typeof(ImageSource), typeof(FlagIcon), new PropertyMetadata(null));
        public static DependencyProperty SourceProperty => SourcePropertyInternal;

        // Uno Skia desktop NREs in BitmapImage.SetSourceAsync(IRandomAccessStream) — see
        // https://github.com/unoplatform/uno/issues/20465 — so we extract embedded assets to a
        // per-process temp folder once and feed them back via UriSource, which goes through the
        // working file-URI path.
        private static readonly string CacheDirectory = InitializeCache();
        private static readonly ConcurrentDictionary<string, Uri> UriCache = new();

        private readonly Image _image;
        private bool _suppress;

        public FlagIcon() {
            Width = 24;
            Height = 18;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            _image = new Image { Stretch = Stretch.Uniform };
            Content = _image;
        }

        public TwemojiFlag Twemoji { get => (TwemojiFlag)GetValue(TwemojiProperty); set => SetValue(TwemojiProperty, value); }
        public CircleFlag Circle { get => (CircleFlag)GetValue(CircleProperty); set => SetValue(CircleProperty, value); }
        public SquareFlag Square { get => (SquareFlag)GetValue(SquareProperty); set => SetValue(SquareProperty, value); }
        public LipisFlag Lipis { get => (LipisFlag)GetValue(LipisProperty); set => SetValue(LipisProperty, value); }

        public ImageSource? Source {
            get => (ImageSource?)GetValue(SourcePropertyInternal);
            private set => SetValue(SourcePropertyInternal, value);
        }

        private void OnKindChanged(FlagSource changed) {
            if (_suppress) return;
            _suppress = true;
            try {
                if (changed != FlagSource.Twemoji && Twemoji != TwemojiFlag.None) Twemoji = TwemojiFlag.None;
                if (changed != FlagSource.Circle && Circle != CircleFlag.None) Circle = CircleFlag.None;
                if (changed != FlagSource.Square && Square != SquareFlag.None) Square = SquareFlag.None;
                if (changed != FlagSource.Lipis && Lipis != LipisFlag.None) Lipis = LipisFlag.None;
            } finally {
                _suppress = false;
            }
            UpdateSource();
        }

        private void UpdateSource() {
            var uri = GetActiveUri();
            if (uri == null) {
                _image.Source = null;
                Source = null;
                return;
            }
            var source = new SvgImageSource(uri);
            _image.Source = source;
            Source = source;
        }

        private Uri? GetActiveUri() {
            if (Twemoji != TwemojiFlag.None) return GetOrExtractUri("twemoji", TwemojiFlagFiles.GetFileName(Twemoji), () => FlagAssetLoader.OpenStream(Twemoji));
            if (Circle != CircleFlag.None) return GetOrExtractUri("circle-flags", CircleFlagFiles.GetFileName(Circle), () => FlagAssetLoader.OpenStream(Circle));
            if (Square != SquareFlag.None) return GetOrExtractUri("square-flags", SquareFlagFiles.GetFileName(Square), () => FlagAssetLoader.OpenStream(Square));
            if (Lipis != LipisFlag.None) return GetOrExtractUri("flag-icons", LipisFlagFiles.GetFileName(Lipis), () => FlagAssetLoader.OpenStream(Lipis));
            return null;
        }

        private static Uri? GetOrExtractUri(string sourceDirName, string? fileName, Func<Stream?> openStream) {
            if (fileName == null) return null;
            var cacheKey = sourceDirName + "/" + fileName;
            if (UriCache.TryGetValue(cacheKey, out var cached)) return cached;

            var dir = Path.Combine(CacheDirectory, sourceDirName);
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, fileName);
            if (!File.Exists(path) && !TryExtract(openStream(), path)) return null;

            var uri = new Uri(path);
            UriCache[cacheKey] = uri;
            return uri;
        }

        private static bool TryExtract(Stream? source, string destinationPath) {
            if (source == null) return false;
            using (source) {
                var tempPath = destinationPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
                using (var dest = File.Create(tempPath)) source.CopyTo(dest);
                try {
                    File.Move(tempPath, destinationPath);
                } catch (IOException) when (File.Exists(destinationPath)) {
                    File.Delete(tempPath);
                }
                return true;
            }
        }

        private static string InitializeCache() {
            var dir = Path.Combine(Path.GetTempPath(), "Flags.Icons.Uno");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
