using Dapper;
using EmpCoreAPIDapper.Models;
using EmpCoreMVCDapper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmpCoreAPIDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DapperConnectionString"));

            var employees = await connection.QueryAsync<Employee>("select * from employees");
            return Ok(employees);

        }

        //[HttpGet("{empId}")]
        //public async Task<ActionResult<Employee>> GetEmployee(Guid empId)
        //{
        //    using var connection = new SqlConnection(_config.GetConnectionString("DapperConnectionString"));
        //    var employee = await connection.QueryAsync<Employee>("select * from employees where id= @Id", new { Id = empId });
        //    return Ok(employee);
        //}

        [HttpPost]
        public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DapperConnectionString"));
            var idd= Guid.NewGuid();
            await connection.ExecuteAsync("insert into employees (id,name,email,salary,dateofbirth,department) values(@id,@Name, @Email, @Salary, @DateOfBirth, @Department)",new {id=idd, name=employee.Name,email=employee.Email,salary=employee.Salary,dateofbirth=employee.DateOfBirth,department=employee.Department});

            return Ok(employee);
        }

        [HttpPut]
        public async Task<ActionResult<Employee>> updateEmployee(Employee employee)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DapperConnectionString"));

            await connection.ExecuteAsync("update employees set name=@Name, email=@Email, salary=@Salary, dateofbirth=@DateOfBirth, department=@Department where id=@Id", employee);

            return Ok(employee);
        }

        [HttpDelete("{empId}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(Guid empId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DapperConnectionString"));
            var employee = await connection.QueryAsync<Employee>("delete from employees where id= @Id", new { Id = empId });
            return Ok(employee);
        }

        [HttpPost]
        [Route("Insert")]
        public async Task InsertEmployeeAsync(Employee employee)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("DapperConnectionString")))
            {
                //await connection.OpenAsync();
                //await connection.ExecuteAsync("InsertEmployee", employee, commandType: CommandType.StoredProcedure);
                //add parameters dynamically
                var parameters = new DynamicParameters();
                parameters.Add("@Id", employee.Id, DbType.Guid);
                parameters.Add("@Name", employee.Name, DbType.String);
                parameters.Add("@Email", employee.Email, DbType.String);
                parameters.Add("@Salary", employee.Salary, DbType.Int64);
                parameters.Add("@DateOfBirth", employee.DateOfBirth, DbType.DateTime2);
                parameters.Add("@Department", employee.Department, DbType.String);

                await connection.ExecuteAsync("InsertEmployee", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("DapperConnectionString")))
            {
                await connection.OpenAsync();
                var employees = await connection.QueryAsync<Employee>("GetAllEmployees", commandType: CommandType.StoredProcedure);
                return employees;
            }
        }

        //one to one relationship
        [HttpGet("{id}")]
        public IActionResult GetEmployeeWithDetail(int employeeId)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DapperConnectionString"));
            connection.Open();

            string sql = @"
                SELECT e.*, ed.*
                FROM Employees1 e
                JOIN EmployeeDetails1 ed ON e.EmployeeId = ed.EmployeeId
                WHERE e.EmployeeId = @EmployeeId";

            var employees = connection.Query<Employeees, EmployeeDetail, Employeees>(
                sql,
                (employee, detail) =>
                {
                    employee.EmployeeDetail = detail;
                    return employee;
                },
                new { EmployeeId = employeeId },
                splitOn: "EmployeeId");

            return Ok(employees.FirstOrDefault());
        }

        //one to many
        [HttpGet]
        public IActionResult GetEmployeeWithTasks(int id)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DapperConnectionString"));
            connection.Open();

            var sql = @"
                SELECT e.*, t.*
                FROM Employees e
                JOIN Tsk t ON e.EmployeeId = t.EmployeeId
                WHERE e.EmployeeId = @EmployeeId";

            var employeeDictionary = new Dictionary<int, Employees>();

            var employees = connection.Query<Employees, Tsk, Employees>(
                sql,
                (employee, task) =>
                {
                    if (!employeeDictionary.TryGetValue(employee.EmployeeId, out var currentEmployee))
                    {
                        currentEmployee = employee;
                        currentEmployee.Tasks = new List<Tsk>();
                        employeeDictionary.Add(currentEmployee.EmployeeId, currentEmployee);
                    }
                    currentEmployee.Tasks.Add(task);
                    return currentEmployee;
                },
                new { EmployeeId = id },
                splitOn: "TaskId");

            var employeeResult = employees.FirstOrDefault();

            if (employeeResult == null)
            {
                return NotFound();
            }

            return Ok(employeeResult);
        }

    }
}
