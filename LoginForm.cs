using System;
using System.Drawing;
using System.Windows.Forms;

namespace NEXA
{
    public partial class LoginForm : Form
    {
        private bool passwordVisible;
        private Button btnPasswordVisibility;

        public LoginForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 650);
            WindowState = FormWindowState.Maximized;
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { btnLogin.PerformClick(); e.SuppressKeyPress = true; } };
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { txtPassword.Focus(); e.SuppressKeyPress = true; } };
            btnPasswordVisibility = new Button();
            btnPasswordVisibility.Text = "Show";
            btnPasswordVisibility.FlatStyle = FlatStyle.Flat;
            btnPasswordVisibility.FlatAppearance.BorderSize = 0;
            btnPasswordVisibility.BackColor = Color.White;
            btnPasswordVisibility.ForeColor = Color.FromArgb(35, 78, 112);
            btnPasswordVisibility.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnPasswordVisibility.Size = new Size(48, 28);
            txtPassword.Width = 315;
            btnPasswordVisibility.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPasswordVisibility.Location = new Point(panel1.ClientSize.Width - 55, 7);
            btnPasswordVisibility.Click += (s, e) => TogglePassword();
            panel1.Controls.Add(btnPasswordVisibility);
            btnPasswordVisibility.BringToFront();
            Resize += (s, e) => LayoutLogin();
            LayoutLogin();
        }

        private void TogglePassword()
        {
            passwordVisible = !passwordVisible;
            txtPassword.UseSystemPasswordChar = !passwordVisible;
            btnPasswordVisibility.Text = passwordVisible ? "Hide" : "Show";
            txtPassword.Focus();
        }

        private void LayoutLogin()
        {
            int leftWidth = Math.Max(360, (int)(ClientSize.Width * 0.40));
            panel_Nexa.SetBounds(0, 0, leftWidth, ClientSize.Height);
            pnlLogin.SetBounds(leftWidth, 0, ClientSize.Width - leftWidth, ClientSize.Height);
            pnlLoginCard.Left = Math.Max(15, (pnlLogin.ClientSize.Width - pnlLoginCard.Width) / 2);
            pnlLoginCard.Top = Math.Max(15, (pnlLogin.ClientSize.Height - pnlLoginCard.Height) / 2);
            pictureBox6.Left = (panel_Nexa.Width - pictureBox6.Width) / 2;
            lblDescription.Left = (panel_Nexa.Width - lblDescription.Width) / 2;
            lblSlogan.Left = (panel_Nexa.Width - lblSlogan.Width) / 2;
            pnlDivider.Left = (panel_Nexa.Width - pnlDivider.Width) / 2;
            btnPasswordVisibility.Left = panel1.ClientSize.Width - btnPasswordVisibility.Width - 8;
        }

        private void lblSlogan_Click(object sender, EventArgs e) { }
        private void lblSubtitle_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void pictureBoxEye_Click(object sender, EventArgs e) { TogglePassword(); }
        private void btnShowPass_Click(object sender, EventArgs e) { TogglePassword(); }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both your username and password.", "Login required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (string.IsNullOrWhiteSpace(txtUsername.Text)) txtUsername.Focus(); else txtPassword.Focus();
                return;
            }

            Hide();
            using (MainForm dashboard = new MainForm()) dashboard.ShowDialog();
            if (!IsDisposed) { txtPassword.Clear(); Show(); }
        }

        private void LoginForm_Load(object sender, EventArgs e) { LayoutLogin(); }
    }
}
