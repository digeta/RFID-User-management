using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekhaneBakiyeKart
{
    public class CihazVeri
    {
        private Int64 _kartno = 0;
        private Decimal _bakiye = 0;
        private DateTime _sonIslemTarih;
        private Boolean _bakiyeOkundu = false;
        private Boolean _bakiyeGuncellendi = false;
        private Boolean _tarihOkundu = false;
        private Boolean _ayniKartOkundu = false;

        public Int64 KartNo
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

        public Decimal Bakiye
        {
            get
            {
                return _bakiye;
            }
            set
            {
                _bakiye = value;
            }
        }

        public DateTime SonIslemTarihi
        {
            get
            {
                return _sonIslemTarih;
            }
            set
            {
                _sonIslemTarih = value;
            }
        }

        public Boolean BakiyeOkundu
        {
            get
            {
                return _bakiyeOkundu;
            }
            set
            {
                _bakiyeOkundu = value;
            }
        }

        public Boolean BakiyeGuncellendi
        {
            get
            {
                return _bakiyeGuncellendi;
            }
            set
            {
                _bakiyeGuncellendi = value;
            }
        }

        public Boolean TarihOkundu
        {
            get
            {
                return _tarihOkundu;
            }
            set
            {
                _tarihOkundu = value;
            }
        }

        public Boolean AyniKartOkundu
        {
            get
            {
                return _ayniKartOkundu;
            }
            set
            {
                _ayniKartOkundu = value;
            }
        }
    }
}
