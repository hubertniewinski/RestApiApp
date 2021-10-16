using Catalog.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Catalog.Repositories
{
    public class MongoDbItemsRepository : IItemsRepository
    {
        private const string dbName = "catalog";
        private const string collectionName = "items";

        private readonly IMongoCollection<Item> itemCollection;
        public MongoDbItemsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase dataBase = mongoClient.GetDatabase(dbName);
            itemCollection = dataBase.GetCollection<Item>(collectionName);
        }

        public void CreateItem(Item item)
        {
            GetItems
        }

        public void DeleteItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public Item GetItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item> GetItems()
        {
            throw new NotImplementedException();
        }

        public void UpdateItem(Item item)
        {
            throw new NotImplementedException();
        }
    }
}