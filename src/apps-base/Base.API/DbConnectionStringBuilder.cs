using Base.API.Consts;

namespace Base.API
{
    public class DbConnectionStringBuilder
    {

        public static string Build(string databaseType, string server, string port, string database, string userId, string password)
        {
            var dbType = databaseType.ToLower();
            if (dbType == AppDatabaseConst.Postgres)
            {
                return $"Server={server};Port={port};Database={database};User Id={userId};Password={password}";
            }
            else if (dbType == AppDatabaseConst.SQLServer)
            {
                return $"Data Source={server},{port};Initial Catalog={database};User ID={userId};Password={password};Persist Security Info=True";
            }
            else if (dbType == AppDatabaseConst.MySQL)
            {
                return $"Server={server};Port={port};Database={database};Uid={userId};Pwd={password}";
            }
            else { }

            return string.Empty;
        }
    }
}
