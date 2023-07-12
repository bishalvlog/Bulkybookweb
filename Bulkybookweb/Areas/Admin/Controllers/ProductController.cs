using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Models.ViewModel;
using Microsoft.Build.ObjectModelRemoting;

namespace Bulkybookweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _housingEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _housingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            var productlist = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType").ToList();
            return View(productlist);

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
        public IActionResult Upsert(int? Id)
        {
           ProductVm  productvm = new ()
            {
                Product = new(),
                categoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()

                }),
                covertype = _unitOfWork.CoverType.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()

                }),
            };
            if (Id == null || Id == 0)
            {
             /*   ViewBag.categorylist = categorylist;
                ViewData["covertype"] = covertype;*/
                //create product
                return View(productvm);
            }
            else
            {
                productvm.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == Id);

                //retrival image  from the product object
                string Imagepath = productvm.Product.ImageUrl;
                return View(productvm);
                //update the product 
            } 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVm obj,IFormFile? productImage)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _housingEnvironment.WebRootPath;

                if(productImage != null)
                {

                    string filename = Guid.NewGuid().ToString();
                    var upload = Path.Combine(wwwRootPath, @"\Images\Product\");
                    var extension = Path.GetExtension(productImage.FileName);
                    if(obj.Product.ImageUrl != null)
                    {
                        var oldimagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        {
                            if(System.IO.File.Exists(oldimagePath))
                            {
                                System.IO.File.Delete(oldimagePath); 
                            }

                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(upload,filename+extension), FileMode.Create))
                    {
                        productImage.CopyTo(fileStreams);

                    }
                    obj.Product.ImageUrl=@"\Images\Product\"+filename+extension;
                }
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["Create"] = "Product added sucessful";
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["Edit"] = "Product uploaded successfull";
                }
                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
            return View(obj); 

        }
        [HttpGet]
        public IActionResult Delete(int? Id)
        {

            ProductVm productvm = new()
            {
                Product = new(),
                categoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()

                }),
                covertype = _unitOfWork.CoverType.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()

                }),
            };
            if (Id == null || Id == 0)
            {
                /*   ViewBag.categorylist = categorylist;
                   ViewData["covertype"] = covertype;*/
                //create product
                return View(productvm);
            }
            else
            {
                productvm.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == Id);
                return View(productvm);
                //update the product 
            }

            return View(productvm);
        }
        [HttpPost ,ActionName("Delete")]
        public IActionResult DeletePost(int? Id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == Id);
            if (obj == null)
            {
                return NotFound();
            }
            var oldimagePath = Path.Combine(_housingEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            {
                if (System.IO.File.Exists(oldimagePath))
                {
                    System.IO.File.Delete(oldimagePath);
                }
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json (new {success =true , message = "Delete successfull"});

            return RedirectToAction("Index");

        }
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var productlist =_unitOfWork.Product.GetAll();
            return Json(new {data =productlist});

        }

        #endregion

    }
}
