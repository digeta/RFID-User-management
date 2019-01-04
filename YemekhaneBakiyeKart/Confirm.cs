using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using YemekhaneBakiyeKart.DB;
using YemekhaneBakiyeKart.Sys;

namespace YemekhaneBakiyeKart
{
    public partial class Confirm : Form
    {
        DBThread db;

        public Confirm()
        {
            InitializeComponent();
        }

        private void Confirm_Load(object sender, EventArgs e)
        {
            this.FormClosing += new FormClosingEventHandler(OnClose);

            db = new DBThread();
            db.OnError += db_OnError;

            this.AcceptButton = btnLogin;
        }

        private void db_OnError(object sender, Error.ErrorOccuredArgs e)
        {
            MessageBox.Show(e.Message);
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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Logins login = new Logins();
            login.Kulad = txtKulad.Text;
            login.Parola = Hashla(txtPass.Text);

            login = db.GirisYap(login);

            if (login.LoginVar)
            {
                db.OnError -= db_OnError;
                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                this.Close();
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.No;

                MessageBox.Show("Kullanıcı adı veya parola yanlış!", "Giriş",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
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
