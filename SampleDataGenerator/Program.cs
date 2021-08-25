using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace SampleDataGenerator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            DataIngestionTool ingest = new DataIngestionTool();
            await ingest.RunBenchmark();
        }
    }
    class DataIngestionTool
    {
        public CosmosClient client;
        public Container container;
        public string databaseId;
        public string containerId;
        public string partitionKeyPath;

        public async Task RunBenchmark()
        {
            bool exit = false;

            while (exit == false)
            {
                Console.Clear();
                Console.WriteLine($"Cosmos DB data generator");
                Console.WriteLine($"-----------------------------------------------------------");
                Console.WriteLine($"[1]   Ingest data");
                Console.WriteLine($"[2]   Exit");

                ConsoleKeyInfo result = Console.ReadKey(true);

                if (result.KeyChar == '1')
                {
                    Console.Clear();
                    await InitialIngest();
                }
                else if (result.KeyChar == '2')
                {
                    exit = true;
                }
            }
        }

        public async Task InitialIngest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                       .AddJsonFile("AppSettings.json")
                       .Build();

            this.client = new CosmosClient(configuration["accountEndpoint"], configuration["accountKey"], new CosmosClientOptions { ConnectionMode = ConnectionMode.Direct, AllowBulkExecution = true });
            this.containerId = configuration["containerId"];
            this.databaseId = configuration["databaseId"];
            this.partitionKeyPath = configuration["partitionKeyPath"];

            //Verify the benchmark is setup
            await InitializeBenchmark();

            int numberOfItems = 10000;
            int totalItemsUploadedSoFar = 0;

            Console.WriteLine($"\n Initial Data Ingest \nPress any key to continue\n...");
            Console.ReadKey(true);

            while (true)
            {
                //Customers to insert
                List<SampleCustomer> customers = SampleCustomer.GenerateManyCustomers(configuration["partitionKeyPath"], numberOfItems);

                foreach (SampleCustomer customer in customers)
                {
                    ItemResponse<SampleCustomer> response = await container.CreateItemAsync<SampleCustomer>(customer);
                    totalItemsUploadedSoFar++;
                    if (totalItemsUploadedSoFar % 10 == 0)
                    {
                        Console.WriteLine($"Bulk inserted {totalItemsUploadedSoFar} items into a Cosmos container");
                    }
                }
            }
        }

        public async Task InitializeBenchmark()
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions { ConsistencyLevel = ConsistencyLevel.Eventual };
                QueryRequestOptions queryRequestOptions = new QueryRequestOptions { ConsistencyLevel = ConsistencyLevel.Eventual };

                this.container = this.client.GetContainer(this.databaseId, this.containerId);
                await this.container.ReadContainerAsync();  //ReadContainer to see if it is created
            }

            catch
            {
                // If container has not been created, create it
                Database database = await this.client.CreateDatabaseIfNotExistsAsync(this.databaseId);
                Container container = await database.CreateContainerIfNotExistsAsync(this.containerId, partitionKeyPath, 6000);
                this.container = container;

                // Ingest some data
                await InitialIngest();
            }
        }
    }
}
