namespace WestcoastEducation.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase

{

    public MainViewModel(CoursesViewModel courses)

    {

        Courses = courses;

    }
 
    public CoursesViewModel Courses { get; }

}
 