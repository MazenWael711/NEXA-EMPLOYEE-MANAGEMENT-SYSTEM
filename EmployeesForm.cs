using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NEXA
{
    public partial class EmployeesForm : Form
    {
        private ContextMenuStrip profileMenu;
        public EmployeesForm()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 700);
            textBox2.KeyPress += textBox2_KeyPress;
            AddEmployeeColumn();
            LoadEmployeesToGrid();
            CreateProfileMenu();

            picprofile.Click += ShowProfileMenu;
            lblSara.Click += ShowProfileMenu;
            AddEmployeeColumn();
            this.Resize += EmployeesForm_Resize;
            ResponsiveLayout();
            button1.Click += (sender, e) => AddEmployee();
            button2.Click += (sender, e) => ClearEmployeeFields();
            button3.Click += (sender, e) => SearchEmployees();
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

        private void pnlheader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblAdmin_Click(object sender, EventArgs e)
        {
            ShowProfileMenu(sender, e);
        }

        private void lblheader_Click(object sender, EventArgs e)
        {

        }

        private void picHome_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        

        private void AddEmployeeColumn()
        {
            if (!dataGridView1.Columns.Contains("Phone"))
                dataGridView1.Columns.Add("Phone", "Phone");
        }

        private void AddEmployee()
        {
            string name = textBox1.Text.Trim();
            string phone = textBox2.Text.Trim();
            string email = textBox3.Text.Trim();
            string department = textBox4.Text.Trim();
            string salaryText = textBox5.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(department) ||
                string.IsNullOrWhiteSpace(salaryText))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (!decimal.TryParse(salaryText, out decimal salary) || salary < 0)
            {
                MessageBox.Show("Enter a valid salary.");
                return;
            }
            int newId = EmployeeData.Employees.Count == 0
     ? 1
     : EmployeeData.Employees.Max(x => x.ID) + 1;
            EmployeeData.Employees.Add(new Employee
            {
                ID = newId,
                FullName = name,
                Department = department,
                Email = email,
                Salary = salary,
                HireDate = DateTime.Today,
                Phone = phone
            });
            dataGridView1.Rows.Add(
                newId,
                name,
                department,
                email,
                salary,
                DateTime.Today.ToShortDateString(),
                phone
            );

            ClearEmployeeFields();
            MessageBox.Show("Employee added successfully!");
        }

        private void ClearEmployeeFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();

            // Show all employees again
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Visible = true;
            }

            textBox1.Focus();
        }
        
        private void SearchEmployees()
        {
            string searchValue = textBox6.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(searchValue))
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Visible = true;
                }
                return;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                bool found = false;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null &&
                        cell.Value.ToString().ToLower().Contains(searchValue))
                    {
                        found = true;
                        break;
                    }
                }

                row.Visible = found;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void EmployeesForm_Resize(object sender, EventArgs e)
        {
            ResponsiveLayout();
        }

        private void ResponsiveLayout()
        {
            int left = pnlSidebar.Width + 6;
            int top = pnlheader.Bottom + 10;

            int availableWidth = ClientSize.Width - left - 15;
            int availableHeight = ClientSize.Height - top - 15;

            if (availableWidth < 400 || availableHeight < 200)
                return;

            // Make input fields responsive
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Keep buttons at the bottom
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            if (availableWidth >= 1050)
            {
                // Wide window: side-by-side layout
                AutoScroll = false;

                int gap = 20;
                int leftWidth = (int)(availableWidth * 0.34);

                leftWidth = Math.Max(380, Math.Min(leftWidth, 440));

                panel5.SetBounds(left, top, leftWidth, availableHeight);

                panel3.SetBounds(
                    left + leftWidth + gap,
                    top,
                    availableWidth - leftWidth - gap,
                    availableHeight
                );
            }
            else
            {
                // Small window: stacked layout
                AutoScroll = true;

                int panelHeight = 590;
                int gap = 15;

                panel5.SetBounds(
                    left, top, availableWidth, panelHeight
                );

                panel3.SetBounds(
                    left,
                    top + panelHeight + gap,
                    availableWidth,
                    Math.Max(350, availableHeight - panelHeight - gap)
                );
            }
        }
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
                    "Name: " + lblSara.Text +
                    "\nRole: " + lblAdmin.Text,
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

            if (clickedControl != null)
            {
                profileMenu.Show(
                    clickedControl,
                    new Point(0, clickedControl.Height)
                );
            }
        }
        private void LoadEmployeesToGrid()
        {
            dataGridView1.Rows.Clear();

            foreach (Employee employee in EmployeeData.Employees)
            {
                dataGridView1.Rows.Add(
                    employee.ID,
                    employee.FullName,
                    employee.Department,
                    employee.Email,
                    employee.Salary,
                    employee.HireDate.ToShortDateString(),
                    employee.Phone
                );
            }
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }

}
