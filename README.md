# eCommerce-first

## ✅ What is eCommerce-first

_eCommerce-first_ is a modular, microservices-style eCommerce backend solution written in C#. It splits core eCommerce responsibilities across multiple API services: product management, orders, authentication, and more — giving you a clean, scalable foundation for building full-fledged online stores or marketplaces.

## 📦 Architecture & Folder Structure

This repo is organized into the following projects/services:

- **ApiGatewaySolution** — API Gateway / entry point for routing requests to individual services  
- **AuthenticationApiSolution** — Handles user authentication, login/signup, and user identity logic  
- **ProductApiSolution** — Manages product catalog, product data, and product-related API endpoints  
- **OrderApiSolution** — Handles order creation, order history, checkout, and order-related endpoints  
- **SharedLibrarySolution** — Contains shared models, DTOs, utilities, and common functionality used across services  

## 🛠️ Tech Stack

Backend
•	C# (.NET 8) — Core language and framework for all microservices
•	ASP.NET Core Web API — Used to build RESTful services for Products, Orders, and Authentication
•	Entity Framework Core — ORM for database access (if used in your services)
•	Clean Architecture / Modular Microservices — Separation into independent APIs and shared libraries
•	API Gateway - Ocelot — Single entry point to route and aggregate API calls across microservices

Data & Storage

•	SQL Server
•	EF Core Migrations — Database schema management

Communication & Integration
•	RESTful JSON APIs — Standard communication pattern
•	Shared Library — For DTOs, models, utilities, and cross-service logic

Security & Identity
•	JWT Authentication — Token-based secure access (if implemented)

Tools & Development
•	Visual Studio / VS Code — Development environment
•	Swagger / Swashbuckle — API documentation & testing
•	Git & GitHub — Version control and source management 

## 🚀 Getting Started — Setup & Run

1. Clone this repository:  
   ```bash
   git clone https://github.com/sukarthi06/eCommerce-first.git

2.	Navigate into each service folder (e.g. ProductApiSolution) and restore dependencies / build the solution.
3.	Configure necessary environment variables / configuration files (e.g. connection strings, authentication settings) if required.
4.	Run the API Gateway — it will route requests to the appropriate microservice.
5.	Use a REST client (Postman / curl / frontend) to interact with the services (e.g. create products, place orders, manage users).
	
⚠️ Note: You may need to ensure dependencies such as database services, configuration for each microservice, and any environment-specific setup depending on your deployment environment.

💡 Why Use eCommerce-first
•	Modular microservice design — easy to maintain and extend individual services (product, orders, auth)
•	Shared library for common models & utilities — avoids duplication and enforces consistency
•	Clear separation of concerns — eases scaling, testing, and potential independent deployment of services

