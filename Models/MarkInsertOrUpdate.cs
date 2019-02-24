using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class MarkInsertOrUpdate
    {
         SqlConnection myConn = new SqlConnection();
        public MarkInsertOrUpdate(String faculty)
        {    
            myConn = new SqlConnection("Server=OVI-PC\\SQLEXPRESS;Integrated security=true;Initial Catalog=" + faculty + ";");
        
        
        }
        public Boolean UploadMark()
        {

            return true;
        }


    }
}