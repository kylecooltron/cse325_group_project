# ClimateSeer

A weather prediction game built with Blazor. Make forecasts for future dates, then earn points when the real weather backs you up.

**Authors:** Kyle Coulon

---

## What it does

You pick a city and a date (2–14 days out), enter what you think the weather will be, and submit your prediction. When the target date arrives, your prediction is scored against real weather data from the Open-Meteo API. The more your prediction differed from the official forecast *and* turned out to be right, the more points you earn. Monthly leaderboards let you compete with other users.

---

## Prerequisites

Before you can run the app you need:

.NET 10 SDK
SQL Server Express

---

## Getting started

### 1. Set up the database

The app uses SQL Server Express with a database called `climate_seer`. The default connection string in `appsettings.json` expects a local SQLEXPRESS instance:

```
Server=localhost\SQLEXPRESS;Database=climate_seer;Trusted_Connection=True;TrustServerCertificate=True;
```

If your setup is different, update `appsettings.json` before continuing.

Once SQL Server is running, create the database tables:

```bash
dotnet ef database update
```

### 2. Set up Google sign-in

The app uses Google OAuth for authentication. You need a Google Cloud project with OAuth credentials.

Go to [Google Cloud Console] to set one up and Copy your Client ID and Client Secret

Set them as environment variables before running the app.
I use a .env file.

### 3. Run the app

```bash
dotnet watch
```

The app will open in your browser automatically. Use `dotnet run` instead if you don't want hot reload.

---

## How to use the app

### Making a prediction

1. Sign in
2. Go to **New Prediction** from the nav menu
3. Click a city on the map (or pick one from the dropdown)
4. Select a target date — must be at least 2 days out, no more than 14
5. The official forecast for that date loads automatically
6. Adjust the fields to enter *your* prediction (you must change at least one field to submit)
7. A point preview shows how much you could earn — bolder predictions and further-out dates earn more
8. Click **Submit Prediction**

### Checking your results

Go to **My Predictions** to see all your submissions. Each prediction shows:
- What you predicted
- What the forecast said at the time
- The current forecast (click "Load Current Forecast")
- Points earned once the date has passed and the prediction is scored

### Leaderboard

Go to **Leaderboard** to see how you rank against other users for the current month. Rankings reset each month.

### Dashboard

The **Dashboard** shows your next upcoming prediction and your most recently scored result at a glance.

---

## Scoring

Points are based on two things:

- **Boldness** — how much your prediction differed from the official forecast (on a 0–1 scale)
- **Days out** — predictions made further in advance are worth more

Your final score for a prediction is `boldness × base points`. If the actual weather moved toward your prediction, you earn that full amount. If it moved away, you earn nothing.

---

## Adding a database migration

If you change a model and need to update the database schema:

```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

---

## External APIs used

- **Open-Meteo** — free weather forecast and historical archive API (no key required)
- **Open-Meteo Geocoding** — converts city names to lat/lon coordinates
- **OpenStreetMap / Leaflet** — map tiles and interactive city picker
