using System;
using System.Collections.ObjectModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace People.Models;

public class Person
{
    public ObjectId Id { get; set; }
    
    [BsonElement("first_name")]
    public string? FirstName { get; set; }
    
    [BsonElement("last_name")]
    public string? LastName { get; set; }
    
    [BsonIgnore]
    public string? FullName => $"{LastName} {FirstName}";
    
    [BsonElement("birth_date")]
    public DateTime BirthDate { get; set; }

    [BsonIgnore]
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;
            if (BirthDate > today.AddYears(-age)) age--;
            
            return age;
        }
    }
    
    [BsonElement("children")]
    public ObservableCollection<Person> Children { get; set; }
}