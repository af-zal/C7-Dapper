using Dapper;
using EmpCoreMVCDapper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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

        [HttpGet("{empId}")]
        public async Task<ActionResult<Employee>> GetEmployee(Guid empId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DapperConnectionString"));
            var employee = await connection.QueryAsync<Employee>("select * from employees where id= @Id", new { Id = empId });
            return Ok(employee);
        }

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

    }
}
