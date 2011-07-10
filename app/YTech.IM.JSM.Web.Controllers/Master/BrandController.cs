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
    public class BrandController : Controller
    {
        //public BrandController()
        //    : this(new MBrandRepository())
        //{
        //}

        public BrandController(IMBrandRepository mBrandRepository)
        {
            Check.Require(mBrandRepository != null, "mBrandRepository may not be null");

            this._mBrandRepository = mBrandRepository;
        }
        private readonly IMBrandRepository _mBrandRepository;


        public ActionResult Index()
        {
            return View();
        }

        [Transaction]
        public virtual ActionResult List(string sidx, string sord, int page, int rows)
        {
            int totalRecords = 0;
            var itemCats = _mBrandRepository.GetPagedBrandList(sidx, sord, page, rows, ref totalRecords);
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
                            itemCat.BrandName, 
                            itemCat.BrandDesc
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public ActionResult Insert(MBrand viewModel, FormCollection formCollection)
        {
            if (!(ViewData.ModelState.IsValid && viewModel.IsValid()))
            {

            }
            MBrand mCompanyToInsert = new MBrand();
            TransferFormValuesTo(mCompanyToInsert, viewModel);
            mCompanyToInsert.SetAssignedIdTo(viewModel.Id);
            mCompanyToInsert.CreatedDate = DateTime.Now;
            mCompanyToInsert.CreatedBy = User.Identity.Name;
            mCompanyToInsert.DataStatus = EnumDataStatus.New.ToString();
            _mBrandRepository.Save(mCompanyToInsert);

            try
            {
                _mBrandRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mBrandRepository.DbContext.RollbackTransaction();

                //throw e.GetBaseException();
                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        [Transaction]
        public ActionResult Delete(MBrand viewModel, FormCollection formCollection)
        {
            MBrand mCompanyToDelete = _mBrandRepository.Get(viewModel.Id);

            if (mCompanyToDelete != null)
            {
                _mBrandRepository.Delete(mCompanyToDelete);
            }

            try
            {
                _mBrandRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mBrandRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        //since upgrade to jqgrid 4, url property not run well
        //so use formcollection "oper" to filter the action
        //this just for insert and update, delete filter not change
        [Transaction]
        public ActionResult InsertOrUpdate(MBrand viewModel, FormCollection formCollection)
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
        public ActionResult Update(MBrand viewModel, FormCollection formCollection)
        {
            MBrand mCompanyToUpdate = _mBrandRepository.Get(viewModel.Id);
            TransferFormValuesTo(mCompanyToUpdate, viewModel);
            mCompanyToUpdate.ModifiedDate = DateTime.Now;
            mCompanyToUpdate.ModifiedBy = User.Identity.Name;
            mCompanyToUpdate.DataStatus = EnumDataStatus.Updated.ToString();
            _mBrandRepository.Update(mCompanyToUpdate);

            try
            {
                _mBrandRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mBrandRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        private void TransferFormValuesTo(MBrand mCompanyToUpdate, MBrand mCompanyFromForm)
        {
            mCompanyToUpdate.BrandName = mCompanyFromForm.BrandName;
            mCompanyToUpdate.BrandDesc = mCompanyFromForm.BrandDesc;
        }


        [Transaction]
        public virtual ActionResult GetList()
        {
            var brands = _mBrandRepository.GetAll();
            StringBuilder sb = new StringBuilder();
            MBrand mBrand = new MBrand();
            sb.AppendFormat("{0}:{1};", string.Empty, "-Pilih Merek-");
            for (int i = 0; i < brands.Count; i++)
            {
                mBrand = brands[i];
                sb.AppendFormat("{0}:{1}", mBrand.Id, mBrand.BrandName);
                if (i < brands.Count - 1)
                    sb.Append(";");
            }
            return Content(sb.ToString());
        }
    }
}
