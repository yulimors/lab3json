using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using lab3json.Models;
using lab3json.Services;
using Microsoft.Maui.Controls;

namespace lab3json.ViewModels;

public class MainViewModel : BindableObject
{
    private Dormitory _dormitory = new();

    public ObservableCollection<Residence> Residences { get; set; } = new();
    public ObservableCollection<Student> Students { get; set; } = new();

    private Residence _selectedResidence;
    public Residence SelectedResidence
    {
        get => _selectedResidence;
        set { _selectedResidence = value; OnPropertyChanged(); }
    }

    private Student _selectedStudent;
    public Student SelectedStudent
    {
        get => _selectedStudent;
        set { _selectedStudent = value; OnPropertyChanged(); }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }

    private string _selectedSearchCriterion;
    public string SelectedSearchCriterion
    {
        get => _selectedSearchCriterion;
        set { _selectedSearchCriterion = value; OnPropertyChanged(); }
    }

    public ObservableCollection<string> SearchCriteria { get; set; } = new()
    {
        "Всі поля",
        "ПІБ", "Факультет", "Курс", "Кімната"
    };

    private string _searchResultText;
    public string SearchResultText
    {
        get => _searchResultText;
        set { _searchResultText = value; OnPropertyChanged(); }
    }

    public ICommand LoadCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SearchCommand { get; }

    public MainViewModel()
    {
        LoadCommand = new Command(async () => await LoadData());
        SaveCommand = new Command(async () => await SaveData());
        SearchCommand = new Command(SearchAll);
        SelectedSearchCriterion = "Всі поля";
    }

    private string _currentFilePath;

    private async Task LoadData()
    {
        var path = await JsonService.PickFileAsync();
        if (path == null) return;

        _currentFilePath = path;
        var loaded = await JsonService.LoadDormitoryAsync(path);
        if (loaded == null) return;

        _dormitory = loaded;

        Residences.Clear();
        Students.Clear();
        foreach (var r in _dormitory.Residences) Residences.Add(r);
        foreach (var s in _dormitory.Students) Students.Add(s);

        SearchResultText = $"Завантажено: {_dormitory.Residences.Count} записів, {_dormitory.Students.Count} студентів";
    }

    private async Task SaveData()
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            await Shell.Current.DisplayAlert("Помилка", "Спершу відкрийте файл JSON", "OK");
            return;
        }

        await JsonService.SaveDormitoryAsync(_dormitory, _currentFilePath);
        await Shell.Current.DisplayAlert("Успіх", "Файл успішно збережений!", "OK");
    }

    private void SearchAll()
    {
        Residences.Clear();
        Students.Clear();

        switch (SelectedSearchCriterion)
        {
            case "Всі поля":
                foreach (var r in _dormitory.Residences.Where(r =>
                    r.RoomNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    r.StudentNameRef.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                    Residences.Add(r);

                foreach (var s in _dormitory.Students.Where(s =>
                    s.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    s.Faculty.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    s.Department.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    s.Course.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                    Students.Add(s);
                break;

            case "ПІБ":
                foreach (var s in _dormitory.Students.Where(s => s.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                    Students.Add(s);
                foreach (var r in _dormitory.Residences.Where(r => r.StudentNameRef.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                    Residences.Add(r);
                break;

            case "Факультет":
                foreach (var s in _dormitory.Students.Where(s => s.Faculty.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                    Students.Add(s);
                break;

            case "Курс":
                foreach (var s in _dormitory.Students.Where(s => s.Course.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                    Students.Add(s);
                break;

            case "Кімната":
                foreach (var r in _dormitory.Residences.Where(r => r.RoomNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                    Residences.Add(r);
                break;
        }

        SearchResultText = $"Знайдено: {Residences.Count} записів, {Students.Count} студентів";
    }

    public void AddResidence(Residence res)
    {
        _dormitory.Residences.Add(res);
        Residences.Add(res);
    }
    public void RemoveResidence(Residence res)
    {
        _dormitory.Residences.Remove(res);
        Residences.Remove(res);
    }
    public void UpdateResidence(Residence oldRes, Residence newRes)
    {
        int index = _dormitory.Residences.IndexOf(oldRes);
        if (index >= 0)
        {
            _dormitory.Residences[index] = newRes;
            int obsIndex = Residences.IndexOf(oldRes);
            if (obsIndex >= 0) Residences[obsIndex] = newRes;
        }
    }

    public void AddStudent(Student st)
    {
        _dormitory.Students.Add(st);
        Students.Add(st);
    }
    public void RemoveStudent(Student st)
    {
        _dormitory.Students.Remove(st);
        Students.Remove(st);
    }
    public void UpdateStudent(Student oldSt, Student newSt)
    {
        int index = _dormitory.Students.IndexOf(oldSt);
        if (index >= 0)
        {
            _dormitory.Students[index] = newSt;
            int obsIndex = Students.IndexOf(oldSt);
            if (obsIndex >= 0) Students[obsIndex] = newSt;
        }
    }
}