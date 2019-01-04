using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace YemekhaneBakiyeKart.Sys
{
    public static class Settings
    {
        private static Logins _login;

        private static Boolean _admin = false;

        private static String _connStrLocal = "";
        
        private static Int32 _yemekhaneId;
        private static String _yemekhane;

        private static Int32 _kampusId;
        private static String _kampus;
        
        private static DataTable _ayarlar;

        private static Decimal _azamiBakiye = 0;

        private static String _serverType = "";
        private static String _serverIP = "";

        public static Logins Login
        {
            get
            {
                return _login;
            }
            set
            {
                _login = value;
            }
        }

        public static Boolean AdminMod
        {
            get
            {
                return _admin;
            }
            set
            {
                _admin = value;
            }
        }

        public static String ConnectionStrLocal
        {
            get
            {
                return _connStrLocal;
            }
            set
            {
                _connStrLocal = value;
            }
        }

        public static Int32 YemekhaneID
        {
            get
            {
                return _yemekhaneId;
            }
            set
            {
                _yemekhaneId = value;
            }
        }

        public static String Yemekhane
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

        public static Int32 KampusID
        {
            get
            {
                return _kampusId;
            }
            set
            {
                _kampusId = value;
            }
        }

        public static String Kampus
        {
            get
            {
                return _kampus;
            }
            set
            {
                _kampus = value;
            }
        }

        public static DataTable Ayarlar
        {
            get
            {
                return _ayarlar;
            }
            set
            {
                _ayarlar = value;
            }
        }

        public static Decimal AzamiBakiye
        {
            get
            {
                return _azamiBakiye;
            }
            set
            {
                _azamiBakiye = value;
            }
        }

        public static String ServerType
        {
            get
            {
                return _serverType;
            }
            set
            {
                _serverType = value;
            }
        }

        public static String ServerIP
        {
            get
            {
                return _serverIP;
            }
            set
            {
                _serverIP = value;
            }
        }
    }
}
