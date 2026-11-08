using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;

using WestcoastEducation.Desktop.Models;

using WestcoastEducation.Desktop.Services;
 
namespace WestcoastEducation.Desktop.ViewModels;
 
public partial class TeachersViewModel : ViewModelBase

{

    private readonly IRestDbService<Teacher> _teacherService;

    private readonly IRestDbService<Course> _courseService;

    public TeachersViewModel(IRestDbService<Teacher> teacherService, IRestDbService<Course> courseService)

    {

        _teacherService = teacherService;

        _courseService = courseService;

        _ = LoadAsync();

    }
 
    public ObservableCollection<Teacher> Teachers { get; } = new();

    [ObservableProperty]

    [NotifyPropertyChangedFor(nameof(IsEditing), nameof(SaveButtonText), nameof(FormHeading))]

    private Teacher? _selectedTeacher;

    [ObservableProperty] private string _firstName = "";

    [ObservableProperty] private string _lastName = "";

    [ObservableProperty] private string _personalIdentityNumber = "";

    [ObservableProperty] private string _phoneNumber = "";

    [ObservableProperty] private string _street = "";

    [ObservableProperty] private string _postalCode = "";

    [ObservableProperty] private string _city = "";

    [ObservableProperty] private string _areaOfExpertise = "";

    [ObservableProperty] private string _message = "";
 
    public bool IsEditing => SelectedTeacher != null;

    public string SaveButtonText => IsEditing ? "Update teacher" : "Add teacher";

    public string FormHeading => IsEditing ? "Edit teacher" : "New teacher";

    partial void OnSelectedTeacherChanged(Teacher? value)

    {

        if (value == null)

        {

            ClearForm();

            return;

        }
 
        FirstName = value.FirstName;

        LastName = value.LastName;

        PersonalIdentityNumber = value.PersonalIdentityNumber;

        PhoneNumber = value.PhoneNumber;

        Street = value.Street;

        PostalCode = value.PostalCode;

        City = value.City;

        AreaOfExpertise = value.AreaOfExpertise;

        Message = "";

    }
 
    [RelayCommand]

    private async Task LoadAsync()

    {

        try

        {

            Message = "Loading...";

            var teachers = await _teacherService.GetAllAsync();
 
            Teachers.Clear();

            foreach (var teacher in teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName))

            {

                Teachers.Add(teacher);

            }
 
            Message = $"{Teachers.Count} teacher(s) loaded.";

        }

        catch (Exception ex)

        {

            Message = "Could not load teachers: " + ex.Message;

        }

    }
 
    [RelayCommand]

    private void New()

    {

        SelectedTeacher = null;

        ClearForm();

    }
 
    [RelayCommand]

    private async Task SaveAsync()

    {

        if (string.IsNullOrWhiteSpace(FirstName) ||

            string.IsNullOrWhiteSpace(LastName) ||

            string.IsNullOrWhiteSpace(PersonalIdentityNumber))

        {

            Message = "First name, last name and personal identity number are required.";

            return;

        }
 
        if (string.IsNullOrWhiteSpace(AreaOfExpertise))

        {

            Message = "Enter the teacher's area of expertise.";

            return;

        }
 
        foreach (var existing in Teachers)

        {

            if (existing.PersonalIdentityNumber == PersonalIdentityNumber.Trim() && existing != SelectedTeacher)

            {

                Message = $"A teacher with personal identity number {PersonalIdentityNumber} already exists.";

                return;

            }

        }
 
        var teacher = new Teacher

        {

            Id = SelectedTeacher?.Id,

            FirstName = FirstName.Trim(),

            LastName = LastName.Trim(),

            PersonalIdentityNumber = PersonalIdentityNumber.Trim(),

            PhoneNumber = PhoneNumber.Trim(),

            Street = Street.Trim(),

            PostalCode = PostalCode.Trim(),

            City = City.Trim(),

            AreaOfExpertise = AreaOfExpertise.Trim()

        };
 
        try

        {

            var wasEditing = IsEditing;

            if (wasEditing)

            {

                var oldName = SelectedTeacher!.FullName;

                await _teacherService.UpdateAsync(teacher);
 
                // Courses store the teacher's name, so update them if the name changed

                if (oldName != teacher.FullName)

                {

                    await RenameTeacherOnCoursesAsync(oldName, teacher.FullName);

                }

            }

            else

            {

                await _teacherService.AddAsync(teacher);

            }
 
            await LoadAsync();

            SelectedTeacher = null;

            Message = wasEditing ? $"Updated {teacher.FullName}." : $"Added {teacher.FullName}.";

        }

        catch (Exception ex)

        {

            Message = "Could not save the teacher: " + ex.Message;

        }

    }
 
    private async Task RenameTeacherOnCoursesAsync(string oldName, string newName)

    {

        var courses = await _courseService.GetAllAsync();
 
        foreach (var course in courses)

        {

            if (course.Teacher == oldName)

            {

                course.Teacher = newName;

                await _courseService.UpdateAsync(course);

            }

        }

    }
 
    private void ClearForm()

    {

        FirstName = "";

        LastName = "";

        PersonalIdentityNumber = "";

        PhoneNumber = "";

        Street = "";

        PostalCode = "";

        City = "";

        AreaOfExpertise = "";

    }

}
 