using Microsoft.EntityFrameworkCore;
using soap.Models;

namespace soap.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        context.Database.Migrate();

        if (context.Artists.Any())
            return;

        context.Artists.AddRange(
            new Artist
            {
                FirstName = "Leonardo",
                LastName = "da Vinci",
                Gender = "Male",
                DateOfBirth = new DateOnly(1452, 4, 15)
            },
            new Artist
            {
                FirstName = "Frida",
                LastName = "Kahlo",
                Gender = "Female",
                DateOfBirth = new DateOnly(1907, 7, 6)
            },
            new Artist
            {
                FirstName = "Pablo",
                LastName = "Picasso",
                Gender = "Male",
                DateOfBirth = new DateOnly(1881, 10, 25)
            },
            new Artist
            {
                FirstName = "Georgia",
                LastName = "O'Keeffe",
                Gender = "Female",
                DateOfBirth = new DateOnly(1887, 11, 15)
            },
            new Artist
            {
                FirstName = "Vincent",
                LastName = "van Gogh",
                Gender = "Male",
                DateOfBirth = new DateOnly(1853, 3, 30)
            },
            new Artist
            {
                FirstName = "Claude",
                LastName = "Monet",
                Gender = "Male",
                DateOfBirth = new DateOnly(1840, 11, 14)
            },
            new Artist
            {
                FirstName = "Artemisia",
                LastName = "Gentileschi",
                Gender = "Female",
                DateOfBirth = new DateOnly(1593, 7, 8)
            },
            new Artist
            {
                FirstName = "Rembrandt",
                LastName = "van Rijn",
                Gender = "Male",
                DateOfBirth = new DateOnly(1606, 7, 15)
            },
            new Artist
            {
                FirstName = "Mary",
                LastName = "Cassatt",
                Gender = "Female",
                DateOfBirth = new DateOnly(1844, 5, 22)
            },
            new Artist
            {
                FirstName = "Salvador",
                LastName = "Dali",
                Gender = "Male",
                DateOfBirth = new DateOnly(1904, 5, 11)
            },
            new Artist
            {
                FirstName = "Yayoi",
                LastName = "Kusama",
                Gender = "Female",
                DateOfBirth = new DateOnly(1929, 3, 22)
            },
            new Artist
            {
                FirstName = "Michelangelo",
                LastName = "Buonarroti",
                Gender = "Male",
                DateOfBirth = new DateOnly(1475, 3, 6)
            },
            new Artist
            {
                FirstName = "Tamara",
                LastName = "de Lempicka",
                Gender = "Female",
                DateOfBirth = new DateOnly(1898, 5, 16)
            },
            new Artist
            {
                FirstName = "Andy",
                LastName = "Warhol",
                Gender = "Male",
                DateOfBirth = new DateOnly(1928, 8, 6)
            },
            new Artist
            {
                FirstName = "Berthe",
                LastName = "Morisot",
                Gender = "Female",
                DateOfBirth = new DateOnly(1841, 1, 14)
            },
            new Artist
            {
                FirstName = "Edvard",
                LastName = "Munch",
                Gender = "Male",
                DateOfBirth = new DateOnly(1863, 12, 12)
            },
            new Artist
            {
                FirstName = "Hilma",
                LastName = "af Klint",
                Gender = "Female",
                DateOfBirth = new DateOnly(1862, 10, 26)
            },
            new Artist
            {
                FirstName = "Henri",
                LastName = "Matisse",
                Gender = "Male",
                DateOfBirth = new DateOnly(1869, 12, 31)
            },
            new Artist
            {
                FirstName = "Louise",
                LastName = "Bourgeois",
                Gender = "Female",
                DateOfBirth = new DateOnly(1911, 12, 25)
            },
            new Artist
            {
                FirstName = "Gustav",
                LastName = "Klimt",
                Gender = "Male",
                DateOfBirth = new DateOnly(1862, 7, 14)
            }
        );

        context.SaveChanges();
    }
}
