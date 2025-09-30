# Ecommerce Project

An online electronics store built with **ASP.NET MVC (C#)** that allows users to browse products, view product details, add items to the shopping cart, and check out. The project also includes data seeding for initial products and a responsive interface.

---

## Table of Contents

* [Features](#features)
* [Tech Stack](#tech-stack)
* [Project Structure](#project-structure)
* [Setup & Installation](#setup--installation)
* [Usage](#usage)
* [Seeding Data](#seeding-data)

---

## Features

* Browse and search through products
* View product details (name, price, description, image)
* Shopping cart: add, remove, update quantities
* Checkout flow (simulated order placement)
* Admin interface to manage products
* SQL script included for seeding product data
* Responsive layout for desktop and mobile
* PayPal payment gateway Integration
* Admin DashBoard (to access : username:admin@admin.com password:admin123)

---

## Tech Stack

* **Backend:** ASP.NET MVC (C#)
* **Frontend:** Razor Views, HTML, CSS
* **Database:** SQL Server / LocalDB
* **ORM:** Entity Framework (Code First)
* **Version Control:** Git & GitHub
* **Deployment Tools**:Azure

---

## Project Structure

```
/Controllers     → Handles requests and routes
/Models          → Domain models and view models
/Views           → Razor views (UI)
/Migrations      → Entity Framework migrations
/wwwroot         → Static files (CSS, images, JS)
seed_products.sql → SQL script to seed products
appsettings.json → Configuration and DB connection
```

---

## Setup & Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/saraabraham/Ecommerce-Project.git
   cd Ecommerce-Project
   ```

2. **Configure database connection**
   Update `appsettings.json` with your SQL Server / LocalDB connection string:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EcommerceDb;Trusted_Connection=True;"
   }
   ```

3. **Apply migrations**

   ```bash
   dotnet ef database update
   ```

4. **Seed the database**
   Run the `seed_products.sql` script to insert sample products.

5. **Run the project**

   ```bash
   dotnet run
   ```

   Or launch via Visual Studio.

6. **Open in browser**
   Navigate to: `http://localhost:5000` (or specified port).

---

## Usage

* Explore product categories from the homepage.
* Click on a product to view details.
* Add items to the shopping cart.
* Proceed to checkout (demo flow).
* Admin users can add/edit/delete products.

---

## Seeding Data

* Use `seed_products.sql` included in the repository.
* Run the script against your database to populate initial product data.

---
