using EFCore_DBLibrary;
using Inventory.Common.LoggerBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using System;
// See https://aka.ms/new-console-template for more information
public class Program
{
    private static Logger _logger;
    
    private static IConfigurationRoot _configuration;

    private static DbContextOptionsBuilder<AdventureWorks2019Context> _optionsBuilder;
   public static void Main ()
    {
        _logger = LoggerBuilderSingleton.InventoryLog;
        _logger.Debug("Start Logging with Serilog");
        int pageSize = 10;
        _logger.Debug("Hello, World!");
        BuildConfiguration();
        _logger.Debug($"CNSTR: {_configuration.GetConnectionString("AdventureWorks")}");
        BuildOptions();
        //ListPeople();
        _logger.Debug("List People Then Order and Take");
        ListPeopleThenOrderAndTake();
        _logger.Debug("Query People, order, then list and take");
        QueryPeopleOrderedToListAndTake();
        _logger.Debug("Filter People by Gonza");
        //FilteredPeople("Gonza");
        
        for (int pageNumber = 0; pageNumber < 10; pageNumber++)
        {
            Console.WriteLine($"Page {pageNumber + 1}");
            FilteredAndPagedResult("Gonza", pageNumber, pageSize);
        }

        _logger.Debug("Filter People by Mich");
        //FilteredPeople("Mich");
        
        for (int pageNumber = 0; pageNumber < 10; pageNumber++)
        {
            Console.WriteLine($"Page {pageNumber + 1}");
            FilteredAndPagedResult("Mich", pageNumber, pageSize);
        }
        _logger.Debug("Filter People by VC");
        //FilteredPeople("VC");
        for (int pageNumber = 0; pageNumber < 10; pageNumber++)
        {
            Console.WriteLine($"Page {pageNumber + 1}");
            FilteredAndPagedResult("VC", pageNumber, pageSize);
        }
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
                _logger.Debug($"{person.FirstName} {person.LastName}");
            });
        }
    }

    static void ListPeopleThenOrderAndTake()
    {
        using (var db = new AdventureWorks2019Context(_optionsBuilder.Options))
        {
            var people = db.People.ToList().OrderByDescending(x => x.LastName);
            foreach (var person in people.Take(10))
            {
                _logger.Debug($"{person.FirstName} {person.LastName}");
            }
        }
    }

    private static void QueryPeopleOrderedToListAndTake()
    {
        using (var db = new AdventureWorks2019Context(_optionsBuilder.Options))
        {
            var query = db.People.OrderByDescending(x => x.LastName);
            var result = query.Take(10);
            foreach (var person in result)
            {
                _logger.Debug($"{person.FirstName} {person.LastName}");
            }
        }
    }

    private static void FilteredPeople(string filter)
    {
        using (var db = new AdventureWorks2019Context(_optionsBuilder.Options))
        {
            var searchTerm = filter.ToLower();
            var query = db.People.Where(x => x.LastName.ToLower().Contains(searchTerm)
            || x.FirstName.ToLower().Contains(searchTerm)
            || x.PersonType.ToLower().Equals(searchTerm));
            foreach (var person in query)
            {
                _logger.Debug($"{person.FirstName} {person.LastName},{person.PersonType}");
            }
        }
    }


    private static void FilteredAndPagedResult(string filter, int pageNumber, int pageSize)
    {
        using (var db = new AdventureWorks2019Context(_optionsBuilder.Options))
        {
            var searchTerm = filter.ToLower();
            var query = db.People.Where(x => x.LastName.ToLower().Contains(searchTerm)
                || x.FirstName.ToLower().Contains(searchTerm)
                || x.PersonType.ToLower().Equals(searchTerm))
            .OrderBy(x => x.LastName)
            .Skip(pageNumber * pageSize)
            .Take(pageSize);
            foreach (var person in query)
            {
                _logger.Debug($"{person.FirstName} {person.LastName},{person.PersonType}");
            }
        }
    }



}

