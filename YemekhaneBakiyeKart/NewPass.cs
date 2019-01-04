using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using YemekhaneBakiyeKart.DB;
//using YemekhaneBakiyeKart.Events;
using YemekhaneBakiyeKart.Sys;

namespace YemekhaneBakiyeKart
{
    public partial class NewPass : Form
    {
        DBThread db;

        public NewPass()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            String oldPass = txtOldPass.Text;
            String newPass = txtNewPass.Text;
            String newPass2 = txtNewPass2.Text;

            if(newPass != newPass2)
            {
                MessageBox.Show("Yeni parolalar uyuşmuyor !", "Parola Değiştirme",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }

            Logins login = new Logins();
            login.Kulad = Settings.Login.Kulad;
            login.Parola = Hashla(oldPass);

            login = db.GirisYap(login);

            if(login.LoginVar)
            {
                db.YetkiIptalEt(Settings.Login.LoginID, 9);
            }
            else
            {
                MessageBox.Show("Kullanımdaki parola yanlış !", "Parola Değiştirme",
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

        private void NewPass_Load(object sender, EventArgs e)
        {
            this.FormClosing += new FormClosingEventHandler(OnClose);

            db = new DBThread();
            db.OnError += db_OnError;
            this.AcceptButton = btnLogin;

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
