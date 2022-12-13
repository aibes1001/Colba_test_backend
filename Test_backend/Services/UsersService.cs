using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using Test_backend.Models;

namespace Test_backend.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersService(IOptions<UserStoreDatabaseSettings> userStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
            userStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                userStoreDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                userStoreDatabaseSettings.Value.UsersCollectionName);
        }

        //Crear usuario
        public async Task CreateAsync(User newUser) =>
            await _usersCollection.InsertOneAsync(newUser);

        //Para evitar crear dos usuarios con el mismo nombre
        public async Task<User?> GetUserByName(string username) =>
            await _usersCollection.Find(u => u.Username == username)
            .FirstOrDefaultAsync();
        
        //Comprovar que existe un usuario con ese mismo nombre y contraseña
        public async Task<User?> GetUserPass(UserLogin user) =>
            await _usersCollection.Find(u => u.Username == user.Username
            && u.Password == user.Password).FirstOrDefaultAsync();


    }
}
