using EFCore_DBLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
// See https://aka.ms/new-console-template for more information
public class Program
{
    private static IConfigurationRoot _configuration;
    private static DbContextOptionsBuilder<AdventureWorks2019Context> _optionsBuilder;
   public static void Main ()
    {
        Console.WriteLine("Hello, World!");
        BuildConfiguration();
        Console.WriteLine($"CNSTR: {_configuration.GetConnectionString("AdventureWorks")}");
        BuildOptions();
        ListPeople();
    }

    static void BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json",optional: true, reloadOnChange:true);

        _configuration = builder.Build();   
    }

    static void BuildOptions()
    {
        _optionsBuilder = new DbContextOptionsBuilder<AdventureWorks2019Context>();
        _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("AdventureWorks"));
    }

    static void ListPeople()
    {
        using (var db = new AdventureWorks2019Context(_optionsBuilder.Options))
        {
            var people = db.People.OrderByDescending(x => x.LastName).Take(20).ToList();
            people.ForEach((person) =>
            {
                Console.WriteLine($"{person.FirstName} {person.LastName}");
            });
        }
    }
}

