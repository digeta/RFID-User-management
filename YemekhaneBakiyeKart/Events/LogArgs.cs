using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YemekhaneBakiyeKart.Events
{
    public class LogArgs : EventArgs
    {
        private Logger.Messages _msgEnum;
        private String _msgStr;
        private String _meth;
        private Int32 _kaytid;
        private Boolean _isErr;
        private Boolean _showMsgBox;
        private System.Drawing.Color _backColor;
        private System.Drawing.Color _fontColor;
        private System.Windows.Forms.Control _control;
               

        public LogArgs(Logger.Messages messageEnum, String messageStr, String method, Int32 kayitID, 
            Boolean isError, Boolean showMsgBox, System.Drawing.Color backColor, System.Drawing.Color fontColor, System.Windows.Forms.Control control)
        {
            this._msgEnum = messageEnum;
            this._msgStr = (messageEnum == Logger.Messages.NoEnumMessage) ? messageStr : StringValueAttribute.GetStringValue(messageEnum);
            this._meth = method;
            this._kaytid = kayitID;
            this._isErr = isError;
            this._showMsgBox = showMsgBox;
            this._backColor = backColor;
            this._fontColor = System.Drawing.ColorTranslator.FromHtml(ColorValueAttribute.GetColorValue(messageEnum));
            this._control = control;
        }

        public Logger.Messages Message
        {
            get
            {
                return _msgEnum;
            }
            set
            {
                _msgEnum = value;
            }
        }

        public string MessageStr
        {
            get
            {
                return _msgStr;
            }
        }

        public string Method
        {
            get
            {
                return _meth;
            }
        }

        public Int32 KayıtID
        {
            get
            {
                return _kaytid;
            }
        }

        public Boolean isError
        {
            get
            {
                return _isErr;
            }
        }

        public Boolean ShowMsgBox
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

        public System.Drawing.Color BackColor
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

        public System.Drawing.Color FontColor
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

        public System.Windows.Forms.Control Kontrol
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
    }
}
