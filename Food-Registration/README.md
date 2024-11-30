# Food-Registration

This is our final project in ITPE3200. 
We chose task 1, which was to create a web-based food registration tool, where customers can find nutritional information about food products, and food producers can create, update and delete nutritional information about their products..

The versions used in subapplication 1 are .NET 8.0 with Entity Framework Core 8.0.10

Subapplication 1 is built with the Model-View-Controller framework of ASP.NET 8.0.
To run the project, you simply have to open your preferred IDE, open the terminal and navigate to the project folder which contains the folders “Food-Registration” and “Food-Registration.Tests”. Enter “Food Registration” and type in “dotnet run” to run the application.

When opening the application, the first view that meets you is the index page. This is what customers will see when they enter the correct address in their web browser. Here, you are presented with a grid view of all the products in the database. You can browse, categorize or search for the food types of your choosing, and the nutritional score is presented without you having to click anything. If you want a more detailed view, you can click “Read More” to get a more detailed view of the nutritional information of a given product.

To test the application as a user, you can register an account with the email “admin@test.com”, choose a password and log in. On this user we have seeded some examples of producers and products into the database to show the functionality of the application.
Once logged in, you might notice that you get access to more functionality than before. You now have the opportunity to create, update and delete producers and products.
When creating or updating a producer or product, you have the opportunity to upload pictures. If you need any test pictures to upload, we have stored a few under /Food-Registration/wwwroot/images for your convenience.
