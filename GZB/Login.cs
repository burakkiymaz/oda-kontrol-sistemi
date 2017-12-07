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

namespace GZB
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-5FJSVVR\\SQLEXPRESS;Initial Catalog=gzs;Integrated Security=True"); // making connection   
            SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM login WHERE kullanici='" + textBox1.Text + "' AND sifre='" + textBox2.Text + "'", con);
            /* in above line the program is selecting the whole data from table and the matching it with the user name and password provided by user. */
            DataTable dt = new DataTable(); //this is creating a virtual table  
            sda.Fill(dt);
            if (dt.Rows[0][0].ToString() == "1")
            {
                /* I have made a new page called home page. If the user is successfully authenticated then the form will be moved to the next form */
                this.Hide();
                new Form1().Show();
            }
            else
                MessageBox.Show("Yanlış kullanıcı adı ve şifre girdiniz");  
        }
    }
}
