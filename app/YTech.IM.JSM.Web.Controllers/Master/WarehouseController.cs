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
    public class WarehouseController : Controller
    {
        //public WarehouseController() : this(new MWarehouseRepository(), new RefAddressRepository(), new MEmployeeRepository(), new MCostCenterRepository())
        //{}

        private readonly IMWarehouseRepository _mWarehouseRepository;
        private readonly IRefAddressRepository _refAddressRepository;
        private readonly IMEmployeeRepository _mEmployeeRepository;
        private readonly IMCostCenterRepository _mCostCenterRepository;
        
        public WarehouseController(IMWarehouseRepository mWarehouseRepository, IRefAddressRepository refAddressRepository, IMEmployeeRepository mEmployeeRepository, IMCostCenterRepository mCostCenterRepository)
        {
            Check.Require(mWarehouseRepository != null, "mWarehouseRepository may not be null");
            Check.Require(refAddressRepository != null, "refAddressRepository may not be null");
            Check.Require(mEmployeeRepository != null, "mEmployeeRepository may not be null");
            Check.Require(mCostCenterRepository != null, "mCostCenterRepository may not be null");
        
            this._mWarehouseRepository = mWarehouseRepository;
            this._refAddressRepository = refAddressRepository;
            this._mEmployeeRepository = mEmployeeRepository;
            this._mCostCenterRepository = mCostCenterRepository;
        }


        public ActionResult Index()
        {
            return View();
        }

        [Transaction]
        public virtual ActionResult List(string sidx, string sord, int page, int rows)
        {
            int totalRecords = 0;
            var warehouses = _mWarehouseRepository.GetPagedWarehouseList(sidx, sord, page, rows, ref totalRecords);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from warehouse in warehouses
                    select new
                    {
                        i = warehouse.Id.ToString(),
                        cell = new string[] {
                            warehouse.Id, 
                            warehouse.WarehouseName, 
                             warehouse.WarehouseStatus,
                            warehouse.EmployeeId != null?  warehouse.EmployeeId.Id : null,
                            warehouse.EmployeeId != null?  warehouse.EmployeeId.PersonId.PersonFirstName : null,
                            warehouse.CostCenterId != null?  warehouse.CostCenterId.CostCenterName : null,
                            warehouse.CostCenterId != null?  warehouse.CostCenterId.Id : null,
                            warehouse.AddressId != null?  warehouse.AddressId.AddressLine1 : null,
                            warehouse.AddressId != null?  warehouse.AddressId.AddressLine2 : null,
                            warehouse.AddressId != null?  warehouse.AddressId.AddressLine3 : null,
                            warehouse.AddressId != null?  warehouse.AddressId.AddressPhone : null,
                            warehouse.AddressId != null?  warehouse.AddressId.AddressCity : null,
                            warehouse.WarehouseDesc
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        
        [Transaction]
        public ActionResult Insert(MWarehouse viewModel, FormCollection formCollection)
        {
            RefAddress address = new RefAddress();
            address.AddressLine1 = formCollection["AddressLine1"];
            address.AddressLine2 = formCollection["AddressLine2"];
            address.AddressLine3 = formCollection["AddressLine3"];
            address.AddressPhone = formCollection["AddressPhone"];
            address.AddressCity = formCollection["AddressCity"];
            address.SetAssignedIdTo(Guid.NewGuid().ToString());
            _refAddressRepository.Save(address);

            MWarehouse mWarehouseToInsert = new MWarehouse();
            TransferFormValuesTo(mWarehouseToInsert, viewModel);
            mWarehouseToInsert.EmployeeId = _mEmployeeRepository.Get(formCollection["EmployeeId"]);
            mWarehouseToInsert.CostCenterId = _mCostCenterRepository.Get(formCollection["CostCenterId"]);
            mWarehouseToInsert.SetAssignedIdTo(viewModel.Id);
            mWarehouseToInsert.CreatedDate = DateTime.Now;
            mWarehouseToInsert.CreatedBy = User.Identity.Name;
            mWarehouseToInsert.DataStatus = EnumDataStatus.New.ToString();
            mWarehouseToInsert.AddressId = address;
            _mWarehouseRepository.Save(mWarehouseToInsert);

            try
            {
                _mWarehouseRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mWarehouseRepository.DbContext.RollbackTransaction();

                //throw e.GetBaseException();
                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        [Transaction]
        public ActionResult Delete(MWarehouse viewModel, FormCollection formCollection)
        {
            MWarehouse mWarehouseToDelete = _mWarehouseRepository.Get(viewModel.Id);

            if (mWarehouseToDelete != null)
            {
                _mWarehouseRepository.Delete(mWarehouseToDelete);
                _refAddressRepository.Delete(mWarehouseToDelete.AddressId);
            }

            try
            {
                _mWarehouseRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mWarehouseRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }


        //since upgrade to jqgrid 4, url property not run well
        //so use formcollection "oper" to filter the action
        //this just for insert and update, delete filter not change
        [Transaction]
        public ActionResult InsertOrUpdate(MWarehouse  viewModel, FormCollection formCollection)
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
        public ActionResult Update(MWarehouse viewModel, FormCollection formCollection)
        {
            MWarehouse mWarehouseToUpdate = _mWarehouseRepository.Get(viewModel.Id);
            TransferFormValuesTo(mWarehouseToUpdate, viewModel);
            mWarehouseToUpdate.EmployeeId = _mEmployeeRepository.Get(formCollection["EmployeeId"]);
            mWarehouseToUpdate.CostCenterId = _mCostCenterRepository.Get(formCollection["CostCenterId"]);
            mWarehouseToUpdate.ModifiedDate = DateTime.Now;
            mWarehouseToUpdate.ModifiedBy = User.Identity.Name;
            mWarehouseToUpdate.DataStatus = EnumDataStatus.Updated.ToString();

            mWarehouseToUpdate.AddressId.AddressLine1 = formCollection["AddressLine1"];
            mWarehouseToUpdate.AddressId.AddressLine1 = formCollection["AddressLine1"];
            mWarehouseToUpdate.AddressId.AddressLine2 = formCollection["AddressLine2"];
            mWarehouseToUpdate.AddressId.AddressLine3 = formCollection["AddressLine3"];
            mWarehouseToUpdate.AddressId.AddressPhone = formCollection["AddressPhone"];
            mWarehouseToUpdate.AddressId.AddressCity = formCollection["AddressCity"];

            _mWarehouseRepository.Update(mWarehouseToUpdate);

            try
            {
                _mWarehouseRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mWarehouseRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        private void TransferFormValuesTo(MWarehouse mWarehouseToUpdate, MWarehouse mWarehouseFromForm)
        {
            mWarehouseToUpdate.WarehouseName = mWarehouseFromForm.WarehouseName;
            mWarehouseToUpdate.WarehouseDesc = mWarehouseFromForm.WarehouseDesc;
            mWarehouseToUpdate.WarehouseStatus = mWarehouseFromForm.WarehouseStatus;
        }

    }
}
