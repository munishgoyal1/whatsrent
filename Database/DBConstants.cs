namespace WhatsRent.Common.Database
{
    /// <summary>
    /// Constant values used in Database library functions
    /// </summary>
    public static class DBConstants
    {
        public const int DB_POOL_SIZE = 25;
        public const string APPL_PARAM_LOAD_QUERY = @"select parameter_name,parameter_value from st_ApplicationParameters";
    }
}
