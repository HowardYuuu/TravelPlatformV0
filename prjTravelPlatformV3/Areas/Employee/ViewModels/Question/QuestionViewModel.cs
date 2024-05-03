using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatformV3.Areas.Employee.ViewModels.Question
{
    public class QuestionViewModel
    {
        public int fId { get; set; }

        [Required(ErrorMessage = "日期為必填")]
        [DisplayName("日期")]
        public DateTime fDate {  get; set; }

        [Required(ErrorMessage = "客戶編號為必填")]
        [DisplayName("客戶編號")]
        public int fCusID { get; set; }

        [Required(ErrorMessage = "員工編號為必填")]
        [DisplayName("員工編號")]
        public int fEmpID {  get; set; }

        
        [DisplayName("主旨")]
        public string? fSub { get; set; }

        [Required(ErrorMessage = "處理狀態為必填")]
        [DisplayName("處理狀態")]
        public string? fStatus { get; set;}


        [DisplayName("問題類型")]
        public int? fType { get; set;}
    }
}
