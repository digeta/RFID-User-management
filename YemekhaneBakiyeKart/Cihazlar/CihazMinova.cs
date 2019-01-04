using System;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using YemekhaneBakiyeKart.Events;

namespace YemekhaneBakiyeKart.Cihazlar
{
    public class CihazMinova:CihazClass
    {
        public event SeriPortVeriGeldi OnSeriPortVeriGeldi;
        public event CihazBaglandi OnCihazBaglandi;

        private SerialPort _seriPort;
        private Byte[] _datas = new Byte[32];
        private delegate void deleg(string data);

        private Boolean _kartOkundu = false;
        private Boolean _minovaReady = false;

        #region "Properties"
        public Boolean MinovaReady
        {
            get
            {
                return _minovaReady;
            }
            set
            {
                _minovaReady = value;
            }
        }
        #endregion

        #region "Methods"
        public Boolean SeriPortBaglan(out Boolean cihazYok)
        {
            cihazYok = true;
            Boolean connected = false;

            try
            {
                if (_seriPort != null)
                {
                    if (_seriPort.IsOpen) { _seriPort.Close(); }
                }

                _seriPort = new SerialPort();
                _seriPort.BaudRate = 115200;
                _seriPort.DataBits = 8;
                _seriPort.Parity = Parity.None;
                _seriPort.StopBits = StopBits.One;
                _seriPort.DtrEnable = true;
                _seriPort.ReadTimeout = 1000; //500
                _seriPort.WriteTimeout = 500; //100

                this._seriPort.DataReceived += new SerialDataReceivedEventHandler(this.SeriPort_DataReceived);
                this._seriPort.ErrorReceived += new SerialErrorReceivedEventHandler(this.SeriPort_ErrorReceived);

                for (int i = 0; i < 100; i++)
                {
                    _seriPort.PortName = "COM" + Convert.ToString(i);

                    try
                    {
                        _seriPort.Open();

                    }
                    catch(Exception ex)
                    {
                        connected = false;
                    }

                    if (_seriPort.IsOpen)
                    {
                        cihazYok = false;
                        connected = true;
                        break;
                    }
                }

                if (connected)
                {
                    Logger.Message = Logger.Messages.DeviceReady;

                    OnCihazBaglandi(this, new CihazBaglandiArgs(true, CihazList.MevcutCihaz.Minova));

                    while (!Terminating)
                    {                        
                    }

                    Logger.Message = Logger.Messages.DevicePowerOff;

                    _seriPort.Close();
                }
                else
                {
                    Logger.Message = Logger.Messages.DeviceFailedResponse;
                }
            }
            catch (Exception ex)
            {
                Logger.isError = true;
                Logger.Method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                Logger.MessageStr = "Minova COM bağlantısı sağlanamadı. " + ex.Message;                
            }

            return connected;
        }

        private void SeriPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                _seriPort.Read(_datas, 0, 16);
                Char b;
                StringBuilder sb = new StringBuilder();
                Boolean minovaYeni = false;

                for (int i = 0; i < _datas.Length; i++)
                {
                    if (_datas[i] != 0)
                    {
                        b = (Char)_datas[i];
                        if (!(((b >= 48) && (b <= 57)) || ((b >= 65) && (b <= 70))))
                        {
                            minovaYeni = true;
                            break;
                        }
                        sb.Append((Char)_datas[i]);
                    }
                }

                if (minovaYeni)
                {
                    for (int i = 4; i < 8; i++)
                    {
                        if (_datas[i] != 0)
                        {
                            //if(_datas[i] )
                            String str = _datas[i].ToString("X");
                            if (str.Length < 2) str = "0" + str;

                            //sb.Append(_datas[i].ToString("X"));
                            sb.Append(str);
                        }
                    }
                }

                String kartBilgi = sb.ToString();
                //OnSeriPortVerisi(this, new SeriPortVeriGeldiArgs(kartBilgi));
                CihazVeri cihazVerisi = new CihazVeri();
                cihazVerisi.KartNo = TersineCevir(kartBilgi);

                Logger.Message = Logger.Messages.DeviceCardRead;
                _kartOkundu = true;

                OnSeriPortVeriGeldi(this, new SeriPortVeriGeldiArgs(cihazVerisi));
            }
            catch (Exception ex)
            {
                Logger.isError = true;
                Logger.Method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                Logger.MessageStr = "Kart Okuma Hatası : " + ex.Message;
            }
        }

        private long TersineCevir(String kartNo)
        {
            Int64 result = 0;
            try
            {
                String kartHex = kartNo;
                Byte[] bytes = ByteToHex(kartHex);
                result = Convert.ToInt64(kartHex, 16);
            }
            catch (Exception ex)
            {
                Logger.isError = true;
                Logger.Method = this.GetType().Name + ", " + MethodBase.GetCurrentMethod().Name;
                Logger.MessageStr = ex.Message;
            }
            return result;
        }

        private Byte[] ByteToHex(String hexString)
        {
            String strHex = "";
            Char c;

            for (Int32 i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                {
                    strHex += c;
                }
            }

            if (strHex.Length % 2 != 0)
            {
                strHex = strHex.Substring(0, strHex.Length - 1);
            }
                        
            Byte[] bytes = new Byte[strHex.Length / 2];

            String hex;
            Int32 j = 0;
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                hex = strHex.Substring(j, 2);
                bytes[i] = (Byte)Convert.ToInt32(hex, 16);
                j = j + 2;
            }
            return bytes;
        }

        private Boolean IsHexDigit(Char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }

        private void SeriPort_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            Logger.MessageStr = e.ToString();
        }

        public void MinovaCihazDinle()
        {
            Logger.Message = Logger.Messages.DeviceWaitingCard;

            _kartOkundu = false;

            while (!_kartOkundu)
            {
                //MinovaKartBul();
            }
        }
        #endregion
    }
}
