using Asis_Batia.Helpers;
using MauiPopup;
using MauiPopup.Views;
using System.Runtime.InteropServices;

namespace Asis_Batia.Popups;

public partial class RulesPopup : BasePopupPage {
    public RulesPopup() {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e) {
        Button pressedButton = sender as Button;
        bool acceptTerms = (bool)pressedButton.CommandParameter;

        if(acceptTerms) {
            UserSession.ShowTermsConditions = false;
        } else {
#if IOS
                exit(0);
#else
            Application.Current.Quit();
#endif
        }
        await PopupAction.ClosePopup(acceptTerms);
    }

#if IOS
    [DllImport("__Internal", EntryPoint = "exit")]
    public static extern void exit(int status);
#endif
}