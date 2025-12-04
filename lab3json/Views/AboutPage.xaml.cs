namespace lab3json.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        // Повертаємося на головну сторінку
        // Використовуємо ///MainPage, бо це головний маршрут в AppShell
        await Shell.Current.GoToAsync("///MainPage");
    }
}