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
    public class SupplierController : Controller
    {
         //public SupplierController() : this(new MSupplierRepository(), new RefAddressRepository())
         //{}

        private readonly IMSupplierRepository _mSupplierRepository;
        private readonly IRefAddressRepository _refAddressRepository;
        public SupplierController(IMSupplierRepository mSupplierRepository, IRefAddressRepository refAddressRepository)
        {
            Check.Require(mSupplierRepository != null, "mSupplierRepository may not be null");
            Check.Require(refAddressRepository != null, "refAddressRepository may not be null");

            this._mSupplierRepository = mSupplierRepository;
            this._refAddressRepository = refAddressRepository;
        }


        public ActionResult Index()
        {
            return View();
        }


        [Transaction]
        public virtual ActionResult List(string sidx, string sord, int page, int rows)
        {
             int totalRecords = 0;
             var sups = _mSupplierRepository.GetPagedSupplierList(sidx, sord, page, rows, ref totalRecords);
             int pageSize = rows;
             int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from sup in sups
                    select new
                    {
                        i = sup.Id.ToString(),
                        cell = new string[] {
                            sup.Id, 
                            sup.SupplierName, 
                            sup.SupplierMaxDebt.HasValue ? sup.SupplierMaxDebt.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                       //GetAccountRef(sup.Id) != null ? GetAccountRef(sup.Id).AccountId.Id : null,
                       //  GetAccountRef(sup.Id) != null ? GetAccountRef(sup.Id).AccountId.AccountName : null,
                            sup.SupplierStatus,
                          sup.AddressId != null?  sup.AddressId.AddressLine1 : null,
                          sup.AddressId != null?  sup.AddressId.AddressLine2 : null,
                          sup.AddressId != null?  sup.AddressId.AddressLine3 : null,
                          sup.AddressId != null?  sup.AddressId.AddressPhone : null,
                          sup.AddressId != null?  sup.AddressId.AddressCity : null,
                            sup.SupplierDesc
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public ActionResult Insert(MSupplier viewModel, FormCollection formCollection)
        {
            RefAddress address = new RefAddress();
            TransferFormValuesTo(address, formCollection);
            address.SetAssignedIdTo(Guid.NewGuid().ToString());
            address.CreatedDate = DateTime.Now;
            address.CreatedBy = User.Identity.Name;
            address.DataStatus = EnumDataStatus.New.ToString();
            _refAddressRepository.Save(address);

            UpdateNumericData(viewModel, formCollection);
            MSupplier mSupplierToInsert = new MSupplier();
            TransferFormValuesTo(mSupplierToInsert, viewModel);
            mSupplierToInsert.SetAssignedIdTo(viewModel.Id);
            mSupplierToInsert.CreatedDate = DateTime.Now;
            mSupplierToInsert.CreatedBy = User.Identity.Name;
            mSupplierToInsert.DataStatus = EnumDataStatus.New.ToString();
            
            mSupplierToInsert.AddressId = address;

            _mSupplierRepository.Save(mSupplierToInsert);

            return Content("success");
        }

        private void TransferFormValuesTo(RefAddress address, FormCollection formCollection)
        {
            address.AddressLine1 = formCollection["AddressLine1"];
            address.AddressLine2 = formCollection["AddressLine2"];
            address.AddressLine3 = formCollection["AddressLine3"];
            address.AddressPhone = formCollection["AddressPhone"];
            address.AddressCity = formCollection["AddressCity"];
          
        }

        [Transaction]
        public ActionResult Delete(MSupplier viewModel, FormCollection formCollection)
        {
            MSupplier mSupplierToDelete = _mSupplierRepository.Get(viewModel.Id);

            if (mSupplierToDelete != null)
            {
                _mSupplierRepository.Delete(mSupplierToDelete);
                _refAddressRepository.Delete(mSupplierToDelete.AddressId);
            }

            try
            {
                _mSupplierRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mSupplierRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        [Transaction]
        public ActionResult Update(MSupplier viewModel, FormCollection formCollection)
        {
            UpdateNumericData(viewModel, formCollection);
            MSupplier mSupplierToUpdate = _mSupplierRepository.Get(viewModel.Id);
            TransferFormValuesTo(mSupplierToUpdate, viewModel);
            mSupplierToUpdate.ModifiedDate = DateTime.Now;
            mSupplierToUpdate.ModifiedBy = User.Identity.Name;
            mSupplierToUpdate.DataStatus = EnumDataStatus.Updated.ToString();

            RefAddress address = mSupplierToUpdate.AddressId;
            TransferFormValuesTo(address, formCollection);
            address.ModifiedDate = DateTime.Now;
            address.ModifiedBy = User.Identity.Name;
            address.DataStatus = EnumDataStatus.Updated.ToString();

            _mSupplierRepository.Update(mSupplierToUpdate);


            bool isSave = false;
            try
            {
                _mSupplierRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mSupplierRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        private static void UpdateNumericData(MSupplier viewModel, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["SupplierMaxDebt"]))
            {
                string SupplierMaxDebt = formCollection["SupplierMaxDebt"].Replace(",", "");
                viewModel.SupplierMaxDebt = Convert.ToDecimal(SupplierMaxDebt);
            }
            else
            {
                viewModel.SupplierMaxDebt = null;
            }
        }

        private static void TransferFormValuesTo(MSupplier mSupplierToUpdate, MSupplier mSupplierFromForm)
        {
            mSupplierToUpdate.SupplierName = mSupplierFromForm.SupplierName;
            mSupplierToUpdate.SupplierDesc = mSupplierFromForm.SupplierDesc;
            mSupplierToUpdate.SupplierMaxDebt = mSupplierFromForm.SupplierMaxDebt;
            mSupplierToUpdate.SupplierStatus = mSupplierFromForm.SupplierStatus;
        }

        //since upgrade to jqgrid 4, url property not run well
        //so use formcollection "oper" to filter the action
        //this just for insert and update, delete filter not change
        [Transaction]
        public ActionResult InsertOrUpdate(MSupplier viewModel, FormCollection formCollection)
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
