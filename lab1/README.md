# Lab 1 - Chat Application

**Student:** Anton Kholodar  
**Group:** KV-41mp  
**Laboratory Work:** Laboratory Work #1 - Backend Web Application Development  
**Report:** [Google Drive Link](https://docs.google.com/document/d/1RZ5DerPykOtyqqe0w6KECOAk2Z0byhGDfPdsp833gX0/edit?usp=sharing)

## Assignment

**Objective:** Learn to use backend web framework tools, REST API development, and testing utilities

**General Task:** Develop a backend Web application (Simple Chat) and test its functionality.

**Theme:** Simple Chat - implement API for user creation, sending and receiving messages in chat.

**Development Tools:** .NET 8, Entity Framework Core, RESTful API, Swagger (instead of Postman)

### Implemented Features:
- ✅ User registration (name, email, gender, date of birth)
- ✅ User login (email, password) with JWT authentication
- ✅ User profile management
- ✅ API for sending and receiving chat messages
- ✅ Swagger API documentation

---

A real-time chat application built with Clean Architecture principles using .NET 8, Entity Framework Core, and JWT authentication.

## 🏗️ Architecture

This project follows **Clean Architecture** with **Domain-Driven Design (DDD)** tactical patterns and clear separation of concerns:

```
├── ChatApp.Domain/          # Business entities and value objects
├── ChatApp.Application/     # Use cases, CQRS, validation
├── ChatApp.Infrastructure/  # Data access, external services
└── ChatApp.API/            # Controllers, middleware, configuration
```

### Architecture Layers

- **Domain Layer**: DDD tactical patterns - rich entities (`User`, `Message`), value objects (`Email`), domain validation
- **Application Layer**: CQRS with MediatR, FluentValidation, DTOs
- **Infrastructure Layer**: Entity Framework Core, BCrypt password hashing, SQLite database
- **API Layer**: RESTful endpoints, JWT authentication, Swagger documentation

## 🚀 Technologies Used

- **.NET 8** - Latest .NET framework
- **Entity Framework Core 9.0** - ORM with SQLite provider
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **JWT Bearer Authentication** - Security
- **BCrypt** - Password hashing
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Unit and integration testing

## 📋 Features

- **User Management**
  - User registration with validation
  - JWT-based authentication
  - Password hashing with BCrypt
  - Online status tracking

- **Real-time Messaging**
  - Send and receive messages
  - Message history retrieval
  - User-to-user communication

- **Security**
  - JWT token-based authentication
  - Secure password hashing
  - Input validation and sanitization
  - Global exception handling

## 🛠️ Setup and Installation

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Backend-laboratory-works-1/lab1
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/ChatApp.API
   ```

5. **Access the application**
   - API: `https://localhost:7206` or `http://localhost:5027`
   - Swagger UI: `https://localhost:7206/swagger`

## 🧪 Testing

The project includes comprehensive unit and integration tests:

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific test project
dotnet test tests/ChatApp.Domain.Tests
dotnet test tests/ChatApp.Application.Tests
dotnet test tests/ChatApp.API.Tests
```

### Test Coverage

- **Domain Tests**: 32 tests (Entity validation, value objects)
- **Application Tests**: Business logic and CQRS handlers
- **API Tests**: Integration tests for controllers and middleware

## 📚 API Documentation

### Authentication Endpoints

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!",
  "gender": 1,
  "dateOfBirth": "1990-01-01"
}
```

#### Login User
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "uuid",
    "name": "John Doe",
    "email": "john.doe@example.com",
    "isOnline": true
  }
}
```

### Message Endpoints

#### Send Message
```http
POST /api/messages
Authorization: Bearer <token>
Content-Type: application/json

{
  "content": "Hello, World!",
  "receiverId": "recipient-uuid"
}
```

#### Get Recent Messages
```http
GET /api/messages/recent?limit=50
Authorization: Bearer <token>
```

## 🗄️ Database Schema

The application uses SQLite with the following main entities:

### Users Table
- `Id` (GUID, Primary Key)
- `Name` (String, Required, Max 100 chars)
- `Email` (String, Required, Unique, Max 256 chars)
- `PasswordHash` (String, Required)
- `Gender` (Integer, Required)
- `DateOfBirth` (DateTime, Required)
- `IsOnline` (Boolean, Default: false)
- `LastSeenAt` (DateTime, Nullable)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime, Nullable)

### Messages Table
- `Id` (GUID, Primary Key)
- `Content` (String, Required, Max 1000 chars)
- `SenderId` (GUID, Foreign Key)
- `ReceiverId` (GUID, Foreign Key)
- `SentAt` (DateTime)
- `IsRead` (Boolean, Default: false)

## 🔧 Configuration

### JWT Settings
Configure JWT in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "ChatApp",
    "Audience": "ChatApp-Users",
    "ExpirationMinutes": 60
  }
}
```

### Database Connection
The application uses SQLite with the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=chatapp.db"
  }
}
```

## 🏛️ Design Patterns Used

- **Clean Architecture** - Separation of concerns across layers
- **Domain-Driven Design (DDD)** - Tactical patterns implementation:
  - **Rich Domain Entities** - Business logic encapsulated in entities (User with validation, business rules)
  - **Value Objects** - Immutable Email with validation and business rules
  - **Domain Services** - Business logic that doesn't belong to entities
  - **Repository Pattern** - Domain-defined data access contracts
- **CQRS** - Command Query Responsibility Segregation with MediatR
- **Dependency Injection** - Built-in .NET DI container
- **Factory Pattern** - Test web application factory for integration tests

## 🔍 Development Notes

### Key Implementation Details
- **DDD Value Object**: Email with immutability, validation, and automatic lowercase conversion
- **Rich Domain Entities**: User entity with encapsulated business logic (age calculation, profile updates, status management)
- **Domain Validation**: Business rules enforced at entity level (age restrictions, name validation)
- **Password hashing** using BCrypt with salt rounds
- **JWT token generation** with user claims
- **Entity Framework value converters** for domain value objects
- **Comprehensive validation** using FluentValidation at application layer

## 🤝 Contributing

1. Create a feature branch
2. Make your changes
3. Ensure all tests pass
4. Follow conventional commit messages
5. Submit a pull request

## 📄 License

This project is part of academic laboratory work. 