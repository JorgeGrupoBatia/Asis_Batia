using Asis_Batia.ViewModel;

namespace Asis_Batia.View;

public partial class MainPage : ContentPage {

    int _cont = 0;

    public MainPage() {
        InitializeComponent();
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        Loaded += MainPage_Loaded;
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
        Loaded -= MainPage_Loaded;
    }

    private void MainPage_Loaded(object sender, EventArgs e) {
        if(_cont < 1) {
            _cont++;
            BindingContext = new MainPageViewModel();
        }
    }
}
