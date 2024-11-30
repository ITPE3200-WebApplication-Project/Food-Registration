using Food_Registration.Models;

namespace Food_Registration.DAL;

public static class DBInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        ItemDbContext context = serviceScope.ServiceProvider.GetRequiredService<ItemDbContext>();

        // First ensure the database is created and all pending migrations are applied
        context.Database.EnsureCreated();


        // Create producers
        var producers = new[]
        {
            new Producer
            {
                Name = "Fresh Farms",
                Description = "Specializing in organic fruits and vegetables",
                ImageUrl = "https://images.unsplash.com/photo-1595273670150-bd0c3c392e46?w=500&q=80&fit=crop",
                OwnerId = "admin@test.com"
            },
            new Producer
            {
                Name = "Quality Meats",
                Description = "Premium meat products from local farms",
                ImageUrl = "https://images.unsplash.com/photo-1553163147-622ab57be1c7?w=500&q=80&fit=crop",
                OwnerId = "admin@test.com"
            },
            new Producer
            {
                Name = "Daily Dairy",
                Description = "Fresh dairy products from happy cows",
                ImageUrl = "https://images.unsplash.com/photo-1528750997573-59b89d56f4f7?w=500&q=80&fit=crop",
                OwnerId = "admin@test.com"
            },
            new Producer
            {
                Name = "Artisan Bakery",
                Description = "Traditional baked goods made fresh daily",
                ImageUrl = "https://images.unsplash.com/photo-1608198093002-ad4e005484ec?w=500&q=80&fit=crop",
                OwnerId = "admin@test.com"
            },
            new Producer
            {
                Name = "Beverage Co",
                Description = "Quality beverages for every occasion",
                ImageUrl = "https://images.unsplash.com/photo-1544145945-f90425340c7e?w=500&q=80&fit=crop",
                OwnerId = "admin@test.com"
            }
        };

        if (context.Producers?.Count() == 0)
        {
            context.AddRange(producers);
            context.SaveChanges();
        }

        // Create products
        var products = new[]
        {
            // Fruits (Fresh Farms)
            new Product
            {
                Name = "Organic Apples",
                Description = "Fresh, crisp organic apples from local orchards",
                ImageUrl = "https://images.unsplash.com/photo-1569870499705-504209102861?w=500&q=80&fit=crop",
                Category = "Fruits",
                ProducerId = producers[0].ProducerId,
                Calories = 52m,
                Carbohydrates = 14m,
                Fat = 0.2m,
                Protein = 0.3m,
                NutritionScore = "A"
            },
            new Product
            {
                Name = "Fresh Bananas",
                Description = "Perfectly ripened bananas",
                ImageUrl = "https://images.unsplash.com/photo-1571771894821-ce9b6c11b08e?w=500&q=80&fit=crop",
                Category = "Fruits",
                ProducerId = producers[0].ProducerId,
                Calories = 89m,
                Carbohydrates = 23m,
                Fat = 0.3m,
                Protein = 1.1m,
                NutritionScore = "A"
            },

            // Vegetables (Fresh Farms)
            new Product
            {
                Name = "Organic Carrots",
                Description = "Sweet and crunchy organic carrots",
                ImageUrl = "https://images.unsplash.com/photo-1598170845058-32b9d6a5da37?w=500&q=80&fit=crop",
                Category = "Vegetables",
                ProducerId = producers[0].ProducerId,
                Calories = 41m,
                Carbohydrates = 10m,
                Fat = 0.2m,
                Protein = 0.9m,
                NutritionScore = "A"
            },
            new Product
            {
                Name = "Fresh Spinach",
                Description = "Nutrient-rich fresh spinach leaves",
                ImageUrl = "https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=500&q=80&fit=crop",
                Category = "Vegetables",
                ProducerId = producers[0].ProducerId,
                Calories = 23m,
                Carbohydrates = 3.6m,
                Fat = 0.4m,
                Protein = 2.9m,
                NutritionScore = "A"
            },

            // Meats (Quality Meats)
            new Product
            {
                Name = "Premium Beef Steak",
                Description = "High-quality beef steak, perfect for grilling",
                ImageUrl = "https://images.unsplash.com/photo-1603048297172-c92544798d5a?w=500&q=80&fit=crop",
                Category = "Meats",
                ProducerId = producers[1].ProducerId,
                Calories = 250m,
                Carbohydrates = 0m,
                Fat = 15m,
                Protein = 26m,
                NutritionScore = "B"
            },
            new Product
            {
                Name = "Chicken Breast",
                Description = "Lean chicken breast, perfect for healthy meals",
                ImageUrl = "https://images.unsplash.com/photo-1604503468506-a8da13d82791?w=500&q=80&fit=crop",
                Category = "Meats",
                ProducerId = producers[1].ProducerId,
                Calories = 165m,
                Carbohydrates = 0m,
                Fat = 3.6m,
                Protein = 31m,
                NutritionScore = "A"
            },

            // Dairy (Daily Dairy)
            new Product
            {
                Name = "Fresh Milk",
                Description = "Fresh whole milk from local farms",
                ImageUrl = "https://images.unsplash.com/photo-1550583724-b2692b85b150?w=500&q=80&fit=crop",
                Category = "Dairy",
                ProducerId = producers[2].ProducerId,
                Calories = 146m,
                Carbohydrates = 12m,
                Fat = 8m,
                Protein = 8m,
                NutritionScore = "B"
            },
            new Product
            {
                Name = "Turkish Yogurt",
                Description = "Creamy Greek yogurt, high in protein",
                ImageUrl = "https://images.pexels.com/photos/373882/pexels-photo-373882.jpeg",
                Category = "Dairy",
                ProducerId = producers[2].ProducerId,
                Calories = 130m,
                Carbohydrates = 9m,
                Fat = 4m,
                Protein = 15m,
                NutritionScore = "A"
            },

            // Bakery (Artisan Bakery)
            new Product
            {
                Name = "Sourdough Bread",
                Description = "Traditional sourdough bread, baked fresh daily",
                ImageUrl = "https://images.unsplash.com/photo-1586444248902-2f64eddc13df?w=500&q=80&fit=crop",
                Category = "Bakery Foods",
                ProducerId = producers[3].ProducerId,
                Calories = 266m,
                Carbohydrates = 33m,
                Fat = 10m,
                Protein = 11m,
                NutritionScore = "C"
            },
            new Product
            {
                Name = "Croissants",
                Description = "Buttery, flaky French croissants",
                ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=500&q=80&fit=crop",
                Category = "Bakery Foods",
                ProducerId = producers[3].ProducerId,
                Calories = 272m,
                Carbohydrates = 31m,
                Fat = 14m,
                Protein = 5.5m,
                NutritionScore = "D"
            },

            // Drinks (Beverage Co)
            new Product
            {
                Name = "Fresh Orange Juice",
                Description = "100% pure squeezed orange juice",
                ImageUrl = "https://images.unsplash.com/photo-1621506289937-a8e4df240d0b?w=500&q=80&fit=crop",
                Category = "Drinks",
                ProducerId = producers[4].ProducerId,
                Calories = 112m,
                Carbohydrates = 26m,
                Fat = 0m,
                Protein = 1.7m,
                NutritionScore = "B"
            },
            new Product
            {
                Name = "Sparkling Water",
                Description = "Natural sparkling mineral water",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/09/19/12/35/cup-4489199_1280.jpg",
                Category = "Drinks",
                ProducerId = producers[4].ProducerId,
                Calories = 0m,
                Carbohydrates = 0m,
                Fat = 0m,
                Protein = 0m,
                NutritionScore = "A"
            },

            // Other (Beverage Co)
            new Product
            {
                Name = "Rainbow Candy Popcorn",
                Description = "Colorful, sweet popcorn that will make your taste buds dance 🌈",
                ImageUrl = "https://img.freepik.com/free-photo/candy-strip-popcorn_23-2147748059.jpg?t=st=1732989160~exp=1732992760~hmac=ff70eedcc8c45089dd77da0be1928bb0c1005f43f3058486860673fecb9699e5&w=1480",
                Category = "Other",
                ProducerId = producers[4].ProducerId,
                Calories = 387m,
                Carbohydrates = 76m,
                Fat = 8m,
                Protein = 3m,
                NutritionScore = "D"
            },
            new Product
            {
                Name = "Chocolate Covered Pickles",
                Description = "Yes, you read that right. Don't knock it till you try it! 🥒🍫",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/01/11/11/33/cake-1971552_1280.jpg",
                Category = "Other",
                ProducerId = producers[0].ProducerId,
                Calories = 150m,
                Carbohydrates = 15m,
                Fat = 9m,
                Protein = 2m,
                NutritionScore = "D"
            },
            new Product
            {
                Name = "Birthday Cake Sushi Roll",
                Description = "Sushi, but make it dessert! Vanilla cake rolled with sprinkles and cream 🍱🎂",
                ImageUrl = "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?w=500&q=80&fit=crop",
                Category = "Other",
                ProducerId = producers[3].ProducerId,
                Calories = 420m,
                Carbohydrates = 67m,
                Fat = 18m,
                Protein = 5m,
                NutritionScore = "E"
            }
        };

        if (context.Products?.Count() == 0 && context.Producers?.Count() > 0)
        {
            context.AddRange(products);
        }
        context.SaveChanges();
    }
}
