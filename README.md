# Ecommerce Platform - Vertical Slice

A full-stack e-commerce application built as a technical assessment. This project implements a product catalog, user authentication, a shopping cart managed with **Angular Signals**, and a secure checkout process.

---

## Technologies Used
* **Backend:** .NET 9.0 (C#)
* **Frontend:** Angular 21
* **Database:** Microsoft SQL Server (MS SQL)
* **Testing:** xUnit

---

## Installation & Setup

### 1. Database Restoration (SQL Server)
This project includes a database backup file (`EcommerceDB.bak`) located in the root of the repository.
1.  Open **SQL Server Management Studio (SSMS)**.
2.  Right-click on **Databases** and select **Restore Database...**.
3.  Select **Device**, click the ellipsis `...`, and add the `EcommerceDB.bak` file.
4.  Ensure the database name is `EcommerceDB`.
5.  Click **OK** to restore.

> **Note:** The backend is configured to use the following connection string:  
> `Server=localhost;Database=EcommerceDB;Trusted_Connection=True;TrustServerCertificate=True;`

### 2. Backend Setup (.NET 9)
1.  From the root directory, navigate to the backend project folder:
    ```bash
    cd backend/backend
    ```
2.  Restore dependencies:
    ```bash
    dotnet restore
    ```
3.  Run the application:
    ```bash
    dotnet run
    ```
    *The project is configured to start the HTTPS profile by default on port 7073.*

### 3. Frontend Setup (Angular)
**Requirements:** Node.js v24.14.0+, Angular CLI 21+.
1.  From the root directory, navigate to the frontend folder:
    ```bash
    cd frontend
    ```
2.  Install dependencies:
    ```bash
    npm install
    ```
3.  Start the development server:
    ```bash
    npm start
    ```
4.  Open your browser to `http://localhost:4200`.

---

## Running Unit Tests
The project includes a dedicated xUnit test project to validate the **Business Logic** (specifically the independent price calculation on the backend).
1.  From the root directory, navigate to the solution folder:
    ```bash
    cd backend
    ```
2.  Run the following command:
    ```bash
    dotnet test
    ```

---

## Key Architectural Features
* **Dependency Injection (DI):** Implemented a `CartService` to handle core business logic, injected into controllers to ensure modularity and testability.
* **State Management:** Utilized **Angular Signals** in the frontend to manage the shopping cart state and counter in real-time.
* **Security:** Implemented JWT-based authentication. The backend independently calculates order totals during checkout to prevent "price spoofing" from the client side.
* **Raw SQL:** Performed all database operations using raw SQL queries and `SqlDataReader` without an ORM.
* **UI/UX:** Used **Ionicons** and responsive CSS grids for a clean, professional look.  
    *(Internet connection required for icons and placeholder images).*
