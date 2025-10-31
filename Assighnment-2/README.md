# School Management API

A simple ASP.NET Core Web API for managing an online learning management system with students, teachers, courses, and enrollments.

## Features

- **Students Management**: Create, read, update, and delete student records
- **Teachers Management**: Manage teacher profiles with email uniqueness
- **Courses Management**: Handle course creation and assignment to teachers
- **Enrollments Management**: Manage student enrollments in courses with grade tracking
- Full CRUD operations on all entities
- Foreign key validation and referential integrity checks
- Duplicate prevention for enrollments
- Circular reference handling in JSON responses

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger/OpenAPI for API documentation

## Getting Started

### Prerequisites

- .NET 6.0 or higher
- SQL Server

### Installation

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:7xxx` and Swagger UI at `https://localhost:7xxx/swagger`

## API Endpoints

### Students
- `GET /api/students` - Get all students
- `GET /api/students/{id}` - Get student by ID
- `POST /api/students` - Create new student
- `PUT /api/students/{id}` - Update student
- `DELETE /api/students/{id}` - Delete student

### Teachers
- `GET /api/teachers` - Get all teachers
- `GET /api/teachers/{id}` - Get teacher by ID
- `POST /api/teachers` - Create new teacher
- `PUT /api/teachers/{id}` - Update teacher
- `DELETE /api/teachers/{id}` - Delete teacher

### Courses
- `GET /api/courses` - Get all courses
- `GET /api/courses/{id}` - Get course by ID
- `POST /api/courses` - Create new course
- `PUT /api/courses/{id}` - Update course
- `DELETE /api/courses/{id}` - Delete course

### Enrollments
- `GET /api/enrollments` - Get all enrollments
- `GET /api/enrollments/{id}` - Get enrollment by ID
- `GET /api/enrollments/student/{studentId}` - Get enrollments by student
- `GET /api/enrollments/course/{courseId}` - Get enrollments by course
- `POST /api/enrollments` - Create new enrollment
- `PUT /api/enrollments/{id}` - Update enrollment
- `DELETE /api/enrollments/{id}` - Delete enrollment

## Database Schema

### Entities

- **Students**: Id, FullName, BirthDate, IsActive
- **Teachers**: Id, FullName, Email (unique), HireDate
- **Courses**: Id, Title, Description, StartDate, TeacherId
- **Enrollments**: Id, StudentId, CourseId, EnrollDate, Grade

### Relationships

- Teacher → Courses (One-to-Many)
- Student ↔ Course via Enrollment (Many-to-Many)

## Validation Rules

- Student/Teacher names: 2-100 characters
- Course titles: 3-200 characters
- Email: Valid email format, unique constraint
- Grades: 0-100 range
- Foreign key validation on all related entities
- Prevent deletion of entities with active relationships

## Sample Data

The database is seeded with:
- 2 Teachers
- 3 Students
- 3 Courses
- 5 Enrollments

## Notes

This is a beginner practice project focusing on CRUD operations and basic Entity Framework Core concepts. It includes real-world features like validation, error handling, and referential integrity checks.
