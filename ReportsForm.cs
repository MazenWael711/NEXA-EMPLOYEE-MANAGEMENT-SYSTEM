using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NEXA
{
    public partial class ReportsForm : Form
    {
        private ContextMenuStrip profileMenu;
        private Panel reportBody;
        private Label pageTitle, pageSubtitle, departmentTitle, hiringTitle, salaryTitle, reportNote;
        private DataGridView departmentGrid;
        private Chart hiringChart, salaryBandChart;
        private Button btnExport;
        public bool LogoutRequested { get; private set; }

        private readonly Color navy = Color.FromArgb(6, 40, 62);
        private readonly Color blue = Color.FromArgb(35, 105, 145);
        private readonly Color muted = Color.FromArgb(105, 119, 132);

        public ReportsForm()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;

            // The reports page is an analytical workspace, not a second dashboard.
            HideOldDashboardWidgets();
            BuildReportWorkspace();
            CreateProfileMenu();

            picprofile.Click += ShowProfileMenu;
            lblSara.Click += ShowProfileMenu;
            lblAdmin.Click += ShowProfileMenu;
            label1.Click += ShowProfileMenu;
            btnDashboard.Click += (s, e) => NavigateTo(new MainForm());
            picdashboard.Click += (s, e) => NavigateTo(new MainForm());
            btnEmployees.Click += (s, e) => NavigateTo(new EmployeesForm());
            picemployees.Click += (s, e) => NavigateTo(new EmployeesForm());
            btndepartments.Click += (s, e) => NavigateTo(new DepartmentsForm());
            picdepartments.Click += (s, e) => NavigateTo(new DepartmentsForm());
            btnsearch.Click += (s, e) => NavigateTo(new formSearchFilter());
            picsearch.Click += (s, e) => NavigateTo(new formSearchFilter());
            piclogout.Click += (s, e) => Logout();

            Resize += (s, e) => LayoutReportWorkspace();
            RefreshReports();
            LayoutReportWorkspace();
        }

        private void HideOldDashboardWidgets()
        {
            pnlTotalEmployees.Visible = false;
            pnlTotalDepartments.Visible = false;
            pnlAverageSalary.Visible = false;
            chartSalary.Visible = false;
            chartEmployees.Visible = false;
            lblSalaryChartTitle.Visible = false;
            lblEmployeesChartTitle.Visible = false;
        }

        private void BuildReportWorkspace()
        {
            reportBody = new Panel { BackColor = Color.FromArgb(245, 247, 250), Location = new Point(pnlSidebar.Width, 107), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            Controls.Add(reportBody);
            reportBody.BringToFront();
            pnlSidebar.BringToFront();
            pnlheader.BringToFront();

            pageTitle = new Label { Text = "Workforce Reports", Font = new Font("Segoe UI", 21, FontStyle.Bold), ForeColor = navy, AutoSize = true };
            pageSubtitle = new Label { Text = "Department-level payroll and salary analysis based on current employee records", Font = new Font("Segoe UI", 9.5f), ForeColor = muted, AutoSize = true };
            btnExport = new Button { Text = "Export Department Report", Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = navy, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(205, 38), Cursor = Cursors.Hand };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += ExportDepartmentReport;

            departmentTitle = SectionTitle("Department Performance");
            departmentGrid = new DataGridView
            {
                BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AllowUserToAddRows = false,
                AllowUserToDeleteRows = false, AllowUserToResizeRows = false, ReadOnly = true,
                RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
                EnableHeadersVisualStyles = false, GridColor = Color.FromArgb(232, 236, 240),
                Font = new Font("Segoe UI", 9), ColumnHeadersHeight = 40, RowTemplate = { Height = 34 }
            };
            departmentGrid.ColumnHeadersDefaultCellStyle.BackColor = navy;
            departmentGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            departmentGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            departmentGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 237, 245);
            departmentGrid.DefaultCellStyle.SelectionForeColor = navy;
            departmentGrid.Columns.Add("Department", "Department");
            departmentGrid.Columns.Add("Employees", "Employees");
            departmentGrid.Columns.Add("AverageSalary", "Average Salary");
            departmentGrid.Columns.Add("Payroll", "Monthly Payroll");
            departmentGrid.Columns.Add("Share", "Headcount Share");

            hiringTitle = SectionTitle("Monthly Payroll by Department");
            hiringChart = CreateReportChart();
            salaryTitle = SectionTitle("Average Salary by Department");
            salaryBandChart = CreateReportChart();
            reportNote = new Label { Font = new Font("Segoe UI", 8.5f), ForeColor = muted, AutoSize = true };

            reportBody.Controls.AddRange(new Control[] { pageTitle, pageSubtitle, btnExport, departmentTitle, departmentGrid, hiringTitle, hiringChart, salaryTitle, salaryBandChart, reportNote });
        }

        private Label SectionTitle(string text)
        {
            return new Label { Text = text, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = navy, AutoSize = true };
        }

        private Chart CreateReportChart()
        {
            Chart c = new Chart { BackColor = Color.White, BorderlineColor = Color.FromArgb(230, 234, 238), BorderlineDashStyle = ChartDashStyle.Solid, BorderlineWidth = 1 };
            ChartArea area = new ChartArea("Main");
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.Interval = 1;
            area.AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            area.AxisX.LineColor = Color.Gainsboro;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 238, 241);
            area.AxisY.LineColor = Color.Transparent;
            area.AxisY.LabelStyle.ForeColor = muted;
            area.AxisX.LabelStyle.ForeColor = muted;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8);
            c.ChartAreas.Add(area);
            c.Legends.Add(new Legend("Legend") { Docking = Docking.Bottom, Font = new Font("Segoe UI", 8), BackColor = Color.White });
            return c;
        }

        private void RefreshReports()
        {
            var employees = EmployeeData.Employees.ToList();
            int total = employees.Count;
            departmentGrid.Rows.Clear();

            var departmentNames = employees.Select(e => e.Department)
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(d => d);
            foreach (string dept in departmentNames)
            {
                var group = employees.Where(e => string.Equals(e.Department, dept, StringComparison.OrdinalIgnoreCase)).ToList();
                decimal payroll = group.Sum(e => e.Salary);
                departmentGrid.Rows.Add(dept, group.Count, "EGP " + group.Average(e => e.Salary).ToString("N0"), "EGP " + payroll.ToString("N0"),
                    total == 0 ? "0%" : (group.Count * 100.0 / total).ToString("N1") + "%");
            }
            if (departmentGrid.Rows.Count == 0)
                departmentGrid.Rows.Add("No department data", 0, "EGP 0", "EGP 0", "0%");

            // Chart 1: monthly payroll by department.
            // This is more informative than a hiring-by-year line chart when all
            // sample employees were added in the same year.
            hiringChart.Series.Clear();
            hiringChart.Legends[0].Enabled = false;
            hiringChart.ChartAreas[0].AxisY.Minimum = 0;
            hiringChart.ChartAreas[0].AxisY.Title = "Monthly payroll (EGP)";
            hiringChart.ChartAreas[0].AxisX.Title = "Department";
            hiringChart.ChartAreas[0].AxisY.LabelStyle.Format = "N0";

            Series payrollSeries = new Series("Monthly Payroll")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                LabelFormat = "N0",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Color = blue,
                ChartArea = "Main"
            };

            var departmentGroups = employees
                .Where(emp => !string.IsNullOrWhiteSpace(emp.Department))
                .GroupBy(emp => emp.Department)
                .OrderBy(group => group.Key)
                .ToList();

            foreach (var group in departmentGroups)
                payrollSeries.Points.AddXY(group.Key, group.Sum(emp => emp.Salary));

            if (payrollSeries.Points.Count == 0)
                payrollSeries.Points.AddXY("No data", 0);

            hiringChart.Series.Add(payrollSeries);

            // Chart 2: average salary by department.
            salaryBandChart.Series.Clear();
            salaryBandChart.Legends[0].Enabled = false;
            salaryBandChart.ChartAreas[0].AxisX.Minimum = 0;
            salaryBandChart.ChartAreas[0].AxisX.Title = "Average salary (EGP)";
            salaryBandChart.ChartAreas[0].AxisX.LabelStyle.Format = "N0";
            salaryBandChart.ChartAreas[0].AxisY.Title = "Department";

            Series averageSalarySeries = new Series("Average Salary")
            {
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true,
                LabelFormat = "N0",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Color = Color.FromArgb(70, 145, 190),
                ChartArea = "Main"
            };

            foreach (var group in departmentGroups)
                averageSalarySeries.Points.AddXY(group.Key, group.Average(emp => emp.Salary));

            if (averageSalarySeries.Points.Count == 0)
                averageSalarySeries.Points.AddXY("No data", 0);

            salaryBandChart.Series.Add(averageSalarySeries);
            salaryBandChart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            salaryBandChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(235, 238, 241);

            reportNote.Text = "Report generated: " + DateTime.Now.ToString("dd MMM yyyy, hh:mm tt") + "   |   Figures are calculated from the current employee records.";
        }

        private void LayoutReportWorkspace()
        {
            if (reportBody == null) return;
            int left = pnlSidebar.Width;
            int bodyWidth = Math.Max(500, ClientSize.Width - left);
            reportBody.SetBounds(left, 107, bodyWidth, Math.Max(400, ClientSize.Height - 107));
            int pad = 24, gap = 18, inner = bodyWidth - pad * 2;
            pageTitle.SetBounds(pad, 20, inner, 34);
            pageSubtitle.SetBounds(pad + 2, 58, inner - 220, 24);
            btnExport.SetBounds(Math.Max(pad, inner + pad - btnExport.Width), 24, btnExport.Width, btnExport.Height);

            int tableTop = 104;
            departmentTitle.SetBounds(pad, tableTop, inner, 25);
            int tableHeight = Math.Max(150, Math.Min(235, (int)(ClientSize.Height * 0.25)));
            departmentGrid.SetBounds(pad, tableTop + 30, inner, tableHeight);

            int chartsTop = tableTop + 30 + tableHeight + 24;
            int chartAreaWidth = (inner - gap) / 2;
            int chartHeight = Math.Max(190, reportBody.Height - chartsTop - 60);
            if (inner < 760)
            {
                chartAreaWidth = inner;
                chartHeight = 230;
            }
            hiringTitle.SetBounds(pad, chartsTop, chartAreaWidth, 24);
            hiringChart.SetBounds(pad, chartsTop + 28, chartAreaWidth, chartHeight);
            salaryTitle.SetBounds(pad + chartAreaWidth + gap, chartsTop, chartAreaWidth, 24);
            salaryBandChart.SetBounds(pad + chartAreaWidth + gap, chartsTop + 28, chartAreaWidth, chartHeight);
            if (inner < 760)
            {
                salaryTitle.SetBounds(pad, chartsTop + chartHeight + 38, inner, 24);
                salaryBandChart.SetBounds(pad, chartsTop + chartHeight + 66, inner, 220);
            }
            reportNote.SetBounds(pad, reportBody.Height - 27, inner, 20);
        }

        private void ExportDepartmentReport(object sender, EventArgs args)
        {
            using (SaveFileDialog dialog = new SaveFileDialog { Filter = "CSV file (*.csv)|*.csv", FileName = "NEXA_Department_Report.csv", Title = "Export Department Report" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                var employees = EmployeeData.Employees.ToList();
                var depts = employees.Where(e => !string.IsNullOrWhiteSpace(e.Department)).GroupBy(e => e.Department).OrderBy(g => g.Key);
                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Department,Employees,Average Salary,Monthly Payroll,Headcount Share");
                foreach (var g in depts)
                {
                    csv.AppendLine(string.Join(",", new[]
                    {
                        Csv(g.Key), g.Count().ToString(CultureInfo.InvariantCulture),
                        g.Average(e => e.Salary).ToString("0.00", CultureInfo.InvariantCulture),
                        g.Sum(e => e.Salary).ToString("0.00", CultureInfo.InvariantCulture),
                        (employees.Count == 0 ? 0 : g.Count() * 100.0 / employees.Count).ToString("0.0", CultureInfo.InvariantCulture) + "%"
                    }));
                }
                File.WriteAllText(dialog.FileName, csv.ToString(), new UTF8Encoding(true));
                MessageBox.Show("Department report exported successfully.", "NEXA Reports", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string Csv(string value) { return "\"" + (value ?? "").Replace("\"", "\"\"") + "\""; }

        private void CreateProfileMenu()
        {
            profileMenu = new ContextMenuStrip();
            ToolStripMenuItem profile = new ToolStripMenuItem("My Profile");
            profile.Click += (s, e) => MessageBox.Show("Name: " + lblSara.Text + "\nRole: " + lblAdmin.Text, "My Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ToolStripMenuItem logout = new ToolStripMenuItem("Logout");
            logout.Click += (s, e) => Logout();
            profileMenu.Items.Add(profile);
            profileMenu.Items.Add(new ToolStripSeparator());
            profileMenu.Items.Add(logout);
        }

        private void ShowProfileMenu(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if (c != null) profileMenu.Show(c, new Point(0, c.Height));
        }

        private void NavigateTo(Form next)
        {
            Hide();
            using (next) next.ShowDialog(this);
            bool logout = next is EmployeesForm && ((EmployeesForm)next).LogoutRequested ||
                next is DepartmentsForm && ((DepartmentsForm)next).LogoutRequested ||
                next is formSearchFilter && ((formSearchFilter)next).LogoutRequested ||
                next is ReportsForm && ((ReportsForm)next).LogoutRequested;
            if (logout) { Logout(); return; }
            if (!IsDisposed) { RefreshReports(); LayoutReportWorkspace(); Show(); }
        }

        private void Logout() { LogoutRequested = true; Close(); }
        private void MainForm_Load(object sender, EventArgs e) { RefreshReports(); }
        private void btnReports_Click(object sender, EventArgs e) { RefreshReports(); }
        private void picreports_Click(object sender, EventArgs e) { RefreshReports(); }
        private void btnlogout_Click(object sender, EventArgs e) { Logout(); }
        private void pnlheader_Paint(object sender, PaintEventArgs e) { }
        private void lblAdmin_Click(object sender, EventArgs e) { ShowProfileMenu(sender, e); }
        private void lblSalaryChartTitle_Click(object sender, EventArgs e) { }
        private void lblEmployeesChartTitle_Click(object sender, EventArgs e) { }
    }
}
