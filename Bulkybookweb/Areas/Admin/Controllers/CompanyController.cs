using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bulkybookweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
       
        public CompanyController(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;
        
        }
        public IActionResult Index()
        {
            var company = _unitOfWork.Company.GetAll().ToList();
            return View(company);

        }
        [HttpGet]
        public IActionResult Upsert(int? Id)
        {
            Company company = new();
               
            if (Id == null || Id == 0)
            {
                return View(company);
            }
            else
            {
                company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == Id);
                return View(company);
               
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
               if (company.Id == 0)
                  {
                   _unitOfWork.Company.Add(company);
                   TempData["success"] = "company create successfully";
                  }
                else
                 {
                   _unitOfWork.Company.update(company);
                    TempData["Edit"] = "Product updated successfull";
                  }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(company);

        }
        [HttpGet]
        public IActionResult Delete (int? id)
        {
            Company company = new();
            if(id == null || id == 0) 
            { 
                return View(company);
            
            }
            else
            {
                company = _unitOfWork.Company.GetFirstOrDefault(u=>u.Id == id); 
                return View(company);   
            }

        }
      /*  [HttpPost]
        public IActionResult Delete(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0) 
                {
                    return NotFound();

                }else
                {
                    _unitOfWork.Company.Remove();
                    TempData["delete"] = "data is delete successfuly";
                }
                return RedirectToAction("Index");   
            }
            return View(company);   
            

        }*/
        #region Api call 
        [HttpGet]
        public IActionResult GetAll()
        {
            var CompanyList = _unitOfWork.Company.GetAll();
            return Json(new { data = CompanyList });
        }
       

        #endregion

    }
}
