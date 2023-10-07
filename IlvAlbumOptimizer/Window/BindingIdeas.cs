using IlvAlbumOptimizer.Illuvidex;
using IlvAlbumOptimizer.Utils;
using ModernWpf;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace IlvAlbumOptimizer
{
    public partial class MainWindow : Window
    {
        private const char NoBreakingSpace = '\u00A0';
        private const string IlluvidexIlluvitarUrl = "https://illuvidex.illuvium.io/asset/0x8cceea8cfb0f8670f4de3a6cd2152925605d19a8/";

        private void FindWhatToBind_Click(object sender, RoutedEventArgs e)
        {
            var token = BearerToken ?? xAuthTextBox.Text;
            var printOnlyOptimized = xPrintOnlyOptimize.IsOn == true;
            var albumBindingIdeas = new AlbumGetBindingIdeas(token);
            Logger.Verbose = printOnlyOptimized;

            PrintBindingIdeas(albumBindingIdeas);
        }

        private async void PrintBindingIdeas(AlbumGetBindingIdeas albumBindingIdeas)
        {
            ClearLog();
            SetControls(enabled: false);
            await Task.Run(albumBindingIdeas.AnalyzeBindingPossibilities);
            SetControls(enabled: true);

            ClearLog();

            foreach (var bindingIdea in albumBindingIdeas.BindingIdeasList)
            {
                Logger.WriteLine($"{bindingIdea.Illuvitar.Name} ({NoBreakingSpace}{bindingIdea.Illuvitar.Id}{NoBreakingSpace}) in {bindingIdea.Occurences} collections");
            }

            xConsole.ScrollToHome();
        }

        private void xConsole_TryToOpenLink(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox
                && e.OriginalSource is DependencyObject original
                && original.FindAscendant<ScrollBar>() is null)
            {
                string lineText = GetLineText(e, textBox);
                OpenLinkIfLineContainsIlluvitarId(lineText);
            }
        }

        private static string GetLineText(MouseEventArgs e, TextBox textBox)
        {
            var textPos = textBox.GetCharacterIndexFromPoint(e.GetPosition(textBox), snapToText: true);
            var lineIndex = textBox.GetLineIndexFromCharacterIndex(textPos);
            var lineText = textBox.GetLineText(lineIndex).Replace("\n", string.Empty).Replace("\r", string.Empty);
            return lineText;
        }

        private static void OpenLinkIfLineContainsIlluvitarId(string lineText)
        {
            if (lineText.Contains(NoBreakingSpace))
            {
                var idStartIndex = lineText.IndexOf(NoBreakingSpace) + 1;
                var idLastIndex = lineText.LastIndexOf(NoBreakingSpace);
                var id = lineText.Substring(idStartIndex, idLastIndex - idStartIndex);
                var urlFromId = $"{IlluvidexIlluvitarUrl}{id}";
                var psi = new ProcessStartInfo(urlFromId)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(psi);
            }
        }

        private void xConsole_SuggestLinkTooltip(object sender, MouseEventArgs e)
        {
            if (sender is TextBox textBox
                && e.OriginalSource is DependencyObject original
                && original.FindAscendant<ScrollBar>() is null)
            {
                string lineText = GetLineText(e, textBox);
                if (!SetTooltipIfLineContainsIlluvitarId(lineText))
                    CloseToolTip();
            }
            else
            {
                CloseToolTip();
            }
        }

        private void CloseToolTip()
        {
            if (xConsole.ToolTip is ToolTip alreadyOpenedTooltip)
            {
                alreadyOpenedTooltip.IsOpen = false;
            }
            xConsole.ToolTip = string.Empty;
        }

        private bool SetTooltipIfLineContainsIlluvitarId(string lineText)
        {
            if (lineText.Contains(NoBreakingSpace))
            {
                if (xConsole.ToolTip is ToolTip alreadyOpenedTooltip)
                {
                    alreadyOpenedTooltip.IsOpen = false;
                    alreadyOpenedTooltip.IsOpen = true;
                    return true;
                }

                var tooltip = new ToolTip();
                tooltip.Content = "Click to open illuvitar in illuvidex!";
                tooltip.Placement = PlacementMode.MousePoint;
                tooltip.PlacementTarget = xConsole;

                xConsole.ToolTip = tooltip;
                return true;
            }

            return false;
        }
    }
}