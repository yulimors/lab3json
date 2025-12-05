using System.Linq;
using System.Text.RegularExpressions;
using lab3json.Models;
using lab3json.ViewModels;
using Microsoft.Maui.Controls;

namespace lab3json.Views;

public partial class AddEditResidencePage : ContentPage
{
    private MainViewModel _vm;
    private Residence _editingItem;
    private const int MaxCapacity = 4;

    public AddEditResidencePage(MainViewModel vm, Residence item = null)
    {
        InitializeComponent();
        _vm = vm;
        _editingItem = item;

        var occupiedStudents = _vm.Residences.Select(r => r.StudentNameRef).ToList();

        foreach (var student in _vm.Students)
        {
            bool isOccupied = occupiedStudents.Contains(student.FullName);
            if (!isOccupied || (item != null && item.StudentNameRef == student.FullName))
            {
                StudentPicker.Items.Add(student.FullName);
            }
        }

        if (item != null)
        {
            RoomEntry.Text = item.RoomNumber;
            NotesEntry.Text = item.Notes; 

            if (!string.IsNullOrEmpty(item.StudentNameRef))
                StudentPicker.SelectedItem = item.StudentNameRef;
        }
      
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string roomStr = RoomEntry.Text?.Trim() ?? "";
        string phoneStr = NotesEntry.Text?.Trim() ?? "";

        if (StudentPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Помилка", "Ви не обрали студента!", "Oк");
            return;
        }
        string selectedStudent = StudentPicker.SelectedItem.ToString();

        if (!int.TryParse(roomStr, out int roomNum) || roomNum <= 0)
        {
            await DisplayAlert("Помилка", "Номер кімнати повинен бути більше 0!", "Oк");
            return;
        }

        if (!Regex.IsMatch(phoneStr, @"^\d{10}$"))
        {
            await DisplayAlert("Помилка", "Телефон має містити 10 цифр!", "Oк");
            return;
        }

        var duplicate = _vm.Residences.FirstOrDefault(r => r.RoomNumber == roomStr && r.StudentNameRef == selectedStudent);
        if (duplicate != null && duplicate != _editingItem)
        {
            await DisplayAlert("Помилка", "Цей студент вже записаний у цю кімнату!", "Oк");
            return;
        }

        var existingPlace = _vm.Residences.FirstOrDefault(r => r.StudentNameRef == selectedStudent);
        if (existingPlace != null && existingPlace != _editingItem)
        {
            await DisplayAlert("Помилка", $"Студент вже живе в кімнаті {existingPlace.RoomNumber}! Спочатку виселіть його.", "Oк");
            return;
        }

        int peopleInRoom = _vm.Residences.Count(r =>
            r.RoomNumber == roomStr &&
            (_editingItem == null || r.StudentNameRef != _editingItem.StudentNameRef));

        if (peopleInRoom >= MaxCapacity)
        {
            await DisplayAlert("Увага", $"Кімната {roomStr} переповнена! (Макс {MaxCapacity} чол.)", "Oк");
            return;
        }

        var newItem = new Residence
        {
            RoomNumber = roomStr,
            StudentNameRef = selectedStudent,
            Notes = phoneStr,
            CheckInDate = DateTime.Now.ToString("yyyy-MM-dd")
        };

        if (_editingItem != null)
            _vm.UpdateResidence(_editingItem, newItem);
        else
            _vm.AddResidence(newItem);

        await Shell.Current.GoToAsync("..");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
