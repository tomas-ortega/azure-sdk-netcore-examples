using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace azure_sdk_practices_netcore
{
    public class AzureCosmosDBPractices 
    {
        public string EndpointUrl = "https://cosmosdbtomyaccount.documents.azure.com:443/";
        public string PrimaryKey = "ZQwYhQWtBdTFfH67SNFkliAzf9kQ7yqcdBRvtxbG9Dj1NHnDDrf1l14PjEond7Zp7eKuvQVPZaTAEDoxFAXWDA==";

        public CosmosClient cosmosClient;

        public AzureCosmosDBPractices() 
        {
            Console.WriteLine("Starting Operations...");

            Task.Run(async () => {
                await this.runSequenceAsync();
            }).Wait();
        }

        public async Task runSequenceAsync() {
            Database createdDatabase;
            Container createdContainer;

            this.cosmosClient = new CosmosClient(this.EndpointUrl, this.PrimaryKey);
            createdDatabase = await this.CreateDatabaseAsync();
            createdContainer = await this.CreateContainerAsync(createdDatabase);
            await this.createElement(createdContainer);
            await this.searchFamilyByLastName(createdContainer);
            await this.DeleteDatabaseAndCleanupAsync(createdDatabase);
        }

        public async Task DeleteDatabaseAndCleanupAsync(Database createdDatabase)
        {
              DatabaseResponse databaseResourceResponse = await createdDatabase.DeleteAsync();
            // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

            Console.WriteLine("Deleted Database: {0}\n", createdDatabase);

            //Dispose of CosmosClient
             this.cosmosClient.Dispose();
        }

        public async Task searchFamilyByLastName(Container createdContainer) 
        {
            string sqlQuery = "SELECT * FROM c WHERE c.LastName = 'Sanchez'";

             Console.WriteLine("Running query: {0}\n", sqlQuery);

             QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
             FeedIterator<Family> queryResultSetIterator = createdContainer.GetItemQueryIterator<Family>(queryDefinition);

             List<Family> families = new List<Family>();

             while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Family family in currentResultSet)
                {
                    families.Add(family);
                    Console.WriteLine("\tRead {0}\n", family);
                }
            }
        }

        public async Task createElement(Container createdContainer)
        {
            Family myFamily = new Family
            {
                Id = "Ortega.1",
                LastName = "Sanchez",
                Parents = new Parent[]
                {
                    new Parent {FirstName = "Manuel"},
                    new Parent {FirstName = "María"}
                },
                Children = new Child[]
                {
                    new Child
                    {
                        FirstName = "Tomy",
                        Gender = "male",
                        Grade = 5,
                        Pets = new Pet[]
                        {
                            new Pet { GivenName = "Canelita"}
                        }
                    }
                },
                Address = new Address {County = "Spain", State = "Málaga", City = "Málaga"},
                IsRegistered = false
            };

            try {
                // Read the item to see if it exists. ReadItemAsync will throw an exception if the item does not exist and return status code 404 (Not found).
                ItemResponse<Family> ortegaFamilyResponse = await createdContainer.ReadItemAsync<Family>(myFamily.Id, new PartitionKey(myFamily.LastName));
                Console.WriteLine("Item in database with id: {0} already exists\n", ortegaFamilyResponse.Resource.Id);
            } catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<Family> ortegaFamilyResponse = await createdContainer.CreateItemAsync<Family>(myFamily, new PartitionKey(myFamily.LastName));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", ortegaFamilyResponse.Resource.Id, ortegaFamilyResponse.RequestCharge);
            }
        }

        public async Task<Container> CreateContainerAsync(Database databaseReference) {
            Container container;
            string containerId = "FamilyContainer";

            container = await databaseReference.CreateContainerIfNotExistsAsync(containerId, "/LastName");

            Console.WriteLine("Created Container: " + containerId);

            return container;
        }

         public async Task<Database> CreateDatabaseAsync() 
         {
             try {
                Database database = null;

                string databaseId = "FamilyDatabase";                

                database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);

                Console.WriteLine("Creating Database: " + database.Id);
                
                return database;
             } catch(Exception ex)
             {
                 Console.WriteLine("Error: " + ex);
                 throw ex;
             }
         }
    }
}