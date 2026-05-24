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
    /// Uno Platform control that renders a flag icon for a given <see cref="FlagKind"/>. PNG kinds
    /// stream the embedded asset into a <see cref="BitmapImage"/>; SVG kinds use
    /// <see cref="SvgImageSource"/> so vector flags scale crisply at any size.
    /// </summary>
    public sealed class FlagIcon : ContentControl {
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(
                nameof(Kind),
                typeof(FlagKind),
                typeof(FlagIcon),
                new PropertyMetadata(FlagKind.None, (d, _) => ((FlagIcon)d).UpdateSource()));

        private static readonly DependencyProperty SourcePropertyInternal =
            DependencyProperty.Register(
                nameof(Source),
                typeof(ImageSource),
                typeof(FlagIcon),
                new PropertyMetadata(null));

        public static DependencyProperty SourceProperty => SourcePropertyInternal;

        // Uno Skia desktop NREs in BitmapImage.SetSourceAsync(IRandomAccessStream) — see
        // https://github.com/unoplatform/uno/issues/20465 — so we extract embedded assets to a
        // per-process temp folder once and feed them back via UriSource, which goes through the
        // working file-URI path.
        private static readonly string CacheDirectory = InitializeCache();
        private static readonly ConcurrentDictionary<FlagKind, Uri> UriCache = new();

        private readonly Image _image;

        public FlagIcon() {
            Width = 24;
            Height = 18;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            _image = new Image { Stretch = Stretch.Uniform };
            Content = _image;
        }

        public FlagKind Kind {
            get => (FlagKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        public ImageSource? Source {
            get => (ImageSource?)GetValue(SourcePropertyInternal);
            private set => SetValue(SourcePropertyInternal, value);
        }

        private void UpdateSource() {
            var kind = Kind;
            if (kind == FlagKind.None) {
                _image.Source = null;
                Source = null;
                return;
            }

            var uri = GetOrExtractUri(kind);
            if (uri == null) {
                _image.Source = null;
                Source = null;
                return;
            }

            ImageSource source = FlagKindResolver.IsSvg(kind)
                ? new SvgImageSource(uri)
                : new BitmapImage(uri);

            _image.Source = source;
            Source = source;
        }

        private static Uri? GetOrExtractUri(FlagKind kind) {
            if (UriCache.TryGetValue(kind, out var cached)) return cached;

            var info = FlagKindResolver.GetInfo(kind);
            if (info == null) return null;

            var path = Path.Combine(CacheDirectory, info.FileName);
            if (!File.Exists(path) && !TryExtract(kind, path)) return null;

            var uri = new Uri(path);
            UriCache[kind] = uri;
            return uri;
        }

        private static bool TryExtract(FlagKind kind, string destinationPath) {
            using var source = FlagAssetLoader.OpenStream(kind);
            if (source == null) return false;

            var tempPath = destinationPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            using (var dest = File.Create(tempPath)) {
                source.CopyTo(dest);
            }

            try {
                File.Move(tempPath, destinationPath);
            } catch (IOException) when (File.Exists(destinationPath)) {
                File.Delete(tempPath);
            }
            return true;
        }

        private static string InitializeCache() {
            var dir = Path.Combine(Path.GetTempPath(), "Flags.Icons.Uno");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
