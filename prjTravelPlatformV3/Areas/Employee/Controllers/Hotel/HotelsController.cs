using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using prjTravelPlatformV3.Models;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using Microsoft.AspNetCore.Hosting;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Hotel
{
    [Area("Employee")]
    public class HotelsController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public HotelsController(dbTravalPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {
            return View();
        }



        //Get modal partial
        public async Task<IActionResult> GetPartial(int? id)
        {
            if (id == 0)
            {
                ViewBag.formId = "Create";
                ViewBag.title = "新增飯店資料";
                var fList = _context.THfacilities.ToList();         
                return PartialView("_ModalPartial", new HotelViewModel() { HotelFacilities = fList });
            }
            if (_context.THotels == null)
            {
                return NotFound();
            }
            var tHotel = await _context.THotels.FindAsync(id);
            if (tHotel == null)
            {
                return NotFound();
            }

            HotelViewModel h = new HotelViewModel
            {
                HotelId = tHotel.FHotelId,
                HotelName = tHotel.FHotelName,
                HotelEngName = tHotel.FHotelEngName,
                HotelAddress = tHotel.FHotelAddress,
                Phone = tHotel.FPhone,
                Region = tHotel.FRegion,
                TexId = tHotel.FTexId,
            };

            var hotelImgs = from hotel in _context.THotels
                            join hImage in _context.THimages on hotel.FHotelId equals hImage.FHotelId into a
                            from b in a.DefaultIfEmpty()
                            where hotel.FHotelId == id
                            select b;
        
            if (hotelImgs.FirstOrDefault() == null)
            {
                h.HotelImages = null;
            }
            else
            {
                foreach (var hotel in hotelImgs)
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Hotel", $"{hotel.FHotelImage}");
                    if (!System.IO.File.Exists(filePath))
                    {
                        hotel.FHotelImage = "noPhoto.jpg";
                    }

                }
                h.HotelImages = hotelImgs.ToList();
            }

            var excludedFacilityIds = _context.THfacilityRelations
                                      .Where(a => a.FHotelId == id)
                                      .Select(a => a.FHotelFacilityId)
                                      .ToList();

            var notExist = _context.THfacilities
                        .Where(c => !excludedFacilityIds.Contains(c.FHotelFacilityId))
                        .ToList();

            var exist = _context.THfacilities
                        .Where(c => excludedFacilityIds.Contains(c.FHotelFacilityId))
                        .ToList();

            h.HotelFacilities = notExist;
            h.ExistHotelFacilities = exist;

            ViewBag.formId = "Edit";
            ViewBag.title = "編輯飯店資料";
            return PartialView("_ModalPartial", h);
        }


        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.THotels == null)
            {
                return Problem("Entity set 'dbTravalPlatformContext.THotels'  is null.");
            }
            var tHotel = await _context.THotels.FindAsync(id);
            if (tHotel != null)
            {
                _context.THotels.Remove(tHotel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool THotelExists(int id)
        {
            return (_context.THotels?.Any(e => e.FHotelId == id)).GetValueOrDefault();
        }


        // POST: Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("FHotelId,FHotelName,FHotelEngName,FHotelAddress,FPhone,FRegion,FTexId")] THotel tHotel)
        //{
        //    if (id != tHotel.FHotelId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(tHotel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!THotelExists(tHotel.FHotelId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(tHotel);
        //}

        // POST: Hotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("HotelId,HotelName,HotelEngName,HotelAddress,Phone,Region,TexId")] HotelViewModel hotel)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        THotel tHotel = new THotel
        //        {
        //            FHotelId = hotel.HotelId,
        //            FHotelName = hotel.HotelName,
        //            FHotelEngName = hotel.HotelEngName,
        //            FHotelAddress = hotel.HotelAddress,
        //            FPhone = hotel.Phone,
        //            FRegion = hotel.Region,
        //            FTexId = hotel.TexId,
        //        };
        //        _context.Add(tHotel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return PartialView("_ModalPartial", hotel);
        //}
        //// GET: Hotels/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.THotels == null)
        //    {
        //        return NotFound();
        //    }

        //    var tHotel = await _context.THotels
        //        .FirstOrDefaultAsync(m => m.FHotelId == id);
        //    if (tHotel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(tHotel);
        //}
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.THotels == null)
        //    {
        //        return NotFound();
        //    }

        //    var tHotel = await _context.THotels.FindAsync(id);
        //    if (tHotel == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(tHotel);
        //}
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.THotels == null)
        //    {
        //        return NotFound();
        //    }

        //    var tHotel = await _context.THotels
        //        .FirstOrDefaultAsync(m => m.FHotelId == id);
        //    if (tHotel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(tHotel);
        //}


    }
}
