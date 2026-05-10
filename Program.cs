using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace desktoppusan
{
    internal static class Program
    {
        private static Random random = new Random();
        private static int currentScore = 0;
        private static Label scoreLabel = null!;
        private static PictureBox pusoPicture = null!;
        private static Form form = null!;
        private static Button resetBtn = null!;
        private static Button scoreboardBtn = null!;
        private static Button skinBtn = null!;

        public static int CurrentScore
        {
            get => currentScore;
            private set
            {
                currentScore = value;
                if (scoreLabel != null)
                {
                    int record = AccountManager.CurrentUser?.HighScore ?? 0;
                    scoreLabel.Text = $"Очки: {currentScore} | Рекорд: {record}";
                }
            }
        }

        public static bool SpendScore(int amount)
        {
            if (CurrentScore >= amount)
            {
                CurrentScore -= amount;
                return true;
            }
            return false;
        }

        public static void ResetGameScore()
        {
            CurrentScore = 0;
            if (scoreLabel != null)
                scoreLabel.ForeColor = Color.Gold;
            if (pusoPicture != null && form != null)
                pusoPicture.Location = new Point((form.ClientSize.Width - 250) / 2, 100);
        }

        private static void CreateShortcutOnDesktop()
        {
            try
            {
                string exePath = Application.ExecutablePath;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string shortcutPath = Path.Combine(desktop, "Puso Clicker.lnk");

                if (File.Exists(shortcutPath)) return;

                string psScript = $@"
$WScriptShell = New-Object -ComObject WScript.Shell
$Shortcut = $WScriptShell.CreateShortcut('{shortcutPath}')
$Shortcut.TargetPath = '{exePath}'
$Shortcut.WorkingDirectory = '{Path.GetDirectoryName(exePath)}'
$Shortcut.Description = 'House of Puso Clicker'
$Shortcut.Save()
";

                System.Diagnostics.Process.Start("powershell.exe", $"-Command \"{psScript}\"");
            }
            catch { }
        }

        [STAThread]
        private static void Main()
        {
            SkinSelector.GetCurrentScoreCallback = () => CurrentScore;
            SkinSelector.SpendScoreCallback = (amount) => SpendScore(amount);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!LoginForm.ShowLoginDialog())
                return;

            CreateShortcutOnDesktop();

            StartGame();
            Application.Run(form);
        }

        private static void StartGame()
        {
            form = new Form
            {
                Text = $"House of Puso Clicker - {AccountManager.CurrentUser!.Username}",
                Size = new Size(500, 600),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(30, 30, 40)
            };

            scoreLabel = new Label
            {
                Text = $"Очки: {CurrentScore} | Рекорд: {AccountManager.CurrentUser.HighScore}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Gold,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };
            form.Controls.Add(scoreLabel);

            pusoPicture = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(250, 250),
                Location = new Point((form.ClientSize.Width - 250) / 2, 100),
                Cursor = Cursors.Hand
            };

            int savedSkinIndex = AccountManager.CurrentUser?.CurrentSkinIndex ?? 0;
            string skinFile = SkinSelector.SkinFiles[savedSkinIndex];

            try
            {
                if (File.Exists(skinFile))
                    pusoPicture.Image = Image.FromFile(skinFile);
                else if (File.Exists("puso.png"))
                    pusoPicture.Image = Image.FromFile("puso.png");
                else
                    pusoPicture.BackColor = Color.LightGray;
            }
            catch
            {
                pusoPicture.BackColor = Color.LightGray;
            }

            pusoPicture.Click += Puso_Click;
            form.Controls.Add(pusoPicture);

            resetBtn = new Button();
            resetBtn.Text = "🔄 Сбросить счёт";
            resetBtn.Font = new Font("Segoe UI", 11);
            resetBtn.BackColor = Color.DimGray;
            resetBtn.ForeColor = Color.White;
            resetBtn.FlatStyle = FlatStyle.Flat;
            resetBtn.Size = new Size(180, 45);
            resetBtn.Location = new Point(50, 430);
            resetBtn.Click += (s, e) => ResetGameScore();
            form.Controls.Add(resetBtn);

            scoreboardBtn = new Button();
            scoreboardBtn.Text = "🏆 Таблица рекордов";
            scoreboardBtn.Font = new Font("Segoe UI", 11);
            scoreboardBtn.BackColor = Color.FromArgb(70, 70, 90);
            scoreboardBtn.ForeColor = Color.White;
            scoreboardBtn.FlatStyle = FlatStyle.Flat;
            scoreboardBtn.Size = new Size(180, 45);
            scoreboardBtn.Location = new Point(260, 430);
            scoreboardBtn.Click += (s, e) => LoginForm.ShowScoreboard();
            form.Controls.Add(scoreboardBtn);

            skinBtn = new Button();
            skinBtn.Text = "🎨 Сменить скин";
            skinBtn.Font = new Font("Segoe UI", 11);
            skinBtn.BackColor = Color.FromArgb(70, 70, 90);
            skinBtn.ForeColor = Color.White;
            skinBtn.FlatStyle = FlatStyle.Flat;
            skinBtn.Size = new Size(180, 45);
            skinBtn.Location = new Point(150, 490);
            skinBtn.Click += (s, e) => SkinSelector.ShowSkinDialog(pusoPicture);
            form.Controls.Add(skinBtn);

            form.Resize += (s, e) =>
            {
                if (pusoPicture != null && form != null)
                    pusoPicture.Location = new Point((form.ClientSize.Width - 250) / 2, 100);
            };
        }

        private static void Puso_Click(object? sender, EventArgs e)
        {
            CurrentScore++;

            pusoPicture.Size = new Size(225, 225);
            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += (ts, te) =>
            {
                pusoPicture.Size = new Size(250, 250);
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();

            if (CurrentScore % 10 == 0 && CurrentScore > 0 && form != null)
            {
                int maxX = form.ClientSize.Width - pusoPicture.Width;
                int maxY = form.ClientSize.Height - pusoPicture.Height - 150;
                int newX = random.Next(20, maxX > 20 ? maxX : 20);
                int newY = random.Next(80, maxY > 80 ? maxY : 80);
                pusoPicture.Location = new Point(newX, newY);
            }

            if (CurrentScore >= 100)
                scoreLabel.ForeColor = Color.OrangeRed;

            if (CurrentScore > AccountManager.CurrentUser!.HighScore)
            {
                AccountManager.UpdateScore(CurrentScore);
                scoreLabel.Text = $"Очки: {CurrentScore} | Рекорд: {CurrentScore} (НОВЫЙ!)";
                scoreLabel.ForeColor = Color.GreenYellow;
            }
        }
    }
}