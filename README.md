# EnterpriseFlow — HR & Workflow Management System

A portfolio-ready enterprise application inspired by common HR information systems and business workflow tools. It demonstrates REST APIs, relational data modeling, workflow rules, auditability, validation, and a responsive browser UI.

## Stack
- ASP.NET Core 8 Web API (C#)
- Entity Framework Core 8 + SQLite (zero separate database-server setup)
- HTML, CSS and vanilla JavaScript frontend
- Swagger/OpenAPI in Development

## Features
- Dashboard metrics for employees and leave requests
- Employee directory: list, create, edit; unique email validation
- Leave submission with date and leave-balance validation
- Manager approval/rejection; only pending requests can be reviewed
- Approved leave is deducted from the employee balance
- Audit trail for submissions, decisions and employee maintenance
- Filter leave requests by status
- API health endpoint at `/health`

## Run locally
Requires the .NET 8 SDK.

```bash
dotnet restore
dotnet run
```

Open the local URL printed by `dotnet run`. The root page is the UI. In Development, Swagger is available at `/swagger`.

The SQLite database `enterprise-flow.db` is created automatically on first startup and seeded with demo employees. To reset demo data, stop the app and delete `enterprise-flow.db`, then run it again.

## Main API endpoints
| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/employees` | List employees |
| GET | `/api/employees/{id}` | Get employee |
| POST | `/api/employees` | Create employee |
| PUT | `/api/employees/{id}` | Update employee |
| DELETE | `/api/employees/{id}` | Delete employee if there is no leave history |
| GET | `/api/leaverequests` | List requests; optional `?status=Pending` or `?employeeId=1` |
| POST | `/api/leaverequests` | Submit a leave request |
| POST | `/api/leaverequests/{id}/review` | Approve/reject a pending request |
| GET | `/api/leaverequests/summary` | Workflow summary counts |
| GET | `/api/audit?limit=100` | Latest audit events |
| GET | `/health` | Health check |

### Example request bodies
Submit leave:
```json
{
  "employeeId": 1,
  "leaveType": "Annual",
  "startDate": "2027-01-11",
  "endDate": "2027-01-12",
  "reason": "Personal commitment"
}
```
Review request:
```json
{
  "decision": "Approved",
  "reviewer": "Priya Nair",
  "comment": "Approved; team coverage confirmed."
}
```

## Important scope note
This is a demonstrator, not production HR software. It intentionally does not implement real authentication/authorization, email notifications, holiday calendars, or concurrent approval locking. Before production use, add ASP.NET Core Identity, role-based authorization, DTOs and stricter API validation, migrations, structured logging, rate limiting, CSRF protections if cookie auth is added, and integration tests. Demo users and data are fictional.

## How to present it in an interview
"I built an HR workflow system to demonstrate enterprise application development. The backend is structured around controllers, a workflow service and EF Core data access. The service validates leave dates and balances, prevents duplicate review, deducts balances only on approval, and records audit events. I exposed REST endpoints and connected them to a responsive UI. Next I would add identity and role-based authorization, automated integration tests, and SQL Server deployment."
