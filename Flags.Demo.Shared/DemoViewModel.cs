using System.Collections.Generic;
using System.Linq;
using Flags.Icons;

namespace Flags.Demo.Shared {
    public class DemoViewModel : ObservableObject {
        private IReadOnlyList<FlagSection> _sections = System.Array.Empty<FlagSection>();
        private FlagEntry? _selected;
        private string _copyText = string.Empty;
        private string? _searchText;
        private DemoSource _source = DemoSource.Twemoji;

        public DemoViewModel() => UpdateSections();

        /// <summary>Section groups for the current <see cref="Source"/> + <see cref="SearchText"/>.</summary>
        public IReadOnlyList<FlagSection> Sections {
            get => _sections;
            private set => Set(ref _sections, value);
        }

        /// <summary>Flat entry list (sections concatenated). Convenience for demos that don't render headers.</summary>
        public IEnumerable<FlagEntry> Flags => _sections.SelectMany(s => s.Entries);

        public FlagEntry? Selected {
            get => _selected;
            set {
                if (Set(ref _selected, value) && value != null) {
                    // FlagSource enum value names are the control's DP names (Twemoji/Circle/Square/Lipis).
                    CopyText = $"<flag:FlagIcon {value.Source}=\"{value.Code}\"/>";
                }
            }
        }

        public string CopyText {
            get => _copyText;
            private set => Set(ref _copyText, value);
        }

        public string? SearchText {
            get => _searchText;
            set { if (Set(ref _searchText, value)) UpdateSections(); }
        }

        public DemoSource Source {
            get => _source;
            set { if (Set(ref _source, value)) UpdateSections(); }
        }

        public IReadOnlyList<DemoSource> Sources => FlagCatalog.AllSources;

        public void Select(FlagEntry entry) => Selected = entry;

        private void UpdateSections() => Sections = FlagCatalog.Sections(Source, SearchText);
    }
}
