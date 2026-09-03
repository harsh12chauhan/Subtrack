# SubTrack

SubTrack is a subscription and recurring-payment management platform built with a React + TypeScript frontend and a multi-service ASP.NET Core backend. It supports authentication, role-based access control, subscription lifecycle management, payments, notifications, and automated subscription renewals.

## Architecture

The backend is organized as multiple ASP.NET Core services plus a dedicated background worker:

| Component | Port | Responsibility |
|---|---:|---|
| **Authentication Service** | `7025` | Registration, login, JWT authentication, profiles, passwords, and user/role administration |
| **Subscription Service** | `7081` | Subscription CRUD, lifecycle/status management, categories, due-subscription detection, and renewals |
| **Payment Service** | `7070` | Payment processing, payment lookup, and transaction history |
| **Notification Service** | `7056` | User notifications, unread counts, read/unread management, and admin notification management |
| **Renewal Worker** | — | Background orchestration for recurring subscription renewals |

```text
React + TypeScript Client
          |
          +----> Authentication Service (:7025)
          +----> Subscription Service  (:7081)
          +----> Payment Service       (:7070)
          +----> Notification Service  (:7056)

Renewal Worker
      |
      +----> Authentication Service
      +----> Subscription Service
      +----> Payment Service
      +----> Notification Service
```

The frontend uses Axios clients to communicate with the backend services. The Renewal Worker coordinates the recurring-billing workflow without being a public HTTP API service itself.

## Features

### Authentication & Authorization

- JWT-based authentication
- Role-based authorization with `User`, `Admin`, and `Worker` roles
- User registration and login
- User profile retrieval and updates
- Password changes
- Admin user listing and role management
- Worker-only endpoints for internal renewal operations

### Subscription Management

- Create and update subscriptions
- View individual subscriptions
- View the authenticated user's subscriptions
- Admin access to all subscriptions
- Subscription statuses: `Active`, `Paused`, and `Cancelled`
- Monthly and yearly billing cycles
- Subscription categories
- Next billing date tracking
- Detection of subscriptions due for renewal
- Worker-driven subscription renewal
- Admin subscription deletion

### Payments

- User-initiated payment processing
- Worker-only internal payment processing
- Payment lookup by ID
- Payment history by subscription
- User transaction history
- Admin access to all payments

### Notifications

- Create notifications from the renewal workflow
- View the current user's notifications
- Unread notification count
- Mark individual notifications as read
- Mark all notifications as read
- Admin access to all notifications
- Admin notification deletion

## Automated Renewal Workflow

SubTrack includes a dedicated `BackgroundService` that automates recurring subscription billing.

```text
Renewal Worker
      |
      v
Authenticate with Auth Service
      |
      v
Find subscriptions due for renewal
      |
      v
Process payment through Payment Service
      |
      +---- Payment failed ----> Create notification
      |
      v
Payment successful
      |
      v
Renew subscription
      |
      v
Create notification
```

The worker uses `IHttpClientFactory`, obtains a JWT from the Authentication Service, checks for due subscriptions, calls the internal payment endpoint, creates notifications, and renews successfully paid subscriptions. It polls the workflow every **30 seconds** and supports cancellation through `CancellationToken`.

## API Overview

The current backend exposes **30 controller endpoints** across authentication, user management, subscriptions, payments, and notifications.

### Authentication — `/auth`

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/auth/register` | Register a new user |
| POST | `/auth/login` | Authenticate a user |

### User Management — `/user`

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/user/profile` | Get the authenticated user's profile |
| PATCH | `/user/update` | Update the authenticated user's profile |
| PATCH | `/user/changepassword` | Change the authenticated user's password |
| GET | `/user/alluser` | Admin: list users |
| PATCH | `/user/role/{userId}` | Admin: update a user's role |

### Subscriptions — `/subscription`

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/subscription/create` | Create a subscription |
| PATCH | `/subscription/update/{subscriptionId}` | Update a subscription |
| GET | `/subscription/all` | Admin: list all subscriptions |
| GET | `/subscription/{subscriptionId}` | Get a subscription by ID |
| GET | `/subscription/user-subscription` | Get the current user's subscriptions |
| PUT | `/subscription/status/{subscriptionId}/{status}` | Update subscription status |
| DELETE | `/subscription/{subscriptionId}` | Admin: delete a subscription |
| PATCH | `/subscription/renew/{subscriptionId}` | Worker: renew a subscription |
| GET | `/subscription/due` | Worker/Admin: get subscriptions due for renewal |
| GET | `/subscription/categories` | Get available subscription categories |

### Payments — `/payment`

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/payment/process` | Process a user payment |
| POST | `/payment/processinternal` | Worker: process an internal payment |
| GET | `/payment/{paymentId}` | Get a payment by ID |
| GET | `/payment/subscription/{subscriptionId}` | Get payments for a subscription |
| GET | `/payment/transactions` | Get the user's transaction history |
| GET | `/payment/all` | Admin: list all payments |

### Notifications — `/notification`

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/notification/create` | Worker: create a notification |
| GET | `/notification/my` | Get the current user's notifications |
| PATCH | `/notification/readall` | Mark all notifications as read |
| PATCH | `/notification/read/{notificationId}` | Mark a notification as read |
| GET | `/notification/unreadcount` | Get unread notification count |
| DELETE | `/notification/delete/{notificationId}` | Admin: delete a notification |
| GET | `/notification/all` | Admin: list all notifications |

## Tech Stack

**Backend**
- C# / ASP.NET Core
- Entity Framework Core
- ASP.NET Identity
- JWT authentication
- Role-based authorization
- SQL Server
- Hosted `BackgroundService`
- `IHttpClientFactory`

**Frontend**
- React
- TypeScript
- Axios
- React Hook Form

**Tooling**
- Postman
- PowerShell

## Project Structure

```text
Subtrack/
├── Client/                         # React + TypeScript frontend
│   └── src/
│       └── Services/               # Axios API clients
│
├── Server/
│   ├── Authentication/             # Authentication & user management
│   ├── Subscriptions/              # Subscription management
│   ├── Payments/                   # Payment processing
│   ├── Notifications/              # Notification management
│   ├── RenewalWorker/              # Automated renewal background worker
│   ├── Substack.slnx
│   └── run-all.ps1                 # Starts all backend components
│
└── Subtrack.postman_collection.json # API testing collection
```

## Getting Started

### Prerequisites

- .NET SDK 8.0 or later
- Node.js 18+ and npm
- SQL Server
- Postman (optional, for API testing)

### Backend

The `Server` directory contains the five backend projects:

1. Authentication
2. Subscriptions
3. Payments
4. Notifications
5. RenewalWorker

Restore and build the solution:

```bash
cd Server
dotnet restore
dotnet build
```

You can run the services individually from their project directories, or start the full backend using the provided PowerShell script:

```powershell
cd Server
.\run-all.ps1
```

Before running the Renewal Worker, configure its service endpoints and worker credentials in the configuration used by `Server/RenewalWorker`.

### Frontend

```bash
cd Client
npm install
npm run dev
```

The frontend currently contains dedicated Axios clients for Authentication, Subscription, Payment, and Notification services.

### API Testing

Import `Subtrack.postman_collection.json` into Postman to explore and test the backend APIs.

## Database

The backend uses Entity Framework Core with service-specific database contexts and migrations. SQL Server is configured for the Notification Service, and the other services maintain their own EF Core data contexts.

Make sure the relevant connection strings and application configuration are set for your local environment before starting the services.

## License

MIT
