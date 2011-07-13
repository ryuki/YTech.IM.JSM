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
    public class DepartmentController : Controller
    {
        //public DepartmentController()
        //  : this(new MDepartmentRepository())
        //{ }

        private readonly IMDepartmentRepository _mDepartmentRepository;
        public DepartmentController(IMDepartmentRepository mDepartmentRepository)
        {
            Check.Require(mDepartmentRepository != null, "mDepartmentRepository may not be null");

            this._mDepartmentRepository = mDepartmentRepository;
        }


        public ActionResult Index()
        {
            return View();
        }

        [Transaction]
        public virtual ActionResult List(string sidx, string sord, int page, int rows)
        {
            int totalRecords = 0;
            var itemCats = _mDepartmentRepository.GetPagedDepartmentList(sidx, sord, page, rows, ref totalRecords);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from itemCat in itemCats
                    select new
                    {
                        i = itemCat.Id.ToString(),
                        cell = new string[] {
                            itemCat.Id, 
                            itemCat.DepartmentName, 
                            itemCat.DepartmentDesc
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public ActionResult Insert(MDepartment viewModel, FormCollection formCollection)
        {

            MDepartment mCompanyToInsert = new MDepartment();
            TransferFormValuesTo(mCompanyToInsert, viewModel);
            mCompanyToInsert.SetAssignedIdTo(viewModel.Id);
            mCompanyToInsert.CreatedDate = DateTime.Now;
            mCompanyToInsert.CreatedBy = User.Identity.Name;
            mCompanyToInsert.DataStatus = EnumDataStatus.New.ToString();
            _mDepartmentRepository.Save(mCompanyToInsert);

            try
            {
                _mDepartmentRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mDepartmentRepository.DbContext.RollbackTransaction();

                //throw e.GetBaseException();
                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        [Transaction]
        public ActionResult Delete(MDepartment viewModel, FormCollection formCollection)
        {
            MDepartment mCompanyToDelete = _mDepartmentRepository.Get(viewModel.Id);

            if (mCompanyToDelete != null)
            {
                _mDepartmentRepository.Delete(mCompanyToDelete);
            }

            try
            {
                _mDepartmentRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mDepartmentRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }


        //since upgrade to jqgrid 4, url property not run well
        //so use formcollection "oper" to filter the action
        //this just for insert and update, delete filter not change
        [Transaction]
        public ActionResult InsertOrUpdate(MDepartment viewModel, FormCollection formCollection)
        {
            if (formCollection["oper"].Equals("add"))
            {
                return Insert(viewModel, formCollection);
            }
            else if (formCollection["oper"].Equals("edit"))
            {
                return Update(viewModel, formCollection);
            }
            else if (formCollection["oper"].Equals("delete"))
            {
                return Delete(viewModel, formCollection);
            }
            return View();
        }


        [Transaction]
        public ActionResult Update(MDepartment viewModel, FormCollection formCollection)
        {
            MDepartment mCompanyToUpdate = _mDepartmentRepository.Get(viewModel.Id);
            TransferFormValuesTo(mCompanyToUpdate, viewModel);
            mCompanyToUpdate.ModifiedDate = DateTime.Now;
            mCompanyToUpdate.ModifiedBy = User.Identity.Name;
            mCompanyToUpdate.DataStatus = EnumDataStatus.Updated.ToString();
            _mDepartmentRepository.Update(mCompanyToUpdate);

            try
            {
                _mDepartmentRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mDepartmentRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        private void TransferFormValuesTo(MDepartment mCompanyToUpdate, MDepartment mCompanyFromForm)
        {
            mCompanyToUpdate.DepartmentName = mCompanyFromForm.DepartmentName;
            mCompanyToUpdate.DepartmentDesc = mCompanyFromForm.DepartmentDesc;
            mCompanyToUpdate.DepartmentStatus = mCompanyFromForm.DepartmentStatus;
        }


        [Transaction]
        public virtual ActionResult GetList()
        {
            var brands = _mDepartmentRepository.GetAll();
            StringBuilder sb = new StringBuilder();
            MDepartment mDepartment;
            sb.AppendFormat("{0}:{1};", string.Empty, "-Pilih Departemen-");
            for (int i = 0; i < brands.Count; i++)
            {
                mDepartment = brands[i];
                sb.AppendFormat("{0}:{1}", mDepartment.Id, mDepartment.DepartmentName);
                if (i < brands.Count - 1)
                    sb.Append(";");
            }
            return Content(sb.ToString());
        }
    }
}
