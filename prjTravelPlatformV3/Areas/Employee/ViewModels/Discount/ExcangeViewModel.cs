namespace prjTravelPlatformV3.Areas.Employee.ViewModels.Discount
{
    public class ExcangeViewModel
    {
        public int fId { get; set; }    
        public string? fProductName { get; set; }
        public int? fPoint { get; set; }

        public decimal? fMoney { get; set;}
        public int? fQty { get; set; }

        public string? fType { get; set;}

        public string? fImgPath { get; set;}

        public string? fNote { get; set; }
    }
}
