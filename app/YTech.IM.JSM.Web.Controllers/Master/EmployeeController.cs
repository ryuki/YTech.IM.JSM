using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using SharpArch.Core;
using SharpArch.Web.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;
using YTech.IM.JSM.Data.Repository;
using YTech.IM.JSM.Enums;

namespace YTech.IM.JSM.Web.Controllers.Master
{
    [HandleError]
    public class EmployeeController : Controller
    {
        public EmployeeController() : this(new MEmployeeRepository(), new RefAddressRepository(), new RefPersonRepository(),new MDepartmentRepository())
        {}
        private readonly IMEmployeeRepository _mEmployeeRepository;
        private readonly IRefAddressRepository _refAddressRepository;
        private readonly IRefPersonRepository _refPersonRepository;
        private readonly IMDepartmentRepository _mDepartmentRepository;
        public EmployeeController(IMEmployeeRepository mEmployeeRepository, IRefAddressRepository refAddressRepository, IRefPersonRepository refPersonRepository,IMDepartmentRepository mDepartmentRepository)
        {
            Check.Require(mEmployeeRepository != null, "mEmployeeRepository may not be null");
            Check.Require(refAddressRepository != null, "refAddressRepository may not be null");
            Check.Require(refPersonRepository != null, "refPersonRepository may not be null");
            Check.Require(mDepartmentRepository != null, "mDepartmentRepository may not be null");

            this._mEmployeeRepository = mEmployeeRepository;
            this._refAddressRepository = refAddressRepository;
            this._refPersonRepository = refPersonRepository;
            this._mDepartmentRepository = mDepartmentRepository;
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }

        [Transaction]
        public virtual ActionResult List(string sidx, string sord, int page, int rows)
        {
            int totalRecords = 0;
            var employees = _mEmployeeRepository.GetPagedEmployeeList(sidx, sord, page, rows, ref totalRecords);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from employee in employees
                    select new
                    {
                        i = employee.Id.ToString(),
                        cell = new string[] {
                            employee.Id, 
                            employee.Id, 
                        employee.PersonId != null ?    employee.PersonId.PersonFirstName : null, 
                          employee.PersonId != null ?    employee.PersonId.PersonLastName : null, 
                         employee.EmployeeStatus, 
                          employee.PersonId != null ?    employee.PersonId.PersonGender : null, 
                        employee.DepartmentId != null?  employee.DepartmentId.Id:null, 
                        employee.DepartmentId != null?  employee.DepartmentId.DepartmentName:null,
                        employee.EmployeeCommissionProductType,
                        employee.EmployeeCommissionProductVal.HasValue ?  employee.EmployeeCommissionProductVal.Value.ToString(Helper.CommonHelper.NumberFormat):null,  
                        employee.EmployeeCommissionServiceType,
                        employee.EmployeeCommissionServiceVal.HasValue ?  employee.EmployeeCommissionServiceVal.Value.ToString(Helper.CommonHelper.NumberFormat):null, 
                          //itemCat.PersonId != null ?    itemCat.PersonId.PersonDob.Value.ToString(Helper.CommonHelper.DateFormat) : null, 
                          //itemCat.PersonId != null ?    itemCat.PersonId.PersonPob : null, 
                          //itemCat.PersonId != null ?    itemCat.PersonId.PersonPhone : null, 
                          //itemCat.PersonId != null ?    itemCat.PersonId.PersonMobile : null, 
                          //itemCat.PersonId != null ?    itemCat.PersonId.PersonEmail : null, 
                          //itemCat.PersonId != null ?    itemCat.PersonId.PersonReligion : null, 
                          //itemCat.PersonId != null ?    itemCat.PersonId.PersonRace : null, 
                          //itemCat.PersonId != null ?    itemCat.PersonId.PersonIdCardType : null, 
                          //itemCat.PersonId != null ?    itemCat.PersonId.PersonIdCardNo : null, 
                         employee.EmployeeDesc
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public virtual ActionResult ListSearch(string sidx, string sord, int page, int rows)
        {
            int totalRecords = 0;
            var employees = _mEmployeeRepository.GetPagedEmployeeList(sidx, sord, page, rows, ref totalRecords);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from employee in employees
                    select new
                    {
                        i = employee.Id.ToString(),
                        cell = new string[] {
                            employee.Id, 
                        employee.PersonId != null ?    employee.PersonId.PersonName : null, 
                        employee.PersonId != null ?    employee.PersonId.PersonGender : null,  
                        employee.DepartmentId != null?  employee.DepartmentId.DepartmentName:null,  
                         employee.EmployeeDesc
                        }
                    }).ToArray()
            }; 
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public ActionResult Insert(MEmployee viewModel, FormCollection formCollection)
        {
            RefPerson person = new RefPerson();
            TransferFormValuesTo(person, formCollection);
            person.SetAssignedIdTo(Guid.NewGuid().ToString());
            person.CreatedDate = DateTime.Now;
            person.CreatedBy = User.Identity.Name;
            person.DataStatus = EnumDataStatus.New.ToString();
            _refPersonRepository.Save(person);

            UpdateNumericData(viewModel, formCollection);
            MEmployee mEmployeeToInsert = new MEmployee();
            TransferFormValuesTo(mEmployeeToInsert, viewModel);
            mEmployeeToInsert.DepartmentId = _mDepartmentRepository.Get(formCollection["DepartmentId"]);
            mEmployeeToInsert.SetAssignedIdTo(viewModel.Id);
            mEmployeeToInsert.CreatedDate = DateTime.Now;
            mEmployeeToInsert.CreatedBy = User.Identity.Name;
            mEmployeeToInsert.DataStatus = EnumDataStatus.New.ToString();

            mEmployeeToInsert.PersonId = person;
            _mEmployeeRepository.Save(mEmployeeToInsert);

            try
            {
                _mEmployeeRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mEmployeeRepository.DbContext.RollbackTransaction();

                //throw e.GetBaseException();
                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        private void TransferFormValuesTo(RefPerson person, FormCollection formCollection)
        {
            person.PersonFirstName = formCollection["PersonFirstName"];
            person.PersonLastName = formCollection["PersonLastName"];
            person.PersonGender = formCollection["PersonGender"];
            if (formCollection["PersonDob"] != null)
                person.PersonDob = Convert.ToDateTime(formCollection["PersonDob"]);
            person.PersonPob = formCollection["PersonPob"];
            person.PersonPhone = formCollection["PersonPhone"];
            person.PersonMobile = formCollection["PersonMobile"];
            person.PersonEmail = formCollection["PersonEmail"];
            person.PersonReligion = formCollection["PersonReligion"];
            person.PersonRace = formCollection["PersonRace"];
            person.PersonIdCardType = formCollection["PersonIdCardType"];
            person.PersonIdCardNo = formCollection["PersonIdCardNo"];
        }

        [Transaction]
        public ActionResult Delete(MEmployee viewModel, FormCollection formCollection)
        {
            MEmployee mEmployeeToDelete = _mEmployeeRepository.Get(viewModel.Id);

            if (mEmployeeToDelete != null)
            {
                _mEmployeeRepository.Delete(mEmployeeToDelete);
            }

            try
            {
                _mEmployeeRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mEmployeeRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        [Transaction]
        public ActionResult Update(MEmployee viewModel, FormCollection formCollection)
        {

            UpdateNumericData(viewModel, formCollection);

            MEmployee mEmployeeToUpdate = _mEmployeeRepository.Get(viewModel.Id);
            TransferFormValuesTo(mEmployeeToUpdate, viewModel);
            mEmployeeToUpdate.DepartmentId = _mDepartmentRepository.Get(formCollection["DepartmentId"]);
            mEmployeeToUpdate.ModifiedDate = DateTime.Now;
            mEmployeeToUpdate.ModifiedBy = User.Identity.Name;
            mEmployeeToUpdate.DataStatus = EnumDataStatus.Updated.ToString();

            RefPerson person = mEmployeeToUpdate.PersonId;
            if (person == null)
            {
                person = new RefPerson();
                TransferFormValuesTo(person, formCollection);
                person.SetAssignedIdTo(Guid.NewGuid().ToString());
                person.CreatedDate = DateTime.Now;
                person.CreatedBy = User.Identity.Name;
                person.DataStatus = EnumDataStatus.New.ToString();
                _refPersonRepository.Save(person);

                mEmployeeToUpdate.PersonId = person;
            }
            else
            {
                TransferFormValuesTo(person, formCollection);
                person.ModifiedDate = DateTime.Now;
                person.ModifiedBy = User.Identity.Name;
                person.DataStatus = EnumDataStatus.Updated.ToString();
            }


            _mEmployeeRepository.Update(mEmployeeToUpdate);

            try
            {
                _mEmployeeRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mEmployeeRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        private static void UpdateNumericData(MEmployee viewModel, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["EmployeeCommissionProductVal"]))
            {
                string EmployeeCommissionProductVal = formCollection["EmployeeCommissionProductVal"].Replace(",", "");
                viewModel.EmployeeCommissionProductVal = Convert.ToDecimal(EmployeeCommissionProductVal);
            }
            else
            {
                viewModel.EmployeeCommissionProductVal = null;
            }
            if (!string.IsNullOrEmpty(formCollection["EmployeeCommissionServiceVal"]))
            {
                string ItemUomSalePrice = formCollection["EmployeeCommissionServiceVal"].Replace(",", "");
                viewModel.EmployeeCommissionServiceVal = Convert.ToDecimal(ItemUomSalePrice);
            }
            else
            {
                viewModel.EmployeeCommissionServiceVal = null;
            }
        }

        private void TransferFormValuesTo(MEmployee mEmployeeToUpdate, MEmployee mEmployeeFromForm)
        {
            mEmployeeToUpdate.DepartmentId = mEmployeeFromForm.DepartmentId;
            mEmployeeToUpdate.EmployeeStatus = mEmployeeFromForm.EmployeeStatus;
            mEmployeeToUpdate.EmployeeDesc = mEmployeeFromForm.EmployeeDesc;
            mEmployeeToUpdate.EmployeeCommissionProductType = mEmployeeFromForm.EmployeeCommissionProductType;
            mEmployeeToUpdate.EmployeeCommissionProductVal = mEmployeeFromForm.EmployeeCommissionProductVal;
            mEmployeeToUpdate.EmployeeCommissionServiceType = mEmployeeFromForm.EmployeeCommissionServiceType;
            mEmployeeToUpdate.EmployeeCommissionServiceVal = mEmployeeFromForm.EmployeeCommissionServiceVal;
        }


        [Transaction]
        public virtual ActionResult GetList()
        {
            var employees = _mEmployeeRepository.GetAll();
            StringBuilder sb = new StringBuilder();
            MEmployee mEmployee = new MEmployee();
            sb.AppendFormat("{0}:{1};", string.Empty, "-Pilih Karyawan-");
            for (int i = 0; i < employees.Count; i++)
            {
                mEmployee = employees[i];
                sb.AppendFormat("{0}:{1} {2}", mEmployee.Id, mEmployee.PersonId.PersonFirstName, mEmployee.PersonId.PersonLastName);
                if (i < employees.Count - 1)
                    sb.Append(";");
            }
            return Content(sb.ToString());
        }

        //since upgrade to jqgrid 4, url property not run well
        //so use formcollection "oper" to filter the action
        //this just for insert and update, delete filter not change
        [Transaction]
        public ActionResult InsertOrUpdate(MEmployee viewModel, FormCollection formCollection)
        {
            if (formCollection["oper"].Equals("add"))
            {
                return Insert(viewModel, formCollection);
            }
            else if (formCollection["oper"].Equals("edit"))
            {
                return Update(viewModel, formCollection);
            }
            return View();
        }
    }
}
