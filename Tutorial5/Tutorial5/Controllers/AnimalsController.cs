using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial5.Models;
using Tutorial5.Models.DTOs;

namespace Tutorial5.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    [HttpGet]
    public IActionResult GetAnimals( string orderBy = "name")
    {

        
        //open connection
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        //create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * from Animal";
        
        //Execute command

        var reader = command.ExecuteReader();

        var animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");
        int descOrdinal = reader.GetOrdinal("Description");
        int areaOrdeal = reader.GetOrdinal("Area");
        int categoryOrdeal = reader.GetOrdinal("Category");
        
        
        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal),
                Description = reader.GetString(descOrdinal),
                Area = reader.GetString(areaOrdeal),
                Category = reader.GetString(categoryOrdeal)
            });
        }
        
        switch (orderBy)
        {
            case "area":
                animals = animals.OrderBy(a => a.Area).ToList();
                break;
            case "description":
                animals = animals.OrderBy(a => a.Description).ToList();
                break;
            case "category":
                animals = animals.OrderBy(a => a.Category).ToList();
                break;
            default:
                animals = animals.OrderBy(a => a.Name).ToList();
                break;
        }

        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
        //open connection
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        //create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"Insert INTO Animal VALUES (@animalName,@animalDesc,@animalCategory,@animalArea)";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@animalDesc", animal.Description);
        command.Parameters.AddWithValue("@animalCategory", animal.Category);
        command.Parameters.AddWithValue("@animalArea", animal.Area);

        command.ExecuteNonQuery();
        
        return Created("",null);
    }

    [HttpPut("{id}")]
    public IActionResult updateAnimal(int id, AddAnimal animal)
    {
        //open connection
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        //create command
        using SqlCommand command = new SqlCommand();
        command.CommandText = $"UPDATE Animals SET Name = @animalName, Descritpion = @animalDesc, Category = @animalCategory, Area = @animalArea WHERE IdAnimal = @animalId";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@animalDesc", animal.Description);
        command.Parameters.AddWithValue("@animalCategory", animal.Category);
        command.Parameters.AddWithValue("@animalArea", animal.Area);
        command.Parameters.AddWithValue("@animalId", id);
        command.ExecuteNonQuery();
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteAnimal(int id)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        //create command
        using SqlCommand command = new SqlCommand();
        command.CommandText = $"DELETE Animals WHERE Id = @animalId";
        command.Parameters.AddWithValue("@animalId", id);
        command.ExecuteNonQuery();
        
        return Ok();
    }
}