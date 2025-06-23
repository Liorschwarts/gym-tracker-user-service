# 🏋️ GymTracker - User Service

## 🎯 Overview
Production-ready microservice for user management and authentication in the GymTracker application.

## 🛠️ Tech Stack
- **Framework:** ASP.NET Core 9.0
- **Database:** PostgreSQL 15+
- **ORM:** Entity Framework Core
- **Authentication:** JWT Tokens
- **Password Hashing:** BCrypt
- **Logging:** Serilog
- **API Documentation:** Swagger/OpenAPI

## 🚀 Features
- ✅ User CRUD operations with soft delete
- ✅ JWT-based authentication
- ✅ Email uniqueness validation
- ✅ Password strength validation with BCrypt
- ✅ Global exception handling
- ✅ Health checks
- ✅ Comprehensive logging with Serilog
- ✅ Clean Architecture (Controllers, Services, Models, DTOs)
- ✅ Production-ready configuration

## 🏗️ Architecture
├── Controllers/         # API endpoints
├── Services/           # Business logic
├── Models/             # Domain entities
├── DTOs/               # Data transfer objects
├── Data/               # Database context
├── Exceptions/         # Custom exceptions
├── Middleware/         # Global middleware
├── Attributes/         # Custom attributes
└── Migrations/         # Database migrations

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL 15+
- (Optional) Docker

### Installation
1. **Clone the repository:**
   ```bash
   git clone <your-repo-url>
   cd GymTracker.UserService