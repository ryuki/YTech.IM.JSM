using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using SharpArch.Core;
using SharpArch.Web.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;
using YTech.IM.JSM.Data.Repository;
using YTech.IM.JSM.Enums;

namespace YTech.IM.JSM.Web.Controllers.Master
{
    [HandleError]
    public class MItemCatController : Controller
    {
        public MItemCatController() : this(new MItemCatRepository())
        {
        }

        private readonly IMItemCatRepository _mItemCatRepository;
        public MItemCatController(IMItemCatRepository mItemCatRepository)
        {
            Check.Require(mItemCatRepository != null, "mItemCatRepository may not be null");

            this._mItemCatRepository = mItemCatRepository;
        }


        public ActionResult Index()
        {
            return View();
        }

        [Transaction]
        public virtual ActionResult List(string sidx, string sord, int page, int rows)
        {
            int totalRecords = 0;
            var itemCats = _mItemCatRepository.GetPagedItemCatList(sidx, sord, page, rows, ref totalRecords);
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
                            itemCat.ItemCatName, 
                            itemCat.ItemCatDesc
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public virtual ActionResult GetList()
        {
            var itemCats = _mItemCatRepository.GetAll();
            StringBuilder sb = new StringBuilder();
            MItemCat mItemCat;
            sb.AppendFormat("{0}:{1}", string.Empty, "-Pilih Kategori Produk-");
            for (int i = 0; i < itemCats.Count; i++)
            {
                mItemCat = itemCats[i];
                sb.AppendFormat(";{0}:{1}", mItemCat.Id, mItemCat.ItemCatName);
            }
            return Content(sb.ToString());
        }

        [Transaction]
        public ActionResult Insert(MItemCat viewModel, FormCollection formCollection)
        {

            MItemCat mItemCatToInsert = new MItemCat();
            TransferFormValuesTo(mItemCatToInsert, viewModel);
            mItemCatToInsert.SetAssignedIdTo(viewModel.Id);
            mItemCatToInsert.CreatedDate = DateTime.Now;
            mItemCatToInsert.CreatedBy = User.Identity.Name;
            mItemCatToInsert.DataStatus = EnumDataStatus.New.ToString();
            _mItemCatRepository.Save(mItemCatToInsert);

            try
            {
                _mItemCatRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mItemCatRepository.DbContext.RollbackTransaction();

                //throw e.GetBaseException();
                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        [Transaction]
        public ActionResult Delete(MItemCat viewModel, FormCollection formCollection)
        {
            MItemCat mItemCatToDelete = _mItemCatRepository.Get(viewModel.Id);

            if (mItemCatToDelete != null)
            {
                _mItemCatRepository.Delete(mItemCatToDelete);
            }

            try
            {
                _mItemCatRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mItemCatRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        [Transaction]
        public ActionResult Update(MItemCat viewModel, FormCollection formCollection)
        {
            MItemCat mItemCatToUpdate = _mItemCatRepository.Get(viewModel.Id);
            TransferFormValuesTo(mItemCatToUpdate, viewModel);
            mItemCatToUpdate.ModifiedDate = DateTime.Now;
            mItemCatToUpdate.ModifiedBy = User.Identity.Name;
            mItemCatToUpdate.DataStatus = EnumDataStatus.Updated.ToString();
            _mItemCatRepository.Update(mItemCatToUpdate);

            try
            {
                _mItemCatRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mItemCatRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        private void TransferFormValuesTo(MItemCat mItemCatToUpdate, MItemCat mItemCatFromForm)
        {
            mItemCatToUpdate.ItemCatName = mItemCatFromForm.ItemCatName;
            mItemCatToUpdate.ItemCatDesc = mItemCatFromForm.ItemCatDesc;
        }

        //since upgrade to jqgrid 4, url property not run well
        //so use formcollection "oper" to filter the action
        //this just for insert and update, delete filter not change
        [Transaction]
        public ActionResult InsertOrUpdate(MItemCat viewModel, FormCollection formCollection)
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
