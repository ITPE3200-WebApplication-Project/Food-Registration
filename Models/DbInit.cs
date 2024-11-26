using Food_Registration.Models;
namespace Food_Registration.Models;

public static class DBInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        ProductDbContext context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>();

        // First ensure the database is created and all pending migrations are applied
        context.Database.EnsureCreated();


        var producers = new List<Producer>
            {
                new Producer {
                    ProducerId = 1,
                    Name = "Orkla",
                    Description = "Orkla is a leading supplier of branded consumer goods and concept solutions to the grocery, out-of-home, specialized retail, pharmacy and bakery sectors.",
                    OwnerId = "orkla@test.com",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a0/Orkla_foods_norge_logo_01.JPG/320px-Orkla_foods_norge_logo_01.JPG"
                },
                new Producer {
                    ProducerId = 2,
                    Name ="Nestle",
                    Description = "Nestle is the world's largest food and beverage company. We have more than 2000 brands ranging from global icons to local favorites, and we are present in 191 countries around the world.",
                    OwnerId = "nestle@test.com",
                    ImageUrl = "https://1000logos.net/wp-content/uploads/2017/03/Nestle-Logo.jpg"
                }
            };

        if (context.Producers?.Count() == 0)
        {
            context.AddRange(producers);
        }
        context.SaveChanges();  // Save producers first

        // Only proceed with products after ensuring producers exist

        var products = new List<Product>
            {
                new Product
                {
                    Name = "Pizza",
                    Description = "Delicious Italian dish with a thin crust topped with tomato sauce, cheese, and various toppings.",
                    ImageUrl = "/images/pizza.jpg",
                    Category = "Other",
                    ProducerId = producers[0].ProducerId,
                    Calories = 266m,
                    Carbohydrates = 33m,
                    Fat = 10m,
                    Protein = 11m
                },
                new Product
                {
                    Name = "Fried Chicken Leg",
                    Description = "Crispy and succulent chicken leg that is deep-fried to perfection, often served as a popular fast food Product.",
                    ImageUrl = "/images/pizza.jpg",
                    Category = "Meats",
                    ProducerId = producers[0].ProducerId,
                    Calories = 266m,
                    Carbohydrates = 33m,
                    Fat = 10m,
                    Protein = 11m
                },
                new Product
                {
                    Name = "French Fries",
                    Description = "Crispy, golden-brown potato slices seasoned with salt and often served as a popular side dish or snack.",
                    ImageUrl = "/images/pizza.jpg",
                    Category = "Other",
                    ProducerId = producers[0].ProducerId,
                    Calories = 266m,
                    Carbohydrates = 33m,
                    Fat = 10m,
                    Protein = 11m
                },
                new Product
                {
                    Name = "Grilled Ribs",
                    Description = "Tender and flavorful ribs grilled to perfection, usually served with barbecue sauce.",
                    ImageUrl = "/images/pizza.jpg",
                    Category = "Meats",
                    ProducerId = producers[0].ProducerId,
                    Calories = 266m,
                    Carbohydrates = 33m,
                    Fat = 10m,
                    Protein = 11m
                },
                new Product
                {
                    Name = "Tacos",
                    Description = "Tortillas filled with various ingredients such as seasoned meat, vegetables, and salsa, folded into a delicious handheld meal.",
                    ImageUrl = "/images/pizza.jpg",
                    Category = "Meats",
                    ProducerId = producers[0].ProducerId,
                    Calories = 266m,
                    Carbohydrates = 33m,
                    Fat = 10m,
                    Protein = 11m
                },
                new Product
                {
                    Name = "Fish and Chips",
                    Description = "Classic British dish featuring battered and deep-fried fish served with thick-cut fried potatoes.",
                    ImageUrl = "/images/vegetables.jpg",
                    Category = "Other",
                    ProducerId = producers[0].ProducerId,
                    Calories = 266m,
                    Carbohydrates = 33m,
                    Fat = 10m,
                    Protein = 11m
                },
                new Product
                {
                    Name = "Cider",
                    Description = "Refreshing alcoholic beverage made from fermented apple juice, available in various flavors.",
                    ImageUrl = "/images/pizza.jpg",
                    Category = "Drinks",
                    ProducerId = producers[0].ProducerId,
                    Calories = 266m,
                    Carbohydrates = 33m,
                    Fat = 10m,
                    Protein = 11m
                },
                new Product
                {
                    Name = "Coke",
                    Description = "Popular carbonated soft drink known for its sweet and refreshing taste.",
                    ImageUrl = "/images/pizza.jpg",
                    Category = "Drinks",
                    ProducerId = producers[0].ProducerId,
                    Calories = 266m,
                    Carbohydrates = 33m,
                    Fat = 10m,
                    Protein = 11m
                }
            };

        if (context.Products?.Count() == 0 && context.Producers?.Count() > 0)
        {
            context.AddRange(products);
        }
        context.SaveChanges();
    }
}
