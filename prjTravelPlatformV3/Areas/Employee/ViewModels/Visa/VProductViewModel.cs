using prjTravelPlatformV3.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.SqlServer.Server;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.Visa
{
    public class VProductViewModel
    {
        public int FId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "未選擇國家")]
        [DisplayName("國家")]
        public int FCountryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "未選擇供應商")]
        [DisplayName("供應商")]
        public int FSupplierId { get; set; }

        [Required(ErrorMessage = "未填商品名稱")]
        [DisplayName("商品名稱")]
        public string FName { get; set; }

        [Required(ErrorMessage = "未選擇商品性質")]
        [DisplayName("新辦/遺失")]
        public bool FNewOrLost { get; set; }

        [Required(ErrorMessage = "未選是否需面試")]
        [DisplayName("面試需要")]
        public bool FInterviewRequirement { get; set; }

        [Required(ErrorMessage = "未選擇商品性質")]
        [DisplayName("實體/電子簽")]
        public bool FEntityOrElectronic { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "未選擇商品性質")]
        [DisplayName("辦理耗時(日)")]
        public int FProcessingTimeId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "未選擇效期")]
        [DisplayName("商品效期")]
        public int FValidityPeriodId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "未選擇可留天數")]
        [DisplayName("可留天數")]
        public int FLengthOfStayId { get; set; }

        [Required(ErrorMessage = "未填商品價格")]
        [DisplayName("商品價格")]
        public decimal FPrice { get; set; }

        [Required(ErrorMessage = "未選擇啟用狀態")]
        [DisplayName("啟用狀態")]
        public bool FEnabled { get; set; }

        
        [DisplayName("所需表單")]
        public virtual ICollection<TVproductFormsRequired> TVproductFormsRequireds { get; set; } = new List<TVproductFormsRequired>();


        private string[]? _formsrequireds { get; set; }

        public string[]? formsrequireds
        {
            get { return _formsrequireds; }
            set
            { _formsrequireds = value; 
                TVproductFormsRequireds = conv(value); }
        }

        private ICollection<TVproductFormsRequired> conv(string[]? forms)
        {
            List<TVproductFormsRequired> list = new List<TVproductFormsRequired>();
            foreach (var item in forms)
            {
                list.Add(new TVproductFormsRequired
                {
                    FProductId = Convert.ToInt32(item.Split(',')[0]),
                    FFormId = Convert.ToInt32(item.Split(',')[1])
                });
            }
            return list;
        }
    }
}
