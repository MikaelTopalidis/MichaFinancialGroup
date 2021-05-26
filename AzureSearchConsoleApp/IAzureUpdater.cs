using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using SharedLibrary.data;
using SharedLibrary.Data;
using System;

namespace AzureSearchConsoleApp
{
    public interface IAzureUpdater
    {
        void Run();
    }


    class AzureUpdater : IAzureUpdater
    {
        private readonly BankAppDataContext _dbContext;
        string indexName = "customers";
        private string searchUrl = "https://michafinancialgroup.search.windows.net";
        private string key = "5882709B4612BBD5CBE089490792F5B3";

        public AzureUpdater(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Run()
        {
            
            CreateIndexIfNotExists();


            var searchClient = new SearchClient(new Uri(searchUrl),
                indexName, new AzureKeyCredential(key));

            var batch = new IndexDocumentsBatch<CustomerInAzure>();
            foreach (var customer in _dbContext.Customers)
            {
                //Update or add new in Azure
                var customerInAzure = new CustomerInAzure
                {
                    Id = customer.CustomerId.ToString(),
                    City = customer.City,
                    Name = customer.Givenname +" "+customer.Surname,
                    PersonalNumber = customer.NationalId,
                    Address = customer.Streetaddress + ", " + customer.Zipcode,
                };
                batch.Actions.Add(new IndexDocumentsAction<CustomerInAzure>(IndexActionType.MergeOrUpload,
                    customerInAzure));

            }
            IndexDocumentsResult result = searchClient.IndexDocuments(batch);

        }

        private void CreateIndexIfNotExists()
        {
            var serviceEndpoint = new Uri(searchUrl);
            var credential = new AzureKeyCredential(key);
            var adminClient = new SearchIndexClient(serviceEndpoint, credential);

            var fieldBuilder = new FieldBuilder();
            var searchFields = fieldBuilder.Build(typeof(CustomerInAzure));

            var definition = new SearchIndex(indexName, searchFields);

            adminClient.CreateOrUpdateIndex(definition);

        }
    }
}