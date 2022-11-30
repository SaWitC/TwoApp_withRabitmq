namespace Sender.Models
{
    public class ProductModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; } 
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
