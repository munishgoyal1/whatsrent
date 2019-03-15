using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.Generic;

namespace WhatsRent.Common.Database
{
    /// <summary>
    /// DBLibrary class provides different flavours of database talker functions
    /// Primarily aimed at SQL Server - will need to be improvised for other databases like Oracle
    /// </summary>
    public class DBLibrary
    {
        /// <summary>
        /// Read only connection string for database connections
        /// </summary>
        private readonly string _connectionString;

        public DBLibrary(string connectionString)
        {
            _connectionString = connectionString; 
        }

        /// <summary>
        /// Method to execute a direct SQL query like "select * from emp"
        /// </summary>
        /// <param name="query">SQL query string</param>
        /// <returns>Dataset (results) associated with the query</returns>
        public DataSet ExecuteQuery(string query)
        {
            DataSet dataset = new DataSet();

            try
            {
                using(SqlConnection connection = new SqlConnection(_connectionString))
                using(SqlCommand command = new SqlCommand(query,connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    // Open the connection
                    connection.Open();

                    // Set the command type
                    command.CommandType = CommandType.Text;

                    // Fill the dataset
                    adapter.Fill(dataset);

                    // Close the connection
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Failed to execute the query. Reason: " + ex.Message);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Query Name: " + query);

                dataset = null;
            }

            return dataset;
        }


        /// <summary>
        /// Method to prepare a sql command
        /// Typically used in queueing batch commands
        /// </summary>
        /// <param name="procName">Name of stored procedure to execute</param>
        /// <param name="paramList">List of parameters to be passed to the stored procedure</param>
        /// <returns>SQL command associated with the procedure and paramlist</returns>
        public SqlCommand GetSqlCommand(string procName, ArrayList paramList)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = procName;
            command.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter param in paramList)
            {
                command.Parameters.Add(param);
            }

            return command;
        }

        /// <summary>
        /// Method to execute SQL Commands in a queue
        /// </summary>
        /// <seealso cref="ExecuteQueuedCommandsBool"/>
        /// <param name="commandQueue">Queue of sql commands</param>
        /// <returns>Arraylist of sql command outcomes</returns>
        public ArrayList ExecuteQueuedCommandsAL(Queue commandQueue)
        {
            ArrayList outParams = new ArrayList();

            SqlConnection connection = new SqlConnection(_connectionString);

            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                while (commandQueue.Count > 0)
                {
                    using (SqlCommand command = ((SqlCommand)commandQueue.Dequeue()))
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();

                        foreach (SqlParameter param in command.Parameters)
                        {
                            if (param.Direction == ParameterDirection.Output)
                                outParams.Add(command.Parameters[param.ParameterName]);
                        }
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Failed to execute commands in queue (AL). Reason: " + ex.Message);
                outParams.Clear();
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
                connection.Dispose();
            }

            return outParams;
        }

        /// <summary>
        /// Method to execute SQL Commands in a queue
        /// </summary>
        /// <seealso cref="ExecuteQueuedCommandsAL"/>
        /// <param name="commandQueue">Queue of sql commands</param>
        /// <returns>A bool indicating success or failure of batch transaction</returns>
        public bool ExecuteQueuedCommands(Queue commandQueue)
        {
            bool status = true;

            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            
            try
            {

                while (commandQueue.Count > 0)
                {
                    using (SqlCommand command = ((SqlCommand)commandQueue.Dequeue()))
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Failed to execute commands in queue (bool). Reason: " + ex.Message);
                status = false;
                transaction.Rollback();
            }
            finally
            {                
                transaction.Dispose();
                connection.Dispose();
            }

            return status;
        }


        /// <summary>
        /// Method to execute a stored procedure on the database
        /// This method returns a scalar outcome
        /// </summary>
        /// <seealso cref="ExecuteProcedureDS"/>
        /// <seealso cref="ExecuteProcedure"/>
        /// <param name="proc">Name of the stored procedure to execute</param>
        /// <param name="paramList">Param list to be passed to stored procedure</param>
        /// <returns>Scalar integer</returns>
        public int ExecuteProcedureScalar(string procName,ArrayList paramList)
        {
            int output = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(procName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    foreach (SqlParameter param in paramList)
                    {
                        command.Parameters.Add(param);
                    }

                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        output = int.Parse(result.ToString());
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Failed to execute the proc scalar. Reason: " + ex.Message); 
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Proc Name: " + procName);

                output = -1;
            }

            return output;
        }

        /// <summary>
        /// Method to execute a stored procedure on the database
        /// This method returns an arraylist outcome
        /// </summary>
        /// <seealso cref="ExecuteProcedureDS"/>
        /// <seealso cref="ExecuteProcedureAL"/>
        /// <param name="proc">Name of the stored procedure to execute</param>
        /// <param name="paramList">Param list to be passed to stored procedure</param>
        /// <returns>Arraylist of results by executing sql procedure</returns>
        public ArrayList ExecuteProcedure(string procName, ArrayList paramList)
        {
            ArrayList outValues = new ArrayList();
            
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(procName, connection))
                {
                    ArrayList outParams = new ArrayList();

                    // Set the type of command as stored procedure
                    command.CommandType = CommandType.StoredProcedure;
                    
                    // Set up query parameters
                    foreach (SqlParameter param in paramList)
                    {
                        command.Parameters.Add(param);

                        if (param.Direction == ParameterDirection.Output)
                        {
                            outParams.Add(param);                    
                        }
                    }

                    // Open the connection
                    connection.Open();
                    
                    // Execute the query
                    command.ExecuteNonQuery();

                    // Get the output values of interest
                    foreach (SqlParameter param in outParams)
                    {
                        outValues.Add(command.Parameters[param.ParameterName]);
                    }

                    // Close the connection
                    connection.Close();

                    // Finally cleanup
                    outParams.Clear();
                    outParams = null;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Failed to execute the proc. Reason: " + ex.Message); 
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Query Name: " + procName);
            }

            return outValues;
        }

        /// <summary>
        /// Method to execute a stored procedure on the database
        /// This method returns an arraylist outcome
        /// </summary>
        /// <seealso cref="ExecuteProcedureDS"/>
        /// <seealso cref="ExecuteProcedureAL"/>
        /// <param name="proc">Name of the stored procedure to execute</param>
        /// <param name="paramList">Param list to be passed to stored procedure</param>
        /// <returns>Arraylist of results by executing sql procedure</returns>
        public bool ExecuteProcedures(List<KeyValuePair<string, ArrayList>> spCommands)
        {
            bool status = true;

            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            SqlCommand command = new SqlCommand();

            command.Connection = connection;
            command.Transaction = transaction;

            try
            {

                //foreach (KeyValuePair<string, ArrayList> spCommand in spCommands)
                foreach (var spCommand in spCommands)
                {
                    string procName = spCommand.Key;
                    command.Parameters.Clear();
                    command.CommandText = procName;
                    command.CommandType = CommandType.StoredProcedure;

                    // Set up query parameters
                    foreach (SqlParameter param in spCommand.Value)
                    {
                        command.Parameters.Add(param);
                    }

                    // Execute the query
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();

                    //cleanup
                    procName = null;
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Failed to execute commands in queue (bool). Reason: " + ex.Message);
                status = false;
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
                connection.Dispose();
            }

            return status;
        }

        /// <summary>
        /// Method to execute a stored procedure on the database
        /// This method returns a dataset
        /// </summary>
        /// <seealso cref="ExecuteProcedureScalar"/>
        /// <seealso cref="ExecuteProcedure"/>
        /// <param name="proc">Name of the stored procedure to execute</param>
        /// <param name="paramList">Param list to be passed to stored procedure</param>
        /// <returns>Dataset containing results</returns>
        public DataSet ExecuteProcedureDS(string proc, ArrayList paramList)
        {
            DataSet dataset = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(proc, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (paramList.Count > 0)
                    {
                        foreach (SqlParameter param in paramList)
                        {
                            command.Parameters.Add(param);
                        }
                    }

                    connection.Open();
                    adapter.Fill(dataset);
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Failed to execute the proc (DS). Reason: " + ex.Message); 
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Query Name: " + proc);

                dataset = null;

            }

            return dataset;
        }

        /// <summary>
        /// Method to prepare a sql command
        /// Typically used in queueing batch commands
        /// </summary>
        /// <param name="procName">Name of stored procedure to execute</param>
        /// <param name="paramList">List of parameters to be passed to the stored procedure</param>
        /// <param name="connection"> Associated SQL connection</param>
        /// <param name="transaction">Associated SQL transaction if any</param>
        /// <returns>SQL command associated with the procedure and paramlist</returns>
        public SqlCommand GetSqlCommand(string procName, ArrayList paramList, SqlConnection connection, SqlTransaction transaction = null)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = procName;
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            command.Transaction = transaction;

            foreach (SqlParameter param in paramList)
            {
                command.Parameters.Add(param);
            }

            return command;
        }


        /// <summary>
        /// Method to execute a given SQL command. Command should contain the connection information
        /// This method returns an arraylist outcome
        /// </summary>
        /// <param name="command">SQL command to be executed</param>
        /// <returns>Arraylist of results by executing sql procedure</returns>
        public ArrayList ExecuteStandaloneSqlCommand(SqlCommand command)
        {
            ArrayList outValues = new ArrayList();

            try
            {
                ArrayList outParams = new ArrayList();

                // Set up query out parameters
                foreach (SqlParameter param in command.Parameters)
                {
                    if (param.Direction == ParameterDirection.Output)
                    {
                        outParams.Add(param);
                    }
                }

                // Execute the query
                command.ExecuteNonQuery();

                // Get the output values of interest
                foreach (SqlParameter param in outParams)
                {
                    outValues.Add(command.Parameters[param.ParameterName]);
                }

                // Finally cleanup
                outParams.Clear();
                outParams = null;
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Failed to execute the command. Reason: " + ex.Message);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Command : " + command.ToString());
                throw;
            }

            return outValues;
        }

    }
}
