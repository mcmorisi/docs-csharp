using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace CSharpExamples.UsageExamples.DeleteMany;

public class DeleteManyAsync
{
    private static IMongoCollection<Restaurant> _restaurantsCollection;
    private const string MongoConnectionString = "<Your MongoDB URI>";

    public static async Task Main(string[] args)
    {
        Setup();

        var filter = Builders<Restaurant>.Filter
            .Eq(r => r.Borough, "Brooklyn");

        var docs = _restaurantsCollection.Find(filter).ToList();

        // Deleting documents using builders
        Console.WriteLine("Deleting documents...");
        var result = await DeleteMultipleRestaurantsBuilderAsync();

        Console.WriteLine($"Deleted documents: {result.DeletedCount}");

        Restore(docs);

        return result;
    }

    private static async Task<DeleteResult> DeleteMultipleRestaurantsBuilderAsync()
    {
        // start-delete-many-async
        var filter = Builders<Restaurant>.Filter
            .Eq(r => r.Borough, "Brooklyn");

        return await _restaurantsCollection.DeleteManyAsync(filter);
        // end-delete-many-async
    }

    private static void Restore(IEnumerable<Restaurant> docs)
    {
        _restaurantsCollection.InsertMany(docs);
        Console.WriteLine("Resetting sample data...done.");
    }

    private static void Setup()
    {
        // This allows automapping of the camelCase database fields to our models. 
        var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

        // Establish the connection to MongoDB and get the restaurants database
        var mongoClient = new MongoClient(MongoConnectionString);
        var restaurantsDatabase = mongoClient.GetDatabase("sample_restaurants");
        _restaurantsCollection = restaurantsDatabase.GetCollection<Restaurant>("restaurants");
    }
}