using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekhaneBakiyeKart.Cihazlar
{
    public delegate void CihazBaglandi(object sender, CihazBaglandiArgs e);
    public delegate void CihazKapandi(object sender, CihazKapandiArgs e);
    public delegate void YeniKartOkundu(object sender, YeniKartOkunduArgs e);
	
    public class CihazBaglandiArgs : EventArgs
    {
        private Boolean _cihazBaglandi;
        private CihazList.MevcutCihaz _mevcutCihaz;

        public CihazBaglandiArgs(Boolean cihazBaglandi, CihazList.MevcutCihaz mevcutCihaz)
        {
            _cihazBaglandi = cihazBaglandi;
            _mevcutCihaz = mevcutCihaz;
        }

        public Boolean CihazBaglimi
        {
            get
            {
                return _cihazBaglandi;
            }
        }

        public CihazList.MevcutCihaz BagliCihaz
        {
            get
            {
                return _mevcutCihaz;
            }
        }
    }

    public class CihazKapandiArgs : EventArgs
    {
        private Boolean _cihazKapandi;

        public CihazKapandiArgs(Boolean cihazKapandi)
        {
            _cihazKapandi = cihazKapandi;
        }

        public Boolean CihazKapalimi
        {
            get
            {
                return _cihazKapandi;
            }
        }
    }

    public class YeniKartOkunduArgs : EventArgs
    {
        private CihazVeri _cihazVerisi;
        public YeniKartOkunduArgs(CihazVeri cihazVerisi)
        {
            _cihazVerisi = cihazVerisi;
        }
        public CihazVeri CihazVerisi
        {
            get
            {
                return _cihazVerisi;
            }
        }
    }
}
