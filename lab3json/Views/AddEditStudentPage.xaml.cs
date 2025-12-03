using lab3json.Models;
using lab3json.ViewModels;
using Microsoft.Maui.Controls;

namespace lab3json.Views;

public partial class AddEditStudentPage : ContentPage
{
    private MainViewModel _vm;
    private Student _editingItem;

    public AddEditStudentPage(MainViewModel vm, Student item = null)
    {
        InitializeComponent();
        _vm = vm;
        _editingItem = item;

        if (item != null)
        {
            NameEntry.Text = item.FullName;
            FacultyEntry.Text = item.Faculty;
            DeptEntry.Text = item.Department;
            CourseEntry.Text = item.Course;
        }
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        var newItem = new Student
        {
            FullName = NameEntry.Text,
            Faculty = FacultyEntry.Text,
            Department = DeptEntry.Text,
            Course = CourseEntry.Text
        };

        if (_editingItem != null)
            _vm.UpdateStudent(_editingItem, newItem);
        else
            _vm.AddStudent(newItem);

        Shell.Current.GoToAsync("..");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}