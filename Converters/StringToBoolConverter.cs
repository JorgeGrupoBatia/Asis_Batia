using System.Globalization;
using Asis_Batia.Helpers;

namespace Asis_Batia.Converters;

public class StringToBoolConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        string nomenclatura = (string)value;

        if(nomenclatura.Equals(Constants.A)) {
            return true;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
