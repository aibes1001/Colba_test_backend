namespace Test_backend.Models
{
    public class MemeStoreDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string MemesCollectionName { get; set; } = null!;
    }
}

