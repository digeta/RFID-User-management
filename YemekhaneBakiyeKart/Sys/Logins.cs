using System;
using System.Collections.Generic;
using System.Data;

namespace YemekhaneBakiyeKart.Sys
{
    public class Logins
    {
        private Int64 _tckimlik = 0;        
        private String _ad = "";
        private String _soyad = "";
        private String _kulad = "";
        private String _parola = "";
        private String _sicilno = "";
        private Int32 _loginID = 0;
        private Boolean _loginVar = false;
        private DataTable _yetkiler;

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

        public String Kulad
        {
            get
            {
                return _kulad;
            }
            set
            {
                _kulad = value;
            }
        }

        public String Parola
        {
            get
            {
                return _parola;
            }
            set
            {
                _parola = value;
            }
        }

        public String SicilNo
        {
            get
            {
                return _sicilno;
            }
            set
            {
                _sicilno = value;
            }
        }

        public Int32 LoginID
        {
            get
            {
                return _loginID;
            }
            set
            {
                _loginID = value;
            }
        }

        public Boolean LoginVar
        {
            get
            {
                return _loginVar;
            }
            set
            {
                _loginVar = value;
            }
        }

        public DataTable Yetkiler
        {
            get
            {
                return _yetkiler;
            }
            set
            {
                _yetkiler = value;
            }
        }
    }
}
