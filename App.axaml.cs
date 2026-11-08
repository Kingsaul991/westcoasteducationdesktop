using Avalonia;

using Avalonia.Controls.ApplicationLifetimes;

using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;

using WestcoastEducation.Desktop.Models;

using WestcoastEducation.Desktop.Services;

using WestcoastEducation.Desktop.ViewModels;

using WestcoastEducation.Desktop.Views;
 
namespace WestcoastEducation.Desktop;
 
public partial class App : Application

{

    public override void Initialize()

    {

        AvaloniaXamlLoader.Load(this);

    }
 
    public override void OnFrameworkInitializationCompleted()

    {

        var services = ConfigureServices();
 
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)

        {

            desktop.MainWindow = new MainWindow

            {

                DataContext = services.GetRequiredService<MainViewModel>()

            };

        }
 
        base.OnFrameworkInitializationCompleted();

    }

    private static ServiceProvider ConfigureServices()

    {

        var services = new ServiceCollection();

        var settings = RestDbSettings.Load();

        services.AddSingleton(settings.CreateHttpClient());

        services.AddSingleton<IRestDbService<Course>>(sp =>

            new RestDbService<Course>(sp.GetRequiredService<HttpClient>(), "courses"));

        services.AddSingleton<IRestDbService<Student>>(sp =>

            new RestDbService<Student>(sp.GetRequiredService<HttpClient>(), "students"));

        services.AddSingleton<IRestDbService<Teacher>>(sp =>

            new RestDbService<Teacher>(sp.GetRequiredService<HttpClient>(), "teachers"));
 

        services.AddTransient<MainViewModel>();
        services.AddTransient<CoursesViewModel>();
 
        return services.BuildServiceProvider();

    }

}
 