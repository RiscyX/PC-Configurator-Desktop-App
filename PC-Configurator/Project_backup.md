You are GitHub Copilot, an AI coding assistant. I’m working on a C# WPF desktop application with a SQL Server database managed via SSMS. The project follows the MVVM pattern and includes full CRUD operations for several domain entities. 

Please generate:
1. A high-level architectural overview, listing the main layers (Models, ViewModels, Views, Data Access).
2. A summary of the existing database schema (tables, primary/foreign keys) and how each table maps to a C# model class.
3. A list of all implemented screens/windows in the WPF app, including their purpose and the ViewModel they bind to.
4. The set of core services or repositories used for data access, and how they are injected into ViewModels.
5. Any missing pieces or TODOs you detect (e.g., validation, error handling, command implementations, unit tests).
6. Suggestions for refactoring or improving consistency (naming conventions, folder structure, DI container usage).
7. A proposed plan or checklist for finalizing the project tonight, broken down into prioritized tasks.

Assume the codebase root is organized as:
- /Models
- /ViewModels
- /Views
- /Services or /Repositories
- /Data (Entity Framework or raw ADO.NET)
- /Resources (XAML styles, images)

Also assume you have full access to the solution and can inspect class and XAML files. Be concise but thorough.
