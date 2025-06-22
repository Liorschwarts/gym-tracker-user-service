# Gym Tracker - User Service

## 🎯 Overview
Microservice for user management and authentication in the Gym Tracker application.

## 🛠️ Tech Stack
- **Framework:** ASP.NET Core 8
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Architecture:** Clean Architecture
- **API Documentation:** Swagger

## 🚀 Features
- ✅ User CRUD operations
- ✅ Clean Architecture (Controllers, Services, Models, Data)
- ✅ PostgreSQL integration
- ✅ Entity Framework migrations
- ✅ Swagger API documentation
- ✅ Dependency Injection

## 📁 Project Structure
├── Controllers/        # API endpoints
├── Services/          # Business logic
├── Models/            # Domain entities
├── Data/              # Database context
└── Migrations/        # Database migrations

## 🔧 Setup Instructions
1. Install PostgreSQL
2. Update connection string in `appsettings.json`
3. Run migrations: `dotnet ef database update`
4. Start application: `dotnet run`
5. Access Swagger: `https://localhost:xxxx/swagger`

## 📋 API Endpoints
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `GET /api/users/health` - Health check