using Asis_Batia.Helpers;
using MauiPopup;
using System.Windows.Input;

namespace Asis_Batia.ViewModel;

public class PopupRulesPageViewModel {
    public ICommand ClosePopupPageCommand { get; set; }
    public ICommand AcceptPopupPageCommand { get; set; }

    public PopupRulesPageViewModel() {
        ClosePopupPageCommand = new Command(async () => await Close());
        AcceptPopupPageCommand = new Command(async () => await Accept());
    }

    public async Task Close() {
        PopupAction.ClosePopup(true);
        Application.Current.Quit();
    }

    public async Task Accept() {
        UserSession.ShowTermsConditions = false;
        PopupAction.ClosePopup(false);
    }
}