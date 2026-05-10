using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace desktoppusan
{
    public static class SkinSelector
    {
        // Список файлов скинов (добавляй сюда новые)
        public static string[] SkinFiles = new string[]
        {
            "puso.png",      // Индекс 0 - стандартный
            "puso2.png",     // Индекс 1
            "puso3.png"      // Индекс 2
        };
        
        public static string[] SkinNames = new string[]
        {
            "Обычный Puso",
            "Puso 2",
            "Puso 3"
        };

        public static int[] SkinPrices = new int[]
        {
            0,      // первый скин бесплатный
            50,     // второй скин стоит 50 очков
            100     // третий скин стоит 100 очков
        };

        // Ссылка на функцию получения очков (устанавливается из Program.cs)
        public static Func<int> GetCurrentScoreCallback = null!;
        
        // Ссылка на функцию траты очков (устанавливается из Program.cs)
        public static Func<int, bool> SpendScoreCallback = null!;

        public static void ShowSkinDialog(PictureBox targetPictureBox)
        {
            if (AccountManager.CurrentUser == null) return;

            Form skinForm = new Form();
            skinForm.Text = "Выбор скина";
            skinForm.Size = new Size(400, 520);
            skinForm.StartPosition = FormStartPosition.CenterScreen;
            skinForm.BackColor = Color.FromArgb(30, 30, 40);
            skinForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            skinForm.MaximizeBox = false;

            Label titleLabel = new Label();
            titleLabel.Text = "Выбери образ Puso";
            titleLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            titleLabel.ForeColor = Color.Gold;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 40;
            skinForm.Controls.Add(titleLabel);

            // Информация об очках
            Label pointsLabel = new Label();
            int currentScore = GetCurrentScoreCallback != null ? GetCurrentScoreCallback() : 0;
            pointsLabel.Text = $"Твои очки: {currentScore}";
            pointsLabel.Font = new Font("Segoe UI", 12);
            pointsLabel.ForeColor = Color.White;
            pointsLabel.TextAlign = ContentAlignment.MiddleCenter;
            pointsLabel.Location = new Point(0, 45);
            pointsLabel.Size = new Size(400, 30);
            skinForm.Controls.Add(pointsLabel);

            // Предпросмотр
            PictureBox preview = new PictureBox();
            preview.SizeMode = PictureBoxSizeMode.StretchImage;
            preview.Size = new Size(200, 200);
            preview.Location = new Point(100, 85);
            preview.BackColor = Color.FromArgb(40, 40, 50);
            skinForm.Controls.Add(preview);

            // Панель с кнопками скинов
            int yPos = 310;
            for (int i = 0; i < SkinFiles.Length; i++)
            {
                int index = i;
                bool isUnlocked = AccountManager.IsSkinUnlocked(index);
                bool isCurrent = (AccountManager.CurrentUser.CurrentSkinIndex == index);

                Button skinBtn = new Button();
                
                if (isUnlocked)
                {
                    skinBtn.Text = isCurrent ? $"{SkinNames[index]} ✓" : SkinNames[index];
                    skinBtn.BackColor = isCurrent ? Color.Gold : Color.FromArgb(60, 60, 80);
                    skinBtn.ForeColor = isCurrent ? Color.Black : Color.White;
                }
                else
                {
                    skinBtn.Text = $"{SkinNames[index]} - {SkinPrices[index]} очков 🔒";
                    skinBtn.BackColor = Color.FromArgb(40, 40, 50);
                    skinBtn.ForeColor = Color.Gray;
                }
                
                skinBtn.Location = new Point(50, yPos);
                skinBtn.Size = new Size(300, 45);
                skinBtn.FlatStyle = FlatStyle.Flat;
                skinBtn.Font = new Font("Segoe UI", 10);
                
                int capturedScore = currentScore;
                
                skinBtn.Click += (s, e) =>
                {
                    if (isUnlocked)
                    {
                        if (File.Exists(SkinFiles[index]))
                        {
                            targetPictureBox.Image = Image.FromFile(SkinFiles[index]);
                            AccountManager.UpdateSkin(index);
                            skinForm.Close();
                        }
                        else
                        {
                            MessageBox.Show($"Файл {SkinFiles[index]} не найден!", "Ошибка");
                        }
                    }
                    else
                    {
                        if (SpendScoreCallback != null && SpendScoreCallback(SkinPrices[index]))
                        {
                            AccountManager.UnlockSkin(index);
                            MessageBox.Show($"Ты купил скин {SkinNames[index]}!", "Поздравляем!");
                            skinForm.Close();
                            ShowSkinDialog(targetPictureBox);
                        }
                        else
                        {
                            int current = GetCurrentScoreCallback != null ? GetCurrentScoreCallback() : 0;
                            MessageBox.Show($"Не хватает очков! Нужно {SkinPrices[index]}, у тебя {current}", "Недостаточно очков");
                        }
                    }
                };
                
                skinForm.Controls.Add(skinBtn);
                yPos += 55;
            }

            // Загружаем предпросмотр текущего скина
            int currentSkin = AccountManager.CurrentUser.CurrentSkinIndex;
            if (currentSkin < SkinFiles.Length && File.Exists(SkinFiles[currentSkin]))
            {
                preview.Image = Image.FromFile(SkinFiles[currentSkin]);
            }

            skinForm.ShowDialog();
        }
    }
}