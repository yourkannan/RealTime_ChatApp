# 📐 Architecture Design Write-up (ASP.NET Core Chat App)

## Key Design Decisions

The application was designed as a real-time, secure, and responsive chat platform using modern web technologies within the ASP.NET Core ecosystem. The use of SignalR provides real-time bi-directional communication between clients and the server, eliminating the need for polling. ASP.NET Core MVC was chosen to cleanly separate concerns between presentation (Views), business logic (Controllers), and data (Models).

Session-based authentication ensures lightweight and fast state management without external tokens. PasswordHasher from ASP.NET Core Identity is used to securely hash and verify user credentials. Razor pages combined with Bootstrap 5 offer a responsive and engaging user interface, including animated backgrounds and real-time UI updates.

## Folder & Layering Structure

The solution follows a conventional layered structure:

* **/Controllers** → Handles HTTP requests (`AuthController`, `ChatController`)
* **/Models** → Contains `User` and `ChatMessage` classes (EF Core entities)
* **/Data** → Contains `ChatContext` (DbContext with relationships)
* **/Hubs** → Hosts SignalR Hub (`ChatHub`) for real-time communication
* **/Helpers** → Includes reusable utility classes (`PasswordHelper`)
* **/Views** → Razor-based UI pages per controller (Login, Register, Chat)
* **/wwwroot** → Static assets (CSS, images, JS)
* **/docs** → Architecture doc, system diagram, and UI screenshots

This structure enables logical separation of concerns and simplifies development, testing, and future enhancements.

## Scalability & Security Basics

### Scalability

* SignalR is designed for scaling out with Redis or Azure SignalR Service when moving to production environments.
* The SignalR Hub is stateless, which supports horizontal scaling using multiple servers or containers.
* All database operations use `async/await` for non-blocking calls, improving throughput under load.
* The app maintains a lightweight frontend and avoids unnecessary dependencies, ensuring fast rendering and better performance.

### Security

* User credentials are stored as secure hashes using ASP.NET Core Identity’s `PasswordHasher`.
* CSRF protection is enforced using anti-forgery tokens in all POST forms.
* Session-based login ensures only authenticated users can access the chat room.
* Input validation and encoding by Razor prevents XSS attacks.
* EF Core handles parameterized queries, mitigating SQL injection risks.
* The app enforces HTTPS redirection and HSTS headers in production environments to secure data in transit.
