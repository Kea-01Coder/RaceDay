# RaceDay - Event Management System
## Part 1
## Project Overview

RaceDay is a full-stack web-based event management system designed specifically for the South African road running, walking, and cycling community. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, track their personal performance history, and prepare for race day using live weather and route information.

This project was built progressively across three parts, demonstrating real-world software development practices used in the sports technology industry.

---

## Table of Contents

- [System Architecture](#system-architecture)
- [Entity Relationship Diagram](#entity-relationship-diagram)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)

---

## System Architecture

RaceDay follows a three-tier architecture:

1. **Database Layer** - SQL Server database with six core entities
2. **API Layer** - RESTful API endpoints for all system functionality
3. **Client Layer** - Web-based user interface (Part 3)

### Core Entities

| Entity          | Description                             |
|-----------------|-----------------------------------------|
| **Users**       | Organisers, Participants, and Admins    |
| **Events**      | Races with details, pricing, and status |
| **Categories**  | Distance/age categories within events   |
| **Enrollments** | Registration and payment tracking       |
| **Results**     | Race results with rankings              |
| **WeatherInfo** | Race day weather forecasts              |

---

## Entity Relationship Diagram

### Relationships

| Relationship             | Cardinality | Explanation                        |
|--------------------------|-------------|------------------------------------|
| Users → Events           | 1 : M       | One Organiser creates many Events  |
| Events → WeatherInfo     | 1 : 1       | One Event has one Weather forecast |
| Events → Categories      | 1 : M       | One Event has many Categories      |
| Categories → Enrollments | 1 : M       | One Category has many Enrollments  |
| Users → Enrollments      | 1 : M       | One User has many Enrollments      |
| Enrollments → Results    | 1 : 1       | One Enrollment has one Result      |


## API Endpoints

All endpoints are prefixed with `/api/v1`. Authentication is handled via JWT tokens.

| HTTP Method | Route                     | Description                                   | Role Required      |
|-------------|---------------------------|-----------------------------------------------|--------------------|
| POST        | `/auth/register`          | Register a new user                           | None               |
| POST        | `/auth/login`             | Login and receive JWT token                   | None               |
| GET         | `/users/me`               | Get current user's profile                    | Authenticated User |
| PUT         | `/users/me`               | Update current user's profile                 | Authenticated User |
| GET         | `/users/{id}/enrollments` | Get user's enrollment history                 | User or Admin      |
| GET         | `/users/{id}/results`     | Get user's race results                       | User or Admin      |
| GET         | `/events`                 | List all events with filters                  | None               |
| GET         | `/events/{id}`            | Get event details with categories and weather | None               |
| POST        | `/events`                 | Create a new event                            | Organiser          |
| PUT         | `/events/{id}`            | Update an existing event                      | Organiser          |
| DELETE      | `/events/{id}`            | Delete an event                               | Organiser or Admin |
| POST        | `/events/{id}/categories` | Add a category to an event                    | Organiser          |
| PUT         | `/categories/{id}`        | Update a category                             | Organiser          |
| DELETE      | `/categories/{id}`        | Delete a category                             | Organiser          |
| POST        | `/events/{id}/enroll`     | Enroll in an event category                   | Participant        |   
| GET         | `/enrollments/{id}`       | Get a specific enrollment                     | User or Admin      |
| PUT         | `/enrollments/{id}`       | Update enrollment details                     | User or Admin      |
| DELETE      | `/enrollments/{id}`       | Cancel enrollment                             | User or Admin      |
| POST        | `/enrollments/{id}/pay`   | Process payment for enrollment                | Participant        |
| GET         | `/events/{id}/participants`| Get all participants in an event             | Organiser or Admin |
| POST        | `/events/{id}/results`    | Submit results (bulk upload)                  | Organiser          |
| GET         | `/events/{id}/results`    | Get all results with leaderboard              | Anyone             |
| GET         | `/results/{id}`           | Get a specific result                         | Anyone             |
| PUT         | `/results/{id}`           | Update a result                               | Organiser or Admin |
| DELETE      | `/results/{id}`           | Delete a result                               | Organiser or Admin |
| POST        | `/events/{id}/weather`    | Add/update weather forecast                   | Organiser or Admin |
| GET         | `/events/{id}/weather`    | Get weather forecast                          | Anyone             |

### Endpoints by Category

| Category       | Count  |
|----------------|--------|
| Authentication | 2      |
| User Profile   | 4      |
| Events         | 5      |
| Categories     | 3      |
| Enrollments    | 6      |
| Results        | 5      |
| Weather        | 2      |
| **Total**      | **27** |

---

## Database Schema

### Tables
 Users
 Events
 WeatherInfo    
 Category
 Enrolment
 Results
 
## Part 2
**API Layer** - RESTful API endpoints for all system functionality

Date Started : 08 October 2026
I started the API project
I first added a model folder and added all the 6 tables into classes and added each and every variable from the sql 
script table 

### Folders and Class
#### Folders
|Folder Name    | Description                                                                   |
|-----------------------------------------------------------------------------------------------|
|Controllers    | Stores all the controllers of the API                                         |
|Data           | Contains the DBContext                                                        |
|Models         | Stores all the model classes                                                  |
|RaceDay Helpers| Stores the class testhelper which avoids repetitive code to call the database |

#### Classes
|Class Name                      | Decsription                                                                                   |
|--------------------------------------------------------------------------------------------------------------------------------|
|AuthController                  | Stores code that allows for implementation of registering and logging in                      |
|CategoriesController            | Stores the code that allows the admin to add, update or delete certain categories in the event|
|EnrollmentsController           | Stores the code that stores participant data after added such as medical information          |
|EventsController                | Stores the code that allows for the admin to add or update or delete events                   |
|ResultsController               | Stores the code that releases results of certain events in the raceday competition            |
|UsersController                 | Stores the code that allows for the seperation of certain users through their ID and adding   | 
|                                | some constraints                                                                              |
|Models (User, Event, Enrollment,| They store the variables that represent the columns in your database table                    | 
|Category, Result, WeatherInfo)  |                                                                                               | 
|20261015161756_InitialCreate    | It describes how to build and undo a database                                                 |
|RaceDatDBContext                | Stores the data that is essential for how the schema is built on C#                           |
|AuthControllerTest              | Tests the login and registering of a user                                                     |
|EventsControllerTest            | Tests that organisers can manage events, verifies that participants can enrol                 |

### Process
1.Authentication: The participant logs in and receives a JWT token. This token proves who they are.
2.Browsing: They call GET /api/events to see what events are available.
3.Viewing Details: They call GET /api/events/1 to see the specific categories (e.g., "5km", "10km") for that event.
4.Enrolling: They send a POST request to /api/events/1/enrol with their category choice and personal details.
5.Database Check:
The controller checks if the event exists.
It checks if the category belongs to that event.
It checks if the event is full.
It checks if the user is already enrolled in that category (using the UC_Enrollment unique constraint we set up).

6.Creation: If all checks pass, it saves the enrollment with a status of "pending" and returns a 201 Created with the amount due.
7.Payment: They call POST /api/enrollments/{id}/pay to pay the entry fee, which changes the status to "confirmed".

How to do this in Swagger
Here is exactly what you should click:
Step 1: Log in as the Participant
        Open POST /api/auth/login.
        Enter the participant's email and password.
        Copy the token from the response.

Step 2: Authorize
        Click the Authorize padlock at the top of Swagger.
        Paste the token and click Authorize.
        
Step 3: View the Event
        Open GET /api/events/{id}.
        Enter 1 (or whatever your Event ID is).
        Click Execute.

Step 4: Enrol
        Open POST /api/events/{id}/enrol.
        Enter the Event ID (e.g., 1).    
        In the request body, paste this:
        json
        {
        "categoryId": 2,
        "participantName": "John Doe",
        "participantIdNumber": "9001015009087",
        "participantDob": "1990-01-01",
        "gender": "Male",
        "emergencyContactName": "Jane Doe",
        "emergencyContactPhone": "0821234567",
        "medicalConditions": "None"
        }
        Click Execute.
        "The API returns a 201 Created, showing my Enrollment ID and the amount due."

Step 5: Prove the role restriction (The "Aha!" moment for your video)
        Without logging out, scroll up to POST /api/events (Create Event).
        Click Try it out and Execute (with any dummy data).
        "Notice I get a 403 Forbidden. Because my token says I am a 'Participant', the [Authorize(Roles = "Organiser,Admin")] attribute blocks me from creating an event. Role-based access is working."

Step 6: Pay (Optional, but shows the flow)
        Open POST /api/enrollments/{id}/pay.
        Enter the Enrollment ID you got in Step 4.
        Body: { "paymentMethod": "card" }
        Execute.
        "The payment is processed (simulated), and my status is updated to 'confirmed'."