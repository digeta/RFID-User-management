using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using YemekhaneBakiyeKart.DB;
using YemekhaneBakiyeKart.Sys;

namespace YemekhaneBakiyeKart
{
    public partial class ChgPass : Form
    {
        DBThread db;
        public Int32 userId = 0;
        public String user = "";

        public ChgPass()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            DialogResult msjSonuc = new DialogResult();
            msjSonuc = MessageBox.Show(user + " , kullanıcısına yeni parola atamak istiyor musunuz ?", "Parola Atama",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (msjSonuc == DialogResult.Yes)
            {
                String adminPass = txtAdminPass.Text;
                String newPass = txtNewPass.Text;
                String newPass2 = txtNewPass2.Text;

                if (newPass != newPass2)
                {
                    MessageBox.Show("Yeni parolalar uyuşmuyor !", "Parola Atama",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                    return;
                }

                Logins login = new Logins();
                login.Kulad = Settings.Login.Kulad;
                login.Parola = Hashla(adminPass);

                login = db.GirisYap(login);

                if (login.LoginVar)
                {
                    if (db.KullaniciParolaAta(userId, Hashla(newPass)) == 100)
                    {
                        MessageBox.Show("Kullanıcı parolası değiştirildi", "Parola Atama",
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        MessageBox.Show("Parola atamada hata !", "Parola Atama",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
                else
                {
                    MessageBox.Show("Yetkili parolası yanlış !", "Parola Atama",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                    return;
                }

                this.Close();
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

        private void ChgPass_Load(object sender, EventArgs e)
        {
            this.FormClosing += new FormClosingEventHandler(OnClose);

            db = new DBThread();
            db.OnError += db_OnError;
            this.AcceptButton = btnLogin;

            lblKulad.Text = user;

            if (Control.IsKeyLocked(Keys.CapsLock)) lblCaps.Visible = true;
        }

        private void db_OnError(object sender, Error.ErrorOccuredArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void OnClose(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
