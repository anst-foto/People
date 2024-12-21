using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace People.Models;

public sealed class MongoDbConfig
{
    [JsonPropertyName("server")]
    public string Server { get; set; }
    
    [JsonPropertyName("port")]
    public int Port { get; set; }
    
    [JsonIgnore]
    public string ConnectionString => $"mongodb://{Server}:{Port}";
    
    [JsonPropertyName("database")]
    public string DataBase { get; set; }
    
    [JsonPropertyName("collection")]
    public string CollectionName { get; set; }

    public static MongoDbConfig? Load(string path = "mongo_db_config.json")
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<MongoDbConfig>(json);
    }
}