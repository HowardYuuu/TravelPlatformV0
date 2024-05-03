using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Discount;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Question;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Question
{
    [Route("/api/Questions/{action}/{id?}")]
    public class QApiController
    {
        private readonly dbTravalPlatformContext _context;

        public QApiController(dbTravalPlatformContext context)
        {
            _context = context;
        }
        //public IActionResult GetData()
        //{
        //    var question = from q in _context.QuestionViews
        //                 select new
        //                 {
        //                     fId =q.編號 ,
        //                     fDate= q.日期,
        //                     fCusID = q.客戶編號,
        //                     fEmpID = q.員工編號,
        //                     fSub = q.主旨,
        //                     fType = q.問題類型,
        //                     fStatus =q.處理狀態
        //                 };

        //    return Json(question);
        //}
    }
}
