NEXA - Updated Windows Forms Project

Open NEXA.sln in Visual Studio on Windows.
Target framework: .NET Framework 4.7.2.

Changes in this update:
- Search & Filter now loads the shared employee list and filters by employee name, ID, email, and department.
- Clear resets all search filters. No Sort button or Sort control was added.
- Search & Filter profile avatar/name/role opens a profile menu with My Profile and Logout.
- Reports & Statistics includes a custom GDI+ salary visualization drawn in a Panel Paint event.
- The GDI+ visualization uses Graphics, Pen, SolidBrush, DrawLine, DrawString, FillRectangle, and DrawRectangle to render average salary by department from the live employee list.
- The other report chart compares monthly payroll by department; both visualizations refresh from the shared employee data.
- Dashboard data refreshes after returning from the employee, department, search, and reports pages.
- Login screen has a Show/Hide password control, Enter-key navigation, and a responsive split layout.
- Main pages open maximized and the Search, Reports, and Departments layouts resize with the window.

Notes:
- Employee and department data are currently held in static in-memory lists, matching the existing project architecture. They remain shared across pages while the application is running, but are not persisted after closing the app.
- This environment does not have the Windows .NET Framework build tools, so the project could not be compiled or run here. Please open the solution in Visual Studio and Build Solution once before presenting it.

CS312 GDI+ / Graphics evidence:
- ReportsForm.cs -> DrawGdiSalaryChart(object sender, PaintEventArgs e)
- The method draws the chart with System.Drawing.Graphics in the Panel.Paint event.
- It uses anti-aliasing, Pens for grid/axis lines, SolidBrush objects for bars and text, and DrawString for department names and salary values.
