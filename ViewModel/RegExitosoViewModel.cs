using CommunityToolkit.Mvvm.Input;
using MauiPopup;

namespace Asis_Batia.ViewModel;

public partial class RegExitosoViewModel : ViewModelBase {

    [RelayCommand]
    private void Accept() {
        PopupAction.ClosePopup();
    }
}