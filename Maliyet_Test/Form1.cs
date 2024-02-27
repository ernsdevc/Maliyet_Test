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

namespace Maliyet_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=MACHINEX\MSSQLSERVER01;Initial Catalog=TestMaliyet;Integrated Security=True");

        void MalzemeListele()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TBLMalzemeler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            cmbMalzeme.ValueMember = "MalzemeID";
            cmbMalzeme.DisplayMember = "Ad";
            cmbMalzeme.DataSource = dt;
        }

        void UrunListele()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TBLUrunler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        void Kasa()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TBLKasa", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        void Urunler()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TBLUrunler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmbUrun.ValueMember = "UrunID";
            cmbUrun.DisplayMember = "Ad";
            cmbUrun.DataSource = dt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MalzemeListele();
            Urunler();
        }

        private void btnUrunListesi_Click(object sender, EventArgs e)
        {
            UrunListele();
        }

        private void btnMalzemeListesi_Click(object sender, EventArgs e)
        {
            MalzemeListele();
        }

        private void btnKasa_Click(object sender, EventArgs e)
        {
            Kasa();
        }

        private void btnMalzemeEkle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("INSERT INTO TBLMalzemeler (Ad,Stok,Fiyat,Notlar) VALUES(@p1,@p2,@p3,@p4)", baglanti);
            komut.Parameters.AddWithValue("@p1", txtMalzemeAd.Text);
            komut.Parameters.AddWithValue("@p2", decimal.Parse(txtMalzemeStok.Text));
            komut.Parameters.AddWithValue("@p3", decimal.Parse(txtFiyat.Text));
            komut.Parameters.AddWithValue("@p4", txtNot.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Malzeme Sisteme Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MalzemeListele();
        }

        private void btnUrunEkle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("INSERT INTO TBLUrunler (Ad) VALUES(@p1)", baglanti);
            komut.Parameters.AddWithValue("@p1", txtUrunAd.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ürün Sisteme Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UrunListele();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("INSERT INTO TBLHareket (UrunID,MalzemeID,Miktar,Maliyet) VALUES(@p1,@p2,@p3,@p4)", baglanti);
            komut.Parameters.AddWithValue("@p1", cmbUrun.SelectedValue);
            komut.Parameters.AddWithValue("@p2", cmbMalzeme.SelectedValue);
            komut.Parameters.AddWithValue("@p3", decimal.Parse(txtMiktar.Text));
            komut.Parameters.AddWithValue("@p4", decimal.Parse(txtMaliyet.Text));
            komut.ExecuteNonQuery();
            baglanti.Close();

            listBox1.Items.Add(cmbMalzeme.Text + " - " + txtMaliyet.Text);

            MessageBox.Show("Malzeme Ürün Listesine Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtMiktar_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMiktar.Text))
            {
                txtMiktar.Text = "0";
                txtMaliyet.Text = "0";
            }

            baglanti.Open();
            SqlCommand komut = new SqlCommand("SELECT * FROM TBLMalzemeler WHERE MalzemeID=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", cmbMalzeme.SelectedValue);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                txtMaliyet.Text = ((dr.GetDecimal(3) / 1000) * Convert.ToInt32(txtMiktar.Text)).ToString();
            }
            baglanti.Close();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            txtUrunID.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            txtUrunAd.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();

            baglanti.Open();
            SqlCommand komut = new SqlCommand("SELECT SUM(Maliyet) FROM TBLHareket WHERE UrunID=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", cmbMalzeme.SelectedValue);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                txtMFiyat.Text = dr.GetDecimal(0).ToString();
            }
            baglanti.Close();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("UPDATE TBLUrunler SET Ad=@Ad, MFiyat=@MFiyat, SFiyat=@SFiyat, Stok=@Stok WHERE UrunID=@UrunID", baglanti);
            komut.Parameters.AddWithValue("@Ad", txtUrunAd.Text);
            komut.Parameters.AddWithValue("@MFiyat", txtMFiyat.Text);
            komut.Parameters.AddWithValue("@SFiyat", txtSFiyat.Text);
            komut.Parameters.AddWithValue("@Stok", txtUrunStok.Text);
            komut.Parameters.AddWithValue("@UrunID", txtUrunID.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ürün Bilgisi Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UrunListele();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
