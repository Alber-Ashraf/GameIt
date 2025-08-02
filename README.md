<h1 align="center">🎮 GameIt</h1>
<p align="center">
  A full-stack web application for discovering, reviewing, and purchasing video games.
  <br />
  Built with <b>.NET 8</b>, <b>Blazor</b>, <b>Clean Architecture</b>, and <b>Stripe Integration</b>.
</p>

---

## 📌 Overview

**GameIt** is a modular game marketplace platform built using **ASP.NET Core Web API**, **Blazor**, and **Clean Architecture**. It offers features such as game browsing, reviews, wishlists, secure Stripe payments, and a full MVC UI experience.

---

## 🚀 Key Features

### 🕹️ Game Management
- ✅ Create, update, delete games
- ✅ View game details (with category, rating, discount, publisher)
- ✅ Filter by category, get featured and similar games

### 🗂️ Category Management
- ✅ Full CRUD for game categories
- ✅ Retrieve categories by name or ID
- ✅ Include games inside category details

### ⭐ Review System
- ✅ Add, update, and delete game reviews
- ✅ Get all reviews for a specific game
- ✅ Review includes user information

### ❤️ Wishlist
- ✅ Add game to wishlist
- ✅ Remove or clear wishlist items
- ✅ Get current user’s wishlist

### 💳 Payments & Purchases
- ✅ Stripe integration for secure checkout
- ✅ Webhook for post-payment verification
- ✅ Automatically add purchased games to user's library
- ✅ Support refund operations
- ✅ Get user purchase history

### 📚 Game Library
- ✅ Each user has their own downloadable game library
- ✅ Games stored after successful payment

### 🔐 Authentication & Identity
- ✅ JWT Authentication (Login & Register)
- ✅ Role-based access (Admin / User)
- ✅ Identity setup with ASP.NET Identity
- ✅ Auth integration in Blazor frontend

### 🌐 Blazor UI (GameIt.UI)
- ✅ Game catalog UI with filtering and featured sections
- ✅ Game details and review system
- ✅ Wishlist and personal library
- ✅ Auth-aware UI (login, logout, register)

### 🧪 Testing
- ✅ Application Layer Unit Tests
- ✅ Integration Tests for Persistence Layer
- ✅ Testing Handlers and Queries with MediatR

### 🧱 Architecture
- ✅ Clean Architecture (Onion Layering)
- ✅ MediatR + CQRS pattern
- ✅ Repository + Unit of Work Pattern
- ✅ FluentValidation on all commands
- ✅ AutoMapper for mapping entities to DTOs
- ✅ Global Exception Handling

### 📦 Infrastructure
- ✅ EF Core with Code-First Migrations
- ✅ Background Cron Jobs (e.g., Remove expired discounts)
- ✅ SendGrid Integration for email services
- ✅ Logging and structured responses

---

## 🛠️ Tech Stack

| Category             | Tools/Frameworks                                              |
|----------------------|---------------------------------------------------------------|
| Language             | C#, .NET 8                                                    |
| Backend              | ASP.NET Core Web API                                          |
| Frontend             | Blazor WebAssembly                                            |
| Database             | SQL Server, EF Core                                           |
| API Communication    | MediatR, CQRS, AutoMapper, NSwag                              |
| Validation           | FluentValidation                                              |
| Authentication       | JWT, ASP.NET Core Identity                                    |
| Payment              | Stripe + Webhooks                                             |
| Email Service        | SendGrid                                                      |
| Testing              | xUnit, Integration Testing                                    |
| Architecture         | Clean Architecture, Repository Pattern, DI, UoW               |

---
## 📸 Screenshots

<img width="1800" height="202" alt="image" src="https://github.com/user-attachments/assets/1aa732cd-2688-41f5-9046-e14a95be7b1b" />
<img width="1802" height="475" alt="image" src="https://github.com/user-attachments/assets/809d4dc4-fd28-435c-b81c-4194ff3154f4" />
<img width="1805" height="332" alt="image" src="https://github.com/user-attachments/assets/c9692fd2-f13c-4d98-817c-d3e833a2524d" />
<img width="1801" height="611" alt="image" src="https://github.com/user-attachments/assets/8d7b002a-684f-4a1c-bfd1-01eb83e43fb9" />
<img width="1788" height="128" alt="image" src="https://github.com/user-attachments/assets/115a098b-a972-468c-a094-3d5dc60290cd" />
<img width="1802" height="194" alt="image" src="https://github.com/user-attachments/assets/8a0cfa16-4841-45e3-8a04-6eafc754537d" />
<img width="1795" height="337" alt="image" src="https://github.com/user-attachments/assets/61b26cb1-0b00-4db0-8d62-66287ebba19a" />
<img width="1799" height="128" alt="image" src="https://github.com/user-attachments/assets/caba72ca-c1b8-4c41-8767-c8be1df6d056" />
<img width="1805" height="333" alt="image" src="https://github.com/user-attachments/assets/44d8253f-cc9b-4100-ade1-621dcb7f613d" />

---

## ⚙️ Getting Started

### 1️⃣ Clone the Repository
``` bash
git clone https://github.com/Alber-Ashraf/GameIt.git
cd GameIt
```

2️⃣ Configure the Database
Set your SQL Server connection string in:
``` bash
GameIt.API/appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "your-sql-server-connection"
}
```

3️⃣ Apply Migrations
``` bash
cd GameIt.Infrastructure
dotnet ef database update
```

4️⃣ Run the API
``` bash
cd GameIt.API
dotnet run
```

5️⃣ Run the Blazor UI
``` bash
cd GameIt.UI
dotnet run
```
---
## 📁 Folder Structure
``` bash
📦 GameIt
├── 📂 GameIt.API                                     # Main ASP.NET Core Web API project
│   ├── 📂 Controllers                                # API Controllers (Game, Purchase, Auth, etc.)
│   ├── 📂 Middlewares                                # Custom middleware (exception handling, etc.)
│   ├── 📂 Models                                     # API-specific models (e.g., requests/responses)
│   ├── 📂 Properties                                 # LaunchSettings for local development
│   ├── GameIt.API.csproj                             # Project file for the API
│   ├── GameIt.Api.http                               # HTTP requests for testing endpoints
│   ├── Program.cs                                    # API entry point & configuration
│   ├── appsettings.json                              # App configuration (connection strings, JWT, etc.)
│   └── appsettings.Development.json                  # Dev-specific app settings

├── 📂 GameIt.Application.UnitTests                   # Unit testing for Application Layer
│   ├── 📂 Features/Game/Queries                      # Tests for Game-related CQRS queries
│   ├── 📂 Mocks                                      # Mocked data and services for testing
│   └── GameIt.Application.UnitTests.csproj           # Project file for unit tests

├── 📂 GameIt.Application                             # Application layer (business logic)
│   ├── 📂 Exception                                  # Custom exceptions for application errors
│   ├── 📂 Features                                   # CQRS Commands and Queries
│   ├── 📂 Interfaces                                 # Service/repository interfaces
│   ├── 📂 MappingProfiles                            # AutoMapper profiles
│   ├── 📂 Models                                     # DTOs and shared models
│   ├── ApplicationServiceRegistration.cs             # Service registration for DI
│   └── GameIt.Application.csproj                     # Project file for application layer

├── 📂 GameIt.BlazorUI                                # Blazor WebAssembly UI project
│   ├── 📂 Contracts                                  # Request/response contracts
│   ├── 📂 Layout                                     # Layout components (Nav, MainLayout, etc.)
│   ├── 📂 MappingProfiles                            # AutoMapper profiles for UI
│   ├── 📂 Models                                     # ViewModels for UI components
│   ├── 📂 Pages                                      # Razor components/pages (Home, Games, Auth, etc.)
│   ├── 📂 Properties                                 # LaunchSettings for Blazor
│   ├── 📂 Providers                                  # Authentication and token providers
│   ├── 📂 Services                                   # UI-facing services (GameService, AuthService, etc.)
│   ├── 📂 wwwroot                                    # Static assets (CSS, images, JS)
│   ├── App.razor                                     # Blazor app entry component
│   ├── GameIt.BlazorUI.csproj                        # Project file for the Blazor UI
│   ├── Program.cs                                    # Blazor app setup and service registration
│   ├── RedirectToLogin.razor                         # Redirect logic for unauthenticated users
│   └── _Imports.razor                                # Global using directives

├── 📂 GameIt.Domain                                  # Domain layer (core business entities)
│   ├── 📂 Common                                     # Base entities, enums, or value objects
│   ├── Category.cs                                   # Category entity
│   ├── Discount.cs                                   # Discount entity
│   ├── Game.cs                                       # Game entity
│   ├── Library.cs                                    # Library entity (user-owned games)
│   ├── Purchase.cs                                   # Purchase entity (transaction details)
│   ├── Review.cs                                     # Review entity (user feedback on games)
│   ├── Wishlist.cs                                   # Wishlist entity
│   └── GameIt.Domain.csproj                          # Project file for domain layer

├── 📂 GameIt.Identity                                # Identity and Authentication
│   ├── 📂 Configurations                             # Fluent API for Identity entities
│   ├── 📂 DbContext                                  # Identity database context
│   ├── 📂 Migrations                                 # EF Core migrations for Identity DB
│   ├── 📂 Models                                     # Identity models (ApplicationUser, etc.)
│   ├── 📂 Services                                   # Auth and user services
│   ├── GameIt.Identity.csproj                        # Project file for Identity
│   └── IdentityServicesRegistration.cs              # DI setup for Identity services

├── 📂 GameIt.Infrastructure                          # Infrastructure services (Email, Payments, Logging)
│   ├── 📂 DiscountService                            # Discount logic (e.g., active/expired)
│   ├── 📂 EmailService                               # SendGrid or email service implementation
│   ├── 📂 Logging                                    # Logging configuration (Serilog, etc.)
│   ├── 📂 Stripe                                     # Stripe payment integration
│   ├── GameIt.Infrastructure.csproj                 # Project file for infrastructure
│   ├── InfrastructureServiceRegistration.cs         # DI setup for infrastructure
│   └── RecurringJobsInitializer.cs                  # Cron jobs for scheduled tasks

├── 📂 GameIt.Persistence.IntegrationTests            # Integration testing for persistence layer
│   ├── GameItDatabaseContextTests.cs                 # Integration tests for EF Core context
│   └── GameIt.Persistence.IntegrationTests.csproj    # Project file for integration tests

├── 📂 GameIt.Persistence                             # Data persistence and database setup
│   ├── 📂 DatabaseContext                            # EF Core DbContext and interfaces
│   ├── 📂 EntityConfigurations                       # Fluent API configs for entities
│   ├── 📂 Migrations                                 # EF Core migrations for main DB
│   ├── 📂 Repositories                               # Repository implementations for data access
│   └── PersistenceServiceRegistration.cs             # DI setup for persistence layer

├── .gitattributes                                    # Git settings for line endings, etc.
├── .gitignore                                        # Files/folders to ignore in version control
└── GameIt.sln                                        # Visual Studio solution file
```
---
## 💡 Author
Developed with ❤️ by Alber Ashraf
