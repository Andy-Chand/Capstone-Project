using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseAccessClass;
using System.Data.SqlClient;
using System.IO;

namespace Radagast_Website
{
    public partial class MainPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Purpose: Sends the user selection from Javascript over to the SetConnectionString function
        /// 
        /// Vers        Date        Coder       Comments
        /// 1.0         2021-03-21  Allan       Initial
        /// </summary>
        /// <param name="i"></param>

        [System.Web.Services.WebMethod()]
        public static void Connection (int i)
        {

            DataAccess DA = new DataAccess();
            DA.SetConnectionString(i);

        }

        /// <summary>
        /// Purpose: Sends the user selection from Javascript over to the SetConnectionStringAndBuildDatabase function
        /// 
        /// Vers        Date        Coder       Comments
        /// 1.0         2021-03-21  Allan       Initial
        /// </summary>
        /// <param name="i"></param>

        [System.Web.Services.WebMethod()]
        public static void ConnectionPlusDB(int i)
        {
            DataAccess DA = new DataAccess();
            DA.SetConnectionStringAndBuildDatabase(i);
        }

    }
}