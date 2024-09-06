using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AngularApp.Server.DBConnection;
using System.Reflection;
using System.Collections.Generic;
using MySqlX.XDevAPI.Common;
using Mysqlx.Datatypes;
using Mysqlx.Crud;

namespace AngularApp.Server.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult GetEmployeesList()
        {

            //string dbQuery = "SELECT * from employees";
            string dbQuery = $"SELECT * from employees";
            Constants appConstants = new Constants();
            DatabaseConnect dbConnection = new();
            System.Diagnostics.Debug.WriteLine("<== dbQuery ==>", JsonSerializer.Serialize(dbQuery));

            // Find any matching user present in DB
            List<User> employeesList = dbConnection.GetUserDetails(dbQuery);
            System.Diagnostics.Debug.WriteLine("<== employeesList ==>", JsonSerializer.Serialize(employeesList));

            /*
            List<Dictionary<string, object>> employeesList = new List<Dictionary<string, object>>();
            foreach (var item in dbResponse)
            {
                List<string> keys = new List<string>(item.Keys);
                Console.WriteLine("Displaying keys...");
                foreach (string res in keys)
                {
                    System.Diagnostics.Debug.WriteLine("res ==> ", res);
                }
            }
            */
            /*
            foreach ( var item in dbResponse)
            {
                    System.Diagnostics.Debug.WriteLine("item ==> ", JsonSerializer.Serialize(item));
                    Dictionary<string, object> empItem = new Dictionary<string, object>();
                    foreach (KeyValuePair<string, object> ele1 in item)
                    {
                        // string newKey = 
                        // System.Diagnostics.Debug.WriteLine("newKey   ", newKey);
                        System.Diagnostics.Debug.WriteLine("{0} and {1}",ele1.Key, ele1.Value);
                        empItem.Add(ele1.Key, ele1.Value);
                        System.Diagnostics.Debug.WriteLine("empItem ==> ", JsonSerializer.Serialize(empItem));

                }
                employeesList.Add(empItem);
            }
            System.Diagnostics.Debug.WriteLine("employeesList ==> ", JsonSerializer.Serialize(employeesList));
            */

            // return Ok(new { employeesList = dbResponse });
            return StatusCode(StatusCodes.Status200OK, new { employeesList = employeesList });

        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // Route can be added at function level in order to append it to relative URL of the API
        public ActionResult GetEmployeeById(int id, string? typeOfUser)
        {

            string dbQuery = "SELECT * from employees WHERE EMP_ID = " + id;
            Constants appConstants = new Constants();
            DatabaseConnect dbConnection = new();
            System.Diagnostics.Debug.WriteLine("dbConnection == ", dbConnection);

            List<User> dbResponse = dbConnection.GetUserDetails(dbQuery);
            System.Diagnostics.Debug.WriteLine("dbConnection Get By Id done");

            try
            {
                /*
                System.Diagnostics.Debug.WriteLine("dbResponse[0] == ", JsonSerializer.Serialize(dbResponse[0].Keys));
                List<string> keys = new List<string>(dbResponse[0].Keys);

                foreach (var item in dbResponse)
                {
                    // System.Diagnostics.Debug.WriteLine("item ==> ", JsonSerializer.Serialize(item));
                    Dictionary<string, object> empItem = new Dictionary<string, object>();
                    foreach (KeyValuePair<string, object> ele1 in item)
                    {
                        System.Diagnostics.Debug.WriteLine("{0} and {1}", ele1.Key, ele1.Value);
                        empItem.Add(ele1.Key.ToLower(), ele1.Value.ToString());
                    }
                    employeesList.Add(empItem);
                }*/
                return StatusCode(StatusCodes.Status200OK, new { employeesList = dbResponse, message = "Matching employee found." });
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("dbResponse[0] == ", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { employeesList = dbResponse, message = "No matching employee found." });
            }
        }

        [HttpPost]
        // [Route("create")]
        // Route can be added at function level in order to append it to relative URL of the API
        public ActionResult CreateEmloyee([FromBody] User user)
        {
            System.Diagnostics.Debug.WriteLine("user == ", JsonSerializer.Serialize(user));

            // var pathBase = HttpContext.Request.PathBase;
            var contextItems = HttpContext.Items;

            System.Diagnostics.Debug.WriteLine("contextItems == ", JsonSerializer.Serialize(contextItems));

            string dbQuery = $"INSERT INTO EMPLOYEES VALUES ({user.employeeID}, '{user.firstName}', " +
                $"'{user.lastName}', {user.age}, '{user.managerName}', {user.managerID}, {user.salary})";
            System.Diagnostics.Debug.WriteLine("d == ", JsonSerializer.Serialize(dbQuery));

            try
            {
                DatabaseConnect dbConnection = new();
                System.Diagnostics.Debug.WriteLine("dbConnection == ", dbConnection);

                dbConnection.Insert(dbQuery);
                System.Diagnostics.Debug.WriteLine("dbConnection Insert done");
                return StatusCode(StatusCodes.Status200OK, new { newUser = user, message = "User creation was successful." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ex == ", ex);
                System.Diagnostics.Debug.WriteLine("ex.Message == ", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { newUser = "", message = "User creation failed." });
            }
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // Route can be added at function level in order to append it to relative URL of the API
        public ActionResult EditEmployee([FromBody] User user, [FromRoute] int id)
        {
            System.Diagnostics.Debug.WriteLine("user == ", JsonSerializer.Serialize(user));
            System.Diagnostics.Debug.WriteLine("id == ", JsonSerializer.Serialize(id));

            DatabaseConnect dbConnection = new();
            
            string dbQuery = $"UPDATE employees SET FIRST_NAME = '{user.firstName}', LAST_NAME = '{user.lastName}', AGE = {user.age}, SALARY = {user.salary} WHERE EMP_ID = {id}";
            System.Diagnostics.Debug.WriteLine("dbQuery to update user == ", dbQuery);

            if( dbConnection.Update(dbQuery) )
            {
                return StatusCode(StatusCodes.Status200OK, new { message = $"User creation was successful for employee Id - {id}" });
            } else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Update operation was not successful." });
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteEmloyee([FromRoute] int id)
        {

            DatabaseConnect dbConnection = new();
            string dbQuery = $"Delete from employees where emp_id = {id}";

            System.Diagnostics.Debug.WriteLine("<==delete dbQuery ==>", JsonSerializer.Serialize(dbQuery));

            if (dbConnection.Delete(dbQuery))
            {
                return StatusCode(StatusCodes.Status200OK, new { message = $"User {id} has been successfully deleted" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Update operation was not successful." });
            }
        }



    }
}
