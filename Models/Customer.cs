namespace CachingWebApi.Models
{
    public class Customer
    {
        public int Id { get; set; } = new Random().Next(1, 100000);

        public string Name { get; set; }

        public int Number { get; set; }
    }
}
