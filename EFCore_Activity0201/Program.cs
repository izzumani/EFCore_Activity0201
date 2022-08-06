using EFCore_DBLibrary;
using EFCore_DBLibrary.DTOs;
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

        ListAllSalespeople();
        /**/
        GenerateSalesReportData();
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

    private static void ListAllSalespeople()
    {
        using (var db = new AdventureWorks2019Context(_optionsBuilder.Options))
        {
            var salespeople = db.SalesPeople
                                //.Include(x => x.BusinessEntity)
                                //.ThenInclude(y => y.BusinessEntity)
                                .AsNoTracking()
                                .Select(x=> new { 
                                x.BusinessEntityId,
                                x.TerritoryId,
                                x.SalesQuota,
                                x.Bonus,
                                x.SalesYtd,
                                x.BusinessEntity.BusinessEntity.FirstName,
                                x.BusinessEntity.BusinessEntity.LastName,
                                })
                                .ToList();
            foreach (var salesperson in salespeople)
            {
                ///var p = db.People.FirstOrDefault(x => x.BusinessEntityId == salesperson.BusinessEntityId);
                ///_logger.Debug(GetSalespersonDetail(salesperson, p));
                //_logger.Debug($"ID: {salesperson.BusinessEntityId}\t|TID: {salesperson.TerritoryId}\t|Quota:{salesperson.SalesQuota}\t" + $"|Bonus: {salesperson.Bonus}\t|YTDSales: {salesperson.SalesYtd}\t|Name: \t" + $"{p?.FirstName ?? ""}, {p?.LastName ?? ""}");
                //_logger.Debug($"ID: {salesperson.BusinessEntityId}\t|TID: {salesperson.TerritoryId}\t|Quota:{salesperson.SalesQuota}\t" + $"|Bonus: {salesperson.Bonus}\t|YTDSales: {salesperson.SalesYtd}\t|Name: \t" + $"{salesperson?.BusinessEntity?.BusinessEntity?.FirstName ?? ""}, {salesperson?.BusinessEntity?.BusinessEntity?.LastName ?? ""}");
                _logger.Debug($"ID: {salesperson.BusinessEntityId}\t|TID: {salesperson.TerritoryId}\t|Quota:{salesperson.SalesQuota}\t" + $"|Bonus: {salesperson.Bonus}\t|YTDSales: {salesperson.SalesYtd}\t|Name: \t" + $"{salesperson?.FirstName ?? ""}, {salesperson?.LastName ?? ""}");
            }
        }
    }


    private static void GenerateSalesReportData()
    {
        using (var db = new AdventureWorks2019Context(_optionsBuilder.Options))
        {
            var salesReportData = db.SalesPeople.Select(sp => new
            {
                beid = sp.BusinessEntityId,
                sp.BusinessEntity.BusinessEntity.FirstName,
                sp.BusinessEntity.BusinessEntity.LastName,
                sp.SalesYtd,
                Territories = sp.SalesTerritoryHistories.Select(y => y.Territory.Name), 
                OrderCount = sp.SalesOrderHeaders.Count(),
                TotalProductsSold = sp.SalesOrderHeaders
                                        .SelectMany(y => y.SalesOrderDetails)
                                        .Sum(z => z.OrderQty)
            }).Where(srdata => srdata.SalesYtd > 3000000)
                .OrderBy(srds => srds.LastName)
                .ThenBy(srds => srds.FirstName)
                .ThenByDescending(srds => srds.SalesYtd)
                .ToList();
            foreach (var srd in salesReportData)
            {
                _logger.Debug($"{srd.beid}| {srd.LastName}, {srd.FirstName}|" 
                    + $"YTD Sales: {srd.SalesYtd} |" 
                    + $"{string.Join(',', srd.Territories)}"
                    + $"Order Count: {srd.OrderCount}"
                    + $"Products Sold: {srd.TotalProductsSold}"
                    );
            }
        }
    }

    private static void GenerateSalesReportDataToDTO()
    {
        using (var db = new AdventureWorks2019Context(_optionsBuilder.Options))
        {
            var salesReportData = db.SalesPeople.Select(sp => new SalesReportListingDto
            {
                BusinessEntityId = sp.BusinessEntityId,
                FirstName = sp.BusinessEntity.BusinessEntity.FirstName,
                LastName = sp.BusinessEntity.BusinessEntity.LastName,
                SalesYtd = sp.SalesYtd,
                Territories = sp.SalesTerritoryHistories.Select(y => y.Territory.Name),
                TotalOrders = sp.SalesOrderHeaders.Count(),
                TotalProductsSold = sp.SalesOrderHeaders
                                        .SelectMany(y => y.SalesOrderDetails)
                                        .Sum(z => z.OrderQty)
            }).Where(srdata => srdata.SalesYtd > 3000000)
                .OrderBy(srds => srds.LastName)
                .ThenBy(srds => srds.FirstName)
                .ThenByDescending(srds => srds.SalesYtd);
               // .ToList();
            foreach (var srd in salesReportData)
            {
                _logger.Debug($"{srd.ToString()}");
                   
            }
        }
    }
    private static string GetSalespersonDetail(SalesPerson sp, Person p)
    {
        return $"ID: {sp.BusinessEntityId}\t|TID: {sp.TerritoryId}\t|Quota:{ sp.SalesQuota}\t" + $"|Bonus: {sp.Bonus}\t|YTDSales: {sp.SalesYtd}\t|Name: \t" +$"{p?.FirstName ?? ""}, {p?.LastName ?? ""}";
    }

}

