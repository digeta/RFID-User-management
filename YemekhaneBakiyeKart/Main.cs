using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using YemekhaneBakiyeKart.Cihazlar;
using YemekhaneBakiyeKart.DB;
using YemekhaneBakiyeKart.Events;
using YemekhaneBakiyeKart.Misc;
using YemekhaneBakiyeKart.Sys;

namespace YemekhaneBakiyeKart
{
    public partial class Main : Form
    {
        private delegate void uiStatusControl(Control control, Boolean status);
        private delegate void UImsgDeleg(String msg, Color backColor, Color fontColor, Logger.Messages message);

        private delegate void frmDoldur(KisiClass kisiClass);
        private delegate void frmResetle();

        private CihazClass cihaz;
        private DBThread dba;

        private KisiClass kisiClass;

        private System.Timers.Timer _cleanTimer;
        private Int32 _elapsed = 0;

        private Boolean _startup = true;
        private Boolean _cleaning = false;

        private Boolean _elleTCgirildi = false;

        private Boolean _kartOkumaAktif = false;

        Version v = Assembly.GetExecutingAssembly().GetName().Version;

        NumberFormatInfo currencyFormat = new CultureInfo(CultureInfo.CurrentCulture.ToString()).NumberFormat;

        private DataTable dtIslemler = new DataTable();

        public Main()
        {
            Logger.OnMessageChanged += new EventHandler<LogArgs>(OnLogMsgChanged);
            InitializeComponent();
        }

        #region "Events"
        private void OnClose(object sender, EventArgs e)
        {
            try
            {
                cihaz.Terminating = true;
                while (!cihaz.Terminating)
                {

                }
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

        private void UIStatusControl(Control control, Boolean status)
        {
            control.Enabled = status;
        }

        void cihaz_OnYeniKartOkundu(object sender, YeniKartOkunduArgs e)
        {
            _cleanTimer.Stop();
            _elapsed = 0;
            _cleanTimer.Start();

            this.BeginInvoke(new uiStatusControl(UIStatusControl), tabControl, true);

            try
            {
                if (_kartOkumaAktif)
                {
                    _kartOkumaAktif = false;

                    this.BeginInvoke(new frmResetle(ResetFormBakiye), null);
                    this.BeginInvoke(new frmResetle(ResetFormKayit), null);

                    KisiClass kisiClass = new KisiClass();
                    kisiClass.KartNo = e.CihazVerisi.KartNo;
                    kisiClass.BakiyeOnceki = e.CihazVerisi.Bakiye;
                    kisiClass.BakiyeKarticindeki = e.CihazVerisi.Bakiye;

                    this.BeginInvoke(new frmDoldur(FormDoldur), kisiClass);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Kart okundu",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void GridKartEsle()
        {
            try
            {
                int rowCount = gridKartlar.Rows.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        Int64 deger = Convert.ToInt64(gridKartlar.Rows[i].Cells["colKART_ID"].Value);
                        if (deger == kisiClass.KartNo)
                        {
                            gridKartlar.Rows[i].Selected = true;
                            gridKartlar.Refresh();
                        }
                        else
                        {
                            gridKartlar.Rows[i].Selected = false;
                            gridKartlar.Refresh();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Kart okundu",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void dba_OnError(object sender, Error.ErrorOccuredArgs e)
        {
            Logger.Method = e.Method;
            Logger.MessageStr = e.Message;
        }

        private void OnLogMsgChanged(object sender, LogArgs e)
        {
            if (e.ShowMsgBox)
            {
                MessageBox.Show(e.MessageStr);
            }
            else
            {
                this.BeginInvoke(new UImsgDeleg(BilgiText), e.MessageStr, e.BackColor, e.FontColor, e.Message);
            }
        }

        private void BilgiText(String msg, Color backColor, Color fontColor, Logger.Messages message)
        {
            Int32 msgNum = Convert.ToInt32(Convert.ToInt32(message).ToString().Substring(0,1));
            TextBox txtBox;

            switch(msgNum)
            {
                case 1:
                case 2:
                case 3:
                    txtBox = txtKartBilgi;
                    break;

                case 4 :
                    txtBox = txtKartBilgi_kayit;
                    break;

                default :
                    txtBox = txtKartBilgi;
                    break;
            }

            txtBox.BackColor = backColor;
            txtBox.ForeColor = fontColor;
            txtBox.Text = msg;
        }

        public Control FindControl(Control container, string tag)
        {
            if (container.Tag == tag) return container;

            foreach (Control ctrl in container.Controls)
            {
                Control foundCtrl = FindControl(ctrl, tag);

                if (foundCtrl != null) return foundCtrl;
            }

            return null;
        }
        #endregion

        private void Main_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new uiStatusControl(UIStatusControl), tabControl, false);
            this.FormClosing += new FormClosingEventHandler(OnClose);
            txtKartNo_kart.KeyDown += txtKartNo_kart_KeyDown;

            _cleanTimer = new System.Timers.Timer();
            _cleanTimer.Elapsed += new System.Timers.ElapsedEventHandler(CleantimerElapsed);
            _cleanTimer.Interval = 1000;

            lblServer.Text = Settings.ServerType;
            lblVersion.Text = "Versiyon : " + Convert.ToString(v.Major) + "." + Convert.ToString(v.Minor);

            currencyFormat.CurrencyNegativePattern = 1;
            currencyFormat.CurrencyPositivePattern = 1;

            cmbListeSay.SelectedIndex = 1;

            dateArama.Value = DateTime.Now;

            dateHarYukilk.Value = DateTime.Now;
            dateHarYukSon.Value = DateTime.Now;

            dateHarHarilk.Value = DateTime.Now;
            dateHarHarSon.Value = DateTime.Now;
            
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 7");

            if (yetkiler.Length <= 0)
            {
                tabControl.TabPages.RemoveAt(3);
            }
            
            Boolean dbAktif;
            dba = new DBThread();
            dba.OnError += dba_OnError;

            lblKulad.Text = Settings.Login.Kulad;

            dtIslemler = dba.KullaniciIslemler(Settings.Login.LoginID, out dbAktif);

            IslemListele();

            gridKartlar.AutoGenerateColumns = false;
            
            if (dbAktif)
            {
                cihaz = new CihazClass();
                cihaz.OnCihazBaglandi += cihaz_OnCihazBaglandi;
                cihaz.OnYeniKartOkundu += cihaz_OnYeniKartOkundu;

                Thread cihazThread = new Thread(new ThreadStart(cihaz.Init));
                cihazThread.Start();

                Thread nedenThread = new Thread(new ThreadStart(TanimIptalDoldur));
                nedenThread.Start();

                Thread perThread = new Thread(new ThreadStart(TanimPersonelDoldur));
                perThread.Start();

                Thread fakThread = new Thread(new ThreadStart(TanimFakulteDoldur));
                fakThread.Start();

                Thread yetkiThread = new Thread(new ThreadStart(TanimYetkiDoldur));
                yetkiThread.Start();
            }
            else
            {
                MessageBox.Show("Veritabanı Bağlantısında Sorun Var ! ");
                Application.Exit();
            }            
        }

        private void txtKartNo_kart_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F2)
            {
                txtKartNo_kart.ReadOnly = false;
            }
        }

        private void cihaz_OnCihazBaglandi(object sender, CihazBaglandiArgs e)
        {
            this.BeginInvoke(new uiStatusControl(UIStatusControl), tabControl, true);
        }
        #region "Bakiye İşlemleri"
        #region "Butonlar"
        private void btnBakiyeYukle_Click(object sender, EventArgs e)
        {
            
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 2");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Bakiye Yükleme",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            BakiyeYukleme();
        }

        private void btnKartAktivasyon_Click(object sender, EventArgs e)
        {
            KartAktifEt();
        }

        private void btnArama_Click(object sender, EventArgs e)
        {
            btnSonucYok.Visible = false;
            IslemListele();
            YuklemeBul();
        }

        private void btnKartBul_Click(object sender, EventArgs e)
        {
            _kartOkumaAktif = true;

            txtTckimlikAra.Text = "";

            btnKisiDuzenle.Visible = false;
            grupKartBilgi.Visible = false;

            ResetFormBakiye();
            ResetFormKayit();
            ResetFormBilgi();

            KartBul();

            _cleanTimer.Stop();
            _elapsed = 0;
            _cleanTimer.Start();
        }

        private void btnBakiyeIptal_Click(object sender, EventArgs e)
        {
            
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 3");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Bakiye Yükleme",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            Int64 yuklemeID = -1;

            if (gridIslemler.Rows.Count > 0)
            {
                Int64.TryParse(Convert.ToString(gridIslemler.SelectedRows[0].Cells["colYuklemeId"].Value), out yuklemeID);
            }

            if (yuklemeID > 0)
            {
                DialogResult msjSonuc = new DialogResult();
                msjSonuc = MessageBox.Show(this, "Seçtiğiniz yükleme iptal edilecek \r\nDevam etmek istiyor musunuz?", "Bakiye İptal",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                Int32 result = 0;

                if (msjSonuc == DialogResult.Yes)
                {
                    Confirm confirm = new Confirm();
                    DialogResult confirmResult = confirm.ShowDialog();

                    if (confirmResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        result = dba.YuklemeIptal(yuklemeID);
                    }
                }

                if (result > 0)
                {
                    MessageBox.Show("Seçtiğiniz bakiye iptal edildi ", "Bakiye İptal", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);

                    IslemListele();
                }
                else if (result < 0)
                {
                    MessageBox.Show("Hata ! \r\nSeçtiğiniz bakiye iptal edilemedi ! ", "Bakiye İptal", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                MessageBox.Show("Hata ! \r\nKayıt seçmediniz ! ", "Bakiye İptal", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void btnB1_Click(object sender, EventArgs e)
        {
            txtBakiyeYuklenecek.Text = "1";
        }

        private void btnB5_Click(object sender, EventArgs e)
        {
            txtBakiyeYuklenecek.Text = "5";
        }

        private void btnB10_Click(object sender, EventArgs e)
        {
            txtBakiyeYuklenecek.Text = "10";
        }

        private void btnB15_Click(object sender, EventArgs e)
        {
            txtBakiyeYuklenecek.Text = "15";
        }

        private void btnB20_Click(object sender, EventArgs e)
        {
            txtBakiyeYuklenecek.Text = "20";
        }

        private void btnB1_5_Click(object sender, EventArgs e)
        {
            txtBakiyeYuklenecek.Text = "1,5";
        }

        private void btnKartAra_Click(object sender, EventArgs e)
        {

        }

        private void btnKisiDuzenle_Click(object sender, EventArgs e)
        {
            
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 6");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kişi Bilgileri",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            if (cmbPersonelTip_bilgi.SelectedIndex > 0)
            {
                KisiClass kisi = new KisiClass();

                Int64 tckimlik = 0;
                Int64.TryParse(txtTckimlikAra.Text, out tckimlik);

                kisi.TCKimlik = tckimlik;
                kisi.KartTip = Convert.ToInt32(cmbPersonelTip_bilgi.SelectedValue);
                kisi.TarifeId = Convert.ToInt32(cmbTarifeTip_bilgi.SelectedValue);

                DialogResult msjSonuc = new DialogResult();
                msjSonuc = MessageBox.Show("Kişi bilgileri değiştirilecek \r\nDevam etmek istiyor musunuz?", "Kart Tanımlama",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                Int32 result = 0;

                if (msjSonuc == DialogResult.Yes)
                {
                    result = dba.PersonelGuncelle(kisi);
                }

                if (result > 0)
                {
                    MessageBox.Show("Kişi bilgileri değiştirildi. ", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    TCdenAra(kisi.TCKimlik);
                }
                else if (result < 0)
                {
                    MessageBox.Show("Hata ! \r\nKişi bilgileri değiştirilemedi ! ", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
        }
        #endregion

        #region "KartOkuma"
        private void BakiyeYukleme()
        {
            _elapsed = 0;
            _cleanTimer.Stop();

            Decimal bakiye = 0;
            Boolean bakiyeGecerlimi = Decimal.TryParse(txtBakiyeYuklenecek.Text, out bakiye);

            if (bakiyeGecerlimi && bakiye > 0)
            {
                if (bakiye < 101)
                {
                    if (!kisiClass.TanimsizKart && kisiClass.TCKimlik > 0 && kisiClass.KartNo > 0)
                    {
                        kisiClass.BakiyeYuklenecek = bakiye;
                        kisiClass.BakiyeYeni = kisiClass.BakiyeOnceki + kisiClass.BakiyeYuklenecek;

                        if (dba.BakiyeYukle(kisiClass) == 100)
                        {
                            Logger.Message = Logger.Messages.BalanceDeposit;

                            if (chkFis.Checked)
                            {
                                Fis fis = new Fis();
                                if (fis.FisHazirla(kisiClass))
                                {
                                    fis.FisVer();
                                }
                            }
                        }
                        else
                        {
                            Logger.Message = Logger.Messages.BalanceDepositFailed;
                        }
                    }
                    else
                    {
                        Logger.Message = Logger.Messages.CardInvalid;
                    }
                }
                else
                {
                    Logger.Message = Logger.Messages.BalanceLimitReached;
                }
            }
            else
            {
                Logger.Message = Logger.Messages.BalanceInvalid;
            }
            _elapsed = 0;
            _cleanTimer.Start();

            this.BeginInvoke(new frmDoldur(FormDoldur), kisiClass);
            this.BeginInvoke(new uiStatusControl(UIStatusControl), btnBakiyeYukle, false);
        }

        private void YuklemeBul()
        {
            DataTable sonucDt = dtIslemler.Clone();

            String araAdSoyad = txtAdArama.Text;
            Int64 araTckimlik = 0;
            Int64.TryParse(txtTcArama.Text, out araTckimlik);
            
            DateTime ilkTarih = new DateTime(dateArama.Value.Year, dateArama.Value.Month, dateArama.Value.Day);

            DateTime sonTarih = ilkTarih.AddHours(23);
            sonTarih = sonTarih.AddMinutes(59);

            DataRow[] drow = dtIslemler.Select("ADSOYAD='" + araAdSoyad + "' OR TCKIMLIK='" + araTckimlik + "' OR (TARIH >='" + ilkTarih + "' AND TARIH <='" + sonTarih + "')");
            drow.CopyToDataTable(sonucDt,LoadOption.PreserveChanges);
            gridIslemler.DataSource = sonucDt;

            if(sonucDt.Rows.Count <= 0)
            {
                btnSonucYok.Visible = true;
            }
        }

        private void btnSonucYok_Click(object sender, EventArgs e)
        {
            btnSonucYok.Visible = false;
            IslemListele();
        }

        private void KartAktifEt()
        {
            Boolean kartAktifEdildi = dba.KartAktivasyon(kisiClass);

            if (kartAktifEdildi)
            {
                /*
                Boolean bakiyeSifirla = cihaz.KartBakiyeGuncelle(fsmVerisi, 0) ? true : false;

                if (bakiyeSifirla)
                {
                    Logger.Message = Logger.Messages.CardActivated;
                }
                else
                {
                    Logger.Message = Logger.Messages.CardActivationFailed;
                }
                */
            }

            btnKartAktivasyon.Visible = false;
            this.BeginInvoke(new frmDoldur(FormDoldur), kisiClass);
        }

        private void KartBul()
        {
            this.BeginInvoke(new uiStatusControl(UIStatusControl), tabControl, false);

            Thread dinleThread = new Thread(new ThreadStart(cihaz.Dinle));
            dinleThread.Start();
        }

        private void IslemListele()
        {
            try
            {
                Boolean dbAktif;
                dtIslemler = dba.KullaniciIslemler(Settings.Login.LoginID, out dbAktif);

                if (dtIslemler.Rows.Count > 0)
                {
                    if (cmbListeSay.SelectedIndex > 0)
                    {
                        DataTable sonucDt = dtIslemler.Clone();
                        dtIslemler.AsEnumerable().Take(Convert.ToInt32(cmbListeSay.Items[cmbListeSay.SelectedIndex])).CopyToDataTable(sonucDt, LoadOption.PreserveChanges);
                        gridIslemler.DataSource = sonucDt;
                    }
                    else
                    {
                        gridIslemler.DataSource = dtIslemler;
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void cmbListeSay_SelectedIndexChanged(object sender, EventArgs e)
        {
            IslemListele();
        }
        #endregion

        #region "UI Events"
        private void CleantimerElapsed(Object sender, EventArgs e)
        {
            _elapsed++;

            if (_elapsed >= 30)
            {
                kisiClass = new KisiClass();

                this.BeginInvoke(new frmResetle(ResetFormBakiye), null);

                Logger.Message = Logger.Messages.DeviceReady;

                _elapsed = 0;
                _cleanTimer.Stop();
            }
        }

        private void FsmBilgiText(String msg, Color backColor, Color fontColor)
        {
            txtKartBilgi.BackColor = backColor;
            txtKartBilgi.ForeColor = fontColor;
            txtKartBilgi.Text = msg;
            txtKartNo.Text = "";

            txtKartBilgi_kisi.BackColor = backColor;
            txtKartBilgi_kisi.ForeColor = fontColor;
            txtKartBilgi_kisi.Text = msg;

            txtKartBilgi_kayit.BackColor = backColor;
            txtKartBilgi_kayit.ForeColor = fontColor;
            txtKartBilgi_kayit.Text = msg;
        }

        private void gridIslemler_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void gridKartlar_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }
        #endregion

        #region "UI Metodlar"
        private void Counter(String count)
        {
            lblTimer.Text = count;
        }

        private void FormDoldur(KisiClass kisi)
        {
            kisiClass = dba.KartKontrol(kisi);

            txtKartNo.Text = Convert.ToString(kisiClass.KartNo);
            txtKartNo_kart.Text = Convert.ToString(kisiClass.KartNo);

            if (!kisiClass.TanimsizKart)
            {
                if (kisiClass.KartIptal)
                {
                    txtKartDurum.Text = "Kart İptal Edilmiş ! ";
                    txtKartDurum.ForeColor = Color.White;
                    txtKartDurum.BackColor = Color.Red;
                }
                else
                {
                    if (!kisiClass.KartAktifEdildi)
                    {
                        txtKartDurum.Text = "Kart Aktif Değil ! ";
                        txtKartDurum.ForeColor = Color.White;
                        txtKartDurum.BackColor = Color.Red;

                        btnKartAktivasyon.Visible = true;
                    }
                    else
                    {
                        txtKartDurum.Text = "Kart Aktif ";
                        txtKartDurum.ForeColor = Color.White;
                        txtKartDurum.BackColor = Color.Green;

                        txtBakiyeYuklenecek.ReadOnly = false;
                        txtBakiyeYuklenecek.Focus();

                        btnBakiyeYukle.ForeColor = Color.DodgerBlue;
                        btnBakiyeYukle.Enabled = true;
                    }
                }

                txtAdSoyad.Text = kisiClass.Ad + " " + kisiClass.Soyad;
                txtTarife.Text = kisiClass.Tarife;

                txtKartBitis.Text = kisiClass.KartGecerlilikBitis.ToString();
                txtBakiyeYuklenecek.Text = "";
                txtBakiyeOnceki.Text = String.Format(currencyFormat, "{0}", kisiClass.BakiyeOnceki);
            }
            else
            {
                txtKartDurum.Text = "Tanımsız Kart ! \r\n Kart Tanımı Yapınız";
                txtKartDurum.ForeColor = Color.White;
                txtKartDurum.BackColor = Color.Red;
            }

            IslemListele();
        }

        private void FormResetle()
        {            
            tabControl.Enabled = false;

            ResetFormBakiye();
            ResetFormKayit();
            ResetFormBilgi();

            tabControl.Enabled = true;
        }

        private void ResetFormBakiye()
        {
            _cleaning = true;

            btnKartAktivasyon.Visible = false;
            btnBakiyeYukle.Enabled = false;

            txtKartNo.Text = "";            
            txtTarife.Text = "";
            txtAdSoyad.Text = "";
            txtKartBitis.Text = "";
            txtBakiyeKart.Text = "";            
            txtBakiyeOnceki.Text = "";
            txtBakiyeYuklenecek.Text = "";

            txtBakiyeYuklenecek.ReadOnly = true;

            txtKartDurum.Text = "";
            txtKartDurum.BackColor = Color.White;

            Logger.Message = Logger.Messages.DeviceReady;

            _cleaning = false;
        }

        private void ResetFormBilgi()
        {
            _cleaning = true;

            txtKartNo_kart.Text = "";
            txtAdsoyad_bilgi.Text = "";
            txtKisitip_bilgi.Text = "";
            txtTarifetip_bilgi.Text = "";            

            cmbPersonelTip_bilgi.SelectedIndex = 0;
            cmbNedenIptal.SelectedIndex = 0;

            txtKartBilgi_kisi.Text = "";
            txtKartBilgi_kisi.BackColor = Color.White;

            gridKartlar.DataSource = null;
            gridKartlar.Refresh();

            btnKisiDuzenle.Visible = false;
            grupKartBilgi.Visible = false;

            _cleaning = false;
        }

        private void ResetFormKayit()
        {
            _cleaning = true;

            txtAd_yeni.Text = "";
            txtSoyad_yeni.Text = "";

            cmbPersonelTip_yeni.SelectedIndex = 0;
            cmbFakulte_yeni.SelectedIndex = 0;

            txtKartBilgi_kayit.Text = "";
            txtKartBilgi_kayit.BackColor = Color.White;

            _cleaning = false;
        }
        #endregion

        private void btnKartBakiyeYaz_Click(object sender, EventArgs e)
        {
            CihazVeri cihazVeri = new CihazVeri();
            cihazVeri.KartNo = kisiClass.KartNo;
            
            cihaz.FsmBakiyeGuncelle(cihazVeri, 15);
        }
        #endregion

        #region "Personel Tanım"
        #region "Metodlar"
        private Boolean TcDogrula(String tckimlik)
        {
            Boolean result = true;

            if (tckimlik.Length != 11)
            {
                result = false;
            }
            else
            {
                if (tckimlik.Substring(0, 1) == "0")
                {
                    result = false;
                }
            }

            if (result)
            {
                String[] tcdizi = new String[11];

                for (int i = 0; i < 11; i++)
                {
                    tcdizi[i] = tckimlik.Substring(i, 1);
                }

                int saglayici_1 = Convert.ToInt32(tcdizi[9]);
                int saglayici_2 = Convert.ToInt32(tcdizi[10]);

                int part_1 = 0;
                for (int i = 0; i < 9; i += 2)
                {
                    part_1 += Convert.ToInt32(tcdizi[i]);
                }
                part_1 = part_1 * 7;

                int part_2 = 0;
                for (int i = 1; i < 8; i += 2)
                {
                    part_2 += Convert.ToInt32(tcdizi[i]);
                }

                if (saglayici_1 != (part_1 - part_2) % 10)
                {
                    result = false;
                }

                int part_3 = 0;
                for (int i = 0; i < 10; i++)
                {
                    part_3 += Convert.ToInt32(tcdizi[i]);
                }

                if (saglayici_2 != part_3 % 10)
                {
                    result = false;
                }
            }

            return result;
        }

        private void TCdenAra(Int64 tckimlik)
        {
            Boolean kisiMevcut = false;
            Boolean kisiMevcutEkampus = false;

            DataTable dt = new DataTable();

            dt = dba.PersonelGetir(tckimlik, out kisiMevcut);

            if (!kisiMevcut)
            {
                dt = dba.PersonelAra(tckimlik, out kisiMevcutEkampus);
            }

            if (kisiMevcutEkampus)
            {
                Logger.Message = Logger.Messages.PersonFoundEkampus;

                txtTckimlik_yeni.Text = Convert.ToString(dt.Rows[0]["TCKIMLIK"]);
                txtAd_yeni.Text = Convert.ToString(dt.Rows[0]["AD"]);
                txtSoyad_yeni.Text = Convert.ToString(dt.Rows[0]["SOYAD"]);
                cmbPersonelTip_yeni.SelectedValue = Convert.ToInt32(dt.Rows[0]["KART_TIP"]);
                cmbTarifeTip_yeni.SelectedValue = Convert.ToInt32(dt.Rows[0]["TARIFE_ID"]);
                cmbFakulte_yeni.SelectedValue = Convert.ToInt32(dt.Rows[0]["FAKULTE_ID"]);
                cmbBolum_yeni.SelectedValue = Convert.ToInt32(dt.Rows[0]["BOLUM_ID"]);

                _elleTCgirildi = false;
                tabKisiler.SelectTab(1);
            }

            gridKartlar.DataSource = dba.KartlarGetir(tckimlik);
            gridKartlar.Refresh();

            int rowCount = gridKartlar.Rows.Count;
            if (rowCount > 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    String deger = Convert.ToString(gridKartlar.Rows[i].Cells["coliptal"].Value);
                    if (deger == "Evet")
                    {
                        gridKartlar.Rows[i].DefaultCellStyle.BackColor = Color.Firebrick;
                        gridKartlar.Rows[i].Selected = false;
                    }
                    else
                    {
                        gridKartlar.Rows[i].Selected = false;
                    }
                }
            }

            if (kisiMevcut)
            {
                txtTckimlikAra.Text = Convert.ToString(dt.Rows[0]["TCKIMLIK"]);
                txtAdsoyad_bilgi.Text = Convert.ToString(dt.Rows[0]["ADSOYAD"]);
                txtKisitip_bilgi.Text = Convert.ToString(dt.Rows[0]["PERTIP_AD"]);
                txtTarifetip_bilgi.Text = Convert.ToString(dt.Rows[0]["TARIFE"]);
                txtKartBilgi_kisi.Text = "";
                txtBakiyeYedek.Text = Convert.ToString(dt.Rows[0]["BAKIYE_YEDEK"]);

                cmbPersonelTip_bilgi.SelectedValue = Convert.ToInt32(dt.Rows[0]["KART_TIP"]);
                cmbTarifeTip_bilgi.SelectedValue = Convert.ToInt32(dt.Rows[0]["TARIFE_ID"]);

                btnKisiDuzenle.Visible = true;
                grupKartBilgi.Visible = true;

                _elleTCgirildi = false;
            }
            else
            {
                if (!kisiMevcutEkampus)
                {
                    Logger.Message = Logger.Messages.PersonNotFound;

                    txtTckimlik_yeni.Text = tckimlik.ToString();
                    tabKisiler.SelectTab(1);
                }
            }

            Decimal yedekBakiye = 0;
            Decimal.TryParse(txtBakiyeYedek.Text, out yedekBakiye);

            if (yedekBakiye > 0)
            {
                lblYedekBakiye.ForeColor = Color.Red;
            }
            else
            {
                lblYedekBakiye.ForeColor = Color.Black;
            }
        }

        private void PersonelKaydet(KisiClass kisi)
        {
            DBThread db = new DBThread();

            Int32 result = db.PersonelEkle(kisi);

            if (result > 0)
            {
                MessageBox.Show("Yeni Kişi Eklendi, Kart Tanımı Yapınız", "Kişi Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);

                txtTckimlik_yeni.Text = "";
                txtAd_yeni.Text = "";
                txtSoyad_yeni.Text = "";
                cmbPersonelTip_yeni.SelectedIndex = 0;
                cmbFakulte_yeni.SelectedIndex = 0;

                txtTckimlikAra.Text = kisi.TCKimlik.ToString();
                tabKisiler.SelectTab(0);

                TCdenAra(kisi.TCKimlik);
            }
            else
            {
                MessageBox.Show("Hata ! Bu T.C. Kimlik , Kart Tipi ve Tarife ile yapılmış kayıt mevcut.", "Kişi Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void KartKaydet()
        {
            Int64 kartNo = 0;
            Int64.TryParse(txtKartNo_kart.Text, out kartNo);

            Int64 tckimlik = 0;
            Int64.TryParse(txtTckimlikAra.Text, out tckimlik);

            if (kartNo <= 0)
            {
                MessageBox.Show("Hata ! Kart Numarası yok, Kart Bul' a basarak kart okutunuz.", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                KisiClass kisi = new KisiClass();
                kisi.TCKimlik = tckimlik;
                kisi.KartNo = kartNo;

                kisi.KartSay = gridKartlar.Rows.Count;

                Int32 result = 0;

                if (kisi.KartSay > 1)
                {
                    DialogResult msjSonuc = new DialogResult();
                    msjSonuc = MessageBox.Show("Kişiye ait bütün kartlar için tek tarife kullanılır. \r\nDevam etmek istiyor musunuz?", "Kart Tanımlama",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (msjSonuc == DialogResult.Yes)
                    {
                        result = dba.KartEkle(kisi);
                    }
                }
                else
                {
                    result = dba.KartEkle(kisi);
                }

                if (result > 0)
                {
                    MessageBox.Show("Yeni Kart Tanımlandı. \r\nKart Aktivasyonu yapınız", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    TCdenAra(kisi.TCKimlik);
                }
                else if (result < 0)
                {
                    MessageBox.Show("Hata ! \r\nBu Kart Önceden Tanımlanmış", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
        }

        private void KartIptal(KisiClass kisi)
        {
            Int32 result = dba.KartIptal(kisi);

            if (result > 0)
            {
                MessageBox.Show("Seçilen Kart İptal Edildi", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                TCdenAra(kisi.TCKimlik);
            }
            else
            {
                MessageBox.Show("Hata ! Kart İptal Edilemedi\r\nKart zaten iptal edilmiş olabilir", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void KartIptalKaldir(KisiClass kisi)
        {
            Int32 result = dba.KartIptalKaldir(kisi);

            if (result > 0)
            {
                MessageBox.Show("Seçilen Kartın İptali Kaldırıldı", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                TCdenAra(kisi.TCKimlik);
            }
            else
            {
                MessageBox.Show("Hata ! Kart İptali Kaldırılamadı", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void KartBakiyeAktar(KisiClass kisi)
        {
            Int32 result = dba.KartBakiyeAktar(kisi);

            if (result > 0)
            {
                MessageBox.Show("Seçilen Karta Bakiye Aktarımı Yapıldı", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                TCdenAra(kisi.TCKimlik);
            }
            else
            {
                MessageBox.Show("Hata ! Bakiye Aktarılamadı\r\nKart İptal edilmiş veya Aktif edilmemiş olabilir", "Kart Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
        #endregion

        #region "UI Events"
        private void cmbPersonelTip_yeni_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 karttip = Convert.ToInt32(cmbPersonelTip_yeni.SelectedValue);
            if (karttip > 0 && !_startup)
            {
                TanimTarifeDoldur(karttip);
            }
            else
            {
                cmbTarifeTip_yeni.DataSource = null;
                cmbTarifeTip_yeni.Items.Clear();
            }
        }

        private void cmbPersonelTip_bilgi_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 karttip = Convert.ToInt32(cmbPersonelTip_bilgi.SelectedValue);
            if (karttip > 0 && !_startup)
            {
                TanimTarifeBilgiDoldur(karttip);
            }
            else
            {
                cmbTarifeTip_bilgi.DataSource = null;
                cmbTarifeTip_bilgi.Items.Clear();
            }
        }

        private void cmbFakulte_yeni_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 fakulteId = Convert.ToInt32(cmbFakulte_yeni.SelectedValue);
            if (fakulteId > 0 && !_startup)
            {
                TanimBolumDoldur(fakulteId);
            }
            else
            {
                cmbBolum_yeni.DataSource = null;
                cmbBolum_yeni.Items.Clear();
            }
        }

        private void txtBakiyeYuklenecek_TextChanged(object sender, EventArgs e)
        {
            _cleanTimer.Stop();
            _elapsed = 0;
            _cleanTimer.Start();
        }
        #endregion

        #region "UI Metodlar"
        private void TanimIptalDoldur()
        {
            cmbNedenIptal.DataSource = null;
            cmbNedenIptal.Items.Clear();

            DataTable dt = new DataTable();

            dt.Columns.Add("IPTAL_NEDEN", typeof(Int32));
            dt.Columns.Add("IPTAL_TANIM", typeof(String));

            dt.Rows.Add(1, "Kayıp Kart");
            dt.Rows.Add(2, "Yeni Kart");
            dt.Rows.Add(3, "Bozuk Kart");

            cmbNedenIptal.DisplayMember = "IPTAL_TANIM";
            cmbNedenIptal.ValueMember = "IPTAL_NEDEN";

            DataRow dr = dt.NewRow();
            dr["IPTAL_TANIM"] = "İptal Nedeni";
            dr["IPTAL_NEDEN"] = 0;

            dt.Rows.InsertAt(dr, 0);

            cmbNedenIptal.DataSource = dt;
            cmbNedenIptal.SelectedIndex = 0;
        }

        private void TanimPersonelDoldur()
        {
            try
            {
                cmbPersonelTip_yeni.DataSource = null;
                cmbPersonelTip_yeni.Items.Clear();

                cmbPersonelTip_bilgi.DataSource = null;
                cmbPersonelTip_bilgi.Items.Clear();

                DataTable dt = dba.PersonelTipGetir();
                DataTable dtBilgi = dba.PersonelTipGetir();

                cmbPersonelTip_yeni.DisplayMember = "PERTIP_AD";
                cmbPersonelTip_yeni.ValueMember = "PERTIP_ID";

                cmbPersonelTip_bilgi.DisplayMember = "PERTIP_AD";
                cmbPersonelTip_bilgi.ValueMember = "PERTIP_ID";

                DataRow dr = dt.NewRow();
                dr["PERTIP_AD"] = "Lütfen Seçiniz";
                dr["PERTIP_ID"] = 0;

                dt.Rows.InsertAt(dr, 0);

                DataRow drBilgi = dtBilgi.NewRow();
                drBilgi["PERTIP_AD"] = "Lütfen Seçiniz";
                drBilgi["PERTIP_ID"] = 0;

                dtBilgi.Rows.InsertAt(drBilgi, 0);

                cmbPersonelTip_yeni.DataSource = dt;
                cmbPersonelTip_yeni.SelectedIndex = 0;

                cmbPersonelTip_bilgi.DataSource = dtBilgi;
                cmbPersonelTip_bilgi.SelectedIndex = 0;
				
                _startup = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Personel Tipi Alınamadı");
                return;
            }
        }

        private void TanimTarifeDoldur(Int32 karttip)
        {
            try
            {
                cmbTarifeTip_yeni.DataSource = null;
                cmbTarifeTip_yeni.Items.Clear();

                DataTable dt = dba.TarifeTipGetir(karttip);

                cmbTarifeTip_yeni.DisplayMember = "TARIFE_TANIM";
                cmbTarifeTip_yeni.ValueMember = "TARIFE_ID";

                DataRow dr = dt.NewRow();
                dr["TARIFE_TANIM"] = "Lütfen Seçiniz";
                dr["TARIFE_ID"] = 0;

                cmbTarifeTip_yeni.DataSource = dt;
                dt.Rows.InsertAt(dr, 0);
                cmbTarifeTip_yeni.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tarifeler Alınamadı");
                return;
            }
        }

        private void TanimTarifeBilgiDoldur(Int32 karttip)
        {
            try
            {
                cmbTarifeTip_bilgi.DataSource = null;
                cmbTarifeTip_bilgi.Items.Clear();

                DataTable dtBilgi = dba.TarifeTipGetir(karttip);

                cmbTarifeTip_bilgi.DisplayMember = "TARIFE_TANIM";
                cmbTarifeTip_bilgi.ValueMember = "TARIFE_ID";

                DataRow drBilgi = dtBilgi.NewRow();
                drBilgi["TARIFE_TANIM"] = "Lütfen Seçiniz";
                drBilgi["TARIFE_ID"] = 0;

                cmbTarifeTip_bilgi.DataSource = dtBilgi;
                dtBilgi.Rows.InsertAt(drBilgi, 0);
                cmbTarifeTip_bilgi.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tarifeler Alınamadı");
                return;
            }
        }

        private void TanimFakulteDoldur()
        {
            try
            {
                cmbFakulte_yeni.DataSource = null;
                cmbFakulte_yeni.Items.Clear();

                DataTable dt = dba.FakulteGetir();

                cmbFakulte_yeni.DisplayMember = "FAKULTE_AD";
                cmbFakulte_yeni.ValueMember = "FAKULTE_ID";
                
                DataRow dr = dt.NewRow();
                dr["FAKULTE_AD"] = "Lütfen Seçiniz";
                dr["FAKULTE_ID"] = 0;

                dt.Rows.InsertAt(dr, 0);
                cmbFakulte_yeni.DataSource = dt;
                cmbFakulte_yeni.SelectedIndex = 0;

                _startup = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fakülteler Alınamadı");
                return;
            }
        }

        private void TanimBolumDoldur(Int32 fakulteId)
        {
            try
            {
                cmbBolum_yeni.DataSource = null;
                cmbBolum_yeni.Items.Clear();

                DataTable dt = dba.BolumGetir(fakulteId);

                cmbBolum_yeni.DisplayMember = "BOLUM_AD";
                cmbBolum_yeni.ValueMember = "BOLUM_ID";

                DataRow dr = dt.NewRow();
                dr["BOLUM_AD"] = "Lütfen Seçiniz";
                dr["BOLUM_ID"] = 0;

                dt.Rows.InsertAt(dr, 0);
                cmbBolum_yeni.DataSource = dt;
                cmbBolum_yeni.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bölümler Alınamadı");
                return;
            }
        }

        private void TanimYetkiDoldur()
        {
            cmbUserRights.DataSource = null;
            cmbUserRights.Items.Clear();

            DataTable dt = dba.YetkilerListe();

            cmbUserRights.DisplayMember = "YETKI_TANIM";
            cmbUserRights.ValueMember = "YETKI_ID";

            cmbUserRights.DataSource = dt;            
        }

        private void PersFormTemizle()
        {
            txtTckimlik_yeni.Text = "0";
            txtAd_yeni.Text = "";
            txtSoyad_yeni.Text = "";

            cmbPersonelTip_yeni.SelectedIndex = 0;
            cmbTarifeTip_yeni.SelectedIndex = 0;
        }
        #endregion

        #region "Butonlar"
        private void btnTckimlikAra_Click(object sender, EventArgs e)
        {
            _elleTCgirildi = true;

            ResetFormBakiye();
            ResetFormKayit();
            ResetFormBilgi();

            while (_cleaning) { }

                Int64 tckimlik = 0;
                Int64.TryParse(txtTckimlikAra.Text, out tckimlik);

                txtTckimlikAra.Text = "";
                txtKartBilgi_kisi.Text = "";

                TCdenAra(tckimlik);
        }

        private void btnKartBakiyeAktar_Click(object sender, EventArgs e)
        {
            
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 5");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kart tanımlama",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            try
            {
                if (gridKartlar.Rows.Count > 0)
                {
                    Int64 tckimlik = 0;
                    Int64.TryParse(txtTckimlikAra.Text, out tckimlik);

                    Int64 kartNo = 0;
                    Int64.TryParse(Convert.ToString(gridKartlar.SelectedRows[0].Cells["colKART_ID"].Value), out kartNo);

                    if (tckimlik > 0 && kartNo > 0)
                    {
                        DialogResult msjSonuc = new DialogResult();
                        msjSonuc = MessageBox.Show("Yedeklenmiş bakiye seçilen karta aktarılacak\r\nDevam etmek istiyor musunuz?", "Kart Tanımlama",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                        if (msjSonuc == DialogResult.Yes)
                        {
                            KisiClass kisi = new KisiClass();

                            kisi.KartNo = kartNo;
                            kisi.TCKimlik = tckimlik;

                            KartBakiyeAktar(kisi);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kart seçmediniz", "Kart Tanımlama",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void btnKartKisiBul_Click(object sender, EventArgs e)
        {
        }

        private void btnKartBul_yeni_Click(object sender, EventArgs e)
        {
            _kartOkumaAktif = true;

            KartBul();

            _cleanTimer.Stop();
            _elapsed = 0;
            _cleanTimer.Start();
        }

        private void btnKartKaydet_Click(object sender, EventArgs e)
        {
            
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 5");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kart tanımlama",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            txtKartNo_kart.ReadOnly = true;
            KartKaydet();
        }

        private void btn_Kaydet_yeni_Click(object sender, EventArgs e)
        {
            
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 4");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kişi Tanımlama",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            txtKartBilgi_kisi.Text = "";
            txtKartBilgi_kayit.Text = "";

            try
            {
                KisiClass kisi = new KisiClass();

                Int64 tckimlik = 0;
                Int64.TryParse(txtTckimlik_yeni.Text, out tckimlik);
                kisi.TCKimlik = tckimlik;

                String ad = txtAd_yeni.Text;
                if (Regex.IsMatch(ad, @"^[\p{L} \.]+$"))
                {
                    kisi.Ad = ad;
                }

                String soyad = txtSoyad_yeni.Text;
                if (Regex.IsMatch(soyad, @"^[\p{L} \.]+$"))
                {
                    kisi.Soyad = soyad;
                }

                kisi.KartTip = Convert.ToInt32(cmbPersonelTip_yeni.SelectedValue);
                kisi.TarifeId = Convert.ToInt32(cmbTarifeTip_yeni.SelectedValue);
                kisi.BirimId = Convert.ToInt32(cmbFakulte_yeni.SelectedValue);

                if (kisi.TCKimlik <= 0 | kisi.Ad == "" | kisi.Soyad == "" | kisi.KartTip <= 0 | kisi.TarifeId <= 0)
                {
                    MessageBox.Show("Lütfen Tüm Alanları Doldurunuz");
                }
                else
                {
                    Boolean tcGecerli = false;
                    tcGecerli = TcDogrula(kisi.TCKimlik.ToString());

                    if (_elleTCgirildi)
                    {
                        if (tcGecerli)
                        {
                            PersonelKaydet(kisi);
                        }
                        else
                        {
                            MessageBox.Show("Hata ! Geçersiz T.C. Kimlik Numarası, Lütfen geçerli bir T.C. Kimlik numarası giriniz.", "Kişi Tanımlama", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }
                    }
                    else
                    {
                        PersonelKaydet(kisi);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnKartIptal_Click(object sender, EventArgs e)
        {
            
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 5");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kart tanımlama",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            try
            {
                if (cmbNedenIptal.SelectedIndex <= 0)
                {
                    MessageBox.Show("İptal nedeni seçmediniz", "Kart Tanımlama",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }

                if (gridKartlar.Rows.Count > 0)
                {
                    Int64 tckimlik = 0;
                    Int64.TryParse(txtTckimlikAra.Text, out tckimlik);

                    Int64 kartNo = 0;
                    Int64.TryParse(Convert.ToString(gridKartlar.SelectedRows[0].Cells["colKART_ID"].Value), out kartNo);

                    if (tckimlik > 0 && kartNo > 0)
                    {
                        DialogResult msjSonuc = new DialogResult();
                        msjSonuc = MessageBox.Show("Seçilen kart iptal edilecek\r\nDevam etmek istiyor musunuz?", "Kart Tanımlama",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                        if (msjSonuc == DialogResult.Yes)
                        {
                            KisiClass kisi = new KisiClass();

                            kisi.KartNo = kartNo;
                            kisi.TCKimlik = tckimlik;

                            KartIptal(kisi);
                        }
                    }

                    Decimal yedekBakiye = 0;
                    Decimal.TryParse(txtBakiyeYedek.Text, out yedekBakiye);

                    if (yedekBakiye > 0)
                    {
                        lblYedekBakiye.ForeColor = Color.Red;
                    }
                    else
                    {
                        lblYedekBakiye.ForeColor = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kart seçmediniz", "Kart Tanımlama",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
        #endregion

        private void txtTckimlik_yeni_TextChanged(object sender, EventArgs e)
        {
            _elleTCgirildi = true;
        }
        #endregion

        private void btnHarYuk_Click(object sender, EventArgs e)
        {
            Int64 tckimlik = 0;
            Int64.TryParse(txtHarYukTC.Text, out tckimlik);
            gridHareketYukleme.DataSource = dba.HareketYukleme(tckimlik, dateHarYukilk.Value, dateHarYukSon.Value);
        }

        private void btnHarHar_Click(object sender, EventArgs e)
        {
            Int64 tckimlik = 0;
            Int64.TryParse(txtHarHarTC.Text, out tckimlik);
            gridHareketHarcama.DataSource = dba.HareketHarcama(tckimlik, dateHarHarilk.Value, dateHarHarSon.Value);
        }

        private void btnBakiyeYukle_EnabledChanged(object sender, EventArgs e)
        {
            if(!btnBakiyeYukle.Enabled)
            {
                btnBakiyeYukle.Text = "Kilitli - Kart Okutun";
            }
            else
            {
                btnBakiyeYukle.Text = "Bakiye Yükle";
            }
        }

        #region "Yönetim"
        private void btnGetUsers_Click(object sender, EventArgs e)
        {
            gridUserList.DataSource = dba.KullaniciListe();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 8");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kullanıcı Ekleme",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }

            NewUser newUser = new NewUser();
            newUser.ShowDialog();

            gridUserList.DataSource = dba.KullaniciListe();
            grupKullaniciYetki.Visible = false;
        }

        private void btnUserPass_Click(object sender, EventArgs e)
        {
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 8");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kullanıcı Ekleme",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }

            if (gridUserList.Rows.Count > 0)
            {
                Int32 userId = 0;
                Int32.TryParse(Convert.ToString(gridUserList.SelectedRows[0].Cells["colUserId"].Value), out userId);

                String user = Convert.ToString(gridUserList.SelectedRows[0].Cells["colUser"].Value);

                if (userId > 0)
                {
                    ChgPass chgPass = new ChgPass();
                    chgPass.userId = userId;
                    chgPass.user = user;
                    chgPass.ShowDialog();                    

                    gridUserList.DataSource = dba.KullaniciListe();
                    grupKullaniciYetki.Visible = false;
                }
            }
        }

        private void btnUserCancel_Click(object sender, EventArgs e)
        {
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 8");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kullanıcı Düzenleme",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }

            if (gridUserList.Rows.Count > 0)
            {
                Int32 userId = 0;
                Int32.TryParse(Convert.ToString(gridUserList.SelectedRows[0].Cells["colUserId"].Value), out userId);

                String user = Convert.ToString(gridUserList.SelectedRows[0].Cells["colUser"].Value);

                if (userId > 0)
                {
                    DialogResult msjSonuc = new DialogResult();
                    msjSonuc = MessageBox.Show(user + " , kullanıcısı iptal edilecek\r\nDevam etmek istiyor musunuz?", "Kullanıcı Tanımlama",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (msjSonuc == DialogResult.Yes)
                    {
                        dba.KullaniciIptalEt(userId);
                    }

                    gridUserList.DataSource = dba.KullaniciListe();
                }
            }
        }

        private void btnUserActive_Click(object sender, EventArgs e)
        {
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 8");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kullanıcı Düzenleme",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            if (gridUserList.Rows.Count > 0)
            {
                Int32 userId = 0;
                Int32.TryParse(Convert.ToString(gridUserList.SelectedRows[0].Cells["colUserId"].Value), out userId);

                String user = Convert.ToString(gridUserList.SelectedRows[0].Cells["colUser"].Value);

                if (userId > 0)
                {
                    DialogResult msjSonuc = new DialogResult();
                    msjSonuc = MessageBox.Show(user + " , kullanıcısını aktif etmek istiyor musunuz?", "Kullanıcı Tanımlama",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (msjSonuc == DialogResult.Yes)
                    {
                        dba.KullaniciAktifEt(userId);
                    }

                    gridUserList.DataSource = dba.KullaniciListe();
                }
            }
        }

        private void btnUserRights_Click(object sender, EventArgs e)
        {
            if (gridUserList.Rows.Count > 0)
            {
                Int32 userId = 0;
                Int32.TryParse(Convert.ToString(gridUserList.SelectedRows[0].Cells["colUserId"].Value), out userId);

                String user = Convert.ToString(gridUserList.SelectedRows[0].Cells["colUser"].Value);
                lblUser.Text = user;

                if (userId > 0)
                {
                    grupKullaniciYetki.Visible = true;
                    gridUserRights.DataSource = dba.KullaniciYetkiler(userId);
                }
            }
        }

        private void btnUserRightAdd_Click(object sender, EventArgs e)
        {
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 8");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kullanıcı Düzenleme",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            if (gridUserList.Rows.Count > 0)
            {
                Int32 userId = 0;
                Int32.TryParse(Convert.ToString(gridUserList.SelectedRows[0].Cells["colUserId"].Value), out userId);

                if (userId > 0)
                {
                    if (cmbUserRights.SelectedIndex < 0)
                    {
                        MessageBox.Show("Yetki seçmediniz", "Kullanıcı Tanımlama",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }

                    DialogResult msjSonuc = new DialogResult();
                    msjSonuc = MessageBox.Show("Seçilen yetkiyi eklemek istiyor musunuz?", "Kullanıcı Tanımlama",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (msjSonuc == DialogResult.Yes)
                    {
                        if (dba.KullaniciYetkiEkle(userId, Convert.ToInt32(cmbUserRights.SelectedValue)) == 100)
                        {
                            MessageBox.Show("Yetki başarıyla eklendi", "Kullanıcı Tanımlama",
                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);

                            grupKullaniciYetki.Visible = true;
                            gridUserRights.DataSource = dba.KullaniciYetkiler(userId);
                        }
                        else
                        {
                            MessageBox.Show("Yetki eklenemedi", "Kullanıcı Tanımlama",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                            return;
                        }
                    }
                }
            }
        }

        private void btnUserRightCancel_Click(object sender, EventArgs e)
        {
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 8");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kullanıcı Düzenleme",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            if (gridUserList.Rows.Count > 0)
            {
                Int32 userId = 0;
                Int32.TryParse(Convert.ToString(gridUserList.SelectedRows[0].Cells["colUserId"].Value), out userId);

                Int32 yetkiId = 0;
                Int32.TryParse(Convert.ToString(gridUserRights.SelectedRows[0].Cells["colRightId"].Value), out yetkiId);

                if (userId > 0 & yetkiId > 0)
                {
                    DialogResult msjSonuc = new DialogResult();
                    msjSonuc = MessageBox.Show("Seçilen yetki iptal edilecek\r\nDevam etmek istiyor musunuz?", "Kullanıcı Tanımlama",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (msjSonuc == DialogResult.Yes)
                    {
                        dba.YetkiIptalEt(userId, yetkiId);
                    }

                    gridUserRights.DataSource = dba.KullaniciYetkiler(userId);
                }
                else
                {
                    MessageBox.Show("Yetki seçmediniz", "Kullanıcı Tanımlama",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                    return;
                }
            }
        }

        private void btnUserRightActive_Click(object sender, EventArgs e)
        {
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 8");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kullanıcı Düzenleme",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
            
            if (gridUserList.Rows.Count > 0)
            {
                Int32 userId = 0;
                Int32.TryParse(Convert.ToString(gridUserList.SelectedRows[0].Cells["colUserId"].Value), out userId);

                if (userId > 0)
                {
                    DialogResult msjSonuc = new DialogResult();
                    msjSonuc = MessageBox.Show("Seçilen yetki aktif edilecek\r\nDevam etmek istiyor musunuz?", "Kullanıcı Tanımlama",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (msjSonuc == DialogResult.Yes)
                    {
                        //KartIptal(kisi);
                    }
                }
            }
        }

        private void gridUserList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            grupKullaniciYetki.Visible = false;
        }
        #endregion

        private void gridUserList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            grupKullaniciYetki.Visible = false;
        }

        private void btnKisiGuncelle_Click(object sender, EventArgs e)
        {
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 6");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kişi Bilgileri",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }
        }

        private void btnKartIptalKaldir_Click(object sender, EventArgs e)
        {
            DataRow[] yetkiler = Settings.Login.Yetkiler.Select("YETKI_ID = 5");

            if (yetkiler.Length <= 0)
            {
                MessageBox.Show("İşlem yetkiniz bulunmamaktadır !", "Kart tanımlama",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }

            try
            {
                if (gridKartlar.Rows.Count > 0)
                {
                    Int64 tckimlik = 0;
                    Int64.TryParse(txtTckimlikAra.Text, out tckimlik);

                    Int64 kartNo = 0;
                    Int64.TryParse(Convert.ToString(gridKartlar.SelectedRows[0].Cells["colKART_ID"].Value), out kartNo);

                    if (tckimlik > 0 && kartNo > 0)
                    {
                        DialogResult msjSonuc = new DialogResult();
                        msjSonuc = MessageBox.Show("Seçilen kartın iptali kaldırılacak\r\nDevam etmek istiyor musunuz?", "Kart Tanımlama",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                        if (msjSonuc == DialogResult.Yes)
                        {
                            KisiClass kisi = new KisiClass();

                            kisi.KartNo = kartNo;
                            kisi.TCKimlik = tckimlik;

                            KartIptalKaldir(kisi);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kart seçmediniz", "Kart Tanımlama",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
    }
}
