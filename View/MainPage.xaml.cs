using Asis_Batia.ViewModel;

namespace Asis_Batia.View;

public partial class MainPage : ContentPage {

    public MainPage() {
        InitializeComponent();
    }

    protected override async void OnAppearing() {
        base.OnAppearing();  
        Loaded += MainPage_Loaded;
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
        Loaded -= MainPage_Loaded;
    }

    private void MainPage_Loaded(object sender, EventArgs e) {
        BindingContext = new MainPageViewModel();
    }
}
