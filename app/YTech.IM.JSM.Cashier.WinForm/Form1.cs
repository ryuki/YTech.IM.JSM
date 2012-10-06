using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YTech.IM.JSM.Cashier.WinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCeReplication sqlCeRep = new SqlCeReplication();
            string cesource = @"E:\My Project\MVC Project\Solutions\YTech.IM.JSM\app\YTech.IM.JSM.Web\DB_IM_JSM.sdf";
            using (sqlCeRep)
            {

                sqlCeRep.InternetUrl = "http://localhost:81/Sync/sqlcesa35.dll";

                sqlCeRep.Publisher = "yahu";
                sqlCeRep.PublisherSecurityMode = SecurityType.DBAuthentication;

                sqlCeRep.PublisherLogin = "NT AUTHORITY\\IUSR";
                //sqlCeRep.PublisherPassword = "superadmin";
                sqlCeRep.PublisherDatabase = "DB_IM_JSM";
                sqlCeRep.Publication = "JSMPub";

                sqlCeRep.Subscriber = "Rully";
                sqlCeRep.SubscriberConnectionString = "Data Source=" + cesource;

                try
                {
                    if (!System.IO.File.Exists(cesource))
                    {
                        sqlCeRep.AddSubscription(AddOption.CreateDatabase);
                    }

                    sqlCeRep.Synchronize();

                    lblInfo.Text = "Synchronized.";
                }
                catch (SqlCeException sqlex)
                {
                    lblInfo.Text = sqlex.Message;
                }
                catch
                    (Exception ex)
                {
                    lblInfo.Text = ex.Message;
                }
            }

        }
    }
}
