using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YemekhaneBakiyeKart
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
            NoEnumMessage = 0,
            [StringValue("Cihaz hazır. Kart Okutabilirsiniz..")]
            DeviceReady = 1001, //Logger.FontColor = System.Drawing.Color.DodgerBlue;
            [StringValue("Cihaz kapanıyor...")]
            DevicePowerOff = 1002, //Logger.FontColor = System.Drawing.Color.Red;
            [StringValue("Cihaz ile bağlantı sağlanamadı ! ")]
            DeviceFailedResponse = 1003, //Logger.FontColor = System.Drawing.Color.Red;
            [StringValue("Kartı Cihaza Yaklaştırın..")]
            DeviceWaitingCard = 1004, //Logger.FontColor = System.Drawing.Color.DodgerBlue;
            [StringValue("Kart Okundu")]
            DeviceCardRead = 1005 //Logger.FontColor = System.Drawing.Color.DodgerBlue;
        }              

        public class StringValueAttribute : System.Attribute
        {

            private string _value;

            public StringValueAttribute(string value)
            {
                _value = value;
            }

            public string Value
            {
                get { return _value; }
            }

        }

        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();
            
            if(_stringValues != null)
            {
                if (_stringValues.ContainsKey(value))
                    output = (_stringValues[value] as StringValueAttribute).Value;
                else
                {
                    FieldInfo fi = type.GetField(value.ToString());
                    StringValueAttribute[] attrs =
                       fi.GetCustomAttributes(typeof(StringValueAttribute),
                                               false) as StringValueAttribute[];
                    if (attrs.Length > 0)
                    {
                        _stringValues.Add(value, attrs[0]);
                        output = attrs[0].Value;
                    }
                }
            }
            else
            {
                _stringValues = new Hashtable();

                FieldInfo fi = type.GetField(value.ToString());
                StringValueAttribute[] attrs =
                   fi.GetCustomAttributes(typeof(StringValueAttribute),
                                           false) as StringValueAttribute[];
                if (attrs.Length > 0)
                {
                    _stringValues.Add(value, attrs[0]);
                    output = attrs[0].Value;
                }
            }

            return output;
        }
    }
}
