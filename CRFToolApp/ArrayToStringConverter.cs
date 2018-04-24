using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CRFToolApp
{
    [ValueConversion(typeof(double[]), typeof(string))]
    public class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var array = value as double[];
            if (value == null)
                return "";

            var str = "";
            foreach (var item in array)
            {
                str += item + " | ";
            }
            return str.Length > 0 ? str.Substring(0, str.Length - 3) : str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
