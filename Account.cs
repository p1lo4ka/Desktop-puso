using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace desktoppusan
{
    public class Account
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public int HighScore { get; set; }
        public DateTime LastPlayed { get; set; }
        public int CurrentSkinIndex { get; set; } = 0;   // Индекс выбранного скина
        public int UnlockedSkins { get; set; } = 1;      // Сколько скинов открыто (1 = только первый)
    }

    public static class AccountManager
    {
        private static readonly string filePath = "users.json";
        private static List<Account> accounts = new List<Account>();
        public static Account? CurrentUser { get; private set; }

        static AccountManager()
        {
            LoadAccounts();
        }

        private static void LoadAccounts()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
            }
        }

        private static void SaveAccounts()
        {
            string json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static bool Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;
                
            if (accounts.Exists(a => a.Username == username))
                return false;

            accounts.Add(new Account
            {
                Username = username,
                Password = password,
                HighScore = 0,
                LastPlayed = DateTime.Now,
                CurrentSkinIndex = 0,
                UnlockedSkins = 1
            });
            SaveAccounts();
            return true;
        }

        public static bool Login(string username, string password)
        {
            var account = accounts.Find(a => a.Username == username && a.Password == password);
            if (account != null)
            {
                CurrentUser = account;
                return true;
            }
            return false;
        }

        public static void UpdateScore(int newScore)
        {
            if (CurrentUser != null && newScore > CurrentUser.HighScore)
            {
                CurrentUser.HighScore = newScore;
                CurrentUser.LastPlayed = DateTime.Now;
                SaveAccounts();
            }
        }

        public static void UpdateSkin(int skinIndex)
        {
            if (CurrentUser != null)
            {
                CurrentUser.CurrentSkinIndex = skinIndex;
                SaveAccounts();
            }
        }

        public static void UnlockSkin(int skinIndex)
        {
            if (CurrentUser != null && skinIndex >= CurrentUser.UnlockedSkins)
            {
                CurrentUser.UnlockedSkins = skinIndex + 1;
                SaveAccounts();
            }
        }

        public static bool IsSkinUnlocked(int skinIndex)
        {
            if (CurrentUser == null) return skinIndex == 0;
            return skinIndex < CurrentUser.UnlockedSkins;
        }

        public static List<Account> GetTopScores(int top = 10)
        {
            return accounts.OrderByDescending(a => a.HighScore).Take(top).ToList();
        }
    }
}