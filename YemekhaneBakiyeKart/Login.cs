using System;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using YemekhaneBakiyeKart.DB;
using YemekhaneBakiyeKart.Events;
using YemekhaneBakiyeKart.Sys;

namespace YemekhaneBakiyeKart
{
    public partial class Login : Form
    {
        DBThread db;
        Main main;

        public Login()
        {
            Logger.OnMessageChanged += new EventHandler<LogArgs>(Login_OnLogMsgChanged);
            InitializeComponent();
        }

        private void Login_OnLogMsgChanged(object sender, LogArgs e)
        {
            MessageBox.Show(e.MessageStr);
        }

        private void Login_Load(object sender, EventArgs e)
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
                login.Yetkiler = db.KullaniciYetkiler(login.LoginID);
                Settings.Login = login;
                
                DataRow[] yetkiler = login.Yetkiler.Select("YETKI_ID = 1");

                if(yetkiler.Length <= 0)
                {
                    MessageBox.Show("Giriş yetkiniz bulunmamaktadır !", "Giriş",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                    return;
                }
				
                this.Hide();

                Logger.OnMessageChanged -= Login_OnLogMsgChanged;
                db.OnError -= db_OnError;

                main = new Main();
                main.Show();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya parola yanlış!", "Giriş", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                if (Control.IsKeyLocked(Keys.CapsLock)) lblCaps.Visible = true;
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
            finally
            {
                Environment.Exit(0);
            }
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
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
