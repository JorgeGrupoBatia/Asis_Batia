using Asis_Batia.ViewModel.Popups;
using MauiPopup.Views;

namespace Asis_Batia.Popups;

public partial class SelectInmueble : BasePopupPage {
    public SelectInmueble(Location currentLocation) {
        InitializeComponent();
        BindingContext = new SelectInmuebleViewModel(currentLocation);
    }
}