using System.Linq;
using System.Text.RegularExpressions;
using lab3json.Models;
using lab3json.ViewModels;
using Microsoft.Maui.Controls;

namespace lab3json.Views;

public partial class AddEditStudentPage : ContentPage
{
    private MainViewModel _vm;
    private Student _editingItem;

    // Курси залишаємо списком, це зручно
    private readonly List<string> _courses = new() { "1", "2", "3", "4", "5", "6", "Інше" };

    public AddEditStudentPage(MainViewModel vm, Student item = null)
    {
        InitializeComponent();
        _vm = vm;
        _editingItem = item;

        // Наповнюємо тільки список курсів
        foreach (var c in _courses) CoursePicker.Items.Add(c);

        if (item != null)
        {
            NameEntry.Text = item.FullName;
            FacultyEntry.Text = item.Faculty; // Просто вставляємо текст
            DeptEntry.Text = item.Department;

            if (_courses.Contains(item.Course)) CoursePicker.SelectedItem = item.Course;
            else CoursePicker.SelectedItem = "Інше";
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Зчитуємо дані
        string pib = NameEntry.Text?.Trim() ?? "";
        string faculty = FacultyEntry.Text?.Trim() ?? ""; // Беремо текст з поля
        string dept = DeptEntry.Text?.Trim() ?? "";
        string course = CoursePicker.SelectedItem?.ToString();

        // --- ВАЛІДАЦІЯ ---

        // 1. ПІБ
        if (string.IsNullOrWhiteSpace(pib))
        {
            await DisplayAlert("Помилка", "Введіть ПІБ!", "OK");
            return;
        }

        var parts = pib.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
        {
            await DisplayAlert("Помилка", "ПІБ має складатися з 3 слів (Прізвище Ім'я По-батькові)!", "OK");
            return;
        }

        if (Regex.IsMatch(pib, @"\d"))
        {
            await DisplayAlert("Помилка", "В імені не може бути цифр!", "OK");
            return;
        }

        // 2. Факультет
        if (string.IsNullOrWhiteSpace(faculty))
        {
            await DisplayAlert("Помилка", "Введіть назву факультету!", "OK");
            return;
        }

        // 3. Курс
        if (string.IsNullOrEmpty(course))
        {
            await DisplayAlert("Помилка", "Оберіть курс!", "OK");
            return;
        }

        // 4. Перевірка на дублікат імені (щоб не ламати поселення)
        var duplicate = _vm.Students.FirstOrDefault(s => s.FullName.Equals(pib, StringComparison.OrdinalIgnoreCase));
        if (duplicate != null && duplicate != _editingItem)
        {
            await DisplayAlert("Помилка", "Студент з таким ПІБ вже існує! Імена мають бути унікальними.", "OK");
            return;
        }

        // -----------------

        var newItem = new Student
        {
            FullName = pib,
            Faculty = faculty,
            Department = dept,
            Course = course
        };

        if (_editingItem != null)
            _vm.UpdateStudent(_editingItem, newItem);
        else
            _vm.AddStudent(newItem);

        await Shell.Current.GoToAsync("..");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}