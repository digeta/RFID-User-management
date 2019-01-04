using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YemekhaneBakiyeKart.Events
{
    public static class Logger
    {
        private static Messages _msgEnum = Messages.NoEnumMessage;
        private static String _msgStr = "";
        private static String _methStr = "";
        private static Int32 _kayitId = 0;
        private static Boolean _isErr = false;
        private static Boolean _showMsgBox = false;
        private static System.Drawing.Color _backColor = System.Drawing.Color.White;
        private static System.Drawing.Color _fontColor = System.Drawing.Color.Black;
        private static System.Windows.Forms.Control _control = null;

        private static Hashtable _stringValues;
        
        public static event EventHandler<LogArgs> OnMessageChanged;        


        public static String MessageStr
        {
            get
            {
                return _msgStr;
            }
            set
            {
                _msgStr = value;
                OnMessageChanged(null, new LogArgs(_msgEnum, _msgStr, _methStr, _kayitId, _isErr, _showMsgBox, _backColor, _fontColor, _control));
            }
        }

        public static Messages Message
        {
            get
            {
                return _msgEnum;
            }
            set
            {
                _msgEnum = value;
                OnMessageChanged(null, new LogArgs(_msgEnum, _msgStr, _methStr, _kayitId, _isErr, _showMsgBox, _backColor, _fontColor, _control));
            }
        }

        public static String Method
        {
            get
            {
                return _methStr;
            }
            set
            {
                _methStr = value;
                //OnMessageChanged(null, new LogArgs(logStr, methStr, isErr));
            }
        }

        public static Int32 KayıtID
        {
            get
            {
                return _kayitId;
            }
            set
            {
                _kayitId = value;
            }
        }

        public static Boolean isError
        {
            get
            {
                return _isErr;
            }
            set
            {
                _isErr = value;
                //OnMessageChanged(null, new LogArgs(logStr, methStr, isErr));
            }
        }

        public static Boolean ShowMsgBox
        {
            get
            {
                return _showMsgBox;
            }
            set
            {
                _showMsgBox = value;
            }
        }

        public static System.Drawing.Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
            }
        }

        public static System.Drawing.Color FontColor
        {
            get
            {
                return _fontColor;
            }
            set
            {
                _fontColor = value;
            }
        }

        public static System.Windows.Forms.Control Kontrol
        {
            get
            {
                return _control;
            }
            set
            {
                _control = value;
            }
        }
        
        public enum Messages
        {
            [StringValue("")]
            [ColorValue("#00000000")]
            NoEnumMessage = 0,

            [StringValue("Cihaz hazır. Kart Okutabilirsiniz..")]
            [ColorValue("#FF1E90FF")]
            DeviceReady = 1001, //Logger.FontColor = System.Drawing.Color.DodgerBlue;

            [StringValue("Cihaz kapanıyor...")]
            [ColorValue("#FFFF0000")]
            DevicePowerOff = 1002, //Logger.FontColor = System.Drawing.Color.Red;

            [StringValue("Cihaz ile bağlantı sağlanamadı !\r\nKablo bağlantısını kontrol edin ve\r\nprogramı yeniden başlatın")]
            [ColorValue("#FFFF0000")]
            DeviceFailedResponse = 1101, //Logger.FontColor = System.Drawing.Color.Red;

            [StringValue("Kartı Cihaza Yaklaştırın..")]
            [ColorValue("#FF1E90FF")]
            DeviceWaitingCard = 1004, //Logger.FontColor = System.Drawing.Color.DodgerBlue;

            [StringValue("Kart Okundu")]
            [ColorValue("#FF1E90FF")]
            DeviceCardRead = 1005, //Logger.FontColor = System.Drawing.Color.DodgerBlue;




            [StringValue("Bakiye Yüklendi")]
            [ColorValue("#FF1E90FF")]
            BalanceDeposit = 2001, //Logger.FontColor = System.Drawing.Color.DodgerBlue;

            [StringValue("Bir seferde en fazla 100 TL yükleyebilirsiniz ! ")]
            [ColorValue("#FFFF0000")]
            BalanceLimitReached = 2101, //Logger.FontColor = System.Drawing.Color.Red;

            [StringValue("Bakiye Yükleme Hatası ! ")]
            [ColorValue("#FFFF0000")]
            BalanceDepositFailed = 2102, //Logger.FontColor = System.Drawing.Color.Red;

            [StringValue("Geçersiz bakiye, lütfen girdiğiniz rakamı kontrol edin ! ")]
            [ColorValue("#FFFF0000")]
            BalanceInvalid = 2103, //Logger.FontColor = System.Drawing.Color.Red;




            [StringValue("Tanımsız Kart ! ")]
            [ColorValue("#FFFF0000")]
            CardInvalid = 3101, //Logger.FontColor = System.Drawing.Color.Red;

            [StringValue("Kart Aktif Edildi")]
            [ColorValue("#FF1E90FF")]
            CardActivated = 3001, //Logger.FontColor = System.Drawing.Color.DodgerBlue;

            [StringValue("Kart Aktif Edilemedi ! ")]
            [ColorValue("#FFFF0000")]
            CardActivationFailed = 3102, //Logger.FontColor = System.Drawing.Color.Red;




            [StringValue("Bu kişinin kaydı yok, yeni kayıt oluşturabilirsiniz ! ")]
            [ColorValue("#FFFF0000")]
            PersonNotFound = 4101, //Logger.FontColor = System.Drawing.Color.Red;

            [StringValue("Bu kişi ile ilgili yemekhane kaydı yok ! \r\nEKAMPUS' ten aktarma yapıldı. ")]
            [ColorValue("#FFFF0000")]
            PersonFoundEkampus = 4102 //Logger.FontColor = System.Drawing.Color.Red;            
        }              
    }
}
