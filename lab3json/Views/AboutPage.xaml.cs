using Microsoft.Maui.Controls;

namespace lab3json.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
        this.Content.Opacity = 0;
        this.Appearing += AboutPage_Appearing;
    }

    private async void AboutPage_Appearing(object sender, EventArgs e)
    {
        await this.Content.FadeTo(1, 500);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await this.Content.FadeTo(0, 300);
        await Shell.Current.GoToAsync("///MainPage");
    }
}