using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IlvAlbumOptimizer
{
    public partial class MainWindow : Window
    {
        private const string donationAddress = "0x40e816b38af1e2cc60859bc7f9be01f3ce78c3c0";

        private void DonateAddress_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(donationAddress);

            if (xDonateAddress.ToolTip is ToolTip tip)
            {
                tip.Content = "Copied!";
                tip.IsOpen = true;
            }

            Task.Run(async () =>
            {
                await Task.Delay(1000);
                Dispatcher.Invoke(() =>
                {
                    if (xDonateAddress.ToolTip is ToolTip tip)
                    {
                        tip.Content = "Click to copy address!";
                        tip.IsOpen = false;
                    }
                });
            });
        }
    }
}