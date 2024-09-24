using Bulky.Models;
using BulkyWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Models.ViewModels;
// It was not picking up Authorize attribute from the package so I created
// a custom variable. There is a conflict somewhere!!
using AuthAttribute = Microsoft.AspNetCore.Authorization;
using Bulky.Utility;

namespace BulkNess12.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[AuthAttribute.Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        // --> FOR ADDING IMAGE <-- //
        public CompanyController(IUnitOfWork unitOfWork)

        {
            _unitOfWork = unitOfWork;

        }

        //--- READ ---//
        public IActionResult Index()
        {
            // Adding the Category property to the Company by using the includeProperties method.
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }




        //--- UPSERT ---//
        public IActionResult Upsert(int? id)
        {

            if (id == null || id == 0)
            {
                // Create Company
                return View(new Company());
            }
            else
            {
                // Update Company.
                Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }

        }
        [HttpPost]
        // --> CREATE & UPDATE <--//
        // Add in the form file, which is related to the image being uploaded in the form.
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {
                //--> COMMENTED OUT TO CHECK THE PREVIOUS CODE FUNCTION < -- //
              
                // If its a new Company, the id will be 0, so Add new
                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj);
                }
                // Otherwise, update
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }

                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(CompanyObj);
            }
        }

        // Using datatables.net to retrieve data from an API
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            // Adding the Category property to the Company by using the includeProperties method.
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            // Retrieving the object to be deleted.
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {

                return Json(new { success = true, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}