using System;
using System.Windows.Forms;

namespace Flags.Icons.WinForms.Demo {
    public static class Program {
        [STAThread]
        public static void Main() {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
