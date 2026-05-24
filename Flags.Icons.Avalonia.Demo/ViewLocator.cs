using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Flags.Demo.Shared;

namespace Flags.Icons.Avalonia.Demo {
    public class ViewLocator : IDataTemplate {
        public Control Build(object? data) {
            if (data is null) return new TextBlock { Text = "null" };
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null) {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data) {
            return data is DemoViewModel;
        }
    }
}
