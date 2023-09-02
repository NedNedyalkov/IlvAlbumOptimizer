using System;
using System.Threading.Tasks;
using System.Windows;

namespace IlvAlbumOptimizer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += SetupLogger;
            Loaded += RefreshAuth_Click;
            Loaded += SetupTokenPaste;
        }

        private void SetControls(bool enabled)
        {
            xOptimizeButton.IsEnabled = enabled;
            xGetAuthButton.IsEnabled = enabled;
            xUnsleeveButton.IsEnabled = enabled;
            xClearLogButton.IsEnabled = enabled;

            xDryRunCheckBox.IsEnabled = enabled;
            xPrintOnlyOptimize.IsEnabled = enabled;
            xDarkTheme.IsEnabled = enabled;
        }

        private async void WrapTask(Task task)
        {
            ClearLog();
            SetControls(enabled: false);
            _ = task.ContinueWith(_ => Dispatcher.Invoke(() => SetControls(enabled: true)));

            await task;
        }
    }
}
