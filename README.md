# Introduction

This is the documentation for our final project in ITPE3200. 
We chose task 1, which was to create a web-based food registration tool, where customers can find nutritional information about food products, and food producers can create, update and delete nutritional information about their products..

The versions used in subapplication 1 are .NET 8.0 with Entity Framework Core 8.0.10

Subapplication 1 is built with the Model-View-Controller framework of ASP.NET 8.0.
To run the project, you simply have to open your preferred IDE, open the terminal and navigate to the project folder which contains the folders “Food-Registration” and “Food-Registration.Tests”. Enter “Food Registration” and type in “dotnet run” to run the application. Since we are importing bootstrap via a http link, you need internet connection.

When opening the application, the first view that meets you is the index page. This is what customers will see when they enter the correct address in their web browser. For ease of terminology, we use “customer” for users which are not logged in. Here, you are presented with a grid view of all the products in the database. You can browse, categorize or search for the food types of your choosing, and the nutritional score is presented without you having to click anything. If you want a more detailed view, you can click “Read More” to get a more complete view of the nutritional information of a given product.

To test the application as a user, you can register an account with the email “admin@test.com”, choose a password and log in. On this user we have seeded some examples of producers and products into the database to show the functionality of the application.
Once logged in, you might notice that you get access to more functionality than before. You now have the opportunity to create, update and delete producers and products.
When creating or updating a producer or product, you have the opportunity to upload pictures. If you need any test pictures to upload, we have stored a few under /Food-Registration/wwwroot/images for your convenience.

The versions used in subapp 2 are .NET 8.9 with Entity Framework Core 8.0.10 and React 18.3.1 with vite 6.0.1 on Node.js v22.5.1

Subapp 2 is built with the same backend (.NET 8.0) and frontend from React.
To run the project you have to go into the Food-Registration-folder and run dotnet run. This will start the backend .NET API-server. To run the React SPA-frontend, you go into the frontend-folder and run npm run dev. Remember to install the npm packages first with npm install -D to install the development packages also. The frontend is now running on localhost on port 5174. It is calling the backend that is running on localhost port 5173. To get started, register an account with the email “admin@test.com”. This will give you access to the correct producers and products like on subapplication 1. 

We have of course used ChatGPT/Claude to some extent. We have found it especially useful during debugging or when there are smaller errors such as syntax-errors or undeclared variables. In addition, we have used it for suggestions on how to implement certain functionality. The challenge, we think, is to find the most useful ways to use these tools. More than once we experienced that it provided very complicated solutions to not that complicated problems. One solution was prompting it to think simpler.

Even though we did overlap quite a bit on the different tasks, here is a broad overview of the main task distribution: 

Student 1 has worked on DAL, error handling and logging, asynchronous database access,  authentication/authorization and the report.
Student 2 has worked on controllers, models, some parts of views and the report. 
Student 3 has worked on CRUD, models, error handling, UX, unit tests and the report.
Student 4 has worked on controllers, UX and logging.
Student 5 has worked on controllers, authentication, database-models and the entire subapplication 2.
