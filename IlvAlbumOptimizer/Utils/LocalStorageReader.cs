using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace IlvAlbumOptimizer.Utils
{
    public class LocalStorageReader
    {
        private const string illuvidexUrl = "illuvidex.illuvium.io";
        private const string GetAuthSqlQuery = "SELECT * FROM data WHERE key = 'persist:auth'";

        public static bool TryGetIlluvidexAuthData(out string token, out string profileName, out string wallet)
        {
            if (TryGetFirefoxIlluvidexAuthData(out token, out profileName, out wallet))
                return true;

            return false;
        }

        private static bool TryGetFirefoxIlluvidexAuthData(out string resultToken, out string resultProfileName, out string resultWallet)
        {
            resultToken = resultProfileName = resultWallet = null;

            var storagePath = FindFirefoxLocalStorage();

            if (storagePath is null)
                return false;

            return TryGetDbAuthData(storagePath, out resultToken, out resultProfileName, out resultWallet);
        }

        private static string FindFirefoxLocalStorage()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var firefoxProfiles = Path.Combine(appData, "Mozilla", "Firefox", "Profiles");
            if (!Directory.Exists(firefoxProfiles))
                return null;

            var profiles = Directory.EnumerateDirectories(firefoxProfiles);
            foreach (var profile in profiles)
            {
                var profilStorageeDefault = Path.Combine(profile, "storage", "default");
                if (!Directory.Exists(profilStorageeDefault))
                    continue;

                var illuvidex = Directory.EnumerateDirectories(profilStorageeDefault)
                    .Where(site => site.Contains(illuvidexUrl))
                    .FirstOrDefault();

                if (illuvidex is not null
                    && Path.Combine(illuvidex, "ls", "data.sqlite") is string localStorage
                    && File.Exists(localStorage))
                {
                    return localStorage;
                }
            }

            return null;
        }

        private static bool TryGetDbAuthData(string storagePath, out string resultToken, out string resultNickname, out string resultWalletAddress)
        {
            resultToken = resultNickname = resultWalletAddress = null;

            var connectionStringToLocalStorage = $"Data Source={storagePath}";
            using var conn = new SqliteConnection(connectionStringToLocalStorage);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = GetAuthSqlQuery;

            using var reader = cmd.ExecuteReader();
            reader.Read();

            try
            {
                var value = reader.GetString("value");
                return TryParseAuthData(value, out resultToken, out resultNickname, out resultWalletAddress);
            }
            catch
            {
                return false;
            }
        }

        public static bool TryParseAuthData(string value, out string resultToken, out string resultNickname, out string resultWalletAddress)
        {
            resultToken = resultNickname = resultWalletAddress = null;

            try
            {
                if (JObject.Parse(value) is not JObject valueJson)
                    return FalseWithMessage($"Failed to parse value: {value} as JObject");

                if (valueJson?["ilvAuthSession"]?.Value<string>() is not string ilvAuthSessionValue)
                    return FalseWithMessage($"Failed to get ilvAuthSession out of {valueJson}");

                if (JObject.Parse(ilvAuthSessionValue) is not JObject ilvAuthSessionJson)
                    return FalseWithMessage($"Failed to parse value: {ilvAuthSessionValue} as JObject");

                if (ilvAuthSessionJson?["walletAddress"]?.Value<string>() is not string walletAddress)
                    return FalseWithMessage($"Failed to get walletAddress out of {ilvAuthSessionJson}");

                if (ilvAuthSessionJson?["profile"] is not JObject profile)
                    return FalseWithMessage($"Failed to get profile out of {ilvAuthSessionJson}");

                if (profile?["token"]?.Value<string>() is not string token)
                    return FalseWithMessage($"Failed to get token out of {profile}");

                if (profile?["nickname"]?.Value<string>() is not string nickname)
                    return FalseWithMessage($"Failed to get nickname out of {profile}");

                resultToken = token;
                resultNickname = nickname;
                resultWalletAddress = walletAddress;
                return true;
            }
            catch(Exception ex)
            {
                Logger.WriteLine(ex.Message);
                return false;
            }
        }

        private static bool FalseWithMessage(string message)
        {
            Logger.WriteLine(message);
            return false;
        }
    }
}
