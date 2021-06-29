using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SqlPooling.Console
{
    public class Program
    {
        private const string CONNECTION_STRING_WITH_POOLING = "server=localhost,1533;database=PoolingTest;uid=SA;pwd=!D0ck3rsql.;pooling=true;max pool size=50; Application Name=AppTest;";
        private const string CONNECTION_STRING_WITHOUT_POOLING = "server=localhost,1533;database=PoolingTest;uid=SA;pwd=!D0ck3rsql.;pooling=false;max pool size=50; Application Name=AppTest;";
        
        private const int TEST_COUNT = 5;

        static async Task Main(string[] args)
        {
            System.Console.WriteLine("### Uygulama ####");

            var query = "select count(*) from MyTable";
            await TestingAsync("POOLING_CONNECTION",CONNECTION_STRING_WITH_POOLING, query);
            await TestingAsync("WITHOUT_POOLING_CONNECTION",CONNECTION_STRING_WITHOUT_POOLING,query );

            System.Console.WriteLine("#################");
            System.Console.ReadLine();
        }

        private static async Task TestingAsync(string title, string connectionString, string query )
        {
            var sw = new Stopwatch();
            System.Console.WriteLine($"### {DateTime.Now:dd-MM-YYYY HH:mm:ss.fff} - {title}  STARTED  ####");
            
            sw.Start();
            for (int i = 0; i < TEST_COUNT; i++)
            {
                var result = await GetDataAsync(connectionString, query);
                PrintConsole(result);
            }
            sw.Stop();
            System.Console.WriteLine($"### {DateTime.Now:dd-MM-YYYY HH:mm:ss.fff} Total Duration:{sw.Elapsed:G} {title} ENDED ####");
        }
        private static void PrintConsole(string content)
        {
            System.Console.WriteLine($"Content:{content}");
        }
        private static async Task<string> GetDataAsync(string connectionString, string query)
        {
            var sb = new StringBuilder();
            await using var sqlConnection = new SqlConnection(connectionString);
            await using var sqlCommand= new SqlCommand(query,sqlConnection);
            await sqlConnection.OpenAsync();
            await using var reader= await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                sb.AppendLine($"Count:{reader[0]}");
            }

            return sb.ToString();
        }
    }
}
