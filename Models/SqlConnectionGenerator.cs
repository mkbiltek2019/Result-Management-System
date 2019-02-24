using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class SqlConnectionGenerator
    {
        public SqlConnectionGenerator()
        {

        }

        public SqlConnection FromFaculty(string Faculty)
        {
            return new SqlConnection("Server=" + Variables.ServerName + ";Integrated security=true;Initial Catalog=" + Faculty + ";");
        }

        public SqlConnection MasterDatabaseConnection()
        {
            return new SqlConnection("Server=" + Variables.ServerName + ";Integrated security=SSPI;database=master;");
        }
    }
}