# SubTrack

A subscription and recurring-payment management platform built with a **microservices architecture** on ASP.NET Core, with a React + TypeScript frontend.

SubTrack lets users register, add their recurring subscriptions (Netflix, Spotify, Microsoft 365, etc.), track billing cycles and categories, and process payments against their account balance — all secured with JWT authentication and role-based access control.

---

## Architecture

SubTrack is split into three independently deployable backend services, each with its own port and responsibility:

| Service | Port | Responsibility |
|---|---|---|
| **Auth Service** | `7025` | User registration, login, JWT issuance |
| **Subscription Service** | `7081` | Subscription CRUD, lifecycle status (Active / Paused / Cancelled) |
| **Payment Service** | `7070` | Payment processing, transaction history |

```
Client (React + TypeScript)
        │
        ├──▶ Auth Service         (:7025)  — register / login
        ├──▶ Subscription Service (:7081)  — manage subscriptions
        └──▶ Payment Service      (:7070)  — process payments, view transactions
```

Each service exposes its own REST API and can be built, run, and scaled independently.

---

## Features

- **Authentication** — user registration and login with JWT bearer tokens
- **Role-based access control** — `User` and `Admin` roles, with admin-only endpoints for viewing all subscriptions/payments across users
- **Subscription lifecycle management** — create, update, pause, cancel, and delete subscriptions
- **Billing tracking** — monthly/yearly billing cycles, category tagging (Video Streaming, Music Streaming, Productivity, Cloud Storage, Design & Creativity, etc.), and next-billing-date tracking
- **Payment processing** — process payments against a subscription, deducting from the user's account balance, with full transaction history

---

## Tech Stack

**Backend**
- ASP.NET Core (C#)
- JWT authentication & role-based authorization
- Microservices architecture (Auth / Subscription / Payment)

**Frontend**
- React
- TypeScript

**Tooling**
- Postman collection included (`Subtrack.postman_collection.json`) for exercising all API endpoints

---

## Project Structure

```
Subtrack/
├── Client/     # React + TypeScript frontend
├── Server/     # ASP.NET Core microservices (Auth, Subscription, Payment)
└── Subtrack.postman_collection.json
```

---

## Getting Started

> These are general setup steps for an ASP.NET Core + React project structured this way — adjust to match your actual solution/project names.

### Prerequisites
- .NET SDK (8.0 or later recommended)
- Node.js (18+) and npm/yarn
- A database (SQL Server / PostgreSQL, depending on your `appsettings.json` configuration)

### Backend
```bash
cd Server
dotnet restore
dotnet run   # run each service (Auth, Subscription, Payment) from its own project folder
```

### Frontend
```bash
cd Client
npm install
npm run dev
```

### API Testing
Import `Subtrack.postman_collection.json` into Postman to explore and test all available endpoints (Auth, Subscription, Payment).

---

## API Overview

| Endpoint | Method | Description |
|---|---|---|
| `/auth/register` | POST | Register a new user |
| `/auth/login` | POST | Authenticate and receive a JWT |
| `/subscription/create` | POST | Create a new subscription |
| `/subscription/all` | GET | Get all subscriptions (Admin) |
| `/subscription/user-subscription` | GET | Get current user's subscriptions |
| `/subscription/update/{id}` | PATCH | Update a subscription |
| `/subscription/status/{id}/{status}` | PUT | Update subscription status (Active/Paused/Cancelled) |
| `/subscription/{id}` | DELETE | Delete a subscription |
| `/payment/process` | POST | Process a payment for a subscription |
| `/payment/{id}` | GET | Get payment by ID |
| `/payment/subscription/{id}` | GET | Get payments for a subscription |
| `/payment/transactions` | GET | Get current user's transaction history |
| `/payment/all` | GET | Get all payments (Admin) |

---

## License

MIT
