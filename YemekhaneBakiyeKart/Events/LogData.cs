using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekhaneBakiyeKart.Events
{
    public class LogData
    {
        private String logStr = "";
        private String methStr = "";
        private Int32 kayitId = 0;
        private Boolean isErr = false;
        private Boolean dbNoLog = false;

        public String Message
        {
            get
            {
                return logStr;
            }
            set
            {
                logStr = value;
            }
        }

        public String Method
        {
            get
            {
                return methStr;
            }
            set
            {
                methStr = value;
            }
        }

        public Int32 KayıtID
        {
            get
            {
                return kayitId;
            }
            set
            {
                kayitId = value;
            }
        }

        public Boolean isError
        {
            get
            {
                return isErr;
            }
            set
            {
                isErr = value;
            }
        }

        public Boolean DBnoLog
        {
            get
            {
                return dbNoLog;
            }
            set
            {
                dbNoLog = value;
            }
        }
    }
}
