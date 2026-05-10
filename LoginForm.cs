using System;
using System.Drawing;
using System.Windows.Forms;

namespace desktoppusan
{
    public static class LoginForm
    {
        public static bool ShowLoginDialog()
        {
            Form loginForm = new Form();
            loginForm.Text = "Авторизация - House of Puso";
            loginForm.Size = new Size(350, 300);
            loginForm.StartPosition = FormStartPosition.CenterScreen;
            loginForm.BackColor = Color.FromArgb(30, 30, 40);
            loginForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            loginForm.MaximizeBox = false;

            Label titleLabel = new Label();
            titleLabel.Text = "Дом Puso";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.Gold;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 50;
            loginForm.Controls.Add(titleLabel);

            Label lblUsername = new Label();
            lblUsername.Text = "Имя пользователя:";
            lblUsername.ForeColor = Color.White;
            lblUsername.Location = new Point(50, 70);
            lblUsername.Size = new Size(250, 25);
            loginForm.Controls.Add(lblUsername);

            TextBox tbUsername = new TextBox();
            tbUsername.Location = new Point(50, 95);
            tbUsername.Size = new Size(250, 30);
            loginForm.Controls.Add(tbUsername);

            Label lblPassword = new Label();
            lblPassword.Text = "Пароль:";
            lblPassword.ForeColor = Color.White;
            lblPassword.Location = new Point(50, 135);
            lblPassword.Size = new Size(250, 25);
            loginForm.Controls.Add(lblPassword);

            TextBox tbPassword = new TextBox();
            tbPassword.Location = new Point(50, 160);
            tbPassword.Size = new Size(250, 30);
            tbPassword.PasswordChar = '*';
            loginForm.Controls.Add(tbPassword);

            Button btnLogin = new Button();
            btnLogin.Text = "Войти";
            btnLogin.Location = new Point(50, 210);
            btnLogin.Size = new Size(120, 40);
            btnLogin.BackColor = Color.Gold;
            btnLogin.FlatStyle = FlatStyle.Flat;
            loginForm.Controls.Add(btnLogin);

            Button btnRegister = new Button();
            btnRegister.Text = "Регистрация";
            btnRegister.Location = new Point(180, 210);
            btnRegister.Size = new Size(120, 40);
            btnRegister.BackColor = Color.DimGray;
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            loginForm.Controls.Add(btnRegister);

            Label lblError = new Label();
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(50, 260);
            lblError.Size = new Size(250, 30);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            loginForm.Controls.Add(lblError);

            bool success = false;

            btnLogin.Click += (s, e) =>
            {
                if (AccountManager.Login(tbUsername.Text, tbPassword.Text))
                {
                    success = true;
                    loginForm.Close();
                }
                else
                {
                    lblError.Text = "Неверное имя или пароль!";
                }
            };

            btnRegister.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tbUsername.Text) || string.IsNullOrWhiteSpace(tbPassword.Text))
                {
                    lblError.Text = "Заполните все поля!";
                }
                else if (AccountManager.Register(tbUsername.Text, tbPassword.Text))
                {
                    lblError.ForeColor = Color.Green;
                    lblError.Text = "Регистрация успешна! Теперь войдите.";
                    if (AccountManager.Login(tbUsername.Text, tbPassword.Text))
                    {
                        success = true;
                        loginForm.Close();
                    }
                }
                else
                {
                    lblError.Text = "Пользователь уже существует!";
                }
            };

            loginForm.ShowDialog();
            return success;
        }

        public static void ShowScoreboard()
        {
            Form scoreForm = new Form();
            scoreForm.Text = "Таблица рекордов";
            scoreForm.Size = new Size(400, 500);
            scoreForm.StartPosition = FormStartPosition.CenterScreen;
            scoreForm.BackColor = Color.FromArgb(30, 30, 40);

            ListBox listBox = new ListBox();
            listBox.Dock = DockStyle.Fill;
            listBox.Font = new Font("Segoe UI", 12);
            listBox.BackColor = Color.FromArgb(40, 40, 50);
            listBox.ForeColor = Color.White;

            var topScores = AccountManager.GetTopScores(15);
            for (int i = 0; i < topScores.Count; i++)
            {
                listBox.Items.Add($"{i + 1}. {topScores[i].Username} — {topScores[i].HighScore} очков");
            }

            if (topScores.Count == 0)
                listBox.Items.Add("Нет сохранённых рекордов");

            scoreForm.Controls.Add(listBox);
            scoreForm.ShowDialog();
        }
    }
}