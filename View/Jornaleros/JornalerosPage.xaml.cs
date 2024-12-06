using Asis_Batia.ViewModel.Jornaleros;

namespace Asis_Batia.View.Jornaleros;

public partial class JornalerosPage : ContentPage {

    JornalerosViewModel _viewModel;

    public JornalerosPage() {
        InitializeComponent();
        _viewModel = new JornalerosViewModel();
        BindingContext = _viewModel;
    }
}