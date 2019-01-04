using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YemekhaneBakiyeKart.Events
{
    public class ColorValueAttribute : System.Attribute
    {
        private string _value;
        private static Hashtable _colorValues;

        public ColorValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public static String GetColorValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            if (_colorValues != null)
            {
                if (_colorValues.ContainsKey(value))
                    output = (_colorValues[value] as ColorValueAttribute).Value;
                else
                {
                    FieldInfo fi = type.GetField(value.ToString());
                    ColorValueAttribute[] attrs =
                       fi.GetCustomAttributes(typeof(ColorValueAttribute),
                                               false) as ColorValueAttribute[];
                    if (attrs.Length > 0)
                    {
                        _colorValues.Add(value, attrs[0]);
                        output = attrs[0].Value;
                    }
                }
            }
            else
            {
                _colorValues = new Hashtable();
				
                FieldInfo fi = type.GetField(value.ToString());
                ColorValueAttribute[] attrs =
                   fi.GetCustomAttributes(typeof(ColorValueAttribute),
                                           false) as ColorValueAttribute[];
                if (attrs.Length > 0)
                {
                    _colorValues.Add(value, attrs[0]);
                    output = attrs[0].Value;
                }
            }

            return output;
        }
    }
}
