using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NEXA
{
    public partial class formSearchFilter : Form
    {
        private ContextMenuStrip profileMenu;
        public bool LogoutRequested { get; private set; }

        public formSearchFilter()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1050, 700);
            StartPosition = FormStartPosition.CenterScreen;
            cmbSearchBy.SelectedIndex = 0;
            cmbDepartment.Items.Clear();
            cmbDepartment.Items.Add("All Departments");
            foreach (string dept in EmployeeData.Employees.Select(x => x.Department)
                .Concat(EmployeeData.DepartmentData.Departments.Select(x => x.Name))
                .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x))
                cmbDepartment.Items.Add(dept);
            cmbDepartment.SelectedIndex = 0;

            buttonSearch.Click += (s, e) => ApplyFilters();
            btnClear.Click += (s, e) => ClearFilters();
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { ApplyFilters(); e.SuppressKeyPress = true; } };
            picAvatar.Click += ShowProfileMenu;
            lblAdminName.Click += ShowProfileMenu;
            lblAdminRole.Click += ShowProfileMenu;
            arrowpicture.Click += ShowProfileMenu;
            CreateProfileMenu();
            btnDashboard.Click += (s, e) => ClosePage(); picdashboard.Click += (s, e) => ClosePage();
            btnEmployees.Click += (s, e) => NavigateTo(new EmployeesForm()); picemployees.Click += (s, e) => NavigateTo(new EmployeesForm());
            btndepartments.Click += (s, e) => NavigateTo(new DepartmentsForm()); picdepartments.Click += (s, e) => NavigateTo(new DepartmentsForm());
            piclogout.Click += (s, e) => Logout();
            LoadEmployees();
            Resize += (s, e) => LayoutPage();
            LayoutPage();
        }

        private void LoadEmployees()
        {
            dgvEmployees.Rows.Clear();
            foreach (Employee employee in EmployeeData.Employees)
                dgvEmployees.Rows.Add(employee.ID, employee.FullName, employee.Department, employee.Email, employee.Salary.ToString("N2"), employee.HireDate.ToShortDateString());
        }

        private void ApplyFilters()
        {
            string term = txtSearch.Text.Trim();
            if (term == "enter search item...") term = string.Empty;
            string department = cmbDepartment.SelectedItem == null ? "All Departments" : cmbDepartment.SelectedItem.ToString();
            string field = cmbSearchBy.SelectedItem == null ? "Employee Name" : cmbSearchBy.SelectedItem.ToString();
            foreach (DataGridViewRow row in dgvEmployees.Rows)
            {
                string value = Convert.ToString(row.Cells[field == "ID" ? 0 : field == "Email" ? 3 : 1].Value);
                string rowDepartment = Convert.ToString(row.Cells[2].Value);
                bool textMatch = string.IsNullOrWhiteSpace(term) || value.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0;
                bool deptMatch = department == "All Departments" || string.Equals(rowDepartment, department, StringComparison.OrdinalIgnoreCase);
                row.Visible = textMatch && deptMatch;
            }
        }

        private void ClearFilters()
        {
            txtSearch.Clear();
            cmbSearchBy.SelectedIndex = 0;
            cmbDepartment.SelectedIndex = 0;
            foreach (DataGridViewRow row in dgvEmployees.Rows) row.Visible = true;
            txtSearch.Focus();
        }

        private void CreateProfileMenu()
        {
            profileMenu = new ContextMenuStrip();
            ToolStripMenuItem profile = new ToolStripMenuItem("My Profile");
            profile.Click += (s, e) => MessageBox.Show("Name: " + lblAdminName.Text + "\nRole: " + lblAdminRole.Text, "My Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ToolStripMenuItem logout = new ToolStripMenuItem("Logout");
            logout.Click += (s, e) => Logout();
            profileMenu.Items.Add(profile); profileMenu.Items.Add(new ToolStripSeparator()); profileMenu.Items.Add(logout);
        }
        private void ShowProfileMenu(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control != null) profileMenu.Show(control, new Point(0, control.Height));
        }
        private void NavigateTo(Form next)
        {
            Hide();
            using (next) next.ShowDialog(this);
            if (next is EmployeesForm && ((EmployeesForm)next).LogoutRequested || next is DepartmentsForm && ((DepartmentsForm)next).LogoutRequested || next is ReportsForm && ((ReportsForm)next).LogoutRequested || next is formSearchFilter && ((formSearchFilter)next).LogoutRequested) { Logout(); return; }
            if (!IsDisposed) { LoadEmployees(); LayoutPage(); Show(); }
        }
        private void ClosePage() { Close(); }
        private void Logout() { LogoutRequested = true; Close(); }
        private void LayoutPage()
        {
            int left = pnlSidebar.Width + 25;
            pnlHeader.SetBounds(left, 0, Math.Max(500, ClientSize.Width - left), 107);
            pnlFilters.SetBounds(left + 10, 130, Math.Max(500, ClientSize.Width - left - 40), pnlFilters.Height);
            dgvEmployees.SetBounds(left + 10, pnlFilters.Bottom + 20, Math.Max(500, ClientSize.Width - left - 40), Math.Max(250, ClientSize.Height - pnlFilters.Bottom - 55));
            dgvEmployees.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }
        private void MainForm_Load(object sender, EventArgs e) { }
        private void btnReports_Click(object sender, EventArgs e) { NavigateTo(new ReportsForm()); }
        private void picreports_Click(object sender, EventArgs e) { NavigateTo(new ReportsForm()); }
        private void btnDashboard_Click(object sender, EventArgs e) { ClosePage(); }
        private void btnlogout_Click(object sender, EventArgs e) { Logout(); }
    }
}
