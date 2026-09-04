# RaceDay - Event Management System

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
 

