using System.Collections.ObjectModel;

namespace lab3json.Models
{
    public class Dormitory
    {
        public ObservableCollection<Residence> Residences { get; set; } = new();
        public ObservableCollection<Student> Students { get; set; } = new();
    }
}