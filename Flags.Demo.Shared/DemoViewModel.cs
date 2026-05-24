using System;
using System.Collections.Generic;

namespace Flags.Demo.Shared {
    public class DemoViewModel : ObservableObject {
        private IEnumerable<FlagEntry> _flags = Array.Empty<FlagEntry>();
        private FlagEntry? _selected;
        private string _copyText = string.Empty;
        private string? _searchText;
        private DemoVariant _variant = DemoVariant.SVG;

        public DemoViewModel() => UpdateFlags();

        public IEnumerable<FlagEntry> Flags {
            get => _flags;
            private set => Set(ref _flags, value);
        }

        public FlagEntry? Selected {
            get => _selected;
            set {
                if (Set(ref _selected, value) && value != null) {
                    CopyText = $"<flag:FlagIcon Kind=\"{value.Kind}\"/>";
                }
            }
        }

        public string CopyText {
            get => _copyText;
            private set => Set(ref _copyText, value);
        }

        public string? SearchText {
            get => _searchText;
            set { if (Set(ref _searchText, value)) UpdateFlags(); }
        }

        public DemoVariant Variant {
            get => _variant;
            set { if (Set(ref _variant, value)) UpdateFlags(); }
        }

        public IReadOnlyList<DemoVariant> Variants => FlagCatalog.AllVariants;

        public void Select(FlagEntry entry) => Selected = entry;

        private void UpdateFlags() => Flags = FlagCatalog.Filter(Variant, SearchText);
    }
}
