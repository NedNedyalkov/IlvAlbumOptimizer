using IlvAlbumOptimizer.Illuvidex;
using IlvAlbumOptimizer.Utils;
using System.Threading.Tasks;
using System.Windows;

namespace IlvAlbumOptimizer
{
    public partial class MainWindow : Window
    {
        private void Unsleeve_Click(object sender, RoutedEventArgs e)
        {
            var token = BearerToken ?? xAuthTextBox.Text;
            var dryRun = xDryRunCheckBox.IsOn == true;
            var printOnlyOptimized = xPrintOnlyOptimize.IsOn == true;
            var unsleever = new AlbumUnsleever(token, dryRun);
            Logger.Verbose = printOnlyOptimized;

            StartUnsleeve(unsleever);
        }

        private void StartUnsleeve(AlbumUnsleever unsleever)
        {
            var specificCollection = xSpecificCollectionComboBox?.SelectedItem?.ToString();
            if (noCollectionString.Equals(specificCollection))
                WrapTask(Task.Run(unsleever.UnsleeveAlbum));
            else
                WrapTask(Task.Run(async () => await unsleever.UnsleeveCollection(specificCollection)));
        }
    }
}