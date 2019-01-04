using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekhaneBakiyeKart.Misc
{
    public class KisiClass
    {
        private Int64 _tckimlik = 0;
        private Int64 _kartno = 0;
        private String _ad = "";
        private String _soyad = "";
        private Int32 _birimId = 0;
        private String _birim = "";
        private Int32 _karttip = 0;
        private Int32 _tarifeId = 0;
        private String _tarife = "";
        private Boolean _kisiAktif = false;
        private Boolean _kartIptal = false;
        private Boolean _kartAktifEdildi = false;
        private String _kartPasifNeden = "";
        private DateTime _kartGecerlilikBaslangic = new DateTime(1900, 1, 1, 0, 0, 0, 0);
        private DateTime _kartGecerlilikBitis = new DateTime(1900, 1, 1, 0, 0, 0, 0);
        private Decimal _bakiyeKarticindeki = 0;
        private Decimal _bakiyeOnceki = 0;
        private Decimal _bakiyeYuklenecek = 0;
        private Decimal _bakiyeYeni = 0;
        private Int32 _yuklemeNoktasi = 0;
        private Int32 _yukleyen = 0;
        private Boolean _tanimsizKart = true;
        private Boolean _kartAktivasyon = false;
        private Int32 _kartSay = 0;

        [StringValue("TCKIMLIK")]public Int64 TCKimlik
        {
            get
            {
                return _tckimlik;
            }
            set
            {
                _tckimlik = value;
            }
        }

        [StringValue("KART_ID")]public Int64 KartNo
        {
            get
            {
                return _kartno;
            }
            set
            {
                _kartno = value;
            }
        }

        [StringValue("AD")]public String Ad
        {
            get
            {
                return _ad;
            }
            set
            {
                _ad = value;
            }
        }

        [StringValue("SOYAD")]public String Soyad
        {
            get
            {
                return _soyad;
            }
            set
            {
                _soyad = value;
            }
        }

        [StringValue("BOLUM_ID")]public Int32 BirimId
        {
            get
            {
                return _birimId;
            }
            set
            {
                _birimId = value;
            }
        }

        [StringValue("BOLUM_AD")]public String Birim
        {
            get
            {
                return _birim;
            }
            set
            {
                _birim = value;
            }
        }

        [StringValue("KART_TIP")]public Int32 KartTip
        {
            get
            {
                return _karttip;
            }
            set
            {
                _karttip = value;
            }
        }

        [StringValue("TARIFE_ID")]public Int32 TarifeId
        {
            get
            {
                return _tarifeId;
            }
            set
            {
                _tarifeId = value;
            }
        }

        [StringValue("TARIFE")]public String Tarife
        {
            get
            {
                return _tarife;
            }
            set
            {
                _tarife = value;
            }
        }

        [StringValue("")]public Boolean KisiAktif
        {
            get
            {
                return _kisiAktif;
            }
            set
            {
                _kisiAktif = value;
            }
        }

        [StringValue("KART_IPTAL")]public Boolean KartIptal
        {
            get
            {
                return _kartIptal;
            }
            set
            {
                _kartIptal = value;
            }
        }

        [StringValue("AKTIF_EDILDI")]public Boolean KartAktifEdildi
        {
            get
            {
                return _kartAktifEdildi;
            }
            set
            {
                _kartAktifEdildi = value;
            }
        }

        [StringValue("IPTAL_NEDEN")]public String KartPasifNeden
        {
            get
            {
                return _kartPasifNeden;
            }
            set
            {
                _kartPasifNeden = value;
            }
        }

        [StringValue("VERILDI_TAR")]public DateTime KartGecerlilikBaslangic
        {
            get
            {
                return _kartGecerlilikBaslangic;
            }
            set
            {
                _kartGecerlilikBaslangic = value;
            }
        }

        [StringValue("BITIS_TAR")]public DateTime KartGecerlilikBitis
        {
            get
            {
                return _kartGecerlilikBitis;
            }
            set
            {
                _kartGecerlilikBitis = value;
            }
        }

        public Decimal BakiyeKarticindeki
        {
            get
            {
                return _bakiyeKarticindeki;
            }
            set
            {
                _bakiyeKarticindeki = value;
            }
        }

        [StringValue("BAKIYE")]public Decimal BakiyeOnceki
        {
            get
            {
                return _bakiyeOnceki;
            }
            set
            {
                _bakiyeOnceki = value;
            }
        }

        [StringValue("YUKLENEN_BAKIYE")]public Decimal BakiyeYuklenecek
        {
            get
            {
                return _bakiyeYuklenecek;
            }
            set
            {
                _bakiyeYuklenecek = value;
            }
        }

        [StringValue("YENI_BAKIYE")]public Decimal BakiyeYeni
        {
            get
            {
                return _bakiyeYeni;
            }
            set
            {
                _bakiyeYeni = value;
            }
        }

        [StringValue("YUKLEME_NOKTASI")]public Int32 YuklemeNoktasi
        {
            get
            {
                return _yuklemeNoktasi;
            }
            set
            {
                _yuklemeNoktasi = value;
            }
        }

        [StringValue("YUKLEYEN")]public Int32 Yukleyen
        {
            get
            {
                return _yukleyen;
            }
            set
            {
                _yukleyen = value;
            }
        }

        [StringValue("")]public Boolean TanimsizKart
        {
            get
            {
                return _tanimsizKart;
            }
            set
            {
                _tanimsizKart = value;
            }
        }

        [StringValue("")]public Boolean KartAktivasyon
        {
            get
            {
                return _kartAktivasyon;
            }
            set
            {
                _kartAktivasyon = value;
            }
        }

        [StringValue("")]public Int32 KartSay
        {
            get
            {
                return _kartSay;
            }
            set
            {
                _kartSay = value;
            }
        }
    }
}
