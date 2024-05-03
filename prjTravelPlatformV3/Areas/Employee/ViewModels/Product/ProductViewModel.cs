using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.Product
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public bool? ProSource { get; set; }

        public int? SupplierId { get; set; }

        public int? TypeId { get; set; }

        public string? Release { get; set; }

        public bool? ProStatus { get; set; }
        public string? Description { get; set; }

        public string? ImagePath { get; set; }
        //public IFormFile? ImageFile { get; set; }
    }
}
