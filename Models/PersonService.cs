using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace People.Models;

public class PersonService
{
    private readonly IMongoCollection<Person> _collection;
    
    public PersonService(MongoDbConfig config)
    {
        var client = new MongoClient(config.ConnectionString);
        var db = client.GetDatabase(config.DataBase);
        _collection = db.GetCollection<Person>(config.CollectionName);
    }
    
    public List<Person> GetAll() => _collection.Find(new BsonDocument()).ToList();

    public void Delete(ObjectId id) => _collection.DeleteOne(p => p.Id == id);
}