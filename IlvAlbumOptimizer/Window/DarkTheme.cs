using ModernWpf;
using System.Windows;
using System.Windows.Media;

namespace IlvAlbumOptimizer
{
    public partial class MainWindow : Window
    {
        private SolidColorBrush themeBrush;
        private SolidColorBrush ThemeBrush => themeBrush ??= TryFindResource("ThemeBrush") as SolidColorBrush;

        private void DarkTheme_Toggled(object sender, RoutedEventArgs e)
        {
            var isDark = xDarkTheme.IsOn;
            ThemeManager.Current.ApplicationTheme = isDark ? ApplicationTheme.Dark : ApplicationTheme.Light;
            ThemeManager.Current.AccentColor = isDark ? Colors.DarkCyan : Colors.LightBlue;
            ThemeBrush.Color = isDark ? Colors.White : Colors.Black;
        }
    }
}