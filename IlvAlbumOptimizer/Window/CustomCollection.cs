using System.Windows;
using System.Windows.Controls;

namespace IlvAlbumOptimizer
{
    public partial class MainWindow : Window
    {
        private const string noCollectionString = "<none>";

        private void SpecificCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = xSpecificCollectionComboBox.SelectedItem.ToString();

            if (selectedItem.Equals(noCollectionString))
            {
                xOptimizeButton.Content = "Optimize Album!";
                xUnsleeveButton.Content = "Unsleeve Album!";
            }
            else
            {
                xOptimizeButton.Content = "Optimize Collection!";
                xUnsleeveButton.Content = "Unsleeve Collection!";
            }
        }
    }
}