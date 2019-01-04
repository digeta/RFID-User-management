using System;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using YemekhaneBakiyeKart.Events;
using YemekhaneBakiyeKart.FSM;

namespace YemekhaneBakiyeKart.Cihazlar
{
    public class CihazFsm:CihazClass
    {
        public event FsmVeriGeldi OnFsmVeriGeldi;
        public event CihazBaglandi OnCihazBaglandi;

        private POC _fsm;
        private Boolean _bakiyeGuncelle = false;
        private Boolean _kartOkundu = false;
        private uint _bakiyeYeni = 0;

        #region "Methods"
        internal Boolean FsmCihazBaglan(out Boolean cihazYok)
        {
            cihazYok = true;
            Boolean connected = false;
            Int32 sayac = 0;

            _fsm = new POC();

            try
            {
                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        if (_fsm.OpenDevice("COM" + Convert.ToString(i)) == POC.ReturnValues.Succesfull)
                        {
                            sayac = i;
                            cihazYok = false;
                            connected = true;
                            break;
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                }

                if (connected)
                {                    
                    Logger.Message = Logger.Messages.DeviceReady;

                    OnCihazBaglandi(this, new CihazBaglandiArgs(true, CihazList.MevcutCihaz.FSM));

                    POC.ReturnValues returnValue = POC.ReturnValues.Failed;
                }
                else
                {
                    Logger.Message = Logger.Messages.DeviceFailedResponse;
                }
            }
            catch (Exception ex)
            {
                Logger.isError = true;
                Logger.ShowMsgBox = false;
                Logger.Method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                Logger.MessageStr = sayac + "FSM COM bağlantısı sağlanamadı. " + ex.Message;
            }

            return connected;
        }

        internal void FsmCihazDinle()
        {
            Logger.Message = Logger.Messages.DeviceWaitingCard;
            
            _kartOkundu = false;
            
            while (!_kartOkundu)
            {                
                FsmKartBul();
            }
        }

        private void FsmKartBul()
        {
            try
            {
                String str = "0";

                POC.ReturnValues returnValueKartNo = POC.ReturnValues.Failed;
                POC.ReturnValues returnValueBakiye = POC.ReturnValues.Failed;
                POC.ReturnValues returnValueTarih = POC.ReturnValues.Failed;

                returnValueKartNo = _fsm.StartCardDetecting(out str);

                if (str != null)
                {
                    _kartOkundu = true;

                    ulong kartid = 0;
                    kartid = ulong.Parse(str);

                    CihazVeri cihazVerisi = new CihazVeri();

                    cihazVerisi.KartNo = Convert.ToInt64(kartid);

                    Logger.Message = Logger.Messages.DeviceCardRead;

                    uint balance = 0;
                    uint specialValue = 0;
                    uint timeout = 1000;
                    DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);

                    DateTime dateKahvalti = new DateTime(2000, 1, 1, 0, 0, 0);
                    DateTime dateOgle = new DateTime(2000, 1, 1, 0, 0, 0);
                    DateTime dateAksam = new DateTime(2000, 1, 1, 0, 0, 0);

                    Byte timesKahvalti = 0;
                    Byte timesOgle = 0;
                    Byte timesAksam = 0;
                    Byte timesTotal = 0;

                    returnValueBakiye = _fsm.GetBalance(kartid, out balance, out specialValue, timeout);
                    cihazVerisi.Bakiye = Decimal.Round(Convert.ToDecimal(balance) / 100, 2, MidpointRounding.AwayFromZero);
                    cihazVerisi.BakiyeOkundu = returnValueBakiye == POC.ReturnValues.Succesfull ? true : false;

                    returnValueTarih = _fsm.GetAccessTimes(kartid, out dateKahvalti, out dateOgle, out dateAksam, out timesKahvalti, out timesOgle, out timesAksam, out timesTotal, 1000);
                    cihazVerisi.TarihOkundu = returnValueTarih == POC.ReturnValues.Succesfull ? true : false;
                    
                    OnFsmVeriGeldi(this, new FsmVeriGeldiArgs(cihazVerisi));
                }
                Application.DoEvents();
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Logger.isError = true;
                Logger.ShowMsgBox = false;
                Logger.Method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                Logger.MessageStr = "Kart Okuma Hatası : " + ex.Message;
            }
        }

        internal Boolean KartBakiyeGuncelle(CihazVeri cihazVerisi, Decimal bakiyeYeni)
        {
            Boolean basarili = false;
            _bakiyeYeni = (uint)(bakiyeYeni * 100);

            try
            {
                POC.ReturnValues returnValueBakiyeGuncelle = POC.ReturnValues.Failed;
                POC.ReturnValues returnValueTarih = POC.ReturnValues.Failed;

                ulong kartid = (ulong)cihazVerisi.KartNo;

                DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);

                DateTime dateKahvalti = new DateTime(2000, 1, 1, 0, 0, 0);
                DateTime dateOgle = new DateTime(2000, 1, 1, 0, 0, 0);
                DateTime dateAksam = new DateTime(2000, 1, 1, 0, 0, 0);

                returnValueBakiyeGuncelle = _fsm.ChargeMoney(kartid, _bakiyeYeni, 1000);
                cihazVerisi.BakiyeGuncellendi = returnValueBakiyeGuncelle == POC.ReturnValues.Succesfull ? true : false;

                returnValueTarih = _fsm.SetAccessTimes(kartid, dateTime, dateTime, dateTime, 0, 0, 0, 0, 1000);
                cihazVerisi.TarihOkundu = returnValueTarih == POC.ReturnValues.Succesfull ? true : false;

                basarili = true;

                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Logger.isError = true;
                Logger.ShowMsgBox = false;
                Logger.Method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                Logger.MessageStr = "Kart Yazma Hatası : " + ex.Message;
                basarili = false;
            }
            return basarili;
        }

        internal void OkumaKapat()
        {
            _kartOkundu = true;
        }
        #endregion
    }
}
