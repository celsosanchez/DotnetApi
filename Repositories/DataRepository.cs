using System.Collections.Concurrent;

namespace RestApi.Repositories;

public class DataRepository : IDataRepository
{
    private static ConcurrentDictionary <string, DatabaseData>? DatabaseCache;
    // private DataSourceContext db;
}
