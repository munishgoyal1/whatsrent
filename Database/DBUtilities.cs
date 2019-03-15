using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WhatsRent.Common.Database
{
    /// <summary>
    /// DBUtilities class provides helper functions to DBLibrary
    /// </summary>
    public class DBUtilities
    {
        /// <summary>
        /// Public constructor of DBUtilities class
        /// </summary>
        /// 
        public DBUtilities()
        {
        }

        /// <summary>
        /// Method to convert a c# parameter to a sql parameter
        /// </summary>
        /// <param name="paramName">Name of the parameter</param>
        /// <param name="paramValue">Value of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="datatype">SQLDB Datatype of the parameter</param>
        /// <returns>Sql typed SqlParameter</returns>
        public SqlParameter CreateSqlParamater(string paramName, SqlDbType datatype, byte precision, byte scale, ParameterDirection direction, object paramValue)
        {
            SqlParameter param = new SqlParameter();
            try
            {
                param.ParameterName = paramName;
                param.SqlDbType = datatype;
                param.Direction = direction;
                param.Precision = precision;
                param.Scale = scale;
                if (paramValue == null)
                    param.Value = DBNull.Value;
                else
                    param.Value = paramValue;
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Error creating the parameter. Reason: " + ex.Message);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Param Name: " + paramName);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Param Value: " + paramValue);
            }
            return param;
        }

        /// <summary>
        /// Method to convert a c# parameter to a sql parameter
        /// </summary>
        /// <param name="paramName">Name of the parameter</param>
        /// <param name="paramValue">Value of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="datatype">SQLDB Datatype of the parameter</param>
        /// <returns>Sql typed SqlParameter</returns>
        public SqlParameter CreateSqlParamater(string paramName, SqlDbType datatype, ParameterDirection direction, object paramValue)
        {
            SqlParameter param = new SqlParameter();
            try
            {
                param.ParameterName = paramName;
                param.SqlDbType = datatype;
                param.Direction = direction;
                if (paramValue == null)
                    param.Value = DBNull.Value;
                else
                    param.Value = paramValue;
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Error creating the parameter. Reason: " + ex.Message);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Param Name: " + paramName);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Param Value: " + paramValue);
            }
            return param;
        }

        /// <summary>
        /// Method to convert a c# parameter to a sql parameter
        /// </summary>
        /// <param name="paramName">Name of the parameter</param>
        /// <param name="paramValue">Value of the parameter</param>
        /// <param name="size">Size of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="datatype">SQLDB Datatype of the parameter</param>
        /// <returns>Sql typed SqlParameter</returns>
        public SqlParameter CreateSqlParamater(string paramName, SqlDbType datatype, int size,ParameterDirection direction, object paramValue)
        {
            SqlParameter param = new SqlParameter();
            try
            {
                param.ParameterName = paramName;
                param.SqlDbType = datatype;
                param.Size = size;
                param.Direction = direction;
                param.Value = paramValue;
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Error creating the parameter. Reason: " + ex.Message);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Param Name: " + paramName);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Param Value: " + paramValue);
            }
            return param;
        }

        /// <summary>
        /// Method to convert a c# parameter to a sql parameter
        /// </summary>
        /// <param name="paramName">Name of the parameter</param>
        /// <param name="paramValue">Value of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="datatype">DB Datatype of the parameter</param>
        /// <returns>Sql typed SqlParameter</returns>
        public SqlParameter CreateSqlParamater(string paramName, DbType datatype, ParameterDirection direction, object paramValue)
        {
            SqlParameter param = new SqlParameter();
            try
            {
                param.ParameterName = paramName;
                param.DbType = datatype;
                param.Direction = direction;
                param.Value = paramValue;
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Error creating the parameter. Reason: " + ex.Message);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Param Name: " + paramName);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Param Value: " + paramValue);
            }
            return param;
        }

        /// <summary>
        /// Method to convert a c# parameter to a sql parameter
        /// </summary>
        /// <param name="paramName">Name of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="datatype">DB Datatype of the parameter</param>
        /// <returns>Sql typed SqlParameter</returns>
        public SqlParameter CreateSqlParamater(string paramName, SqlDbType datatype, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter();
            try
            {
                param.ParameterName = paramName;
                param.SqlDbType = datatype;
                param.Direction = direction;
            }
            catch (Exception ex)
            {
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Error creating the parameter. Reason: " + ex.Message);
                Trace.TraceError(DateTime.Now.ToString() + " : " + "--------------");
                Trace.TraceError(DateTime.Now.ToString() + " : " + "Param Name: " + paramName);
            }
            return param;
        }
    }
}
