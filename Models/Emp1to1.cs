using Dapper.Contrib.Extensions;

namespace EmpCoreAPIDapper.Models
{
    [Table("Employees1")]
    public class Employeees
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public EmployeeDetail EmployeeDetail { get; set; }
    }

    [Table("EmployeeDetails1")]
    public class EmployeeDetail
    {
        public int EmployeeId { get; set; }
        public string Address { get; set; }
        // Other properties
    }
}
