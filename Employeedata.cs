using System;
using System.Collections.Generic;

namespace NEXA
{
    public class Employee
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string Phone { get; set; }
    }

    public static class EmployeeData
    {
        public static List<Employee> Employees =
            new List<Employee>();
        public class Department
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public static class DepartmentData
        {
            public static List<Department> Departments =
                new List<Department>();
        }
    }
}