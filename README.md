# SkyNile Flight Reservation System

<div align="center">
  <img src="https://github.com/user-attachments/assets/9bda807f-3740-4708-b616-a5a05476af0f" width="200" height="200">
</div>

## Table of Contents
1. [Project Overview](#project-overview)
2. [Agile Methodology](#agile-methodology)
3. [Features](#features)
4. [Project Architecture](#project-architecture)
5. [Assumptions](#assumptions)
6. [Technologies and Tools Used](#technologies-and-tools-used)
7. [Installation Guide](#installation-guide)
8. [API Endpoints](#api-endpoints)
9. [Future Enhancements](#future-enhancements)
10. [Contributors](#contributors)
11. [Contact](#contact)

---

## Project Overview
SkyNile Flight Reservation System is a comprehensive **RESTful Web API** designed to manage airline operations for SkyNile Airlines. This system facilitates flight booking, scheduling, and management for three main roles:

- **Admin**: Manages flights, schedules crew, and oversees system operations.
- **Customer**: Searches, books, updates, or cancels flight reservations.
- **Crew Member**: Views and manages their upcoming flight schedules.

The project follows industry-standard practices for scalability, maintainability, and performance, leveraging cutting-edge technologies like .NET Core 8 and Azure.

---
## Agile Methodology

SkyNile Flight Reservation System follows agile development principles to ensure iterative progress, collaboration, and adaptability. The team uses **Trello** as a project management tool to track tasks and facilitate effective communication.

### Trello Board Structure

Our Trello board is organized into the following columns:
1. **To Do**: Tasks prioritized for the current sprint, awaiting assignment or action.
2. **In Progress**: Tasks currently under development or being actively worked on.
3. **Done**: Completed tasks that have passed all required reviews and testing.
4. **Next Spring Discussion**: A dedicated space for scheduling discussions, setting meeting agendas, and outlining tasks for the upcoming sprint.
5. **Future Work**: A repository of long-term tasks, ideas, and potential extensions for incorporating advanced technologies or new features.

 
### Key Features of Our Trello Workflow
- **Sprint Planning**: Tasks are assigned to sprints based on priority and team capacity.
- **Labels and Tags**: Used for categorizing tasks (e.g., `Feature`, `Bug`, `Enhancement`).
- **Checklists**: Provide detailed subtasks within each card for precise tracking.
- **Automation**: Leveraging Trello's automation tools to send notifications and move tasks between columns.

### Trello Board Link
[SkyNile Trello Board](https://trello.com/b/YIOREsQw/my-trello-board) 

By adhering to this methodology, the team ensures transparency, accountability, and continuous delivery of value to the SkyNile Flight Reservation System.

---

## Features

### Admin Features
- Perform **CRUD** operations on flights.
- Assign available crew members to flights using an intelligent scheduling algorithm.
- Suggest alternative dates for new flights if conflicts arise.

### Customer Features
- Dynamic flight search leveraging reflection to detect incomplete inputs.
- Advanced sorting algorithms utilizes **weight scoring equation** to rank flights by user-defined preferences (e.g., fastest, cheapest, or balanced).
- Book, update, or cancel flight reservations.
- Automated refunds for canceled flights.
- Receive email notifications for booking confirmation, flight cancellations, and **24-hour departure reminders**.
- Dynamic pricing based on flight load.

### Crew Member Features
- View upcoming scheduled flights, ensuring adequate rest periods are maintained.

---

## Project Architecture
The application follows a **3-tier architecture**:

1. **Business Model Layer**: Defines business entities and core logic.
2. **Data Access Layer (DAL)**: Manages database interactions using the Repository Pattern with Unit of Work.
3. **API Layer**: Exposes RESTful endpoints for interaction with clients.

---

## Assumptions
1. Flights at the same airport do not overlap; a 30-minute gap exists between consecutive flight departures.
2. Crew members must rest for at least 48 hours before their next scheduled flight.

---

## Technologies and Tools Used
- **Backend**: C#, .NET Core 8
- **Database**: SQL Server (deployed on Azure)
- **Authentication & Authorization**: JWT, Microsoft Identity
- **Data Management**: Entity Framework Core, LINQ
- **Caching**: IMemoryCache for efficient flight search results
- **Background Jobs**: Hangfire
- **Email Services**: MailKit with HTML templates
- **Validation**: C# Data Annotations
- **Error Handling**: Global exception handling using IExceptionHandler
- **Testing & Debugging**: Postman, Swagger
- **Version Control**: Git/GitHub

---

## Installation Guide

### Prerequisites
- Install [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- Install [SQL Server](https://www.microsoft.com/en-us/sql-server)

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/MohamedNasser525/SkyNile.git
   ```
2. Navigate to the project directory:
   ```bash
   cd SkyNile
   ```
3. Set up the database:
   - Update the connection string in `appsettings.json` for Azure SQL Server.
   - Run migrations to create the database schema:
     ```bash
     dotnet ef database update
     ```
4. Restore dependencies:
   ```bash
   dotnet restore
   ```
5. Run the application:
   ```bash
   dotnet run
   ```
6. Access Swagger for API documentation:
   - Navigate to `http://localhost:{Your Local Host Number}/swagger` in your browser.
---

## API Endpoints

### Admin Endpoints
- **Create Flight**: `POST /api/admin/flights`
- **Update Flight**: `PUT /api/admin/flights/{id}`
- **Delete Flight**: `DELETE /api/admin/flights/{id}`
- **Assign Crew Member**: `POST /api/admin/flights/{flightId}/assign-crew`

### Customer Endpoints
- **Search Flights**: `GET /api/customer/flights`
- **Book Flight**: `POST /api/customer/flights/{flightId}/book`
- **Update Booking**: `PUT /api/customer/bookings/{bookingId}`
- **Cancel Booking**: `DELETE /api/customer/bookings/{bookingId}`

### Crew Member Endpoints
- **View Scheduled Flights**: `GET /api/crew/schedule`

---

## Future Enhancements
- Integration with Friendly UI with Frontend development
---
## Contributors
- **Mohamed Nasser**
- **Mohamed Walid Mohamed**
---
## Contact
Feel free to reach out for questions, collaborations Or business Inquiries:
- **Mohamed Nasser Email**: mo.nasser525@gmail.com
- **Mohamed Walid Emails**: Moh.Walid2002@gmail.com
- **Mohamed Nasser Linkedin**: [Profile](https://www.linkedin.com/in/mohamednasser101/)
- **Mohamed Walid Linkedin**: [Profile](https://www.linkedin.com/in/mohamed-walid-317b281b9/)
