using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CryptoKeySite.Models
{
   public class MongoRepository<T>
      where T : IModelWithId
   {
      private readonly IMongoCollection<T> _collection;

      public MongoRepository()
      {
         MongoClient dbClient = new MongoClient(new MongoClientSettings
         {
            Server = new MongoServerAddress(MongoConfig.Host, MongoConfig.Port)
         });

         IMongoDatabase database = dbClient.GetDatabase(MongoConfig.Database);

         _collection = database.GetCollection<T>(typeof(T).Name);
      }

      public async Task<List<T>> Select() => await _collection.Find(new BsonDocument()).ToListAsync();

      public async Task<List<T>> SelectAscending(Expression<Func<T, object>> field) => await _collection.Find(new BsonDocument()).Sort(Builders<T>.Sort.Ascending(field)).ToListAsync();

      public async Task<List<T>> SelectDescending(Expression<Func<T, object>> field) => await _collection.Find(new BsonDocument()).Sort(Builders<T>.Sort.Descending(field)).ToListAsync();

      public async Task<T> SelectById(Guid id) => await _collection.Find(Builders<T>.Filter.Eq(b => b.Id, id)).SingleOrDefaultAsync();

      public async Task Insert(T item) { await _collection.InsertOneAsync(item); }

      public async Task DeleteById(Guid id) { await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(b => b.Id, id)); }
   }
}