using Asis_Batia.View;
using CommunityToolkit.Mvvm.Input;

namespace Asis_Batia.ViewModel;

public partial class FormuPrinAsisViewModel : ViewModelBase {

    [RelayCommand]
    private async Task NextPage() {
        await Shell.Current.GoToAsync(nameof(FormuSegAsis), true);
    }
}
