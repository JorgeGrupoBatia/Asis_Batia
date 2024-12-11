using Asis_Batia.Helpers;
using MauiPopup;
using MauiPopup.Views;

namespace Asis_Batia.Popups;

public partial class GenericPopup : BasePopupPage {

    public GenericPopup(string mensaje, string imageUrl = "paloma", string buttonText = Constants.ACEPTAR, bool showButtonCancel = false) {
        InitializeComponent();
        lblMessage.Text = mensaje;
        image.Source = imageUrl;
        buttonAccept.Text = buttonText;
        buttonCancel.IsVisible = showButtonCancel;
    }

    private void buttonCancel_Clicked(object sender, EventArgs e) {
        PopupAction.ClosePopup(this);
    }

    private void buttonAceptar_Clicked(object sender, EventArgs e) {
        PopupAction.ClosePopup(this);
    }
}