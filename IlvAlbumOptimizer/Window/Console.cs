using IlvAlbumOptimizer.Utils;
using System;
using System.Windows;

namespace IlvAlbumOptimizer
{
    public partial class MainWindow : Window
    {
        private void SetupLogger(object sender, RoutedEventArgs args)
        {
            Logger.Setup(Write, WriteLine, WriteOnPrevLine);

            void Write(string message) => Dispatcher.Invoke(() =>
            {
                xConsole.AppendText(message);
                xConsole.ScrollToEnd();
            });
            void WriteLine(string message) => Dispatcher.Invoke(() =>
            {
                xConsole.AppendText(message + Environment.NewLine);
                xConsole.ScrollToEnd();
            });
            void WriteOnPrevLine(string message) => Dispatcher.Invoke(() =>
            {
                xConsole.Text = xConsole.Text[0..xConsole.Text.LastIndexOf(Environment.NewLine)];
                WriteLine(message);
            });
        }

        private void ClearLog_Click(object sender, RoutedEventArgs args) => ClearLog();

        private void ClearLog() => xConsole.Clear();
    }
}