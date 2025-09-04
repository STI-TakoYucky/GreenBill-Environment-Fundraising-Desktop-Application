# WPF Project - Navigation & User Controls

This project is a **C# WPF application** that follows the MVVM (Model-View-ViewModel) pattern.  
The setup includes **User Controls, ViewModels, navigation services, and dependency injection** for better scalability and maintainability.

---

## 📂 Project Structure

- **Views/**
  - Contains all WPF User Controls (XAML files).
  - Example: `HomePage.xaml`, `Dashboard.xaml`.

- **ViewModels/**
  - Contains all the ViewModel classes.
  - Example: `HomePageViewModel.cs`, `DashboardViewModel.cs`.
  - All ViewModels extend from `Core.ViewModel`.

- **Core/**
  - Contains base classes like `ViewModel` and common utilities (e.g., `RelayCommand`).

- **Services/**
  - Contains services such as `INavigationService` for handling navigation.

- **App.xaml / App.xaml.cs**
  - Handles application startup.
  - Registers views and view models with **DataTemplates**.
  - Injects services (like `INavigationService`) into the application.

---

## 🛠 How It Works

### 1. Create a User Control
Create a new User Control for each page or component.  
Example:
```xml
<UserControl x:Class="MyApp.Views.HomePage"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Grid>
        <TextBlock Text="Welcome to Home Page!" />
    </Grid>
</UserControl>
````

---

### 2. Create a ViewModel

Each User Control has a corresponding ViewModel that extends `Core.ViewModel`.

Example:

```csharp
public class HomePageViewModel : Core.ViewModel
{

}
```

---

### 3. Register View and ViewModel in `App.xaml`

Link a View to its ViewModel using a **DataTemplate** in `App.xaml`.

```xml
<Application.Resources>
    <DataTemplate DataType="{x:Type vm:HomePageViewModel}">
        <views:HomePage />
    </DataTemplate>
</Application.Resources>
```

---

### 4. Inject ViewModel Services in `App.xaml.cs`

Register services (like `INavigationService`) and inject them into ViewModels.

```csharp
public App()
{
    IServiceCollection services = new ServiceCollection();

    services.AddSingleton<MainWindow>(provider => new MainWindow
    {
        DataContext = provider.GetRequiredService<MainWindowViewModel>()
    });

    // Add this line
    services.AddSingleton<PageViewModel>();
   
 
    services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

    serviceProvider = services.BuildServiceProvider();
}
```

---
## 🔀 Changing Current View

Go to MainWindowViewModel.cs
```csharp
 public MainWindowViewModel(INavigationService navService)
 {
     Navigation = navService;
     // Change to the ViewModel of the first view you want to display
     Navigation.NavigateTo<HomePageViewModel>();

     NavigateToSignin = new RelayCommand(o =>
     {
         ShowNavigation = false;
         Navigation.NavigateTo<SigninViewModel>();
     });
 }
```
---

## 🔀 Navigation

Navigation between pages is handled via `INavigationService`.

### Example: ViewModel with Navigation

```csharp
public class ViewModel : Core.ViewModel
{
    private INavigationService _navigationService;

    public INavigationService Navigation
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand NavigateToHome { get; set; }

    public ViewModel(INavigationService navService)
    {
        Navigation = navService;
        NavigateToHome = new RelayCommand(o =>
            Navigation.NavigateTo<HomePageViewModel>()
        );
    }
}
```

* `INavigationService` is injected into the ViewModel.
* `NavigateToHome` command navigates to `HomePageViewModel`.

---

## ▶️ Example Usage

* **Click a button** → Calls `NavigateToHome` command.
* **NavigationService** replaces the current ViewModel with `HomePageViewModel`.
* **App.xaml DataTemplate** automatically loads `HomePage.xaml` for display.

---

## Hiding Navigation Bar

Go to the view model of the view and extend the INavigationAware

```csharp
public class ViewModel : INavigationAware
{
      public bool ShowNavigation => false;
}

---
