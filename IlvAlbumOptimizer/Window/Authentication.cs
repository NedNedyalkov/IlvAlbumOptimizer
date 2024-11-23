using IlvAlbumOptimizer.Illuvidex.API;
using IlvAlbumOptimizer.Illuvidex.Objects;
using IlvAlbumOptimizer.Properties;
using IlvAlbumOptimizer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IlvAlbumOptimizer
{
    public partial class MainWindow : Window
    {
        public string BearerToken { get; set; }

        private void SetupTokenPaste(object sender, RoutedEventArgs e)
        {
            DataObject.AddPastingHandler(xAuthTextBox, OnPaste);
            DataObject.AddPastingHandler(xWalletTextBox, OnPasteWallet);
        }

        private async void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.SourceDataObject.GetData(DataFormats.Text) is not string pastedText)
                return;

            ClearLog();

            var token = GetBearerTokenFromPastedText(pastedText);
            if (token is not null && await CheckAuthData(token, string.Empty, string.Empty))
            {
                UpdateAuthUI(true, token, token, xWalletTextBox.Text);
                DataObject d = new DataObject();
                d.SetData(DataFormats.Text, string.Empty);
                e.DataObject = d;
            }
            else
            {
                UpdateAuthUI(false, token, string.Empty, string.Empty);
            }
        }

        private void OnPasteWallet(object sender, DataObjectPastingEventArgs e)
        {
            if (e.SourceDataObject.GetData(DataFormats.Text) is not string pastedText)
                return;

            SaveWallet(pastedText);
        }

        private static void SaveWallet(string wallet)
        {
            Settings.Default.Wallet = wallet;
            Settings.Default.Save();
            Settings.Default.Reload();
        }

        private string GetBearerTokenFromPastedText(string pastedText)
        {
            foreach (var line in pastedText.Split(['\n', '\r']))
            {
                if (line.StartsWith("authorization"))
                    return line.Replace("authorization: ", string.Empty);
            }

            return null;
        }

        private void RefreshAuth_Click(object sender, RoutedEventArgs args)
        {
            ClearLog();
            _ = RefreshAuthAsync();
        }

        private async Task RefreshAuthAsync()
        {
            SetControls(enabled: false);
            Logger.WriteLine("Trying to Authenticate... ");

            var (isAuthenticated, token, profile, wallet) = await TryToAuthenticate();
            UpdateAuthUI(isAuthenticated, token, profile, wallet);

            SetControls(enabled: true);
        }

        private void UpdateAuthUI(bool isAuthenticated, string token, string profile, string wallet)
        {
            if (isAuthenticated)
                BearerToken = token;
            else
                Logger.WriteLine("Try to login again in Firefox or use your Auth token directly.");

            xAuthLabel.Content = isAuthenticated ? "Profile" : "Token";
            xAuthTextBox.Text = isAuthenticated ? profile : "Paste token here";

            if (isAuthenticated)
            {
                SaveWallet(wallet);
                xWalletTextBox.Text = wallet;
            }
            else if (string.IsNullOrWhiteSpace(xWalletTextBox.Text))
            {
                xWalletTextBox.Text = "Paste wallet here";
            }
        }

        private async Task<(bool isAuthenticated, string token, string profile, string wallet)> TryToAuthenticate()
        {
            bool isAuthenticated = false;
            if (LocalStorageReader.TryGetIlluvidexAuthData(out var token, out var profile, out var wallet))
            {
                isAuthenticated = await CheckAuthData(token, profile, wallet);
            }

            return (isAuthenticated, token, profile, wallet);
        }

        private async Task<bool> CheckAuthData(string token, string profile, string wallet)
        {
            Logger.WriteLine($"Found data for profile: {profile} ({wallet})");
            Logger.WriteLine(Environment.NewLine + token + Environment.NewLine);
            var album = await new IlluvidexClient(token).FetchAlbum();
            var isAuthenticated = album is not null;
            Logger.WriteOnPrevLine(isAuthenticated ? $" Valid token!" : $"Expired token!");

            LoadCollections(album);
            return isAuthenticated;
        }

        private void LoadCollections(Album album)
        {
            xSpecificCollectionComboBox.IsEnabled = album is not null;

            if (album is null)
                return;

            var collections = new List<string>() { noCollectionString };
            collections.AddRange(album.CollectionIds);
            xSpecificCollectionComboBox.ItemsSource = collections;
            xSpecificCollectionComboBox.SelectedIndex = 0;
        }

        private void LoadWalletFromSettings(object _1 = null, RoutedEventArgs _2 = null)
        {
            xWalletTextBox.Text = Settings.Default.Wallet;
        }
    }
}