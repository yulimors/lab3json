using lab3json.Models;
using lab3json.ViewModels;
using Microsoft.Maui.Controls;

namespace lab3json.Views;

public partial class AddEditResidencePage : ContentPage
{
    private MainViewModel _vm;
    private Residence _editingItem;

    public AddEditResidencePage(MainViewModel vm, Residence item = null)
    {
        InitializeComponent();
        _vm = vm;
        _editingItem = item;

        if (item != null)
        {
            RoomEntry.Text = item.RoomNumber;
            StudentNameEntry.Text = item.StudentNameRef;
            DateEntry.Text = item.CheckInDate;
            NotesEntry.Text = item.Notes;
        }
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        var newItem = new Residence
        {
            RoomNumber = RoomEntry.Text,
            StudentNameRef = StudentNameEntry.Text,
            CheckInDate = DateEntry.Text,
            Notes = NotesEntry.Text
        };

        if (_editingItem != null)
            _vm.UpdateResidence(_editingItem, newItem);
        else
            _vm.AddResidence(newItem);

        Shell.Current.GoToAsync("..");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}