using System.Text;
using System.Data.OleDb;

namespace TypographySearch
{
    class DBUtils
    {
        // @"Provider=SQLNCLI10.1;Data Source=LIDER1C;Persist Security Info=True;UID=sa;PWD=123;Initial Catalog=Typography_2020";

        public string Provider { get; set; } = "SQLNCLI10.1";
        public string Datasource { get; set; } = "LIDER1C";
        public string Database { get; set; } = "Typography_2021";
        public string Username { get; set; } = "sa";
        public string Password { get; set; } = "123";

        public OleDbConnection GetConnection()
        {
            StringBuilder connectionString = new StringBuilder();
            connectionString.Append("Provider=" + Provider + ";");
            connectionString.Append("Data Source=" + Datasource + ";");
            connectionString.Append("Initial Catalog=" + Database + ";");
            connectionString.Append("UID=" + Username + ";");
            connectionString.Append("PWD=" + Password + ";");
            connectionString.Append("Persist Security Info=True;");

            OleDbConnection connection = new OleDbConnection(connectionString.ToString());
            return connection;
        }
    }
}
