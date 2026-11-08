using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;

using WestcoastEducation.Desktop.Models;

using WestcoastEducation.Desktop.Services;
 
namespace WestcoastEducation.Desktop.ViewModels;
 
public partial class CoursesViewModel : ViewModelBase

{

    private readonly IRestDbService<Course> _courseService;

    private readonly IRestDbService<Teacher> _teacherService;
 
    // The services come from Dependency Injection

    public CoursesViewModel(IRestDbService<Course> courseService, IRestDbService<Teacher> teacherService)

    {

        _courseService = courseService;

        _teacherService = teacherService;

        _ = LoadAsync();

    }
 
    public ObservableCollection<Course> Courses { get; } = new();

    public ObservableCollection<string> TeacherNames { get; } = new();

    public string[] DurationUnits { get; } = { "Weeks", "Days" };
 
    // The course clicked in the list (null = adding a new course)

    [ObservableProperty]

    [NotifyPropertyChangedFor(nameof(IsEditing), nameof(SaveButtonText), nameof(FormHeading))]

    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]

    private Course? _selectedCourse;
 
    // Form fields

    [ObservableProperty] private string _courseNumber = "";

    [ObservableProperty] private string _title = "";

    [ObservableProperty] private decimal? _duration = 1;

    [ObservableProperty] private string _durationUnit = "Weeks";

    [ObservableProperty] private DateTime? _startDate = DateTime.Today;

    [ObservableProperty] private DateTime? _endDate = DateTime.Today;

    [ObservableProperty] private bool _isDistance;

    [ObservableProperty] private string? _teacher;

    [ObservableProperty] private string _message = "";
 
    public bool IsEditing => SelectedCourse != null;

    public string SaveButtonText => IsEditing ? "Update course" : "Add course";

    public string FormHeading => IsEditing ? "Edit course" : "New course";
 
    // Runs automatically when a course is clicked in the list

    partial void OnSelectedCourseChanged(Course? value)

    {

        if (value == null)

        {

            ClearForm();

            return;

        }
 
        CourseNumber = value.CourseNumber;

        Title = value.Title;

        Duration = value.Duration;

        DurationUnit = value.DurationUnit;

        StartDate = value.StartDate;

        EndDate = value.EndDate;

        IsDistance = value.IsDistance;

        Teacher = value.Teacher;

        Message = "";

    }
 
    [RelayCommand]

    private async Task LoadAsync()

    {

        try

        {

            Message = "Loading...";

            var courses = await _courseService.GetAllAsync();

            var teachers = await _teacherService.GetAllAsync();
 
            Courses.Clear();

            foreach (var course in courses.OrderBy(c => c.StartDate))

            {

                Courses.Add(course);

            }
 
            TeacherNames.Clear();

            foreach (var teacher in teachers)

            {

                TeacherNames.Add(teacher.FullName);

            }
 
            Message = $"{Courses.Count} course(s) loaded.";

        }

        catch (Exception ex)

        {

            Message = "Could not load courses: " + ex.Message;

        }

    }
 
    [RelayCommand]

    private void New()

    {

        SelectedCourse = null;

        ClearForm();

    }
 
    // Adds a new course, or updates the selected one (VG)

    [RelayCommand]

    private async Task SaveAsync()

    {

        if (string.IsNullOrWhiteSpace(CourseNumber) || string.IsNullOrWhiteSpace(Title))

        {

            Message = "Course number and title are required.";

            return;

        }
 
        if (StartDate == null || EndDate == null)

        {

            Message = "Choose a start date and an end date.";

            return;

        }
 
        if (EndDate < StartDate)

        {

            Message = "The end date cannot be before the start date.";

            return;

        }
 
        foreach (var existing in Courses)

        {

            if (existing.CourseNumber == CourseNumber.Trim() && existing != SelectedCourse)

            {

                Message = $"Course number {CourseNumber} already exists.";

                return;

            }

        }
 
        var course = new Course

        {

            Id = SelectedCourse?.Id,

            CourseNumber = CourseNumber.Trim(),

            Title = Title.Trim(),

            Duration = (int)(Duration ?? 1),

            DurationUnit = DurationUnit,

            StartDate = DateTime.SpecifyKind(StartDate.Value.Date, DateTimeKind.Utc),

            EndDate = DateTime.SpecifyKind(EndDate.Value.Date, DateTimeKind.Utc),

            IsDistance = IsDistance,

            Teacher = Teacher ?? ""

        };
 
        try

        {

            var wasEditing = IsEditing;

            if (wasEditing)

            {

                await _courseService.UpdateAsync(course);

            }

            else

            {

                await _courseService.AddAsync(course);

            }
 
            await LoadAsync();

            SelectedCourse = null;

            Message = wasEditing ? $"Updated {course}." : $"Added {course}.";

        }

        catch (Exception ex)

        {

            Message = "Could not save the course: " + ex.Message;

        }

    }
 
    // Only possible when a course is selected

    [RelayCommand(CanExecute = nameof(IsEditing))]

    private async Task DeleteAsync()

    {

        if (SelectedCourse?.Id == null)

        {

            return;

        }
 
        var name = SelectedCourse.ToString();
 
        try

        {

            await _courseService.DeleteAsync(SelectedCourse.Id);

            await LoadAsync();

            SelectedCourse = null;

            Message = $"Deleted {name}.";

        }

        catch (Exception ex)

        {

            Message = "Could not delete the course: " + ex.Message;

        }

    }
 
    private void ClearForm()

    {

        CourseNumber = "";

        Title = "";

        Duration = 1;

        DurationUnit = "Weeks";

        StartDate = DateTime.Today;

        EndDate = DateTime.Today;

        IsDistance = false;

        Teacher = null;

    }

}
 