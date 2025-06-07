# Lab 2 - Real-time Chat Application with Admin Dashboard

**Student:** Anton Kholodar  
**Group:** KV-41mp  
**Laboratory Work:** Laboratory Work #2 - Real-time Data Exchange and Admin Management  
**Report:** [Google Drive Link](https://docs.google.com/document/d/1RZ5DerPykOtyqqe0w6KECOAk2Z0byhGDfPdsp833gX0/edit?usp=sharing)

## Assignment

**Objective:** Extend Lab 1 chat application with real-time data exchange capabilities and admin functionality to monitor online users.

**General Task:** Develop real-time communication features and administrative interface for user monitoring.

**Theme:** Real-time Chat with Admin Dashboard - implement SignalR for instant messaging and admin endpoints for user management.

**Development Tools:** .NET 8, Entity Framework Core, SignalR, JWT Authentication, RESTful API, Swagger

### Implemented Features:
- ✅ Real-time messaging with SignalR
- ✅ User role management (User/Admin)
- ✅ Admin dashboard for monitoring online users
- ✅ JWT authentication with role-based authorization
- ✅ Connection tracking and status notifications
- ✅ Group messaging and admin notifications
- ✅ Enhanced database schema with roles

---

A professional real-time chat application with administrative capabilities, built using Clean Architecture principles, .NET 8, SignalR, and advanced security features.

## 🏗️ Enhanced Architecture

This project follows **Clean Architecture** with **Domain-Driven Design (DDD)** tactical patterns and real-time communication:

```
├── ChatApp.Domain/          # Business entities, roles, and value objects
├── ChatApp.Application/     # Use cases, CQRS, admin queries
├── ChatApp.Infrastructure/  # SignalR, data access, connection services
└── ChatApp.API/            # Controllers, hubs, middleware, configuration
```

### Architecture Layers

- **Domain Layer**: DDD tactical patterns - rich entities (`User` with roles, `Message`), value objects (`Email`), UserRole enum
- **Application Layer**: CQRS with MediatR, FluentValidation, admin queries, DTOs
- **Infrastructure Layer**: Entity Framework Core, SignalR connection service, BCrypt password hashing, SQLite database
- **API Layer**: RESTful endpoints, SignalR hubs, JWT authentication with roles, admin controllers, Swagger documentation

## 🚀 Technologies Used

- **.NET 8** - Latest .NET framework
- **Entity Framework Core 9.0** - ORM with SQLite provider
- **SignalR** - Real-time communication framework
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **JWT Bearer Authentication** - Security with role claims
- **BCrypt** - Password hashing
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Unit and integration testing

## 📋 Features

### **User Management**
- User registration with validation and role assignment
- JWT-based authentication with role claims (User/Admin)
- Password hashing with BCrypt
- Real-time online status tracking
- Role-based access control

### **Real-time Communication**
- Instant messaging with SignalR
- Connection and disconnection notifications
- Group management (Users, Admins)
- Real-time status updates for administrators
- Multi-device connection support

### **Administrative Dashboard**
- Monitor online users in real-time
- View detailed user information
- Track user connection statistics
- System-wide statistics (total/online/offline users)
- Admin-only notifications and events

### **Security & Performance**
- JWT token-based authentication with role claims
- SignalR authentication via access tokens
- Role-based endpoint authorization
- Input validation and sanitization
- Global exception handling
- Efficient connection management with memory caching

## 🛠️ Setup and Installation

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Backend-laboratory-works-1/lab2
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update --project src/ChatApp.Infrastructure --startup-project src/ChatApp.API
   ```

4. **Build the solution**
   ```bash
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run --project src/ChatApp.API
   ```

6. **Access the application**
   - API: `https://localhost:7206` or `http://localhost:5027`
   - Swagger UI: `https://localhost:7206/swagger`
   - SignalR Hub: `wss://localhost:7206/chathub`

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

- **Domain Tests**: Entity validation, value objects, role management
- **Application Tests**: Business logic, CQRS handlers, admin queries
- **API Tests**: Integration tests for controllers, SignalR hubs, admin endpoints

### Test Data

The application includes seeded test data:
- **Admin User**: `admin@chatapp.com` (password: `Admin123!`)
- **Regular Users**: John Doe, Jane Smith, Bob Johnson (password: `User123!`)
- **Sample Messages**: Initial chat history for testing

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
  "email": "admin@chatapp.com",
  "password": "Admin123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "uuid",
    "name": "Admin User",
    "email": "admin@chatapp.com",
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
  "content": "Hello, World!"
}
```

#### Get Recent Messages
```http
GET /api/messages/recent?limit=50
Authorization: Bearer <token>
```

### Admin Endpoints (Requires Admin Role)

#### Get Online Users
```http
GET /api/admin/online-users
Authorization: Bearer <admin-token>
```

**Response:**
```json
{
  "onlineUserIds": ["user-guid-1", "user-guid-2"],
  "onlineUserCount": 2,
  "timestamp": "2025-06-07T12:00:00Z"
}
```

#### Get Online Users Detailed
```http
GET /api/admin/online-users/detailed
Authorization: Bearer <admin-token>
```

#### Get User Status
```http
GET /api/admin/users/status/{userId}
Authorization: Bearer <admin-token>
```

#### Get System Statistics
```http
GET /api/admin/stats
Authorization: Bearer <admin-token>
```

### Real-time SignalR Events

#### Connect to SignalR Hub
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub?access_token=YOUR_JWT_TOKEN")
    .build();

// Listen for events
connection.on("ReceiveMessage", (data) => {
    console.log("New message:", data);
});

connection.on("UserConnected", (data) => {
    console.log("User connected:", data);
});

connection.on("UserDisconnected", (data) => {
    console.log("User disconnected:", data);
});

connection.on("UserStatusChanged", (data) => {
    console.log("User status changed:", data); // Admin only
});

// Start connection
connection.start().then(() => {
    console.log("Connected to SignalR Hub!");
    
    // Send a message
    connection.invoke("SendMessage", "Hello from SignalR!");
    
    // Join admin group (if admin)
    connection.invoke("JoinAdminGroup");
});
```

## 🗄️ Enhanced Database Schema

The application uses SQLite with enhanced schema for Lab 2:

### Users Table
- `Id` (GUID, Primary Key)
- `Name` (String, Required, Max 100 chars)
- `Email` (String, Required, Unique, Max 256 chars)
- `PasswordHash` (String, Required)
- `Gender` (Integer, Required)
- `DateOfBirth` (DateTime, Required)
- `IsOnline` (Boolean, Default: false)
- `LastSeenAt` (DateTime, Nullable)
- **`Role` (Integer, Required, Default: 1)** ⭐ **NEW in Lab 2**
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime, Nullable)

### Messages Table
- `Id` (GUID, Primary Key)
- `Content` (String, Required, Max 1000 chars)
- `SenderId` (GUID, Foreign Key)
- **`IsEdited` (Boolean, Default: false)** ⭐ **NEW in Lab 2**
- **`EditedAt` (DateTime, Nullable)** ⭐ **NEW in Lab 2**
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime, Nullable)

### Role System
- **User Role (1)**: Default role for regular users
- **Admin Role (2)**: Administrative privileges, access to admin endpoints

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

### SignalR Configuration
- **Hub Endpoint**: `/chathub`
- **Authentication**: JWT via `access_token` query parameter
- **CORS Policy**: Configured for real-time communication
- **Groups**: Automatic assignment based on user roles

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
  - **Rich Domain Entities** - User with role management, business rules
  - **Value Objects** - Immutable Email with validation
  - **Domain Services** - Business logic for role management
  - **Repository Pattern** - Domain-defined data access contracts
- **CQRS** - Command Query Responsibility Segregation with MediatR
- **Dependency Injection** - Built-in .NET DI container
- **Factory Pattern** - Test web application factory for integration tests
- **Observer Pattern** - SignalR event-driven real-time communication

## 🔍 Development Notes

### Key Implementation Details
- **Role-based Authorization**: `[Authorize(Roles = "Admin")]` protection for admin endpoints
- **SignalR Authentication**: JWT token validation for WebSocket connections
- **Connection Management**: In-memory caching for tracking user connections
- **Real-time Events**: UserConnected, UserDisconnected, UserStatusChanged, ReceiveMessage
- **Group Management**: Automatic assignment to Users and Admins groups
- **Multi-device Support**: Multiple connections per user with proper tracking

### Lab 2 Enhancements
- **Real-time Communication**: SignalR implementation with JWT authentication
- **Admin Dashboard**: Complete administrative interface for user monitoring
- **Role System**: User/Admin roles with database persistence
- **Enhanced Security**: Role-based authorization and SignalR authentication
- **Connection Tracking**: Live monitoring of user online status
- **Data Seeding**: Sample users and messages for testing

## 🚀 Testing the Application

### Manual Testing Steps

1. **Start the application**
   ```bash
   dotnet run --project src/ChatApp.API
   ```

2. **Open Swagger UI**: `https://localhost:7206/swagger`

3. **Login as admin**: Use `admin@chatapp.com` / `Admin123!`

4. **Test admin endpoints** with the received JWT token

5. **Connect to SignalR** using browser console or SignalR client

6. **Monitor real-time events** and user connections

## 🤝 Contributing

1. Create a feature branch from `develop`
2. Make your changes following Clean Architecture principles
3. Ensure all tests pass
4. Follow conventional commit messages
5. Submit a pull request

## 📄 License

This project is part of academic laboratory work for backend web development learning. 