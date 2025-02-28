using System.IO.Compression;
using Microsoft.Data.SqlClient;

namespace WebApplication;

public class MyService
{
    public string GetUserData(string userId)
    {
        string connectionString = "YourConnectionString";
        string query = "SELECT * FROM Users WHERE UserId = '" + userId + "'";

        using (SqlConnection connection = new(connectionString))
        {
            SqlCommand command = new(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            return $"{reader["UserName"]}, {reader["Email"]}";
        }
    }
    
    static void ZipTraversal(string[] args)
    {
        string zipPath = "/home/snoopy/extract/evil.zip";
        Console.WriteLine("Enter Path of Zip File to extract:");
        //string zipPath = Console.ReadLine();
        Console.WriteLine("Enter Path of Destination Folder");
        string extractPath = "";

        using (ZipArchive archive = ZipFile.OpenRead(zipPath))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                // High: Arbitrary File Write via Archive Extraction (Zip Slip)
                entry.ExtractToFile(Path.Combine(extractPath, entry.FullName));
                Console.WriteLine(extractPath);
            }
        }
    } 
}