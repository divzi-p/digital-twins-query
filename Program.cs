// See https://aka.ms/new-console-template for more information
using System;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using System.Net.Http;
using Azure.Core.Pipeline;
using Azure.Core;
//using Azure.DigitalTwins.Core.QueryBuilder;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;

namespace Dtwins
{
    public static class DigitalTwinsQuery
    {
                private static readonly string adtInstanceUrl ="https://adt-az220-training-divp1221.api.sea.digitaltwins.azure.net";

        // The code also follows a best practice of using a single, static,
        // instance of the HttpClient
        private static readonly HttpClient httpClient = new HttpClient();
        public async static Task Main(){
                //ManagedIdentityCredential cred =new ManagedIdentityCredential("https://digitaltwins.azure.net");
                DefaultAzureCredential cred=new DefaultAzureCredential();
                DigitalTwinsClient client =new DigitalTwinsClient(new Uri(adtInstanceUrl),cred,
                 new DigitalTwinsClientOptions { Transport = new HttpClientTransport(httpClient) });

            // This code snippet demonstrates the simplest way to iterate over the digital twin results, where paging
            // happens under the covers.
    
                string query = "SELECT * FROM DIGITALTWINS";
                AsyncPageable<BasicDigitalTwin> asyncPageableResponse = client.QueryAsync<BasicDigitalTwin>(query);
              //  AsyncPageable<BasicDigitalTwin> asyncPageableResponse = client.QueryAsync<BasicDigitalTwin>(basicQuery);
                
            // Iterate over the twin instances in the pageable response.
            // The "await" keyword here is required because new pages will be fetched when necessary,
            // which involves a request to the service.
                    await foreach (BasicDigitalTwin twin in asyncPageableResponse)
                    {
                        Console.WriteLine($"Found digital twin '{twin.Id}'");
                        //Response<BasicDigitalTwin> twinResponse = await client.GetDigitalTwinAsync<BasicDigitalTwin>(twinId);
                        
                        Console.WriteLine($"Model id: {twin.Metadata.ModelId}");
                        //gets all the properties of each twin
                        foreach (string prop in twin.Contents.Keys)
                        {
                            if (twin.Contents.TryGetValue(prop, out object value))
                                Console.WriteLine($"Property '{prop}': {value}");
                            
                        }
                        //each model get the model details- From the Dtdl model, you can parse it to get properties and the unit types defined.
                        Response<DigitalTwinsModelData> md1 = await client.GetModelAsync(twin.Metadata.ModelId);
                            DigitalTwinsModelData model1 = md1.Value;
                            Console.WriteLine($"Model Output: {model1.DtdlModel}");
                            
                        //Get relationships, source and target o
                        AsyncPageable<BasicRelationship> rels = client.GetRelationshipsAsync<BasicRelationship>(twin.Id);
                        var results = new List<BasicRelationship>();
                        await foreach (BasicRelationship rel in rels)
                        {
                            results.Add(rel);
                            Console.WriteLine($"Found relationship: {rel.Id}");
                            Console.WriteLine($"Found relationship Source: {rel.SourceId}");
                            Console.WriteLine($"Found relationship Target: {rel.TargetId}");
                            //Print its properties
                            Console.WriteLine($"Relationship properties:");
                            foreach(KeyValuePair<string, object> property in rel.Properties)
                            {
                                Console.WriteLine("{0} = {1}", property.Key, property.Value);
                            }
                         }
                        
                    }
                    

        }
    }
}
