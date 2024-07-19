# Leave Management System

This repository contains the Leave Management System built with .NET Core. It uses Web API, Entity Framework, and follows a layered architecture.

## Technologies Used
- .NET Core
- Web API
- Entity Framework
- Unit test

## Getting Started

### Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Running the .NET API Application

1. **Clone the repository**:
    ```bash
    git clone https://github.com/Rizwanul-Islam/Leave-Management-System.git
    cd Leave-Management-System
    ```

2. **Navigate to the project directory**:
    ```bash
    cd HR.LeaveManagement.Api
    ```

3. **Restore the dependencies**:
    ```bash
    dotnet restore
    ```

4. **Update the connection string**:
    - Open `appsettings.json` located in the `HR.LeaveManagement.Api` project.
    - Update the `ConnectionStrings` section with your SQL Server connection details.

    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=your_server_name;Database=your_database_name;User Id=your_username;Password=your_password;"
    }
    ```

5. **Run the application**:
    ```bash
    dotnet run
    ```

### Database Setup

- There is a `Scripts` folder containing SQL scripts to create the necessary tables in the SQL Server database.
- Navigate to the `Scripts` folder and execute the scripts in your SQL Server management tool.

### Contributing

Contributions are welcome! Please fork the repository and submit a pull request for any enhancements or bug fixes.

