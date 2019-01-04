using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekhaneBakiyeKart.Misc
{
    public class KulClass
    {
        private Int64 _tckimlik = 0;
        private String _ad = "";
        private String _soyad = "";
        private String _user = "";
        private String _pass = "";
        private Int32 _yemekhane = 0;

        [StringValue("USR_TCKIMLIK")]
        public Int64 TCKimlik
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

        [StringValue("USR_AD")]
        public String Ad
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

        [StringValue("USR_SOYAD")]
        public String Soyad
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

        [StringValue("USR_USER")]
        public String Username
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        [StringValue("USR_PASS")]
        public String Password
        {
            get
            {
                return _pass;
            }
            set
            {
                _pass = value;
            }
        }

        [StringValue("USR_YEMEKHANE")]
        public Int32 Yemekhane
        {
            get
            {
                return _yemekhane;
            }
            set
            {
                _yemekhane = value;
            }
        }
    }
}
