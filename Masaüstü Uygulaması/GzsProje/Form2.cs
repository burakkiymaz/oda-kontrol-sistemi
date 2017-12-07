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
namespace GzsProje
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        SqlConnection Baglanti = new SqlConnection("Data Source=DESKTOP-5FJSVVR\\SQLEXPRESS;Initial Catalog=gzs;Integrated Security=True");
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
            try
            {
                serialPort1.PortName = comboBox1.Text;
                if (!serialPort1.IsOpen)
                    serialPort1.Open();

            }
            catch
            {
                MessageBox.Show("Seri Porta Bağlı !!");
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < System.IO.Ports.SerialPort.GetPortNames().Length; i++)
            {
                comboBox1.Items.Add(System.IO.Ports.SerialPort.GetPortNames()[i]);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                // serialPort1.DiscardInBuffer();
                if (serialPort1.IsOpen)
                    serialPort1.Close();
                MessageBox.Show("Seri Port kapatıldı");
            }
            catch
            {
                MessageBox.Show("Seri Portta hata var");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try {
                
                
                string sonuc = serialPort1.ReadLine().ToString();
                string[] pot = sonuc.Split(',');
               // label1.Text = sonuc + "";
                label3.Text = pot[0].ToString();
                label5.Text = pot[1].ToString();
                label7.Text = pot[2].ToString();
               // serialPort1.DiscardInBuffer()
                progressBar1.Value = Convert.ToInt32(label3.Text.ToString());
                progressBar2.Value = Convert.ToInt32(label5.Text.ToString());
                progressBar3.Value = Convert.ToInt32(label7.Text.ToString());
                sicaklikVerileriEkle(label3.Text.ToString());
                dogalgazVerileriEkle(label5.Text.ToString());
                KarbonVerileriEkle(label7.Text.ToString());
 
            }catch(Exception ex)
            {
               // MessageBox.Show("Timer Verileri cekilirken hata  var"+ex);
            }

            
        }
        public void verileriGetir() {
            try {
                SqlDataAdapter DataAdapter = new SqlDataAdapter();
          DataTable DataTable = new DataTable();
          if (Convert.ToBoolean(Baglanti.State) == false) { Baglanti.Open(); }
       
          DataAdapter.SelectCommand=new SqlCommand("Select * from Sicaklik_Deger order by tarih desc",Baglanti);      
          DataAdapter.Fill(DataTable);
          dataGridView1.DataSource = DataTable;
            }catch(Exception ex)
            {
                MessageBox.Show("Hata oluştu Datagridview da"+ex);
            }
        }
        public void sicaklikVerileriEkle(string receiveddata) 
        {
            try
            {
                string durumSicaklik = "";
                if (Convert.ToInt32(receiveddata) <= 20)
                {
                    durumSicaklik = "Soğuk";
                }
                else if (Convert.ToInt32(receiveddata) > 20 && Convert.ToInt32(receiveddata) <= 30)
                {
                    durumSicaklik = "Normal";
                }
                else {
                    durumSicaklik = "Sıcak";
                }
                if (Convert.ToBoolean(Baglanti.State) == false) { Baglanti.Open(); }
                SqlCommand Komut = new SqlCommand();
                Komut.Connection = Baglanti;
                Komut.CommandText = ("insert into Sicaklik_Deger(tarih,sicaklik,durum) values(SYSDATETIME(),'" + receiveddata + "','"+durumSicaklik+"')");
                Komut.ExecuteNonQuery();
                verileriGetir();
                Baglanti.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata var " + ex);
            }

        }
        
        public void dogalgazVerileriEkle(string receiveddata)
        {
            try
            {
                string durumDogalgaz = "";
                if (Convert.ToInt32(receiveddata) <= 20)
                {
                    durumDogalgaz = "Sıkıntı Yok";
                }
                else if (Convert.ToInt32(receiveddata) > 20 && Convert.ToInt32(receiveddata) <= 30)
                {
                    durumDogalgaz = "Normal";
                }
                else
                {
                    durumDogalgaz = "Acil Durum";
                }
                if (Convert.ToBoolean(Baglanti.State) == false) { Baglanti.Open(); }
                SqlCommand Komut = new SqlCommand();
                Komut.Connection = Baglanti;
                Komut.CommandText = ("insert into Dogalgaz_Deger(tarih,dogalgaz,durum) values(SYSDATETIME(),'" + receiveddata + "','" + durumDogalgaz + "')");
                Komut.ExecuteNonQuery();
                verileriGetir2();
                Baglanti.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata var " + ex);
            }

        }
       
        
        public void verileriGetir2()
        {
            try
            {
                SqlDataAdapter DataAdapter = new SqlDataAdapter();
                DataTable DataTable = new DataTable();
                if (Convert.ToBoolean(Baglanti.State) == false) { Baglanti.Open(); }

                DataAdapter.SelectCommand = new SqlCommand("Select * from Dogalgaz_Deger order by tarih desc", Baglanti);
                DataAdapter.Fill(DataTable);
                dataGridView2.DataSource = DataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu Datagridview da" + ex);
            }
        }

       
       
        public void KarbonVerileriEkle(string receiveddata)
        {
            try
            {
                string durumKarbon = "";
                if (Convert.ToInt32(receiveddata) <= 20)
                {
                    durumKarbon = "Sıkıntı Yok";
                }
                else if (Convert.ToInt32(receiveddata) > 20 && Convert.ToInt32(receiveddata) <= 30)
                {
                    durumKarbon = "Normal";
                }
                else
                {
                    durumKarbon = "Acil Durum";
                }
                if (Convert.ToBoolean(Baglanti.State) == false) { Baglanti.Open(); }
                SqlCommand Komut = new SqlCommand();
                Komut.Connection = Baglanti;
                Komut.CommandText = ("insert into Karbon(tarih,deger,durum) values(SYSDATETIME(),'" + receiveddata + "','" + durumKarbon + "')");
                Komut.ExecuteNonQuery();
                verileriGetir3();
                Baglanti.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata var " + ex);
            }

        }
        public void verileriGetir3()
        {
            try
            {
                SqlDataAdapter DataAdapter = new SqlDataAdapter();
                DataTable DataTable = new DataTable();
                if (Convert.ToBoolean(Baglanti.State) == false) { Baglanti.Open(); }

                DataAdapter.SelectCommand = new SqlCommand("Select * from Karbon order by tarih desc", Baglanti);
                DataAdapter.Fill(DataTable);
                dataGridView3.DataSource = DataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu Datagridview da" + ex);
            }
        }
    


    }
}
