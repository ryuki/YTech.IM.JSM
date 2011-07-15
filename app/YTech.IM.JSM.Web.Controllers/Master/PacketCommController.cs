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
using YTech.IM.JSM.Enums;



namespace YTech.IM.JSM.Web.Controllers.Master
{
    [HandleError]
    public class PacketCommController : Controller
    {
        private readonly IMPacketCommRepository _mPacketCommRepository;
        private readonly IMPacketRepository _mPacketRepository;
        private readonly IMEmployeeRepository _mEmployeeRepository;
        public PacketCommController(IMPacketCommRepository mPacketCommRepository, IMPacketRepository mPacketRepository, IMEmployeeRepository mEmployeeRepository)
        {
            Check.Require(mPacketCommRepository != null, "mPacketCommRepository may not be null");
            Check.Require(mPacketRepository != null, "mPacketRepository may not be null");
            Check.Require(mEmployeeRepository != null, "mEmployeeRepository may not be null");

            this._mPacketCommRepository = mPacketCommRepository;
            this._mPacketRepository = mPacketRepository;
            this._mEmployeeRepository = mEmployeeRepository;
        }

        [Transaction]
        public virtual ActionResult GetListForSubGrid(string id)
        {
            var packets = _mPacketCommRepository.GetByEmployeeId(id);

            var jsonData = new
            {
                rows = (
                    from packetComm in packets
                    select new
                    {
                        i = packetComm.Id.ToString(),
                        cell = new string[] {
                           packetComm.PacketId != null ? packetComm.PacketId.PacketName : null, 
                           packetComm.PacketCommType,
                            packetComm.PacketCommVal.HasValue ?  packetComm.PacketCommVal.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                           packetComm.PacketCommDesc
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #region popup
        public virtual ActionResult PopupAdd()
        {
            return View();
        }

        [Transaction]
        public virtual ActionResult PopupList(string sidx, string sord, int page, int rows, string employeeId)
        {
            int totalRecords = 0;
            var packetComms = _mPacketCommRepository.GetPagedPacketCommList(sidx, sord, page, rows, ref totalRecords, employeeId);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from packetComm in packetComms
                    select new
                    {
                        i = packetComm.Id,
                        cell = new string[] {
                            packetComm.Id,
                             packetComm.PacketId != null ? packetComm.PacketId.Id : null, 
                             packetComm.PacketId != null ? packetComm.PacketId.PacketName : null, 
                           packetComm.PacketCommType,
                            packetComm.PacketCommVal.HasValue ?  packetComm.PacketCommVal.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                           packetComm.PacketCommDesc
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public ActionResult PopupInsert(MPacketComm viewModel, FormCollection formCollection, string EmployeeId)
        {
            UpdateNumericData(viewModel, formCollection);
            MPacketComm packetComm = new MPacketComm();
            TransferFormValuesTo(packetComm, viewModel, formCollection, EmployeeId);

            packetComm.SetAssignedIdTo(Guid.NewGuid().ToString());
            packetComm.CreatedDate = DateTime.Now;
            packetComm.CreatedBy = User.Identity.Name;
            packetComm.DataStatus = EnumDataStatus.New.ToString();

            //IList<MItemUom> listItemUom = new List<MItemUom>();

            //mItemToInsert.ItemUoms = listItemUom;

            _mPacketCommRepository.Save(packetComm);

            try
            {
                _mPacketCommRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mPacketCommRepository.DbContext.RollbackTransaction();

                //throw e.GetBaseException();
                return Content(e.GetBaseException().Message);
            }

            return Content("Data komisi paket berhasil disimpan");
        }

        [Transaction]
        public ActionResult PopupUpdate(MPacketComm viewModel, FormCollection formCollection, string EmployeeId)
        {
            UpdateNumericData(viewModel, formCollection);
            MPacketComm packetComm = _mPacketCommRepository.Get(viewModel.Id);
            TransferFormValuesTo(packetComm, viewModel, formCollection, EmployeeId);

            packetComm.ModifiedDate = DateTime.Now;
            packetComm.ModifiedBy = User.Identity.Name;
            packetComm.DataStatus = EnumDataStatus.Updated.ToString();

            //IList<MItemUom> listItemUom = new List<MItemUom>();

            //mItemToInsert.ItemUoms = listItemUom;

            _mPacketCommRepository.Update(packetComm);

            try
            {
                _mPacketCommRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mPacketCommRepository.DbContext.RollbackTransaction();

                //throw e.GetBaseException();
                return Content(e.GetBaseException().Message);
            }

            return Content("Data komisi paket berhasil disimpan");
        }

        [Transaction]
        public ActionResult PopupDelete(MPacketComm viewModel, FormCollection formCollection)
        {
            MPacketComm packetComm = _mPacketCommRepository.Get(viewModel.Id);

            if (packetComm != null)
            {
                _mPacketCommRepository.Delete(packetComm);
            }

            try
            {
                _mPacketCommRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mPacketCommRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("Data komisi paket berhasil dihapus");
        }

        private void TransferFormValuesTo(MPacketComm packetComm, MPacketComm viewModel, FormCollection formCollection, string EmployeeId)
        {
            packetComm.PacketId = _mPacketRepository.Get(formCollection["PacketId"]);
            packetComm.EmployeeId = _mEmployeeRepository.Get(EmployeeId);

            packetComm.PacketCommVal = viewModel.PacketCommVal;
            packetComm.PacketCommType = viewModel.PacketCommType;
            packetComm.PacketCommStatus = viewModel.PacketCommStatus;
            packetComm.PacketCommDesc = viewModel.PacketCommDesc;
        }

        private void UpdateNumericData(MPacketComm viewModel, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["PacketCommVal"]))
            {
                string PacketCommVal = formCollection["PacketCommVal"].Replace(",", "");
                viewModel.PacketCommVal = Convert.ToDecimal(PacketCommVal);
            }
            else
            {
                viewModel.PacketCommVal = null;
            }
        }
        #endregion



    }
}
