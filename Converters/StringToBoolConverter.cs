using System.Globalization;

namespace Asis_Batia.Converters;

public class StringToBoolConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        string nomenclatura = (string)value;

        if(nomenclatura.Equals("A")) {
            return true;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
