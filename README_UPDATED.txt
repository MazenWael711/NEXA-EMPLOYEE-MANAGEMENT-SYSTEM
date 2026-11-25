NEXA - Updated Windows Forms Project

Open NEXA.sln in Visual Studio on Windows.
Target framework: .NET Framework 4.7.2.

Changes in this update:
- Search & Filter now loads the shared employee list and filters by employee name, ID, email, and department.
- Clear resets all search filters. No Sort button or Sort control was added.
- Search & Filter profile avatar/name/role opens a profile menu with My Profile and Logout.
- Reports & Statistics now calculates employee count, department count, and average salary from shared application data, and updates both charts from the same data.
- Dashboard data refreshes after returning from the employee, department, search, and reports pages.
- Login screen has a Show/Hide password control, Enter-key navigation, and a responsive split layout.
- Main pages open maximized and the Search, Reports, and Departments layouts resize with the window.

Notes:
- Employee and department data are currently held in static in-memory lists, matching the existing project architecture. They remain shared across pages while the application is running, but are not persisted after closing the app.
- This environment does not have the Windows .NET Framework build tools, so the project could not be compiled or run here. Please open the solution in Visual Studio and Build Solution once before presenting it.
