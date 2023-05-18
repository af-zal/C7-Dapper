using Dapper.Contrib.Extensions;

namespace EmpCoreMVCDapper.Models
{

    [Table("employee")]
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public long Salary { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? Department { get; set; }
    }


}


//Dapper.Contrib - additional functionality for mapping entities to database tables and performing CRUD operations.
//It simplifies the mapping and querying process by using conventions and attributes.

//extension methods like Insert, Update, Delete, and Get to perform CRUD operations on your entity objects.
//connection.Insert(employee);