#pragma warning disable CS8618, CS0169, CS8602, CS8604, CS8625

using System;
using System.Drawing;
using System.Windows.Forms;

namespace desktoppusan
{
    public class ModernMenu
    {
        private Form parentForm;
        private Panel menuPanel;
        private Button toggleButton;
        private bool isMenuOpen = false;
        private int menuHeight = 200;
        
        private Button resetBtn;
        private Button scoreboardBtn;
        private Button skinBtn;
        
        private PictureBox targetPictureBox;
        private Action? resetCallback;

        public ModernMenu(Form form, PictureBox pictureBox)
        {
            this.parentForm = form;
            this.targetPictureBox = pictureBox;
            CreateMenu();
        }

        public void SetResetCallback(Action callback)
        {
            resetCallback = callback;
        }

        private void CreateMenu()
        {
            toggleButton = new Button();
            toggleButton.Text = "▼";
            toggleButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            toggleButton.Size = new Size(40, 30);
            toggleButton.Location = new Point(parentForm.ClientSize.Width - 50, 10);
            toggleButton.BackColor = Color.FromArgb(50, 50, 60);
            toggleButton.ForeColor = Color.White;
            toggleButton.FlatStyle = FlatStyle.Flat;
            toggleButton.Click += ToggleMenu;
            parentForm.Controls.Add(toggleButton);

            menuPanel = new Panel();
            menuPanel.BackColor = Color.FromArgb(40, 40, 50);
            menuPanel.Size = new Size(200, menuHeight);
            menuPanel.Location = new Point(parentForm.ClientSize.Width - 220, 45);
            menuPanel.Visible = false;
            parentForm.Controls.Add(menuPanel);

            resetBtn = new Button();
            resetBtn.Text = "🔄 Сбросить счёт";
            resetBtn.Font = new Font("Segoe UI", 10);
            resetBtn.BackColor = Color.DimGray;
            resetBtn.ForeColor = Color.White;
            resetBtn.FlatStyle = FlatStyle.Flat;
            resetBtn.Size = new Size(180, 45);
            resetBtn.Location = new Point(10, 10);
            resetBtn.Click += (s, e) => ResetScore();
            menuPanel.Controls.Add(resetBtn);

            scoreboardBtn = new Button();
            scoreboardBtn.Text = "🏆 Таблица рекордов";
            scoreboardBtn.Font = new Font("Segoe UI", 10);
            scoreboardBtn.BackColor = Color.FromArgb(70, 70, 90);
            scoreboardBtn.ForeColor = Color.White;
            scoreboardBtn.FlatStyle = FlatStyle.Flat;
            scoreboardBtn.Size = new Size(180, 45);
            scoreboardBtn.Location = new Point(10, 65);
            scoreboardBtn.Click += (s, e) => LoginForm.ShowScoreboard();
            menuPanel.Controls.Add(scoreboardBtn);

            skinBtn = new Button();
            skinBtn.Text = "🎨 Скины →";
            skinBtn.Font = new Font("Segoe UI", 10);
            skinBtn.BackColor = Color.FromArgb(70, 70, 90);
            skinBtn.ForeColor = Color.White;
            skinBtn.FlatStyle = FlatStyle.Flat;
            skinBtn.Size = new Size(180, 45);
            skinBtn.Location = new Point(10, 120);
            skinBtn.Click += (s, e) => ShowSkinCarousel();
            menuPanel.Controls.Add(skinBtn);

            parentForm.Resize += (s, e) => UpdatePositions();
        }

        private void ToggleMenu(object? sender, EventArgs e)
        {
            isMenuOpen = !isMenuOpen;
            menuPanel.Visible = isMenuOpen;
            toggleButton.Text = isMenuOpen ? "▲" : "▼";
            
            if (isMenuOpen)
            {
                menuPanel.Height = menuHeight;
                if (targetPictureBox != null)
                {
                    targetPictureBox.Visible = false;
                }
            }
            else
            {
                if (targetPictureBox != null)
                {
                    targetPictureBox.Visible = true;
                }
            }
        }

        private void ResetScore()
        {
            resetCallback?.Invoke();
        }

        private void ShowSkinCarousel()
        {
            isMenuOpen = false;
            menuPanel.Visible = false;
            toggleButton.Text = "▼";
            
            if (targetPictureBox != null)
            {
                targetPictureBox.Visible = true;
            }

            SkinCarousel.ShowCarousel(targetPictureBox, () =>
            {
                UpdateSkinButtonText();
            });
        }

        private void UpdateSkinButtonText()
        {
            int unlockedCount = 1;
            int totalCount = 1;
            
            if (AccountManager.CurrentUser != null)
            {
                unlockedCount = AccountManager.CurrentUser.UnlockedSkins;
            }
            
            if (SkinCarousel.SkinFiles != null)
            {
                totalCount = SkinCarousel.SkinFiles.Length;
            }
            
            if (skinBtn != null)
            {
                skinBtn.Text = $"🎨 Скины → ({unlockedCount}/{totalCount})";
            }
        }

        private void UpdatePositions()
        {
            if (toggleButton != null)
            {
                toggleButton.Location = new Point(parentForm.ClientSize.Width - 50, 10);
            }
            if (menuPanel != null)
            {
                menuPanel.Location = new Point(parentForm.ClientSize.Width - 220, 45);
            }
        }
    }

    public static class SkinCarousel
    {
        public static string[] SkinFiles => SkinSelector.SkinFiles;
        public static string[] SkinNames => SkinSelector.SkinNames;
        
        private static PictureBox? targetPictureBox;
        private static Form? carouselForm;
        private static int currentIndex = 0;
        private static Label? nameLabel;
        private static Label? priceLabel;
        private static Button? buyButton;
        private static Action? onCloseCallback;

        public static void ShowCarousel(PictureBox target, Action? onClose = null)
        {
            targetPictureBox = target;
            onCloseCallback = onClose;
            
            if (AccountManager.CurrentUser != null)
            {
                currentIndex = AccountManager.CurrentUser.CurrentSkinIndex;
            }
            else
            {
                currentIndex = 0;
            }

            carouselForm = new Form();
            carouselForm.Text = "Выбери скин";
            carouselForm.Size = new Size(500, 450);
            carouselForm.StartPosition = FormStartPosition.CenterScreen;
            carouselForm.BackColor = Color.FromArgb(30, 30, 40);
            carouselForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            carouselForm.MaximizeBox = false;
            carouselForm.KeyPreview = true;
            carouselForm.KeyDown += Carousel_KeyDown;

            Label titleLabel = new Label();
            titleLabel.Text = "КОЛЛЕКЦИЯ СКИНОВ";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.Gold;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 50;
            carouselForm.Controls.Add(titleLabel);

            Button leftBtn = new Button();
            leftBtn.Text = "◀";
            leftBtn.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            leftBtn.Size = new Size(50, 200);
            leftBtn.Location = new Point(20, 120);
            leftBtn.BackColor = Color.FromArgb(50, 50, 60);
            leftBtn.ForeColor = Color.White;
            leftBtn.FlatStyle = FlatStyle.Flat;
            leftBtn.Click += (s, e) => Navigate(-1);
            carouselForm.Controls.Add(leftBtn);

            PictureBox preview = new PictureBox();
            preview.SizeMode = PictureBoxSizeMode.StretchImage;
            preview.Size = new Size(250, 250);
            preview.Location = new Point(120, 80);
            preview.BackColor = Color.FromArgb(40, 40, 50);
            preview.Name = "preview";
            carouselForm.Controls.Add(preview);

            Button rightBtn = new Button();
            rightBtn.Text = "▶";
            rightBtn.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            rightBtn.Size = new Size(50, 200);
            rightBtn.Location = new Point(420, 120);
            rightBtn.BackColor = Color.FromArgb(50, 50, 60);
            rightBtn.ForeColor = Color.White;
            rightBtn.FlatStyle = FlatStyle.Flat;
            rightBtn.Click += (s, e) => Navigate(1);
            carouselForm.Controls.Add(rightBtn);

            nameLabel = new Label();
            nameLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            nameLabel.ForeColor = Color.White;
            nameLabel.TextAlign = ContentAlignment.MiddleCenter;
            nameLabel.Size = new Size(300, 30);
            nameLabel.Location = new Point(100, 350);
            carouselForm.Controls.Add(nameLabel);

            priceLabel = new Label();
            priceLabel.Font = new Font("Segoe UI", 11);
            priceLabel.ForeColor = Color.Gray;
            priceLabel.TextAlign = ContentAlignment.MiddleCenter;
            priceLabel.Size = new Size(300, 30);
            priceLabel.Location = new Point(100, 380);
            carouselForm.Controls.Add(priceLabel);

            buyButton = new Button();
            buyButton.Size = new Size(150, 40);
            buyButton.Location = new Point(175, 350);
            buyButton.FlatStyle = FlatStyle.Flat;
            buyButton.Visible = false;
            carouselForm.Controls.Add(buyButton);

            UpdatePreview(preview);
            carouselForm.ShowDialog();
        }

        private static void Navigate(int direction)
        {
            currentIndex += direction;
            if (currentIndex < 0) currentIndex = SkinFiles.Length - 1;
            if (currentIndex >= SkinFiles.Length) currentIndex = 0;
            
            if (carouselForm != null)
            {
                PictureBox? preview = carouselForm.Controls.Find("preview", true)[0] as PictureBox;
                if (preview != null)
                {
                    UpdatePreview(preview);
                }
            }
        }

        private static void UpdatePreview(PictureBox preview)
        {
            string skinFile = SkinFiles[currentIndex];
            if (System.IO.File.Exists(skinFile))
            {
                preview.Image = Image.FromFile(skinFile);
            }

            if (nameLabel != null)
            {
                nameLabel.Text = SkinNames[currentIndex];
            }
            
            bool isUnlocked = false;
            bool isCurrent = false;
            
            if (AccountManager.CurrentUser != null)
            {
                isUnlocked = AccountManager.IsSkinUnlocked(currentIndex);
                isCurrent = (AccountManager.CurrentUser.CurrentSkinIndex == currentIndex);
            }

            if (priceLabel == null || buyButton == null) return;

            if (isUnlocked)
            {
                if (isCurrent)
                {
                    priceLabel.Text = "✓ Текущий скин";
                    priceLabel.ForeColor = Color.GreenYellow;
                    buyButton.Visible = false;
                }
                else
                {
                    priceLabel.Text = "Разблокирован";
                    priceLabel.ForeColor = Color.Green;
                    buyButton.Text = "Выбрать";
                    buyButton.BackColor = Color.Gold;
                    buyButton.ForeColor = Color.Black;
                    buyButton.Visible = true;
                    buyButton.Click -= BuyButton_Click;
                    buyButton.Click += BuyButton_Click;
                }
            }
            else
            {
                int price = SkinSelector.SkinPrices[currentIndex];
                int userScore = Program.CurrentScore;
                priceLabel.Text = $"Цена: {price} очков";
                priceLabel.ForeColor = userScore >= price ? Color.Gold : Color.Red;
                buyButton.Text = userScore >= price ? "Купить" : "Не хватает очков";
                buyButton.BackColor = userScore >= price ? Color.Gold : Color.DimGray;
                buyButton.ForeColor = Color.Black;
                buyButton.Visible = true;
                buyButton.Click -= BuyButton_Click;
                buyButton.Click += BuyButton_Click;
            }
        }

        private static void BuyButton_Click(object? sender, EventArgs e)
        {
            bool isUnlocked = false;
            
            if (AccountManager.CurrentUser != null)
            {
                isUnlocked = AccountManager.IsSkinUnlocked(currentIndex);
            }
            
            if (isUnlocked)
            {
                if (System.IO.File.Exists(SkinFiles[currentIndex]) && targetPictureBox != null)
                {
                    targetPictureBox.Image = Image.FromFile(SkinFiles[currentIndex]);
                    AccountManager.UpdateSkin(currentIndex);
                    MessageBox.Show($"Скин {SkinNames[currentIndex]} выбран!", "Успех");
                    carouselForm?.Close();
                    onCloseCallback?.Invoke();
                }
            }
            else
            {
                int price = SkinSelector.SkinPrices[currentIndex];
                if (SkinSelector.SpendScoreCallback != null && SkinSelector.SpendScoreCallback(price))
                {
                    AccountManager.UnlockSkin(currentIndex);
                    MessageBox.Show($"Ты купил скин {SkinNames[currentIndex]}!", "Поздравляем!");
                    if (carouselForm != null)
                    {
                        PictureBox? preview = carouselForm.Controls.Find("preview", true)[0] as PictureBox;
                        if (preview != null)
                        {
                            UpdatePreview(preview);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Не хватает очков! Нужно {price}", "Ошибка");
                }
            }
        }

        private static void Carousel_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                Navigate(-1);
            }
            else if (e.KeyCode == Keys.Right)
            {
                Navigate(1);
            }
            else if (e.KeyCode == Keys.Enter && buyButton != null && buyButton.Visible)
            {
                BuyButton_Click(null, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                carouselForm?.Close();
            }
        }
    }
}

#pragma warning restore CS8618, CS0169, CS8602, CS8604, CS8625