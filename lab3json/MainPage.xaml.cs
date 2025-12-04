using lab3json.Models;
using lab3json.ViewModels;
using Microsoft.Maui.Controls;

namespace lab3json.Views;

public partial class MainPage : ContentPage
{
    private MainViewModel _vm => BindingContext as MainViewModel;

    public MainPage()
    {
        InitializeComponent();
    }

    // Проживання
    private async void OnAddResidenceClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushAsync(new AddEditResidencePage(_vm));
    }

    private async void OnEditResidenceClicked(object sender, EventArgs e)
    {
        if (_vm.SelectedResidence == null) { await DisplayAlert("Увага", "Оберіть запис", "OK"); return; }
        await Shell.Current.Navigation.PushAsync(new AddEditResidencePage(_vm, _vm.SelectedResidence));
    }

    private async void OnDeleteResidenceClicked(object sender, EventArgs e)
    {
        if (_vm.SelectedResidence != null)
        {
            if (await DisplayAlert("Видалити?", "Видалити цей запис?", "Так", "Ні"))
            {
                _vm.RemoveResidence(_vm.SelectedResidence);
                _vm.SelectedResidence = null;
            }
        }
    }

    // Студенти
    private async void OnAddStudentClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushAsync(new AddEditStudentPage(_vm));
    }

    private async void OnEditStudentClicked(object sender, EventArgs e)
    {
        if (_vm.SelectedStudent == null) { await DisplayAlert("Увага", "Оберіть студента", "OK"); return; }
        await Shell.Current.Navigation.PushAsync(new AddEditStudentPage(_vm, _vm.SelectedStudent));
    }

    private async void OnDeleteStudentClicked(object sender, EventArgs e)
    {
        if (_vm.SelectedStudent != null)
        {
            if (await DisplayAlert("Видалити?", "Видалити студента? (Це видалить його і з кімнати)", "Так", "Ні"))
            {
                _vm.RemoveStudent(_vm.SelectedStudent);
                _vm.SelectedStudent = null;
            }
        }
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///AboutPage");
    }
}