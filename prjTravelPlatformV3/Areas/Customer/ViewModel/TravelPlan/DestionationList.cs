namespace prjTravelPlatform_release.Areas.Customer.ViewModel.TravelPlan
{
    public class DestionationList
    {
        public string? FDestinationId { get; set; }
        public string? FDestinationName { get; set; }
        public string? FDestinationContent { get; set; }
        public int? FtypeId { get; set; }
        public string? Ftype { get; set; }//資料庫是int
        public int? FareaId { get; set; }
        public string? FAreaName { get; set; }//資料庫是int
        public int? FPrice { get; set; }
        public int? FStock { get; set; }
        public string? FState { get; set; }
        public string? FAddress { get; set; }
        public int? FPriority { get; set; }
        public int? FCount { get; set; }
        public bool? FFollow { get; set; }
        public byte[]? FphotoPath { get; set; }
        public int FTotalFollow {  get; set; }
    }
}
