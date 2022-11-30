namespace Sender.Models
{
    public class ProductModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; } =DateTime.Now;
        public DateTime LastUpdate { get; set; }=DateTime.Now;
        public bool IsDeleted { get; set; }
    }
}
