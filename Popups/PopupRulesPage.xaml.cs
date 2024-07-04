using MauiPopup;
using MauiPopup.Views;

namespace Asis_Batia.Popups;

public partial class PopupRulesPage : BasePopupPage {
    public PopupRulesPage() {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e) {
        Button pressedButton = sender as Button;
        bool acceptTerms = (bool)pressedButton.CommandParameter;
        await PopupAction.ClosePopup(acceptTerms);
    }
}