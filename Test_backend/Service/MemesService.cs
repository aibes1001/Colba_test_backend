using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Test_backend.Models;

namespace Test_backend.Services
{
    //CRUD operations service
    public class MemesService
    {
        private readonly IMongoCollection<Meme> _memesCollection;

        public MemesService(IOptions<MemeStoreDatabaseSettings> memeStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
            memeStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                memeStoreDatabaseSettings.Value.DatabaseName);

            _memesCollection = mongoDatabase.GetCollection<Meme>(
                memeStoreDatabaseSettings.Value.MemesCollectionName);
        }


        //See that C# allow method overload (GetAsync)
        //First method return all memes that contain the collection 
        public async Task<List<Meme>> GetAsync() =>
            await _memesCollection.Find(_ => true).ToListAsync();

        //Second method use an attribute to search a meme and return de first meme 
        // that find with the id.
        public async Task<Meme?> GetIdAsync(string id) =>
            await _memesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        //Method to get a meme by its name
        public async Task<Meme?> GetNameAsync(string name) =>
            await _memesCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

        //Method to create new meme in the collection
        public async Task CreateAsync(Meme newMeme) =>
            await _memesCollection.InsertOneAsync(newMeme);

        //Update a meme registred with new information
        public async Task UpdateAsync(string id, Meme updateMeme) =>
            await _memesCollection.ReplaceOneAsync(x => x.Id == id, updateMeme);

        //Delete a meme 
        public async Task RemoveAsync(string id) =>
            await _memesCollection.DeleteOneAsync(x => x.Id == id);


    }
}