# ASP.Net Core MVC Record Store App

A record store app built with C# and ASP.NET Core MVC, packed with features like user authentication, an easy-to-use admin panel for content management, and more. You can view the screenshots [here](MvcRecordStore/Screenshots/)

## Features

- **User Authentication & Role-Based Authorization**
  - Signup, login, and logout functionality.
  - Only authorized users with the "Admin" role can create/edit objects or use the admin panel.
- **Admin Panel**
  - The panel is accesible to each user with the "Admin" role through the dropdown menu in the navbar.
  - View artists, users, records, labels, genres and orders.
  - Edit or delete existing objects in the database.
- **Pagination and Search Bar**
  - All the pages are paginated and sorted by different orders to make navigation easier.
  - You can search for each artist, record, etc. that you may be looking for.
- **SOLID & Clean Code Architecture**
  - Separated the logics from the controller so that they're now in a unique service.
  - Each service has many methods and almost all of these methods have a single function. (based on the SOLID principles)
- **Payment Gateway**
  - Thanks to Parbad, this project now has a trial gateway to simulate the payment process.
- **Shopping Cart & Order History**
  - You can add records to your shopping cart and finalize the purchase later on.
  - You are also able to view your order history.


## Project Structure

```bash
.
├── Areas
│   └── Identity                # All the Identity files required for user authentication, etc.
├── bin                         # Stores compiled output files
├── Controllers
│   ├── AdminController.cs      # Controller for the admin panel
│   ├── ArtistsController.cs    # Controller for the Artist model
│   ├── CartController.cs       # Controller for the shopping cart
│   ├── GenresController.cs     # Controller for the Genre model
│   ├── HomeController.cs       # Controller for the home page (or pages not related to a specific model)
│   ├── LabelsController.cs     # Controller for the Label model
│   ├── OrdersController.cs     # Controller for the Order model
│   └── RecordsController.cs    # Controller for the Record model
├── Data
│   ├── StoreDbContext.cs       # Main DbContext file
│   ├── StoreUser.cs            # Custom user model
│   └── SeedData.cs             # Automatically seeds the database if it's empty
├── Extensions                  # Contains extension methods to add custom services
├── Migrations                  # Entity Framework Core database migrations
├── Models
│   ├── ViewModels              # All the view models are stored here
│   ├── Artist.cs               # The Artist model
│   ├── CartItem.cs             # The CartItem model
│   ├── ErrorViewModel.cs       # View model to display errors
│   ├── Genre.cs                # The Genre model
│   ├── Invoice.cs              # The Invoice model
│   ├── Label.cs                # The Label model
│   ├── Order.cs                # The Order model
│   ├── Record.cs               # The Record model
│   └── RecordPrice.cs          # The RecordPrice model which contains a combination of Format and Price belonging to each Record
├── obj                         # Holds temporary build files and intermediate object files used during compilation
├── Properties                  # Contains configuration files
├── Screenshots                 # Screenshots of the app
├── Views
│   ├── Admin                   # All the views for the admin panel
│   ├── Artists                 # All the views for the Artists model
│   ├── Cart                    # All the views for the Cart model
│   ├── Genres                  # All the views for the Genre model
│   ├── Home                    # All the views for the home page
│   ├── Labels                  # All the views for the Label model
│   ├── Orders                  # All the views for the Order model
│   ├── Records                 # All the views for the Record model
│   ├── Shared                  # Base Layouts
│   ├── _ViewImports.cshtml     # Used for dependency injection
│   └── _ViewStart.cshtml       # Sets the default layout
├── wwwroot                     # Static files
├── appsettings.json            # Self-explanatory
├── MvcRecordStore.csproj       # Defines dependencies, target frameworks, and build settings
└── Program.cs                  # The entry point of the app
```

## Setup Instructions

1. Prerequisites:

Before setting up the project, ensure you have the following installed:  

  - [.NET SDK (Latest LTS version)](https://dotnet.microsoft.com/download)  
  - SQLite
  - [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) (optional but recommended)  
  - [Entity Framework Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (install it using `dotnet tool install --global dotnet-ef`)  

2. Clone the Repository:

    ```bash
    git clone https://github.com/TinyPuff/MvcRecordStore.git
    cd MvcRecordStore  
    ```

3. Apply Migrations:

   ```bash
   dotnet ef database update
   ```

4. Build & Run the Application:

   ```bash
   dotnet build
   dotnet run
   ```

5. Access the application at `http://local:5000/` (if you see a different port in the command line/terminal, you should use that instead of 5000).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.