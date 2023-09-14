using IlvAlbumOptimizer.Illuvidex;
using IlvAlbumOptimizer.Utils;
using System.Threading.Tasks;
using System.Windows;

namespace IlvAlbumOptimizer
{
    public partial class MainWindow : Window
    {
        private void Optimize_Click(object sender, RoutedEventArgs args)
        {
            var wallet = xWalletTextBox.Text;
            var token = BearerToken ?? xAuthTextBox.Text;
            var dryRun = xDryRunCheckBox.IsOn == true;
            var printOnlyOptimized = xPrintOnlyOptimize.IsOn == true;
            var optimizer = new AlbumOptimizer(token, wallet, dryRun);
            Logger.Verbose = printOnlyOptimized;

            StartOptimize(optimizer);
        }

        private void StartOptimize(AlbumOptimizer optimizer)
        {
            var specificCollection = xSpecificCollectionComboBox?.SelectedItem?.ToString();
            if (noCollectionString.Equals(specificCollection))
                WrapTask(Task.Run(optimizer.OptimizeAlbum));
            else
                WrapTask(Task.Run(async () => await optimizer.OptimizeCollection(specificCollection)));
        }
    }
}