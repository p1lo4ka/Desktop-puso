using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace desktoppusan
{
    public class ScoreEntry
    {
        public string Username { get; set; } = "";
        public int Score { get; set; }
        public DateTime Date { get; set; }
        public string DeviceId { get; set; } = "";
        public string IpHash { get; set; } = "";
    }

    public static class GlobalLeaderboard
    {
        private static List<ScoreEntry> localScores = new List<ScoreEntry>();
        private static bool isInitialized = false;

        public static void Initialize()
        {
            isInitialized = true;
            System.Diagnostics.Debug.WriteLine("Глобальная таблица инициализирована");
        }

        public static async Task<bool> SubmitScore(string username, int score)
        {
            if (!isInitialized) return false;
            
            try
            {
                if (!await AntiCheat.CheckScoreValid(username, score))
                    return false;

                // Проверяем, лучше ли новый рекорд старого для этого пользователя
                int currentBest = await GetUserBestScore(username);
                if (score <= currentBest)
                    return false;

                var entry = new ScoreEntry
                {
                    Username = username,
                    Score = score,
                    Date = DateTime.UtcNow,
                    DeviceId = AntiCheat.GetDeviceId(),
                    IpHash = AntiCheat.GetIpHash()
                };

                // Удаляем старый рекорд пользователя, если есть
                localScores.RemoveAll(e => e.Username == username);
                localScores.Add(entry);
                
                return true;
            }
            catch { return false; }
        }

        public static async Task<List<ScoreEntry>> GetTopScores(int top = 10)
        {
            try
            {
                var result = new List<ScoreEntry>(localScores);
                result.Sort((a, b) => b.Score.CompareTo(a.Score));
                
                if (result.Count > top)
                    result = result.GetRange(0, top);
                    
                return await Task.FromResult(result);
            }
            catch { return new List<ScoreEntry>(); }
        }
        
        public static async Task<int> GetUserBestScore(string username)
        {
            try
            {
                int best = 0;
                foreach (var entry in localScores)
                {
                    if (entry.Username == username && entry.Score > best)
                        best = entry.Score;
                }
                return await Task.FromResult(best);
            }
            catch { return 0; }
        }
    }
}