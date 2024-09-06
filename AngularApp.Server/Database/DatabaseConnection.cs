using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace AngularApp.Server.DBConnection
{
    class DatabaseConnect
    {
        private MySqlConnection? connection;
        private string server = "";
        private string database = "";
        private string uid = "";
        private string password = "";

        //Constructor
        public DatabaseConnect() => Initialize();

        //Initialize values
        private void Initialize()
        {
            System.Diagnostics.Debug.WriteLine("setting up connection");
            server = "localhost";
            database = "dotnet_learning";
            uid = "newuser";
            password = "newuser";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
            System.Diagnostics.Debug.WriteLine("connectionString ==> ", connectionString);
            System.Diagnostics.Debug.WriteLine("connection ==> ", connection);

        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                this.connection?.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in opening connection = ", ex);
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        // MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        // MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try {
                this.connection?.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in closing connection ==> ", ex);
                // MessageBox.Show(ex.Message);
                return false;
            }
        }

        public List<Dictionary<string, object>> GetData(string query)
        {
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                // One Way to get the results in JSON format
                var columns = new List<string>();
                var rows = new List<Dictionary<string, object>>();

                for (var i = 0; i < dataReader.FieldCount; i++)
                {
                    columns.Add(dataReader.GetName(i));
                }
                // System.Diagnostics.Debug.WriteLine(" <== columns ==> ", JsonSerializer.Serialize(columns) );
                while (dataReader.Read())
                {
                    rows.Add(columns.ToDictionary(column => column, column => dataReader[column]));
                }
                // System.Diagnostics.Debug.WriteLine(" <== rows ==> ", JsonSerializer.Serialize(rows));
                
                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                // return list to be displayed
                return rows;
            } else
            {
                return [];
            }
        }

        //Insert statement
        public void Insert(string query)
        {
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        // Get User entry from DB
        public List<User> GetUserDetails(string query)
        {
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                List<User> list = new List<User>();

                try
                {
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("<== Adding dataReader.GetInt32(\"EMP_ID\") ==>", JsonSerializer.Serialize(dataReader.GetInt32("EMP_ID")));

                        list.Add(new User()
                        {
                            employeeID = dataReader.GetInt32("EMP_ID"),
                            firstName = dataReader.GetString("FIRST_NAME"),
                            lastName = dataReader.GetString("LAST_NAME").Length > 0 ? dataReader.GetString("LAST_NAME") : "",
                            age = dataReader.GetInt32("AGE"),
                            managerName = dataReader.GetString("MANAGER_NAME") != null ? dataReader.GetString("MANAGER_NAME") : "",
                            managerID = dataReader.GetInt32("MANAGER_ID") > 0? dataReader.GetInt32("MANAGER_ID") : null,
                            salary = dataReader.GetInt32("SALARY")
                        });
                        System.Diagnostics.Debug.WriteLine("<== Added dataReader.GetInt32(\"EMP_ID\") ==>", JsonSerializer.Serialize(dataReader.GetInt32("EMP_ID")));
                    };
                    
                    dataReader.Close();

                    //close Connection
                    this.CloseConnection();

                    // return list to be displayed
                    return (list.Count > 0) ? list : [];
                } catch(Exception exception) {
                    // return list to be displayed
                    System.Diagnostics.Debug.WriteLine(" <== Exception in DB GetUserDetails", exception);
                    return (list.Count > 0) ? list : [];
                }                
            }
            else
            {
                return [];
            }
        }

        //Update statement
        public Boolean Update(string query)
        {
            //open connection
            if (this.OpenConnection() == true)
            {
                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();

                    return true;
                } catch(Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in updating user <==> ", exception);
                    return false;
                }
            } else
            {
                return false;
            }
        }

        
        //Delete statement
        public Boolean Delete(string query)
        {
            //open connection
            if (this.OpenConnection() == true)
            {
                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();

                    return true;
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in updating user <==> ", exception);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /*
        //Select statement
        public List<string>[] Select()
        {
        }

        //Count statement
        public int Count()
        {
        }

        //Backup
        public void Backup()
        {
        }

        //Restore
        public void Restore()
        {
        }

        */
    }
}
