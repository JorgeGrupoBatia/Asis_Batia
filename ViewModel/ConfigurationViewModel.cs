using Asis_Batia.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Asis_Batia.ViewModel;

public partial class ConfigurationViewModel : ViewModelBase {

    [ObservableProperty]
    bool _isToggled;

    public ConfigurationViewModel() {
        IsToggled = UserSession.IsBiometricsActivated;
    }

    [RelayCommand]
    void ActivateBiometric() {
        UserSession.IsBiometricsActivated = IsToggled;
    }
}
