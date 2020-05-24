using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FullCRUDImplementationWithJquery.DataAccess
{
    public class DatabaseConnection
    {
        private string sql_string;
        private string strCon;

        public string SqlString
        {
            set { sql_string = value; }
        }

        public string Connection_string
        {
            set { strCon = value; }
        }

        public SqlConnection GetConnection()
        {
            Connection_string = ConfigurationManager.ConnectionStrings["ORP_Connection"].ConnectionString;
            SqlConnection con = new SqlConnection(strCon);
            con.Open();
            return con;
        }
    }
}