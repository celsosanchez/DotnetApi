using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace RestApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<DataController> _logger;

    public DataController(ILogger<DataController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetCounters")]
    public IEnumerable<DataModel> Get()
    {
        _logger.Log(LogLevel.Information, "Accessing Mongo");

        try{

        var client = new MongoClient("mongodb://root:example@localhost:27017/");
        System.Console.WriteLine(client.ListDatabases().First());
        }
        catch(Exception e)
        {
            System.Console.WriteLine(e);
        } 
        return Enumerable.Range(1, 5).Select(index =>
        {
            var generatedPackagesTotal = Random.Shared.Next(0, 300);
            var generatedPackagesBad = Random.Shared.Next(0, generatedPackagesTotal);
            return new DataModel
            {
                TotalPackageCounter = generatedPackagesTotal,
                BadPackageCounter = generatedPackagesBad,
                GoodPackageCounter = generatedPackagesTotal - generatedPackagesBad,
            };
        }
    )
    .ToArray();
    }
}
