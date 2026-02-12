# Unosquare CoE .NET Technical Assessment
This repository hosts the projects used to evaluate members of the Unosquare CoE .NET

## Assessment Delivery Guidance

**Priority:**
In this assessment, priority is given to completing all requested functionality.
Additional architectural improvements are appreciated but will **not** compensate for missing or incomplete core requirements.

**Recommendation:**
Focus on delivering all requested features first.
Extra design patterns, abstractions, or architectural changes are only beneficial if they do not reduce the completeness of the deliverable.

---

## Time Management Tips

- **Read all requirements first** before writing code.
- **Plan your approach**: break down the work into small, testable steps.
- **Deliver the core features early** — aim to have a working version by the halfway mark.
- **Use remaining time** for validation, error handling, and any bonus features.
- **Avoid over-engineering**: keep architecture simple unless complexity is required by the problem.
- **Test as you go** to avoid last-minute surprises.


## Versions

- .NET 10
    - Microsoft.EntityFrameworkCore.Sqlite
    - Serilog.AspNetCore
- Angular 21
- React 19

## Scaffolding Overview

### Scenarios (Tracks)

Pick one of the following scenarios during the assessment:

- Orders System
- Products System
- Support Tickets

### Frontend (Angular)

The Angular app includes placeholder routes and pages for each scenario:

- `/orders`
- `/products`
- `/support-tickets`

These pages are intentionally minimal and serve as starting points for the assessment.

### Backend (.NET)

The API includes placeholder controller endpoints for each scenario:

- `api/orders`
- `api/products`
- `api/support-tickets`

Request models are organized under `code/backend/TA-API/Models/Requests`.

## How to Build

- Frontend (Angular)

```
pushd code/frontend/angular
npm install
npm start
```
- Frontend (React)
```
pushd code/frontend/react
npm install
npm start
```
- Backend

```
pushd code/backend/
dotnet clean
dotnet build
dotnet run --project TA-API/TA-API.csproj
```

## Rick and Morty API Integration

A complete API has been built to consume the Rick and Morty API with the following features:

### Backend Features
- **CRUD Operations**: Create, Read, Update, Delete characters
- **Authentication**: API Key validation and Bearer token support
- **Data Validation**: Comprehensive validation for character data (name, status, species, etc.)
- **Rick and Morty API Integration**: Fetch characters from https://rickandmortyapi.com/api
- **Unit Tests**: 8 passing unit tests (4 for service, 4 for controller)

### Running the Backend
```bash
cd code/backend/
dotnet run --project TA-API/TA-API.csproj
```

Default credentials:
- Username: `admin` / Password: `Admin@123`
- API Key: `ta-api-secret-key-2025`

### Running Tests
```bash
cd code/backend/
dotnet test TA-API.Tests/TA-API.Tests.csproj
```

### Angular Home Page
The Angular frontend includes a home page that displays Rick and Morty characters in green cards. The page:
- Consumes the API to fetch all characters
- Displays each character in a separate green div
- Shows character details (name, status, species, gender)
- Includes loading and error states
- Responsive design for mobile and desktop
