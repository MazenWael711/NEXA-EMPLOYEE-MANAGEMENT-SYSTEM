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
    public partial class MainForm : Form
    {
        private ContextMenuStrip profileMenu;
        public MainForm()
        {
            InitializeComponent();
            CreateProfileMenu();

            picprofile.Click += ShowProfileMenu;
            lblSara.Click += ShowProfileMenu;
            lblAdmin.Click += ShowProfileMenu;
            this.Resize += (sender, e) => ResponsiveDashboard();
            btnEmployees.Click += (sender, e) => OpenChild(new EmployeesForm());
            picemployees.Click += (sender, e) => OpenChild(new EmployeesForm());
            btndepartments.Click += (sender, e) => OpenChild(new DepartmentsForm());
            picdepartments.Click += (sender, e) => OpenChild(new DepartmentsForm());

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ResponsiveDashboard();
            UpdateDashboard();
        }

        private void btnReports_Click(object sender, EventArgs e) { OpenChild(new ReportsForm()); }

        private void OpenChild(Form child)
        {
            this.Hide();
            using (child)
            {
                child.ShowDialog(this);
                bool logout = child is EmployeesForm && ((EmployeesForm)child).LogoutRequested
                    || child is ReportsForm && ((ReportsForm)child).LogoutRequested
                    || child is DepartmentsForm && ((DepartmentsForm)child).LogoutRequested
                    || child is formSearchFilter && ((formSearchFilter)child).LogoutRequested;
                if (logout) { this.Close(); return; }
            }
            if (!this.IsDisposed)
            {
                UpdateDashboard();
                this.Show();
            }
        }

        private void picreports_Click(object sender, EventArgs e) { OpenChild(new ReportsForm()); }

        private void btnsearch_Click(object sender, EventArgs e)
        {
            OpenChild(new formSearchFilter());
        }

        private void btnlogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pnlheader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblAdmin_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalEmployee_Click(object sender, EventArgs e)
        {

        }

        private void picnotification_Click(object sender, EventArgs e)
        {

        }

        private void picAverageSalary_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void picTotalDepartment_Click(object sender, EventArgs e)
        {

        }
        private void ResponsiveDashboard()
        {
            int sidebarWidth = pnlSidebar.Width;
            int gap = 20;

            int availableWidth = ClientSize.Width - sidebarWidth - 40;

            if (availableWidth < 400)
                return;

            // Header
            pnlheader.SetBounds(
                sidebarWidth, 0, availableWidth + 40, 107
            );

            if (availableWidth >= 1100)
            {
                AutoScroll = false;

                // Four statistic cards in one row
                int cardWidth = (availableWidth - gap * 3) / 4;
                int cardHeight = 143;
                int cardTop = 130;
                int startX = sidebarWidth + 20;

                pnlTotalEmployee.SetBounds(
                    startX, cardTop, cardWidth, cardHeight);

                pnlTotalDepartment.SetBounds(
                    startX + cardWidth + gap, cardTop,
                    cardWidth, cardHeight);

                pnlAverageSalary.SetBounds(
                    startX + (cardWidth + gap) * 2, cardTop,
                    cardWidth, cardHeight);

                pnlNewEmployee.SetBounds(
                    startX + (cardWidth + gap) * 3, cardTop,
                    cardWidth, cardHeight);

                // Two charts side by side
                int chartTop = 310;
                int chartHeight = Math.Max(250, ClientSize.Height - chartTop - 30);
                int chartWidth = (availableWidth - gap) / 2;

                PnlDepartmentChart.SetBounds(
                    startX, chartTop, chartWidth, chartHeight);

                pnlEmployeeDistribution.SetBounds(
                    startX + chartWidth + gap, chartTop,
                    chartWidth, chartHeight);
            }
            else
            {
                // Smaller window: two cards per row
                AutoScroll = true;

                int cardWidth = (availableWidth - gap) / 2;
                int cardHeight = 143;
                int startX = sidebarWidth + 20;

                pnlTotalEmployee.SetBounds(
                    startX, 130, cardWidth, cardHeight);

                pnlTotalDepartment.SetBounds(
                    startX + cardWidth + gap, 130,
                    cardWidth, cardHeight);

                pnlAverageSalary.SetBounds(
                    startX, 290, cardWidth, cardHeight);

                pnlNewEmployee.SetBounds(
                    startX + cardWidth + gap, 290,
                    cardWidth, cardHeight);

                // Charts stacked vertically
                int chartTop = 460;
                int chartWidth = availableWidth;
                int chartHeight = 320;

                PnlDepartmentChart.SetBounds(
                    startX, chartTop, chartWidth, chartHeight);

                pnlEmployeeDistribution.SetBounds(
                    startX, chartTop + chartHeight + gap,
                    chartWidth, chartHeight);
            }

            // Resize charts inside their panels
            chartEmployees.SetBounds(
                10, 56,
                PnlDepartmentChart.Width - 20,
                PnlDepartmentChart.Height - 66
            );

            chart1.SetBounds(
                12, 41,
                pnlEmployeeDistribution.Width - 24,
                pnlEmployeeDistribution.Height - 53
            );
        }
        private void UpdateDashboard()
        {
            var employees = EmployeeData.Employees;

            // Total Employees
            lblemployeenumber.Text = employees.Count.ToString();

            // Get all departments from Employees + Departments Page
            var allDepartments = employees
                .Select(x => x.Department)
                .Concat(DepartmentData.Departments.Select(x => x.Name))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Total Departments
            lblTotalDepartmentNumber.Text =
                allDepartments.Count.ToString();

            // Average Salary
            decimal averageSalary = employees.Count > 0
                ? employees.Average(x => x.Salary)
                : 0;

            lblAverageSalaryNumber.Text =
                averageSalary.ToString("N0");

            // New Employees (last 30 days)
            int newEmployees = employees.Count(x =>
                x.HireDate >= DateTime.Today.AddDays(-30));

            lblNewEmployeeNumber.Text =
                newEmployees.ToString();

            // Bar Chart: Employees per Department
            chartEmployees.Series.Clear();

            var barSeries =
                new System.Windows.Forms.DataVisualization.Charting.Series("Employees")
                {
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column,
                    IsValueShownAsLabel = true
                };

            foreach (var department in allDepartments)
            {
                int employeeCount = employees.Count(x =>
                    string.Equals(
                        x.Department,
                        department,
                        StringComparison.OrdinalIgnoreCase
                    ));

                barSeries.Points.AddXY(department, employeeCount);
            }

            chartEmployees.Series.Add(barSeries);

            // Doughnut Chart: Employee Distribution
            chart1.Series.Clear();

            var doughnutSeries =
                new System.Windows.Forms.DataVisualization.Charting.Series("Distribution")
                {
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut,
                    IsValueShownAsLabel = true,
                    Label = "#VALX",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    LabelForeColor = Color.White,
                    IsVisibleInLegend = true,
                    LegendText = "#VALX"
                };

            doughnutSeries["DoughnutLabelStyle"] = "Inside";

            foreach (var department in allDepartments)
            {
                int employeeCount = employees.Count(x =>
                    string.Equals(
                        x.Department,
                        department,
                        StringComparison.OrdinalIgnoreCase
                    ));

                // Only show departments that have employees
                if (employeeCount > 0)
                {
                    doughnutSeries.Points.AddXY(
                        department,
                        employeeCount
                    );
                }
            }

            chart1.Series.Add(doughnutSeries);
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
