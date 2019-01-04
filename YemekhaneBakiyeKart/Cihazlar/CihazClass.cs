using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using YemekhaneBakiyeKart.Events;

namespace YemekhaneBakiyeKart.Cihazlar
{
    public class CihazClass
    {
        protected Boolean _connected = false;
        private Boolean _cihazYok = false;        
        private Boolean _terminating = false;

        public event CihazBaglandi OnCihazBaglandi;
        public event CihazKapandi OnCihazKapandi;

        public event YeniKartOkundu OnYeniKartOkundu;

        public CihazList.MevcutCihaz mevcutCihaz;

        private CihazVeri cihazVerisi;

        private CihazFsm fsmCihaz;
        private CihazMinova minovaCihaz;



        #region "Properties"
        public Boolean CihazBaglimi
        {
            get
            {
                return _connected;
            }
            set
            {
                _connected = value;
            }
        }

        public Boolean Terminating
        {
            get
            {
                return _terminating;
            }
            set
            {
                _terminating = value;
            }
        }

        public Boolean CihazYok
        {
            get
            {
                return _cihazYok;
            }
            set
            {
                _cihazYok = value;
            }
        }
        #endregion

        public void Init()
        {
            fsmCihaz = new CihazFsm();
            fsmCihaz.OnFsmVeriGeldi += fsmCihaz_OnFsmVeriGeldi;
            fsmCihaz.OnCihazBaglandi += onCihazBaglandi;

            minovaCihaz = new CihazMinova();
            minovaCihaz.OnSeriPortVeriGeldi += minovaCihaz_OnSeriPortVeriGeldi;
            minovaCihaz.OnCihazBaglandi += onCihazBaglandi;

            _terminating = false;

            _connected = fsmCihaz.FsmCihazBaglan(out _cihazYok);

            if (!_cihazYok)
            {
                mevcutCihaz = CihazList.MevcutCihaz.FSM;
            }
            else
            {
                _connected = minovaCihaz.SeriPortBaglan(out _cihazYok);                

                if (_cihazYok)
                {
                    onCihazBaglandi(this, new CihazBaglandiArgs(true, CihazList.MevcutCihaz.Yok));
                }
            }
        }

        #region "Methods"
        public void Dinle()
        {
            if (mevcutCihaz == CihazList.MevcutCihaz.FSM)
            {
                if (CihazBaglimi)
                {
                    fsmCihaz.FsmCihazDinle();
                }
                else
                {
                    Logger.Message = Logger.Messages.DeviceFailedResponse;
                }
            }

            if (mevcutCihaz == CihazList.MevcutCihaz.Minova)
            {
                if (CihazBaglimi)
                {
                    minovaCihaz.MinovaCihazDinle();
                }
                else
                {
                    Logger.Message = Logger.Messages.DeviceFailedResponse;
                }                
            }
        }

        public void FsmBakiyeGuncelle(CihazVeri cihazVerisi, Decimal bakiyeYeni)
        {
            fsmCihaz.KartBakiyeGuncelle(cihazVerisi, bakiyeYeni);
        }
        #endregion

        #region "Events"
        public void onCihazBaglandi(object sender, CihazBaglandiArgs e)
        {
            _connected = true;
            mevcutCihaz = e.BagliCihaz;

            OnCihazBaglandi(this, new CihazBaglandiArgs(true, CihazList.MevcutCihaz.Yok));
        }

        private void minovaCihaz_OnSeriPortVeriGeldi(object source, SeriPortVeriGeldiArgs e)
        {
            cihazVerisi = new CihazVeri();
            cihazVerisi = e.CihazVerisi;

            OnYeniKartOkundu(this, new YeniKartOkunduArgs(cihazVerisi));
        }

        private void fsmCihaz_OnFsmVeriGeldi(object sender, FsmVeriGeldiArgs e)
        {
            cihazVerisi = new CihazVeri();
            cihazVerisi = e.CihazVerisi;

            OnYeniKartOkundu(this, new YeniKartOkunduArgs(cihazVerisi));
        }
        #endregion
    }
}
