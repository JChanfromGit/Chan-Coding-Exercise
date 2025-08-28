# Chan-Coding-Exercise
A Built backend API for managing pizzas and toppings using ASP.NET Core 8 Web API

Prerequisites


Before running the application, ensure you have the following installed:

.NET 8 SDK
Visual Studio 2022 (recommended) or Visual Studio Code

Git (for cloning the repository)

Getting Started


Clone the Repository & Build the Project:

cd [your-testing-directory]

git clone https://github.com/JChanfromGit/Chan-Coding-Exercise.git

cd Chan-Coding-Exercise

dotnet build

Running the Application (ways):

Option 1: Using Visual Studio 2022 (Recommended)

• Open the API Coding Exercise.csproj file in Visual Studio 2022

• Click the "Start" button (or press F5) to run with debugging, launching the debugger and Swagger UI (via your default browser)

Option 2: Using Command Line (.NET CLI)

cd [your-testing-directory]

cd Chan-Coding-Exercise

dotnet run

A quick note for this option:
 
The API will be available at:

HTTPS: https://localhost:7113

HTTP: http://localhost:5283

Swagger UI: https://localhost:7113/swagger

Option 3: Using Visual Studio Code

• Open the project folder in Visual Studio Code

• Press Ctrl+Shift+P (or Cmd+Shift+P on Mac)

• Type "Tasks: Run Task" and select it

• Choose "build" or use the integrated terminal: 'dotnet run'


API Endpoints

Toppings

Method   | Endpoint              | Description
---------|----------------------|---------------------------
GET      | /api/Toppings        | Get all toppings
GET      | /api/Toppings/{id}   | Get topping by ID
POST     | /api/Toppings        | Create a new topping
PUT      | /api/Toppings/{id}   | Update an existing topping
DELETE   | /api/Toppings/{id}   | Delete a topping

Pizzas

Method   | Endpoint                   | Description
---------|----------------------------|----------------------------------
GET      | /api/Pizzas               | Get all pizzas with toppings
GET      | /api/Pizzas/{id}          | Get pizza by ID with toppings
POST     | /api/Pizzas               | Create a new pizza with toppings
PUT      | /api/Pizzas/{id}          | Update pizza details
PUT      | /api/Pizzas/{id}/toppings | Update pizza toppings
DELETE   | /api/Pizzas/{id}          | Delete a pizza


Pre-loaded Pizzas: All Meat Special, Hawaiian Delight, Spinach Garden

Pre-loaded Toppings: Ham, Pineapple, Longganisa, Bacon, Kesong Puti, Spinach, Chorizo de Bilbao, Mushrooms
