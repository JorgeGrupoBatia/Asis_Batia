using Asis_Batia.ViewModel;
using MauiPopup.Views;

namespace Asis_Batia.View;

public partial class RegExitoso : BasePopupPage {
    public RegExitoso() {
        InitializeComponent();
        BindingContext = new RegExitosoViewModel();
    }
}