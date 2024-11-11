using Asis_Batia.Helpers;
using MauiPopup;
using MauiPopup.Views;

namespace Asis_Batia.Popups;

public partial class BiometricsPopup : BasePopupPage {

    public BiometricsPopup() {
        InitializeComponent();
    }

    private void buttonAceptar_Clicked(object sender, EventArgs e) {
        UserSession.IsBiometricsActivated = true;
        buttonCancel_Clicked(null, null);
    }

    private void buttonCancel_Clicked(object sender, EventArgs e) {
        PopupAction.ClosePopup(this);
    }
}