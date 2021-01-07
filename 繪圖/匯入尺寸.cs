using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace 繪圖
{
    public partial class 匯入尺寸 : Form
    {
        public double[] custom_size = new double[29];
        public string custom_number;
        public 匯入尺寸()
        {
            InitializeComponent();
            try
            {
                string sqlConnectionCommand = "server=localhost;database=clothes;uid=ctu;pwd=ru04eji6dk284";
                string sqlSelectCommand = "Select custmeasure.Cust_Num, customer.Cust_Name, custmeasure.BodyM_Date From custmeasure Inner Join customer On custmeasure.Cust_Num=customer.Cust_Num";
                MySqlConnection sqlConnection = new MySqlConnection(sqlConnectionCommand);
                MySqlCommand sqlCommand = new MySqlCommand();
                try
                {
                    sqlConnection.Open();
                }
                catch (MySqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 0:
                            Console.WriteLine("無法連線到資料庫.");
                            break;
                        case 1045:
                            Console.WriteLine("使用者帳號或密碼錯誤,請再試一次.");
                            break;
                    }
                }
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = sqlSelectCommand;
                MySqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    Datagridview.Rows.Add();
                    Datagridview.Rows[Datagridview.Rows.Count - 1].Cells[0].Value = sqlDataReader[0];
                    Datagridview.Rows[Datagridview.Rows.Count - 1].Cells[1].Value = sqlDataReader[1];
                    Datagridview.Rows[Datagridview.Rows.Count - 1].Cells[2].Value = sqlDataReader[2];
                }
                sqlDataReader.Close();
                sqlCommand.Dispose();
                sqlConnection.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("發生資料庫連接錯誤" + ex.Message, "訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string cnum = (string)Datagridview.SelectedRows[0].Cells[0].Value;
                custom_number = cnum;
                string sqlConnectionCommand = "server=localhost;database=clothes;uid=ctu;pwd=ru04eji6dk284";
                string sqlSelectCommand = "Select * From custmeasure Where Cust_Num='" + cnum + "'";
                MySqlConnection sqlConnection = new MySqlConnection(sqlConnectionCommand);
                MySqlCommand sqlCommand = new MySqlCommand();
                try
                {
                    sqlConnection.Open();
                }
                catch (MySqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 0:
                            Console.WriteLine("無法連線到資料庫.");
                            break;
                        case 1045:
                            Console.WriteLine("使用者帳號或密碼錯誤,請再試一次.");
                            break;
                    }
                }
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = sqlSelectCommand;
                MySqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    for (int i = 0; i < 29; i++)
                    {
                        double.TryParse(sqlDataReader[i + 10].ToString(), out custom_size[i]);
                    }
                }
                sqlDataReader.Close();
                sqlCommand.Dispose();
                sqlConnection.Close();


                DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                MessageBox.Show("發生資料庫連接錯誤\n" + ex.Message, "訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

    }

    class SqlOperateInfo
    {
        //Suppose your ServerName is "aa",DatabaseName is "bb",UserName is "cc", Password is "dd"
        private string sqlConnectionCommand = "Data Source=aa;Initial Catalog=bb;User ID=cc;Pwd=dd";
        //This table contains two columns:KeywordID int not null,KeywordName varchar(100) not null
        private string dataTableName = "Basic_Keyword_Test";
        private string storedProcedureName = "Sp_InertToBasic_Keyword_Test";
        private string sqlSelectCommand = "Select KeywordID, KeywordName From Basic_Keyword_Test";
        //sqlUpdateCommand could contain "insert" , "delete" , "update" operate
        private string sqlUpdateCommand = "Delete From Basic_Keyword_Test Where KeywordID = 1";
        public void UseSqlReader()
        {
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionCommand);
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = sqlSelectCommand;
            sqlConnection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                //Get KeywordID and KeywordName , You can do anything you like. Here I just output them.
                int keywordid = (int)sqlDataReader[0];
                //the same as: int keywordid = (int)sqlDataReader["KeywordID"]
                string keywordName = (string)sqlDataReader[1];
                //the same as: string keywordName = (int)sqlDataReader["KeywordName"]
                Console.WriteLine("KeywordID = " + keywordid + " , KeywordName = " + keywordName);
            }
            sqlDataReader.Close();
            sqlCommand.Dispose();
            sqlConnection.Close();
        }
        public void UseSqlStoredProcedure()
        {
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionCommand);
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = storedProcedureName;
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            //you can use reader here,too.as long as you modify the sp and let it like select * from ....
            sqlCommand.Dispose();
            sqlConnection.Close();
        }
        public void UseSqlDataSet()
        {
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionCommand);
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = sqlSelectCommand;
            sqlConnection.Open();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = sqlCommand;
            DataSet dataSet = new DataSet();
            //sqlCommandBuilder is for update the dataset to database
            SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
            sqlDataAdapter.Fill(dataSet, dataTableName);
            //Do something to dataset then you can update it to 　Database.Here I just add a row
            DataRow row = dataSet.Tables[0].NewRow();
            row[0] = 10000;
            row[1] = "new row";
            dataSet.Tables[0].Rows.Add(row);
            sqlDataAdapter.Update(dataSet, dataTableName);
            sqlCommand.Dispose();
            sqlDataAdapter.Dispose();
            sqlConnection.Close();
        }
    }
}
