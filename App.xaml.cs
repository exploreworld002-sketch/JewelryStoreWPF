using System.Windows;
using System.Windows.Media;

namespace JewelryStoreWPF
{
    public partial class App : Application
    {
        public static bool IsDarkMode { get; set; } = true;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set global application settings
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }
}