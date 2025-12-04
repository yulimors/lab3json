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
    private const int MaxCapacity = 4; // Максимум 4 студенти в кімнаті

    public AddEditResidencePage(MainViewModel vm, Residence item = null)
    {
        InitializeComponent();
        _vm = vm;
        _editingItem = item;

        // 1. Фільтруємо студентів (щоб не показувати тих, хто вже живе)
        var occupiedStudents = _vm.Residences.Select(r => r.StudentNameRef).ToList();

        foreach (var student in _vm.Students)
        {
            // Показуємо студента, якщо він вільний АБО якщо це той самий, кого ми редагуємо
            bool isOccupied = occupiedStudents.Contains(student.FullName);
            if (!isOccupied || (item != null && item.StudentNameRef == student.FullName))
            {
                StudentPicker.Items.Add(student.FullName);
            }
        }

        // 2. Заповнення полів при редагуванні
        if (item != null)
        {
            RoomEntry.Text = item.RoomNumber;
            NotesEntry.Text = item.Notes; // Телефон

            if (!string.IsNullOrEmpty(item.StudentNameRef))
                StudentPicker.SelectedItem = item.StudentNameRef;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string roomStr = RoomEntry.Text?.Trim() ?? "";
        string phoneStr = NotesEntry.Text?.Trim() ?? "";

        // --- ВАЛІДАЦІЯ (ПЕРЕВІРКИ) ---

        // 1. Чи обрано студента?
        if (StudentPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Помилка", "Ви не обрали студента!", "Oк");
            return;
        }
        string selectedStudent = StudentPicker.SelectedItem.ToString();

        // 2. Номер кімнати (число > 0)
        if (!int.TryParse(roomStr, out int roomNum) || roomNum <= 0)
        {
            await DisplayAlert("Помилка", "Номер кімнати повинен бути більше 0!", "Oк");
            return;
        }

        // 3. Телефон (рівно 10 цифр)
        if (!Regex.IsMatch(phoneStr, @"^\d{10}$"))
        {
            await DisplayAlert("Помилка", "Телефон має містити 10 цифр!", "Oк");
            return;
        }

        // 4. Перевірка на "Клона" (чи є вже такий запис в цій кімнаті)
        var duplicate = _vm.Residences.FirstOrDefault(r => r.RoomNumber == roomStr && r.StudentNameRef == selectedStudent);
        if (duplicate != null && duplicate != _editingItem)
        {
            await DisplayAlert("Помилка", "Цей студент вже записаний у цю кімнату!", "Oк");
            return;
        }

        // 5. Перевірка "Студент-квартирант" (чи живе він в іншій кімнаті)
        var existingPlace = _vm.Residences.FirstOrDefault(r => r.StudentNameRef == selectedStudent);
        if (existingPlace != null && existingPlace != _editingItem)
        {
            await DisplayAlert("Помилка", $"Студент вже живе в кімнаті {existingPlace.RoomNumber}! Спочатку виселіть його.", "Oк");
            return;
        }

        // 6. Перевірка місткості (не більше 4 людей)
        int peopleInRoom = _vm.Residences.Count(r =>
            r.RoomNumber == roomStr &&
            (_editingItem == null || r.StudentNameRef != _editingItem.StudentNameRef));

        if (peopleInRoom >= MaxCapacity)
        {
            await DisplayAlert("Увага", $"Кімната {roomStr} переповнена! (Макс {MaxCapacity} чол.)", "Oк");
            return;
        }

        // -----------------------------

        var newItem = new Residence
        {
            RoomNumber = roomStr,
            StudentNameRef = selectedStudent,
            Notes = phoneStr,
            // Дату ставимо автоматично "сьогодні", щоб модель не була пустою, але користувач її не вводить
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