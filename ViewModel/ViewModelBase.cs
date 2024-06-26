using Asis_Batia.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Asis_Batia.ViewModel;

public abstract partial class ViewModelBase : ObservableObject {

    public readonly HttpHelper _httpHelper;

    protected ViewModelBase() {
        _httpHelper = new HttpHelper();
    }
}
