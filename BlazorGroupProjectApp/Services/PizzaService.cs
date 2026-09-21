using BlazorGroupProjectApp.Models;

namespace BlazorGroupProjectApp.Services;

public class PizzaService
{
    private static readonly List<PizzaSpecial> _specials = new()
    {
        new PizzaSpecial { Id = 1, Name = "Basic Cheese Pizza",    BasePrice =  9.99m, Description = "It's cheesy and delicious. Why wouldn't you want one?",             ImageUrl = "img/pizzas/cheese.jpg" },
        new PizzaSpecial { Id = 2, Name = "The Baconatorizor",     BasePrice = 11.99m, Description = "It has EVERY kind of bacon",                                         ImageUrl = "img/pizzas/bacon.jpg" },
        new PizzaSpecial { Id = 3, Name = "Classic pepperoni",     BasePrice = 10.50m, Description = "It's the pizza you grew up with, but Blazing hot!",                  ImageUrl = "img/pizzas/pepperoni.jpg" },
        new PizzaSpecial { Id = 4, Name = "Buffalo chicken",       BasePrice = 12.75m, Description = "Spicy chicken, hot sauce and bleu cheese, guaranteed to warm you up", ImageUrl = "img/pizzas/meaty.jpg" },
        new PizzaSpecial { Id = 5, Name = "Mushroom Lovers",       BasePrice = 11.00m, Description = "It has mushrooms. Isn't that obvious?",                              ImageUrl = "img/pizzas/mushroom.jpg" },
        new PizzaSpecial { Id = 6, Name = "Veggie Delight",        BasePrice = 11.50m, Description = "It's like salad, but on a pizza",                                    ImageUrl = "img/pizzas/salad.jpg" },
        new PizzaSpecial { Id = 7, Name = "Margherita",            BasePrice =  9.99m, Description = "Traditional Italian pizza with tomatoes and basil",                  ImageUrl = "img/pizzas/margherita.jpg" },
    };

    public async Task<List<PizzaSpecial>> GetSpecialsAsync()
    {
        await Task.Delay(500); // simulates a real async data fetch
        return _specials.OrderByDescending(s => s.BasePrice).ToList();
    }
}
