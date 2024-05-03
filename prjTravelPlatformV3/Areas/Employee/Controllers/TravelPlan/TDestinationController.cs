using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
//using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Areas.Employee.ViewModels.TravelPlan;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.TravelPlan
{
    [Area("Employee")]
	public class TDestinationController : Controller
	{
		private readonly dbTravalPlatformContext _context;
		public TDestinationController(dbTravalPlatformContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult GetPartial(string? id)
		{
			ViewBag.type = _context.Ttypes.ToList();
			ViewBag.priority = _context.Tpriorities.ToList();
			ViewBag.area = _context.TtravelAreas.ToList();
			if (id == "0" || id == null)
			{
				ViewBag.Title = "新增景點";
				ViewBag.formId = "Create";
				ViewBag.model = "AddContext";

				return PartialView("_ModalPartial", new DestionationEditView());
			}
			ViewBag.Title = "編輯景點";
			ViewBag.formId = "Edit";
			ViewBag.model = "EditContext";

			var db = _context.Tdestinations.Find(id);
			var images = _context.TdestinationPhotos
								.Where(photo => photo.FdestinationId == id)
								.ToList();
			var imageList = images.Select(photo => photo.FphotoPath).ToList();
			DestionationEditView h = new DestionationEditView()
			{
				FDestinationId = id,
				FDestinationName = db.FdestinationName,
				FDestinationContent = db.FdestinationContent,
				FDestinationTypeId = db.FdestinationTypeId,
				FAreaId = db.FareaId,
				FPrice = db.Fprice,
				FStock = db.Fstock,
				FState = db.Fstate,
				FAddress = db.Faddress,
				FPriority = db.Fpriority,
				TdestinationPhotos = imageList
			};
			return PartialView("_ModalPartial", h);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DestionationEditView userinfo)
		{
			if (ModelState.IsValid)
			{
				try
				{
					Tdestination des = new Tdestination();
					des.FdestinationId = Guid.NewGuid().ToString();
					des.FdestinationName = userinfo.FDestinationName;
					des.FdestinationContent = userinfo.FDestinationContent;
					des.FdestinationTypeId = userinfo.FDestinationTypeId;
					des.FareaId = userinfo.FAreaId;
					des.Fprice = userinfo.FPrice;
					des.Fstock = userinfo.FStock;
					des.Fstate = false;
					des.Faddress = userinfo.FAddress;
					des.Fpriority = userinfo.FPriority;
					_context.Add(des);
					ReadUploadImage(des.FdestinationId);

					await _context.SaveChangesAsync();
					return Json(new { success = true, message = "資料新增成功" });
				}
				catch (Exception e)
				{
					return Json(new { success = false, message = $"資料新增失敗：{e.Message}" });
				}
			}
			var errors = ModelState.ToDictionary(
				kvp => kvp.Key,
				kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
			);
			return Json(new
			{
				success = false,
				message = "資料驗證失敗",
				errors
			});
		}

		private IActionResult ReadUploadImage(string id)
		{
			foreach (var file in Request.Form.Files)
			{
				using (BinaryReader br = new BinaryReader(file.OpenReadStream()))
				{
					TdestinationPhoto tdphoto = new TdestinationPhoto();
					tdphoto.FdestinationId = id;
					tdphoto.FphotoPath = br.ReadBytes((int)file.Length);
					_context.Update(tdphoto);
				}
			}
			return Content("True");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DestionationEditView userinfo, List<byte[]> photos)
		{
			if (ModelState.IsValid)
			{
				try
				{
					Tdestination h = _context.Tdestinations.FirstOrDefault(x => x.FdestinationId == userinfo.FDestinationId);
					{
						h.FdestinationId = userinfo.FDestinationId;
						h.FdestinationName = userinfo.FDestinationName;
						h.FdestinationContent = userinfo.FDestinationContent;
						h.FdestinationTypeId = userinfo.FDestinationTypeId;
						h.FareaId = userinfo.FAreaId;
						h.Fprice = userinfo.FPrice;
						h.Fstock = userinfo.FStock;
						h.Faddress = userinfo.FAddress;
						h.Fpriority = userinfo.FPriority;
					};

					if (photos != null && photos.Any())
					{
						foreach (var photo in photos)
						{
							//byte[] photoData = photo;

							var photosToDelete = _context.TdestinationPhotos
								.AsEnumerable()
								.FirstOrDefault(p => ByteArrayCompare(p.FphotoPath, photo) && p.FdestinationId == userinfo.FDestinationId);
							if (photosToDelete != null)
							{
								_context.TdestinationPhotos.Remove(photosToDelete);
							}
						}
					}


					_context.Update(h);
					ReadUploadImage(h.FdestinationId);
					await _context.SaveChangesAsync();

					return Json(new { success = true, message = "資料修改成功" });
				}
				catch (Exception e)
				{
					return Json(new { success = false, message = $"資料修改失敗：{e.Message}" });
				}
			}      
			var errors = ModelState.ToDictionary(
				kvp => kvp.Key,
				kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
			);
			return Json(new
			{
				success = false,
				message = "資料驗證失敗",
				errors
			});
		}
		private bool ByteArrayCompare(byte[] a1, byte[] a2)
		{
			if (a1.Length != a2.Length)
			{
				return false;
			}

			for (int i = 0; i < a1.Length; i++)
			{
				if (a1[i] != a2[i])
				{
					return false;
				}
			}
			return true;
		}

		//景點影響方案上下架
		public IActionResult GetTravelPlan(string id)
		{
			var destionation = _context.Tdestinations.FirstOrDefault(d => d.FdestinationId == id);
			if (destionation.Fstate == true)
			{
				var Context = (from h in _context.TdestinationDetails
							   join x in _context.TtravelPlans on h.FtravelId equals x.FtravelId

							   where h.FdestinationId == id
							   where x.Fstate == true
							   select x.FtravelName).ToList();
				if (Context.Any())
				{
					string down = "以下方案會一併下架：" + string.Join(", ", Context);
					return Json(down);

				}
				return Json(0);
			}
		
			string up = "上架";
			return Json(up);
		}

		//上下架
		public IActionResult Del(string id)
		{
			var destionation = _context.Tdestinations.FirstOrDefault(d => d.FdestinationId == id);
			if (destionation.Fstate == false)
			{
				var photo = _context.TdestinationPhotos.FirstOrDefault(y => y.FdestinationId == id);
				if (photo == null) {
					return Ok(false);
				}
			}
			destionation.Fstate = !destionation.Fstate;
			_context.Update(destionation);
			if (destionation.Fstate == false)
			{
				var destinationDetails = _context.TdestinationDetails
					.Where(d => d.FdestinationId == id)
					.Select(d => d.FtravelId)
					.ToList();

				var travelPlansToUpdate = _context.TtravelPlans
					.Where(tp => destinationDetails.Contains(tp.FtravelId))
					.ToList();

				foreach (var plan in travelPlansToUpdate)
				{
					plan.Fstate = false;
					_context.TtravelPlans.Update(plan);
				}
				var allDesOnDetail = from x in _context.TdestinationDetails
									 where x.FdestinationId == id
									 select x;
				foreach (var des in allDesOnDetail)
				{
					_context.TdestinationDetails.Remove(des);
				}
			}
			_context.SaveChanges();
			return Ok(true);
		}

		//Excel上傳
		[HttpPost]
		public async Task<JsonResult> UploadUserList(IFormFile file)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			if (file != null && file.Length > 0)
			{
				try
				{
					// 使用 EPPlus 套件讀取 Excel 文件
					using (MemoryStream memoryStream = new MemoryStream())
					{
						await file.CopyToAsync(memoryStream);
						using (ExcelPackage ep = new ExcelPackage(memoryStream))
						{
							List<Tdestination> destinations = new List<Tdestination>();
							ExcelWorksheet sheet = ep.Workbook.Worksheets[0];

							int startRow = 2;

							while (sheet.Cells[startRow, 1].Value != null)
							{

								var typeid = _context.Ttypes.FirstOrDefault(x => x.Ftype == sheet.Cells[startRow, 4].Value.ToString());
								var areaid = _context.TtravelAreas.FirstOrDefault(x => x.FareaName == sheet.Cells[startRow, 5].Value.ToString());
								var priorityid = _context.Tpriorities.FirstOrDefault(x => x.FpriorityName == sheet.Cells[startRow, 9].Value.ToString());
								Tdestination destination = new Tdestination
								{
									FdestinationId = sheet.Cells[startRow, 1].Value.ToString(),
									FdestinationName = sheet.Cells[startRow, 2].Value.ToString(),
									FdestinationContent = sheet.Cells[startRow, 3].Value?.ToString(),
									FdestinationTypeId = typeid.FtypeId,
									FareaId = areaid.FareaId,
									Fprice = Convert.ToInt32(sheet.Cells[startRow, 6].Value),
									Fstock = Convert.ToInt32(sheet.Cells[startRow, 7].Value),
									Fstate = false,
									Faddress = sheet.Cells[startRow, 8].Value?.ToString(),
									Fpriority = priorityid.Fid,
									Fcount = 0,
								};

								destinations.Add(destination);

								startRow++;
							}
							_context.Tdestinations.AddRange(destinations);
							_context.SaveChangesAsync();
						}
					}

					return Json("File Uploaded Successfully!");
				}
				catch (Exception ex)
				{
					return Json("Error occurred. Error details: " + ex.Message);
				}
			}
			else
			{
				return Json("No file selected or file is empty.");
			}
		}


	}
}
