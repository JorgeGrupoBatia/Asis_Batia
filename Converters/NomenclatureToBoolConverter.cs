using System.Globalization;
using Asis_Batia.Helpers;

namespace Asis_Batia.Converters;

public class NomenclatureToBoolConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        string nomenclature = (string)value;

        return !nomenclature.Equals(Constants.SIN_REGISTRO);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
