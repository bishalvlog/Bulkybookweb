using BulkyBook.DataAccess.Migrations;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bulkybookweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult CoverTypeList()
        {
            IEnumerable<CoverType> covertypes = _unitOfWork.CoverType.GetAll();
            return View(covertypes);

        }
        //get
        public IActionResult Create()
        {
            return View();
        }

        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType coverType)
        {
          

            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.Save();
                TempData["Create"] = "Create successfull";
                return RedirectToAction("CoverTypeList");
            }
            return View(coverType);
        }
        [HttpGet]

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var covertype = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id ==Id);
            if (covertype == null)
            {
                return NotFound();
            }
            return View(covertype);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Update(obj);
                _unitOfWork.Save();
                TempData["Edit"] = "update sucessful";
                return RedirectToAction("CoverTypeList");
            }
            return View(obj);

        }
        [HttpGet]
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var covertypeform = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == Id);

            if (covertypeform == null)
            {
                return NotFound();
            }
            return View(covertypeform); ;
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? Id)
        {
            var covertypeform = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == Id);

            if (covertypeform == null)
            {
                return NotFound();
            } 
            _unitOfWork.CoverType.Remove(covertypeform);
            _unitOfWork.Save();
            TempData["Delete"] = "Delete Successfull";

            return RedirectToAction("CoverTypeList");

        }

    }
}
