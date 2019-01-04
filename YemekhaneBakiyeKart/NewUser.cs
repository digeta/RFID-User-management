using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using YemekhaneBakiyeKart.DB;
using YemekhaneBakiyeKart.Misc;

namespace YemekhaneBakiyeKart
{
    public partial class NewUser : Form
    {
        DBThread dbax;

        public NewUser()
        {
            InitializeComponent();
        }

        private void btn_Kaydet_yeni_Click(object sender, EventArgs e)
        {
            Int64 tckimlik = 0;
            Int64.TryParse(txtTckimlik_yeni.Text, out tckimlik);

            if(tckimlik <= 0)
            {
                MessageBox.Show("Girilen TC. kimlik geçersiz", "Yeni kullanıcı",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            String ad = txtAd_yeni.Text.Trim();
            if (!Regex.IsMatch(ad, @"^[\p{L} \.]+$"))
            {
                MessageBox.Show("Girilen Ad geçersiz", "Yeni kullanıcı",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            String soyad = txtSoyad_yeni.Text.Trim();
            if (!Regex.IsMatch(soyad, @"^[\p{L} \.]+$"))
            {
                MessageBox.Show("Girilen Soyad geçersiz", "Yeni kullanıcı",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            
            
            KulClass kulClass = new KulClass();
            kulClass.TCKimlik = tckimlik;
            kulClass.Ad = ad;
            kulClass.Soyad = soyad;
            kulClass.Username = txtUser_yeni.Text.Trim();
            kulClass.Password = Hashla(txtPass_yeni.Text.Trim());
            kulClass.Yemekhane = Convert.ToInt32(cmbCafeList.SelectedValue);

            if(kulClass.Username == "" | kulClass.Password == "")
            {
                MessageBox.Show("Kullanıcı adı veya parola boş bırakılamaz", "Yeni kullanıcı",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            if (kulClass.Yemekhane <= 0)
            {
                MessageBox.Show("Yemekhane seçmedini", "Yeni kullanıcı",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            if (dbax.KullaniciEkle(kulClass) == 100)
            {
                MessageBox.Show("Kullanıcı eklendi", "Yeni kullanıcı",
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);

                this.Close();
            }
            else
            {
                MessageBox.Show("HATA : Kullanıcı eklenemedi !", "Yeni kullanıcı",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
        }

        public String Hashla(String password)
        {
            HashAlgorithm hashAlgorithm = (HashAlgorithm)new SHA1Managed();
            using (hashAlgorithm)
            {
                byte[] bytes = Encoding.Default.GetBytes(password);
                return BitConverter.ToString(hashAlgorithm.ComputeHash(bytes));
            }
        }

        private void NewUser_Load(object sender, EventArgs e)
        {
            dbax = new DBThread();
            dbax.OnError += dbax_OnError;

            TanimYemekhaneDoldur();

            if (Control.IsKeyLocked(Keys.CapsLock)) lblCaps.Visible = true;
        }

        private void dbax_OnError(object sender, Error.ErrorOccuredArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void TanimYemekhaneDoldur()
        {
            try
            {
                cmbCafeList.DataSource = null;
                cmbCafeList.Items.Clear();


                DataTable dt = dbax.YemekhanelerGetir();

                cmbCafeList.DisplayMember = "YEMEKHANE";
                cmbCafeList.ValueMember = "YEMEKHANE_ID";

                DataRow dr = dt.NewRow();
                dr["YEMEKHANE"] = "Lütfen Seçiniz";
                dr["YEMEKHANE_ID"] = 0;

                dt.Rows.InsertAt(dr, 0);

                cmbCafeList.DataSource = dt;
                cmbCafeList.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yemekhane listesi getirilemedi");
                return;
            }
        }

        private void txtPass_yeni_KeyDown(object sender, KeyEventArgs e)
        {
            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                lblCaps.Visible = true;
            }
            else
            {
                lblCaps.Visible = false;
            }
        }
    }
}
