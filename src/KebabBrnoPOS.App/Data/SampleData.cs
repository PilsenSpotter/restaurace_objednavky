using System.Collections.Generic;
using KebabBrnoPOS.App.Models;

namespace KebabBrnoPOS.App.Data;

public static class SampleData
{
    public static IReadOnlyList<MenuItemModel> Menu => new List<MenuItemModel>
    {
        new("kebab-houska", "Kebab v housce", "Kebaby", 149, 30, "200 g masa, zelenina, omáčka dle výběru", new[] { "Jemná", "Střední", "Pálivá" }),
        new("kebab-tortilla", "Kebab tortilla", "Kebaby", 159, 30, "Grilované maso, tortilla, zelenina", new[] { "Jemná", "Střední", "Pálivá" }),
        new("kebab-box", "Kebab box", "Boxy", 169, 25, "Maso, hranolky/rýže, zelenina, omáčka", new[] { "Hranolky", "Rýže" }),
        new("cheese-box", "Sýr box", "Boxy", 149, 15, "Smažený sýr, hranolky, tatarka", new[] { "Tatarka", "Kečup", "BBQ" }),
        new("falafel-wrap", "Falafel wrap", "Veggie", 139, 20, "Falafel, zelenina, hummus", new[] { "Jemná", "Střední" }),
        new("salat", "Salát bowl", "Veggie", 119, 18, "Mix salát, feta, olivy", new[] { "Balsamico", "Jogurt" }),
        new("fries", "Hranolky", "Přílohy", 59, 50, "Křupavé hranolky", new[] { "Kečup", "Majonéza", "BBQ" }),
        new("onion-rings", "Cibulové kroužky", "Přílohy", 69, 25, "Smažené, křupavé", new[] { "BBQ", "Chilli mayo" }),
        new("cola", "Cola 0.5l", "Pití", 39, 60, "Chlazená", new[] { "Bez ledu", "S ledem" }),
        new("fanta", "Fanta 0.5l", "Pití", 39, 45, "Chlazená", new[] { "Bez ledu", "S ledem" }),
        new("water", "Voda 0.5l", "Pití", 25, 80, "Neperlivá / Perlivá", new[] { "Neperlivá", "Perlivá" })
    };
}
