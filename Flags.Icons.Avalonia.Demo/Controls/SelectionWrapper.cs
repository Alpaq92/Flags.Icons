using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace Flags.Icons.Avalonia.Demo.Controls {
    [PseudoClasses("selectednow")]
    public class SelectionWrapper : UserControl {
        static SelectionWrapper() {
            CurrentSelectedProperty.Changed.AddClassHandler<SelectionWrapper>((x, _) => x.UpdateSelectedNow());
            SelectedNowProperty.Changed.AddClassHandler<SelectionWrapper>((x, e) => {
                if (e.NewValue is bool v && v) {
                    x.PseudoClasses.Add(":selectednow");
                }
                else {
                    x.PseudoClasses.Remove(":selectednow");
                }
            });
        }

        protected override void OnDataContextEndUpdate() {
            base.OnDataContextEndUpdate();
            UpdateSelectedNow();
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e) {
            base.OnPointerPressed(e);
            CurrentSelected = DataSource;
        }

        public void UpdateSelectedNow() {
            SelectedNow = DataSource != null && DataSource == CurrentSelected;
        }

        public static readonly StyledProperty<object?> DataSourceProperty =
            AvaloniaProperty.Register<SelectionWrapper, object?>(nameof(DataSource));

        public object? DataSource {
            get => GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }

        public static readonly StyledProperty<object?> CurrentSelectedProperty =
            AvaloniaProperty.Register<SelectionWrapper, object?>(nameof(CurrentSelected));

        public object? CurrentSelected {
            get => GetValue(CurrentSelectedProperty);
            set => SetValue(CurrentSelectedProperty, value);
        }

        public static readonly DirectProperty<SelectionWrapper, bool> SelectedNowProperty =
            AvaloniaProperty.RegisterDirect<SelectionWrapper, bool>(
                nameof(SelectedNow),
                wrapper => wrapper.CurrentSelected == wrapper.DataSource);

        private bool _selectedNow;

        public bool SelectedNow {
            get => _selectedNow;
            private set => SetAndRaise(SelectedNowProperty, ref _selectedNow, value);
        }
    }
}
