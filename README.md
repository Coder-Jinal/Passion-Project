# Event Management System

## Overview
The **Event Management System** is a web-based application designed to streamline event organization and attendee management. It enables users to **create, update, delete, and view events**, while attendees can **register, manage their participation, and browse event details** seamlessly. Secure authentication ensures that only authorized users can modify or delete records.

## Features
- **Event Management:** Create, edit, and remove events effortlessly.
- **Attendee Registration:** Users can register for events and manage their attendance.
- **Secure Authentication:** Login is required to modify or delete event and attendee records.
- **Dynamic Event Linking:** Attendees can link and unlink from events in real-time.
- **Optimized UI/UX:** A clean and intuitive interface for easy navigation and event management.

## Technologies Used
- **Backend:** ASP.NET Core
- **Frontend:** Razor Pages, HTML, CSS
- **Database:** SQL Server
- **Authentication:** Identity-based login system
- **Performance Optimization:** Real-time updates and data integrity checks

## Challenges & Solutions
A major challenge was implementing a **seamless attendee registration system** that ensures smooth linking and unlinking without data inconsistencies. This was resolved by dynamically filtering available events and maintaining proper relational integrity between attendees and events.

## Future Enhancements
- **Event Analytics:** Insights into attendee engagement and event performance.
- **Automated Notifications:** Email/SMS alerts for event updates and reminders.
- **Enhanced Search & Filters:** Advanced filtering options for better event discovery.

## Installation & Setup
1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/event-management-system.git
   ```
2. **Navigate to the project folder**
   ```bash
   cd event-management-system
   ```
3. **Install dependencies**
   ```bash
   dotnet restore
   ```
4. **Set up the database (SQL Server)**
   - Update the connection string in `appsettings.json`
   - Run migrations to set up the database
   ```bash
   dotnet ef database update
   ```
5. **Run the application**
   ```bash
   dotnet run
   ```
6. **Open in browser:**
   - Navigate to `http://localhost:5000`

---
**Developed by:** *Jinal Patel*
