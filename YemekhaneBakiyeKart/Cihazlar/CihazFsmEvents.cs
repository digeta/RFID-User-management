using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekhaneBakiyeKart.Cihazlar
{
    public delegate void FsmVeriGeldi(object sender, FsmVeriGeldiArgs e);
    public delegate void FsmCihazBaglandi(object sender, FsmCihazBaglandiArgs e);
    public delegate void FsmCihazKapandi(object sender, FsmCihazKapandiArgs e);

    public class FsmVeriGeldiArgs : EventArgs
    {
        private CihazVeri _cihazVerisi;

        public FsmVeriGeldiArgs(CihazVeri cihazVeri)
        {
            _cihazVerisi = cihazVeri;
        }
        public CihazVeri CihazVerisi
        {
            get
            {
                return _cihazVerisi;
            }
        }
    }

    public class FsmCihazBaglandiArgs : EventArgs
    {
        private Boolean _fsmBaglandi;

        public FsmCihazBaglandiArgs(Boolean fsmBaglandi)
        {
            _fsmBaglandi = fsmBaglandi;
        }
        
        public Boolean FsmCihazBaglimi
        {
            get
            {
                return _fsmBaglandi;
            }
        }
    }

    public class FsmCihazKapandiArgs : EventArgs
    {
        private Boolean _fsmKapandi;

        public FsmCihazKapandiArgs(Boolean fsmKapandi)
        {
            _fsmKapandi = fsmKapandi;
        }
        
        public Boolean FsmCihazKapalimi
        {
            get
            {
                return _fsmKapandi;
            }
        }
    }
}
