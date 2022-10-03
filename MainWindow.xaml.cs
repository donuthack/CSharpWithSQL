using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace TutorialBDTestReg
{
    public partial class MainWindow : Window
    {
        private DataTable myDatas;

        public System.Data.DataTable MyDatas { get => myDatas; set => myDatas = value; }

        public System.Data.SqlClient.SqlConnectionStringBuilder Csb => new System.Data.SqlClient.SqlConnectionStringBuilder()
        {
            DataSource = "localhost",
            InitialCatalog = "TestDB",
            IntegratedSecurity = true,
            PersistSecurityInfo = false,
            ConnectTimeout = 10,
            ApplicationName = "Das ist meine SQL Verbindung"

        };

        public MainWindow()
        {
            InitializeComponent();
        }
         /// <summary>
         /// Function für hashing
         /// </summary>
         /// <param name="s"></param>
         /// <returns></returns>
        public static string ToSHA256(string s)
        {
            var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));

            var sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Login form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2(object sender, RoutedEventArgs e)
        {
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Csb.ConnectionString))
            {
                connection.Open();
                string findenUsername = "SELECT * FROM dbo.RegistrationDB WHERE Benutzername = @NAME AND PasswortHash = @PASSWORD;" +
                                        "UPDATE dbo.RegistrationDB SET LetzteAnmeldung = GETDATE() WHERE Benutzername = @NAME;";
                string statement = findenUsername;
                string name = Name11.Text;
                string password = Name12.Text;
                //string hashOfPass = ToSHA256(password);

                using (System.Data.SqlClient.SqlCommand sqlCmd = new System.Data.SqlClient.SqlCommand(statement, connection))
                {
                    sqlCmd.Parameters.AddWithValue("@NAME", name);
                    sqlCmd.Parameters.AddWithValue("@PASSWORD",ToSHA256(password));
                    SqlDataReader rdr = sqlCmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        Name13.Text = "Du bist online!";                     
                    }
                    else
                    {
                        Name13.Text = "Error . Bitte, siecht deine passwort oder username.:c";
                    }


                }
            }
        }

        /// <summary>
        /// Registration form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1(object sender, RoutedEventArgs e)
        {
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Csb.ConnectionString))
            {
                connection.Open();
                string insertData = "INSERT INTO dbo.RegistrationDB (Benutzername,PasswortHash,LetzteAnmeldung) VALUES (@NAME,@PASSWORD,GETDATE());";
                string statement = insertData;
                string name = Name11_Copy.Text;
                string password = Name12_Copy.Text;

                using (System.Data.SqlClient.SqlCommand sqlCmd = new System.Data.SqlClient.SqlCommand(statement, connection))
                {
                    sqlCmd.Parameters.AddWithValue("@NAME", name);
                    sqlCmd.Parameters.AddWithValue("@PASSWORD", ToSHA256(password));
                    sqlCmd.ExecuteNonQuery();
                    Name13_Copy.Text = name + " ist in der DB.";
                    connection.Close();


                }
            }
        }

    }

}



