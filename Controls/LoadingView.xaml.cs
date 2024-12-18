namespace Asis_Batia.Controls;

public partial class LoadingView : ContentView {

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(LoadingView), false, BindingMode.TwoWay);

    public static readonly BindableProperty TextLoadingProperty =
        BindableProperty.Create(nameof(TextLoading), typeof(string), typeof(LoadingView), null, BindingMode.TwoWay);

    public bool IsLoading {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public string TextLoading {
        get => (string)GetValue(TextLoadingProperty);
        set => SetValue(TextLoadingProperty, value);
    }

    public LoadingView() {
        InitializeComponent();
        Content.BindingContext = this;
    }
}