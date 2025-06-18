using LocationSample.Models;
using LocationSample.PageModels;

namespace LocationSample.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}