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

    private bool _isFileLoaded;
    public bool IsFileLoaded
    {
        get => _isFileLoaded;
        set { _isFileLoaded = value; OnPropertyChanged(); }
    }

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
        "Все", "ПІБ", "Факультет", "Курс", "Кімната"
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
        SelectedSearchCriterion = "Все";
        IsFileLoaded = false;
    }

    private string _currentFilePath;

    private async Task LoadData()
    {
        try
        {
            var path = await JsonService.PickFileAsync();
            if (path == null) return;

            _currentFilePath = path;
            var loaded = await JsonService.LoadDormitoryAsync(path);
            if (loaded == null)
            {
                await Shell.Current.DisplayAlert("Помилка", "Не вдалося прочитати файл", "Oк");
                return;
            }

            _dormitory = loaded;

            UpdateCollections(_dormitory.Residences, _dormitory.Students);

            SearchResultText = $"Завантажено {_dormitory.Residences.Count} записів, {_dormitory.Students.Count} студентів";
            IsFileLoaded = true;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Помилка", ex.Message, "OK");
        }
    }

    private void UpdateCollections(IEnumerable<Residence> resData, IEnumerable<Student> studData)
    {
        Residences.Clear();
        var sortedResidences = resData.OrderBy(r => r.RoomNumber.Length).ThenBy(r => r.RoomNumber);
        foreach (var r in sortedResidences) Residences.Add(r);

        Students.Clear();
        var sortedStudents = studData.OrderBy(s => s.FullName);
        foreach (var s in sortedStudents) Students.Add(s);
    }

    private async Task SaveData()
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            await Shell.Current.DisplayAlert("Помилка", "Спершу відкрийте файл JSON", "OK");
            return;
        }
        try
        {
            await JsonService.SaveDormitoryAsync(_dormitory, _currentFilePath);
            await Shell.Current.DisplayAlert("Успіх", "Файл збережено!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Помилка", ex.Message, "OK");
        }
    }

    private void SearchAll()
    {
        if (!IsFileLoaded) return;

        // Якщо поле пусте — показуємо все
        if (string.IsNullOrWhiteSpace(SearchText) || SelectedSearchCriterion == "Все")
        {
            UpdateCollections(_dormitory.Residences, _dormitory.Students);
            SearchResultText = $"Відображено всі записи";
            return;
        }

        var tempRes = new List<Residence>();
        var tempStud = new List<Student>();

        switch (SelectedSearchCriterion)
        {
            case "ПІБ":
                tempStud.AddRange(_dormitory.Students.Where(s => s.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
                tempRes.AddRange(_dormitory.Residences.Where(r => r.StudentNameRef.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
                break;

            case "Факультет":
                tempStud.AddRange(_dormitory.Students.Where(s => s.Faculty.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
                break;

            case "Курс":
                tempStud.AddRange(_dormitory.Students.Where(s => s.Course.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
                break;

            case "Кімната":
                tempRes.AddRange(_dormitory.Residences.Where(r => r.RoomNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
                break;
        }

        UpdateCollections(tempRes.Distinct(), tempStud.Distinct());

        SearchResultText = $"Знайдено: {Residences.Count} проживань, {Students.Count} студентів";
    }


    public void AddResidence(Residence res)
    {
        _dormitory.Residences.Add(res);
        SearchAll();
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
            SearchAll();
        }
    }

    public void AddStudent(Student st)
    {
        _dormitory.Students.Add(st);
        SearchAll();
    }
    public void RemoveStudent(Student st)
    {
        var residenceToDelete = Residences.FirstOrDefault(r => r.StudentNameRef == st.FullName);
        if (residenceToDelete != null)
        {
            _dormitory.Residences.Remove(residenceToDelete);
            Residences.Remove(residenceToDelete);
        }
        _dormitory.Students.Remove(st);
        Students.Remove(st);
    }
    public void UpdateStudent(Student oldSt, Student newSt)
    {
        string oldName = oldSt.FullName;
        string newName = newSt.FullName;

        if (oldName != newName)
        {
            var residencesToUpdate = _dormitory.Residences.Where(r => r.StudentNameRef == oldName).ToList();
            foreach (var res in residencesToUpdate)
            {
                res.StudentNameRef = newName;
            }
        }

        int index = _dormitory.Students.IndexOf(oldSt);
        if (index >= 0)
        {
            _dormitory.Students[index] = newSt;
            SearchAll();
        }
    }
}
