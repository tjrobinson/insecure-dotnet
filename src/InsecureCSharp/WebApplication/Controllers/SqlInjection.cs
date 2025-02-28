using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyAliasedSqlCommand = Microsoft.Data.SqlClient.SqlCommand;
using myStringType = string;

namespace WebApplication.Controllers;

// C#12 Feature: Primary constructor
[ApiController]
[Route("[controller]")]
public class SqlInjection : ControllerBase
{
    private readonly MyService _myService;

    public SqlInjection(MyService myService)
    {
        _myService = myService;
    }

    [HttpGet($"sql-injection/{{id}}")]
    public string SimpleSqlInjection(string id)
    {
        string conString = "I AM a connection String";
        
        // C# 12 feature: Alias Any Type
        myStringType badSql = "SELECT * FROM users WHERE userId = '" + id + "'";
        
        // High: SQL injection
        using (var cmd = new MyAliasedSqlCommand(badSql))
        {
            using (SqlConnection con = new(conString))
            {
                con.Open();
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();
                string res = "";
                while (reader.Read())
                {
                    res += reader["userName"];
                }
                return res;
            }
        }
    }

    [HttpGet($"rawstringliteral/{{id}}")]
    public string SqlInjectionWithRawStringLiteral(string id)
    {
        string conString = "I AM a connection String";
        
        // C# 11 feature: Raw string literals
        var badSqlInRawStringLiteral = $"SELECT * FROM users WHERE userId = '{id}'";
        
        // High: SQL injection
        using (var cmd = new MyAliasedSqlCommand(badSqlInRawStringLiteral))
        {
            using (SqlConnection con = new(conString))
            {
                con.Open();
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();
                string res = "";
                while (reader.Read())
                {
                    res += reader["userName"];
                }
                return res;
            }
        }
    }
    
    [HttpGet($"sql-injection-in-dependency-1/{{userId}}")]
    public string SqlInjectionInDependency1(string userId)
    {
        // myService injected via primary constructor (C# 12 feature)
        var result = _myService.GetUserData(userId); // High: SQL injection vulnerability inside GetUserData() - NOT DETECTED - presumably because it's SAST not DAST, doesn't know which instance of MyService is used
        
        return result;
    }
    
    [HttpGet($"sql-injection-in-dependency-2/{{userId}}")]
    public string SqlInjectionInDependency2(string userId)
    {
        var result = GetUserData(userId); // High: SQL injection vulnerability inside GetUserData()
        
        return result;
    }
    
    private string GetUserData(string userId)
    {
        string connectionString = "YourConnectionString";
        string query = "SELECT * FROM Users WHERE UserId = '" + userId + "'";

        using (SqlConnection connection = new(connectionString))
        {
            SqlCommand command = new(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            
            // High: SQL injection
            return $"{reader["UserName"]}, {reader["Email"]}";
        }
    }
}