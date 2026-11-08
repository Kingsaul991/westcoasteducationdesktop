namespace WestcoastEducation.Desktop.ViewModels;
 
// Holds one ViewModel per tab. They are handed in by Dependency Injection.
public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(CoursesViewModel courses, StudentsViewModel students, TeachersViewModel teachers)
    {
        Courses = courses;
        Students = students;
        Teachers = teachers;
    }
 
    public CoursesViewModel Courses { get; }
    public StudentsViewModel Students { get; }
    public TeachersViewModel Teachers { get; }
}