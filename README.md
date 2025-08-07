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

- .NET 8
    - Microsoft.EntityFrameworkCore.Sqlite 8.0.4
    - Serilog.AspNetCore 8.0.1
- Angular 18.1.0

## How to Build

- Frontend

``` 
pushd code/frontend/ta-web
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