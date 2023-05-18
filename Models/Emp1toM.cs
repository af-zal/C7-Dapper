﻿using Dapper.Contrib.Extensions;

namespace EmpCoreAPIDapper.Models
{
    public class Employees
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public List<Task> Tasks { get; set; }
    }

    public class Tsk
    {
        public int TaskId { get; set; }
        public string Description { get; set; }
        public int EmployeeId { get; set; }
        // Other properties
    }
}
