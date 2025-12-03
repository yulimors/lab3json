using lab3json.Views;

namespace lab3json
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
            Routing.RegisterRoute(nameof(AddEditResidencePage), typeof(AddEditResidencePage));
            Routing.RegisterRoute(nameof(AddEditStudentPage), typeof(AddEditStudentPage));
        }
    }
}