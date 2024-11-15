using FoodTruckCentral.Mobile.ViewModels;

namespace FoodTruckCentral.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        //InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }

    protected override bool OnBackButtonPressed()
    {
        // Disable back button on login page
        return true;
    }
}