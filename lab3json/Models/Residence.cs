using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace lab3json.Models
{
    public class Residence : INotifyPropertyChanged
    {
        private string roomNumber = string.Empty;
        private string studentNameRef = string.Empty;
        private string checkInDate = string.Empty;
        private string notes = string.Empty;

        public string RoomNumber { get => roomNumber; set { roomNumber = value; OnPropertyChanged(); } }
        public string StudentNameRef { get => studentNameRef; set { studentNameRef = value; OnPropertyChanged(); } }
        public string CheckInDate { get => checkInDate; set { checkInDate = value; OnPropertyChanged(); } }
        public string Notes { get => notes; set { notes = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}