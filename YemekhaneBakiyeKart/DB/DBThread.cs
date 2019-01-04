using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;

using YemekhaneBakiyeKart.Error;
using YemekhaneBakiyeKart.Events;
using YemekhaneBakiyeKart.Misc;
using YemekhaneBakiyeKart.Sys;

namespace YemekhaneBakiyeKart.DB
{
    public class DBThread
    {
        public event ErrorOccured OnError;
        public KisiClass KartKontrol(KisiClass kisi)
        {
            kisi.TanimsizKart = true;

            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;
                
                String sqlStr = "stp_KART_KONTROL";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("KART_ID", kisi.KartNo);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    kisi.TCKimlik = Convert.ToInt64(dt.Rows[0]["TCKIMLIK"]);
                    kisi.Ad = Convert.ToString(dt.Rows[0]["AD"]);
                    kisi.Soyad = Convert.ToString(dt.Rows[0]["SOYAD"]);
                    kisi.TarifeId = Convert.ToInt32(dt.Rows[0]["TARIFE_ID"]);
                    kisi.Tarife = Convert.ToString(dt.Rows[0]["TARIFE"]);
                    kisi.KartIptal = Convert.ToBoolean(dt.Rows[0]["KART_IPTAL"]);
                    kisi.KartAktifEdildi = Convert.ToBoolean(dt.Rows[0]["AKTIF_EDILDI"]);
                    kisi.BakiyeOnceki = Decimal.Round(Convert.ToDecimal(dt.Rows[0]["BAKIYE"]), 2, MidpointRounding.AwayFromZero);
                    kisi.Yukleyen = Settings.Login.LoginID;
                    kisi.TanimsizKart = false;
                }
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return kisi;
        }

        public KisiClass YeniBakiyeGetir(KisiClass kisi)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_BAKIYE_GETIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("KART_ID", kisi.KartNo);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    kisi.BakiyeYeni = Convert.ToInt64(dt.Rows[0]["YENI_BAKIYE"]);
                }
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return kisi;
        }

        public DataTable KullaniciIslemler(Int32 kullaniciId, out Boolean dbAktif)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_ISLEMLER_GETIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("YUKLEYEN", kullaniciId);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);

                dbAktif = true;
            }
            catch (Exception ex)
            {
                dbAktif = false;

                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public Int32 BakiyeYukle(KisiClass kisi)
        {
            Int32 inserted = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_BAKIYE_YUKLE";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", kisi.TCKimlik);
                sqlComm.Parameters.AddWithValue("KART_ID", kisi.KartNo);
                sqlComm.Parameters.AddWithValue("ONCEKI_BAKIYE", kisi.BakiyeOnceki);
                sqlComm.Parameters.AddWithValue("YUKLENEN_BAKIYE", kisi.BakiyeYuklenecek);
                sqlComm.Parameters.AddWithValue("YENI_BAKIYE", kisi.BakiyeYeni);
                sqlComm.Parameters.AddWithValue("YUKLEME_NOKTASI", kisi.YuklemeNoktasi);
                sqlComm.Parameters.AddWithValue("YUKLEYEN", kisi.Yukleyen);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                inserted = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                inserted = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return inserted;
        }

        public Int32 KartBakiyeAktar(KisiClass kisi)
        {
            Int32 updated = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KART_BAKIYE_AKTAR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", kisi.TCKimlik);
                sqlComm.Parameters.AddWithValue("KART_ID", kisi.KartNo);
                sqlComm.Parameters.AddWithValue("LOGIN", kisi.Yukleyen);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                updated = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                updated = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return updated;
        }

        public Int32 YuklemeIptal(Int64 yuklemeID)
        {
            Int32 updated = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_YUKLEME_IPTAL";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("YUKLEME_ID", yuklemeID);
                sqlComm.Parameters.AddWithValue("IPTAL_EDEN", Settings.Login.LoginID);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                updated = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                updated = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return updated;
        }

        #region "Kart Aktivasyon"
        public Boolean KartAktivasyon(KisiClass kisi)
        {
            Boolean aktiveEdildi = false;

            kisi.BakiyeOnceki = 0;
            kisi.BakiyeYuklenecek = kisi.BakiyeKarticindeki;
            kisi.BakiyeYeni = kisi.BakiyeKarticindeki;

            aktiveEdildi = KartAktifYap(kisi) == 100 ? true : false;

            if (aktiveEdildi)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Int32 KartAktifYap(KisiClass kisi)
        {
            Int32 updated = 0;
            SqlConnection sqlConn = new SqlConnection();            

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;                

                String sqlStr = "stp_KART_AKTIF_YAP";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("KART_ID", kisi.KartNo);
                sqlComm.Parameters.AddWithValue("TCKIMLIK", kisi.TCKimlik);
                sqlComm.Parameters.AddWithValue("ONCEKI_BAKIYE", kisi.BakiyeOnceki);
                sqlComm.Parameters.AddWithValue("YUKLENEN_BAKIYE", kisi.BakiyeYuklenecek);
                sqlComm.Parameters.AddWithValue("YENI_BAKIYE", kisi.BakiyeYeni);
                sqlComm.Parameters.AddWithValue("YUKLEME_NOKTASI", kisi.YuklemeNoktasi);
                sqlComm.Parameters.AddWithValue("YUKLEYEN", kisi.Yukleyen);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                updated = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                updated = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return updated;
        }
        #endregion

        #region "Personel Tanım"
        public DataTable PersonelGetir(Int64 tckimlik, out Boolean kisiMevcut)
        {
            kisiMevcut = false;
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_PERSONEL_GETIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", tckimlik);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    kisiMevcut = true;
                }
            }
            catch (Exception ex)
            {
                kisiMevcut = false;

                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public DataTable PersonelAra(Int64 tckimlik, out Boolean kisiMevcutEkampus)
        {
            kisiMevcutEkampus = false;
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_PERSONEL_ARA";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", tckimlik);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToInt64(dt.Rows[0]["TCKIMLIK"]) > 0)
                    {
                        kisiMevcutEkampus = true;
                    }
                }
            }
            catch (Exception ex)
            {
                kisiMevcutEkampus = false;

                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public DataTable KartlarGetir(Int64 tckimlik)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KARTLAR_GETIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", tckimlik);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public DataTable PersonelTipGetir()
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_PERTIP_GETIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);

                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public DataTable TarifeTipGetir(Int32 karttip)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_TARIFETIP_GETIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("KART_TIP", karttip);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);

                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public DataTable FakulteGetir()
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_FAKULTE_GETIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);

                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public DataTable BolumGetir(Int32 fakulteId)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_BOLUM_GETIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("FAKULTE_ID", fakulteId);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);

                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public Int32 PersonelEkle(KisiClass kisi)
        {
            Int32 inserted = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_PERSONEL_EKLE";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", kisi.TCKimlik);
                sqlComm.Parameters.AddWithValue("AD", kisi.Ad);
                sqlComm.Parameters.AddWithValue("SOYAD", kisi.Soyad);
                sqlComm.Parameters.AddWithValue("KART_TIP", kisi.KartTip);
                sqlComm.Parameters.AddWithValue("TARIFE_ID", kisi.TarifeId);
                sqlComm.Parameters.AddWithValue("BOLUM_ID", kisi.BirimId);
                sqlComm.Parameters.AddWithValue("LOGIN", Settings.Login.LoginID);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                inserted = Convert.ToInt32(resultParameter.Value);                
            }
            catch (Exception ex)
            {
                inserted = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return inserted;
        }

        public Int32 PersonelGuncelle(KisiClass kisi)
        {
            Int32 updated = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_PERSONEL_GUNCELLE";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", kisi.TCKimlik);
                sqlComm.Parameters.AddWithValue("KART_TIP", kisi.KartTip);
                sqlComm.Parameters.AddWithValue("TARIFE_ID", kisi.TarifeId);
                sqlComm.Parameters.AddWithValue("LOGIN", kisi.Yukleyen);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                updated = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                updated = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return updated;
        }

        public Int32 KartEkle(KisiClass kisi)
        {
            Int32 inserted = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KART_EKLE";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", kisi.TCKimlik);
                sqlComm.Parameters.AddWithValue("KART_ID", kisi.KartNo);
                sqlComm.Parameters.AddWithValue("LOGIN", Settings.Login.LoginID);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                inserted = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                inserted = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return inserted;
        }

        public Int32 KartIptal(KisiClass kisi)
        {
            Int32 updated = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KART_IPTAL";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", kisi.TCKimlik);
                sqlComm.Parameters.AddWithValue("KART_ID", kisi.KartNo);
                sqlComm.Parameters.AddWithValue("IPTAL_EDEN", Settings.Login.LoginID);
                sqlComm.Parameters.AddWithValue("NEDEN_IPTAL", 1);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                updated = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                updated = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return updated;
        }

        public Int32 KartIptalKaldir(KisiClass kisi)
        {
            Int32 updated = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KART_IPTAL_KALDIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", kisi.TCKimlik);
                sqlComm.Parameters.AddWithValue("KART_ID", kisi.KartNo);
                sqlComm.Parameters.AddWithValue("AKTIF_EDEN", Settings.Login.LoginID);


                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                updated = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                updated = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return updated;
        }
        #endregion

        #region "Login"
        public Logins GirisYap(Logins login)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_LOGIN";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("KULAD", login.Kulad);
                sqlComm.Parameters.AddWithValue("PAROLA", login.Parola);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    login.LoginVar = true;
                    login.LoginID = Convert.ToInt32(dt.Rows[0]["ID"]);
                    login.SicilNo = Convert.ToString(dt.Rows[0]["SICILNO"]);
                    login.Ad = Convert.ToString(dt.Rows[0]["AD"]);
                    login.Soyad = Convert.ToString(dt.Rows[0]["SOYAD"]);
                }
                else
                {
                    login.LoginVar = false;
                }
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return login;
        }
        #endregion

        #region "Hareketler"
        public DataTable HareketYukleme(Int64 tckimlik, DateTime ilkTarih, DateTime sonTarih)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                ilkTarih = Convert.ToDateTime(ilkTarih.ToShortDateString());
                sonTarih = Convert.ToDateTime(sonTarih.ToShortDateString());

                sonTarih = sonTarih.AddHours(23);
                sonTarih = sonTarih.AddMinutes(59);

                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_HAREKET_YUKLEME";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", tckimlik);
                sqlComm.Parameters.AddWithValue("TARIH_ILK", ilkTarih);
                sqlComm.Parameters.AddWithValue("TARIH_SON", sonTarih);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public DataTable HareketHarcama(Int64 tckimlik, DateTime ilkTarih, DateTime sonTarih)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                ilkTarih = Convert.ToDateTime(ilkTarih.ToShortDateString());
                sonTarih = Convert.ToDateTime(sonTarih.ToShortDateString());

                sonTarih = sonTarih.AddHours(23);
                sonTarih = sonTarih.AddMinutes(59);

                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_HAREKET_HARCAMA";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("TCKIMLIK", tckimlik);
                sqlComm.Parameters.AddWithValue("TARIH_ILK", ilkTarih);
                sqlComm.Parameters.AddWithValue("TARIH_SON", sonTarih);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }
        #endregion

        #region "Yönetim"
        public Int32 KullaniciEkle(KulClass kulClass)
        {
            Int32 inserted = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KULLANICI_EKLE";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("@LOGIN", Settings.Login.LoginID);
                sqlComm.Parameters.AddWithValue("@TCKIMLIK", kulClass.TCKimlik);
                sqlComm.Parameters.AddWithValue("@AD", kulClass.Ad);
                sqlComm.Parameters.AddWithValue("@SOYAD", kulClass.Soyad);
                sqlComm.Parameters.AddWithValue("@USER", kulClass.Username);
                sqlComm.Parameters.AddWithValue("@PASS", kulClass.Password);
                sqlComm.Parameters.AddWithValue("@YEMEKHANE_ID", kulClass.Yemekhane);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                inserted = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                inserted = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return inserted;
        }

        public DataTable KullaniciListe()
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KULLANICI_LISTELE";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public Int32 KullaniciParolaAta(Int32 userId, String pass)
        {
            Int32 updated = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KULLANICI_PAROLA_ATA";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                                
                sqlComm.Parameters.AddWithValue("@USER_ID", userId);
                sqlComm.Parameters.AddWithValue("@PASS", pass);
                sqlComm.Parameters.AddWithValue("@LOGIN", Settings.Login.LoginID);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                updated = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                updated = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return updated;
        }

        public Int32 KullaniciAktifEt(Int32 userId)
        {
            Int32 updated = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KULLANICI_AKTIF";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("@LOGIN", Settings.Login.LoginID);
                sqlComm.Parameters.AddWithValue("@USER_ID", userId);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                updated = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                updated = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return updated;
        }

        public Int32 KullaniciIptalEt(Int32 userId)
        {
            Int32 updated = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KULLANICI_IPTAL";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("@LOGIN", Settings.Login.LoginID);
                sqlComm.Parameters.AddWithValue("@USER_ID", userId);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                updated = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                updated = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return updated;
        }

        public DataTable YetkilerListe()
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_YETKILER_GETIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public DataTable KullaniciYetkiler(Int32 userId)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KULLANICI_YETKILER";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("USER_ID", userId);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }

        public Int32 KullaniciYetkiEkle(Int32 userId, Int32 yetkiId)
        {
            Int32 inserted = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KULLANICI_YETKI_EKLE";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("@LOGIN", Settings.Login.LoginID);
                sqlComm.Parameters.AddWithValue("@USER_ID", userId);
                sqlComm.Parameters.AddWithValue("@YETKI_ID", yetkiId);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                inserted = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                inserted = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return inserted;
        }

        public Int32 YetkiIptalEt(Int32 userId, Int32 yetkiId)
        {
            Int32 updated = 0;
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_KULLANICI_YETKI_IPTAL";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("@LOGIN", Settings.Login.LoginID);
                sqlComm.Parameters.AddWithValue("@USER_ID", userId);
                sqlComm.Parameters.AddWithValue("@YETKI_ID", yetkiId);

                SqlParameter resultParameter = new SqlParameter("SONUC", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                sqlComm.Parameters.Add(resultParameter);

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                sqlComm.ExecuteNonQuery();

                updated = Convert.ToInt32(resultParameter.Value);
            }
            catch (Exception ex)
            {
                updated = 0;
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return updated;
        }

        public DataTable YemekhanelerGetir()
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = new SqlConnection();

            try
            {
                sqlConn.ConnectionString = Settings.ConnectionStrLocal;

                String sqlStr = "stp_YEMEKHANELER_GETIR";

                SqlCommand sqlComm = new SqlCommand(sqlStr, sqlConn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                if (sqlConn.State != System.Data.ConnectionState.Open) sqlConn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlComm);
                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                String method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                OnError(this, new ErrorOccuredArgs(method, ex.Message));
            }
            finally
            {
                if (sqlConn.State != System.Data.ConnectionState.Closed) sqlConn.Close();
            }
            return dt;
        }
        #endregion
    }
}