using Asis_Batia.ViewModel;

namespace Asis_Batia.View;

public partial class FormuPrinAsis : ContentPage {

    FormuPrinAsisViewModel _viewModel;

    public FormuPrinAsis() {
        InitializeComponent();
        _viewModel=new FormuPrinAsisViewModel();
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing() {
        base.OnAppearing();
        await _viewModel.OnAppearing();
    }
}