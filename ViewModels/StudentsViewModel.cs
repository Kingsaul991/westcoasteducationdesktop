using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;

using WestcoastEducation.Desktop.Models;

using WestcoastEducation.Desktop.Services;
 
namespace WestcoastEducation.Desktop.ViewModels;
 
public partial class StudentsViewModel : ViewModelBase

{

    private readonly IRestDbService<Student> _studentService;
 
    // The service comes from Dependency Injection

    public StudentsViewModel(IRestDbService<Student> studentService)

    {

        _studentService = studentService;

        _ = LoadAsync();

    }
 
    public ObservableCollection<Student> Students { get; } = new();
 
    // Form fields

    [ObservableProperty] private string _firstName = "";

    [ObservableProperty] private string _lastName = "";

    [ObservableProperty] private string _personalIdentityNumber = "";

    [ObservableProperty] private string _phoneNumber = "";

    [ObservableProperty] private string _street = "";

    [ObservableProperty] private string _postalCode = "";

    [ObservableProperty] private string _city = "";

    [ObservableProperty] private string _message = "";
 
    [RelayCommand]

    private async Task LoadAsync()

    {

        try

        {

            Message = "Loading...";

            var students = await _studentService.GetAllAsync();
 
            Students.Clear();

            foreach (var student in students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName))

            {

                Students.Add(student);

            }
 
            Message = $"{Students.Count} student(s) loaded.";

        }

        catch (Exception ex)

        {

            Message = "Could not load students: " + ex.Message;

        }

    }
 
    [RelayCommand]

    private async Task AddAsync()

    {

        if (string.IsNullOrWhiteSpace(FirstName) ||

            string.IsNullOrWhiteSpace(LastName) ||

            string.IsNullOrWhiteSpace(PersonalIdentityNumber))

        {

            Message = "First name, last name and personal identity number are required.";

            return;

        }
 
        foreach (var existing in Students)

        {

            if (existing.PersonalIdentityNumber == PersonalIdentityNumber.Trim())

            {

                Message = $"A student with personal identity number {PersonalIdentityNumber} already exists.";

                return;

            }

        }
 
        var student = new Student

        {

            FirstName = FirstName.Trim(),

            LastName = LastName.Trim(),

            PersonalIdentityNumber = PersonalIdentityNumber.Trim(),

            PhoneNumber = PhoneNumber.Trim(),

            Street = Street.Trim(),

            PostalCode = PostalCode.Trim(),

            City = City.Trim()

        };
 
        try

        {

            await _studentService.AddAsync(student);

            await LoadAsync();

            ClearForm();

            Message = $"Added {student.FullName}.";

        }

        catch (Exception ex)

        {

            Message = "Could not add the student: " + ex.Message;

        }

    }
 
    [RelayCommand]

    private void ClearForm()

    {

        FirstName = "";

        LastName = "";

        PersonalIdentityNumber = "";

        PhoneNumber = "";

        Street = "";

        PostalCode = "";

        City = "";

    }

}
 