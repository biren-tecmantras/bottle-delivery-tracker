# Bottle Delivery Tracker

A simple React + .NET system to log daily drinking bottle deliveries from a single vendor and view monthly totals for vendor payouts. There is no login page—just a lightweight dashboard where your team can add counts and see the accumulated monthly summary.

## Architecture
- **frontend/**: React (Create React App) single-page dashboard.
- **backend/**: .NET 8 minimal API using SQLite via Entity Framework Core for persistence.

## Backend (dotnet)

### Prerequisites
- .NET 8 SDK installed.
- sqlite3 (optional, for inspecting the database file).

### Run locally
```bash
cd backend
dotnet restore
dotnet run
```
The API listens on `http://localhost:5000` by default. It will create `bottles.db` in the backend folder automatically.

### Available endpoints
| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/deliveries` | List daily entries ordered newest first. |
| `POST` | `/api/deliveries` | Add a new delivery (JSON payload: `{ "date": "YYYY-MM-DD", "count": 10 }`). |
| `GET` | `/api/monthly-summary` | Get aggregated bottle totals per month. |
| `GET` | `/health` | Simple health check (returns `{ "status": "up" }`). |

## Frontend (React)

### Prerequisites
- Node.js 18+ (including npm).

### Run locally
```bash
cd frontend
npm install
npm start
```
This opens the dashboard at `http://localhost:3000`. It hits the backend at `http://localhost:5000` by default; adjust `frontend/.env` if your API uses a different address.

## Workflow
1. Launch the backend so the SQLite database is in place.
2. Run the frontend.
3. Use the form to log the daily bottle count; the dashboard automatically refreshes the daily list and monthly summary.

No authentication is included—anyone with access to the UI can add or view entries.
