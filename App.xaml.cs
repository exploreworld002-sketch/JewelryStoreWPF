using System.Windows;

namespace JewelryStoreWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set global application settings
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }
}