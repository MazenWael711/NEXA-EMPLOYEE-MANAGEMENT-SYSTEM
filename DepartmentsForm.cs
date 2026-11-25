using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NEXA.EmployeeData;

namespace NEXA
{
    public partial class DepartmentsForm : Form
    {
        public DepartmentsForm()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 700);
            CreateProfileMenu();

            LoadDepartments();
            Resize += (s, e) => ResponsiveDepartments();
            ResponsiveDepartments();

            btnAddDepartment.Click += (s, e) => AddDepartment();
            btnClear.Click += (s, e) => ClearDepartmentFields();
            button3.Click += (s, e) => SearchDepartments();

            picAvatar.Click += ShowProfileMenu;
            picUserArrow.Click += ShowProfileMenu;
            lblUserName.Click += ShowProfileMenu;
            lblUserRole.Click += ShowProfileMenu;
            btnDashboard.Click += (sender, e) => NavigateTo(new MainForm());
            picdashboard.Click += (sender, e) => NavigateTo(new MainForm());
            btnEmployees.Click += (sender, e) => NavigateTo(new EmployeesForm());
            picemployees.Click += (sender, e) => NavigateTo(new EmployeesForm());
            btndepartments.Click += (sender, e) => NavigateTo(new DepartmentsForm());
            picdepartments.Click += (sender, e) => NavigateTo(new DepartmentsForm());

            btnsearch.Click += (sender, e) => NavigateTo(new formSearchFilter());
            picsearch.Click += (sender, e) => NavigateTo(new formSearchFilter());
            btnlogout.Click += (sender, e) => { LogoutRequested = true; this.Close(); };
            piclogout.Click += (sender, e) => { LogoutRequested = true; this.Close(); };
        }

        public bool LogoutRequested { get; private set; }

        private void NavigateTo(Form next)
        {
            this.Hide();
            using (next)
            {
                next.ShowDialog(this);
                bool logout = next is EmployeesForm && ((EmployeesForm)next).LogoutRequested
                    || next is ReportsForm && ((ReportsForm)next).LogoutRequested
                    || next is DepartmentsForm && ((DepartmentsForm)next).LogoutRequested
                    || next is formSearchFilter && ((formSearchFilter)next).LogoutRequested;
                if (logout) { LogoutRequested = true; this.Close(); return; }
            }
            if (!this.IsDisposed) this.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void btnReports_Click(object sender, EventArgs e) { NavigateTo(new ReportsForm()); }

        private void picreports_Click(object sender, EventArgs e) { NavigateTo(new ReportsForm()); }

        private void btnlogout_Click(object sender, EventArgs e) { LogoutRequested = true; this.Close(); }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvDepartments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void lblHeaderTitle_Click(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void AddDepartment()
        {
            string name = txtDepartmentName.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter department name.");
                txtDepartmentName.Focus();
                return;
            }

            bool exists = DepartmentData.Departments.Any(
                d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
            );

            if (exists)
            {
                MessageBox.Show("Department already exists.");
                return;
            }

            int newId = DepartmentData.Departments.Count == 0
                ? 1
                : DepartmentData.Departments.Max(d => d.ID) + 1;

            Department department = new Department
            {
                ID = newId,
                Name = name,
                Description = description
            };

            DepartmentData.Departments.Add(department);

            dataGridView1.Rows.Add(
                department.ID,
                department.Name,
                department.Description
            );

            ClearDepartmentFields();

            MessageBox.Show("Department added successfully!");
        }
        private void ClearDepartmentFields()
        {
            txtDepartmentName.Clear();
            txtDescription.Clear();
            textBox6.Clear();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Visible = true;
            }

            txtDepartmentName.Focus();
        }
        private void SearchDepartments()
        {
            string search = textBox6.Text.Trim().ToLower();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                bool found = string.IsNullOrWhiteSpace(search);

                if (!found)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null &&
                            cell.Value.ToString().ToLower().Contains(search))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                row.Visible = found;
            }
        }
        private void LoadDepartments()
        {
            dataGridView1.Rows.Clear();

            foreach (Department department in DepartmentData.Departments)
            {
                dataGridView1.Rows.Add(
                    department.ID,
                    department.Name,
                    department.Description
                );
            }
        }
      
        private void ResponsiveDepartments()
        {
            int left = pnlSidebar.Width;
            int contentWidth = Math.Max(700, ClientSize.Width - left);
            pnlHeader.SetBounds(left, 0, contentWidth, ClientSize.Height);
            int contentTop = 106;
            int leftPanelWidth = Math.Max(320, Math.Min(390, (contentWidth - 80) / 3));
            int panelHeight = Math.Max(450, ClientSize.Height - contentTop - 20);
            pnlAddDepartment.SetBounds(20, contentTop, leftPanelWidth, panelHeight);
            panel3.SetBounds(pnlAddDepartment.Right + 20, contentTop, Math.Max(350, contentWidth - pnlAddDepartment.Right - 40), panelHeight);
            dataGridView1.SetBounds(20, 81, Math.Max(300, panel3.ClientSize.Width - 40), Math.Max(250, panel3.ClientSize.Height - 105));
        }

        private ContextMenuStrip profileMenu;

        private void CreateProfileMenu()
        {
            profileMenu = new ContextMenuStrip();

            ToolStripMenuItem myProfile =
                new ToolStripMenuItem("My Profile");

            ToolStripMenuItem logout =
                new ToolStripMenuItem("Logout");

            myProfile.Click += (sender, e) =>
            {
                MessageBox.Show(
                    "Name: " + lblUserName.Text +
                    "\nRole: " + lblUserRole.Text,
                    "My Profile",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            };

            logout.Click += (sender, e) =>
            {
                LogoutRequested = true;
                this.Close();
            };

            profileMenu.Items.Add(myProfile);
            profileMenu.Items.Add(new ToolStripSeparator());
            profileMenu.Items.Add(logout);
        }

        private void ShowProfileMenu(object sender, EventArgs e)
        {
            Control clickedControl = sender as Control;

            if (clickedControl != null && profileMenu != null)
            {
                profileMenu.Show(
                    clickedControl,
                    new Point(0, clickedControl.Height)
                );
            }
        }
    }
}
