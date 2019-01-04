using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YemekhaneBakiyeKart.Events
{
    public class StringValueAttribute : System.Attribute
    {
        private string _value;
        private static Hashtable _stringValues;

        public StringValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public static String GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            if (_stringValues != null)
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
