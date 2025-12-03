using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace lab3json.Models
{
    public class Student : INotifyPropertyChanged
    {
        private string fullName = string.Empty;
        private string faculty = string.Empty;
        private string department = string.Empty;
        private string course = string.Empty;

        public string FullName { get => fullName; set { fullName = value; OnPropertyChanged(); } }
        public string Faculty { get => faculty; set { faculty = value; OnPropertyChanged(); } }
        public string Department { get => department; set { department = value; OnPropertyChanged(); } }
        public string Course { get => course; set { course = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}