using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Reporting.WebForms;
using SharpArch.Core;
using SharpArch.Web.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;
using YTech.IM.JSM.Data.Repository;
using YTech.IM.JSM.Enums;
using YTech.IM.JSM.Web.Controllers.ViewModel;

namespace YTech.IM.JSM.Web.Controllers.Transaction
{
    public partial class InventoryController
    {
        private const string CONST_TRANSDET = "TransDet";

        [Transaction]
        public virtual ActionResult Billing()
        {
            BillingFormViewModel viewModel = BillingFormViewModel.CreateBillingViewModel(_mRoomRepository, _mEmployeeRepository, _mCustomerRepository, _tTransRoomRepository);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Billing(TTrans Trans, FormCollection formCollection)
        {
            if (formCollection["btnIn"] != null)
            {
                return SaveTransRoom(Trans, formCollection);
            }
            else if (formCollection["btnPaid"] != null)
            {
                return UpdateTransRoom(Trans, formCollection);
            }
            else if (formCollection["btnCancel"] != null)
            {
                return CancelTransRoom(Trans, formCollection);
            }
            else if (formCollection["btnPrint"] != null)
            {
                SetReportDataForPrint(formCollection["TransId"]);

                var e = new
                {
                    Success = false,
                    Message = "redirect",
                    RoomStatus = EnumTransRoomStatus.New.ToString()
                };
                return Json(e, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        private void SetReportDataForPrint(string TransId)
        {
            ReportDataSource[] repCol = new ReportDataSource[3];
            TTrans trans = _tTransRepository.Get(TransId);
            IList<TTrans> listTrans = new List<TTrans>();
            listTrans.Add(trans);
            var list = from t in listTrans
                       select new
                       {
                           t.TransFactur,
                           t.TransDate,
                           t.TransSubTotal,
                           t.TransBy,
                           t.TransDesc,
                           t.TransDiscount,
                           t.TransStatus,
                           CustomerName = GetCustomerName(t.TransBy)
                       }
       ;
            ReportDataSource reportDataSource = new ReportDataSource("TransViewModel", list.ToList());
            repCol[0] = reportDataSource;

            IList<TTransDet> listDetail = trans.TransDets;
            Session["DetailRowCount"] = listDetail.Count;
            var listDet = from det in listDetail
                          select new
                          {
                              EmployeeId = det.EmployeeId.Id,
                              EmployeeName = det.EmployeeId.PersonId.PersonName,
                              PacketId = det.PacketId.Id,
                              det.PacketId.PacketName,
                              det.TransDetPrice,
                              det.TransDetQty,
                              det.TransDetDisc,
                              det.TransDetCommissionProduct,
                              det.TransDetCommissionService,
                              det.TransDetNo,
                              det.TransDetTotal
                          }
      ;
            reportDataSource = new ReportDataSource("TransDetViewModel", listDet.ToList());
            repCol[1] = reportDataSource;

            TTransRoom troom = _tTransRoomRepository.Get(TransId);
            IList<TTransRoom> listTransroom = new List<TTransRoom>();
            listTransroom.Add(troom);
            var listRoom = from det in listTransroom
                           select new
                           {
                               det.RoomId.Id,
                               det.RoomInDate,
                               det.RoomOutDate,
                               det.RoomStatus,
                               det.RoomVoucherPaid,
                               det.RoomCashPaid,
                               det.RoomCreditPaid,
                               det.RoomCommissionProduct,
                               det.RoomCommissionService
                           }
      ;
            reportDataSource = new ReportDataSource("TransRoomViewModel", listRoom.ToList());
            repCol[2] = reportDataSource;
            Session["ReportData"] = repCol;
        }

        private string GetCustomerName(string customerId)
        {
            if (!string.IsNullOrEmpty(customerId))
            {
                MCustomer cust = _mCustomerRepository.Get(customerId);
                if (cust != null)
                {
                    return cust.PersonId.PersonName;
                }
            }
            return string.Empty;
        }

        private ActionResult CancelTransRoom(TTrans Trans, FormCollection formCollection)
        {
            _tTransRoomRepository.DbContext.BeginTransaction();

            TTransRoom troom = _tTransRoomRepository.Get(formCollection["TransId"]);
            if (troom != null)
            {
                _tTransRoomRepository.Delete(troom);
            }
            TTrans trans = _tTransRepository.Get(formCollection["TransId"]);
            if (trans != null)
            {
                _tTransRepository.Delete(trans);
            }

            string Message = string.Empty;
            bool Success = true;
            try
            {
                _tTransRoomRepository.DbContext.CommitTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Success;
            }
            catch (Exception ex)
            {
                Success = false;
                Message = ex.GetBaseException().Message;
                _tTransRoomRepository.DbContext.RollbackTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Failed;
            }
            var e = new
            {
                Success,
                Message,
                RoomStatus = EnumTransRoomStatus.New.ToString()
            };
            return Json(e, JsonRequestBehavior.AllowGet);
        }

        private ActionResult UpdateTransRoom(TTrans Trans, FormCollection formCollection)
        {
            _tTransRoomRepository.DbContext.BeginTransaction();
            TTrans trans = _tTransRepository.Get(formCollection["TransId"]);

            if (!string.IsNullOrEmpty(formCollection["TransDiscount"]))
            {
                string TransDiscount = formCollection["TransDiscount"].Replace(",", "");
                trans.TransDiscount = Convert.ToDecimal(TransDiscount);
            }
            else
            {
                trans.TransDiscount = null;
            }
            trans.TransPaymentMethod = EnumPaymentMethod.Tunai.ToString();
            trans.TransBy = Trans.TransBy;
            trans.TransDate = Trans.TransDate;
            trans.TransFactur = Helper.CommonHelper.GetFacturNo(EnumTransactionStatus.Service);
            trans.WarehouseId = Helper.CommonHelper.GetDefaultWarehouse();
            //update subtotal 
            trans.TransSubTotal = trans.TransDets.Sum(x => x.TransDetTotal);
            _tTransRepository.Update(trans);

            decimal totalHpp = 0;
            //update stock
            foreach (TTransDet det in trans.TransDets)
            {
                foreach (TTransDetItem detItem in det.TTransDetItems)
                {
                    SaveStockItem(trans.TransDate, trans.TransDesc, detItem.ItemId, detItem.ItemQty, false, trans.WarehouseId);
                    //sum hpp for each stock
                    totalHpp += UpdateStock(trans.TransDate, trans.TransDesc, trans.TransStatus, detItem.ItemId, 0, detItem.ItemQty, det, false, trans.WarehouseId);

                }
            }

            TTransRoom troom = _tTransRoomRepository.Get(formCollection["TransId"]);
            troom.RoomStatus = EnumTransRoomStatus.Paid.ToString();
            if (!string.IsNullOrEmpty(formCollection["hidpaymentCash"]))
            {
                string paymentCash = formCollection["hidpaymentCash"].Replace(",", "");
                troom.RoomCashPaid = Convert.ToDecimal(paymentCash);
            }
            if (!string.IsNullOrEmpty(formCollection["hidpaymentVoucher"]))
            {
                string paymentVoucher = formCollection["hidpaymentVoucher"].Replace(",", "");
                troom.RoomVoucherPaid = Convert.ToDecimal(paymentVoucher);
            }
            if (!string.IsNullOrEmpty(formCollection["hidpaymentCreditCard"]))
            {
                string paymentCreditCard = formCollection["hidpaymentCreditCard"].Replace(",", "");
                troom.RoomCreditPaid = Convert.ToDecimal(paymentCreditCard);
            }
            troom.RoomOutDate = Convert.ToDateTime(string.Format("{0:dd-MMM-yyyy} {1:HH:mm}", Trans.TransDate, DateTime.Now));
            troom.ModifiedBy = User.Identity.Name;
            troom.ModifiedDate = DateTime.Now;
            troom.DataStatus = EnumDataStatus.Updated.ToString();
            _tTransRoomRepository.Update(troom);

            //save journal
            SaveJournal(trans, totalHpp);

            string Message = string.Empty;
            bool Success = true;
            try
            {
                _tTransRoomRepository.DbContext.CommitTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Success;
            }
            catch (Exception ex)
            {
                Success = false;
                Message = ex.GetBaseException().Message;
                _tTransRoomRepository.DbContext.RollbackTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Failed;
            }
            var e = new
            {
                Success,
                Message,
                RoomStatus = EnumTransRoomStatus.Paid.ToString()
            };
            return Json(e, JsonRequestBehavior.AllowGet);
            // return View("Status");
        }

        private ActionResult SaveTransRoom(TTrans Trans, FormCollection formCollection)
        {
            _tTransRepository.DbContext.BeginTransaction();
            if (Trans == null)
            {
                Trans = new TTrans();
            }
            Trans.SetAssignedIdTo(formCollection["TransId"]);
            Trans.TransStatus = EnumTransactionStatus.Service.ToString();
            if (!string.IsNullOrEmpty(formCollection["TransDiscount"]))
            {
                string TransDiscount = formCollection["TransDiscount"].Replace(",", "");
                Trans.TransDiscount = Convert.ToDecimal(TransDiscount);
            }

            //Trans.TransDate = DateTime.Today;
            Trans.CreatedDate = DateTime.Now;
            Trans.CreatedBy = User.Identity.Name;
            Trans.DataStatus = Enums.EnumDataStatus.New.ToString();
            _tTransRepository.Save(Trans);

            TTransRoom troom = new TTransRoom();
            troom.SetAssignedIdTo(Trans.Id);
            if (!string.IsNullOrEmpty(formCollection["RoomInDate"]))
                troom.RoomInDate = Convert.ToDateTime(string.Format("{0:dd-MMM-yyyy} {1}", Trans.TransDate, formCollection["RoomInDate"]));
            if (!string.IsNullOrEmpty(formCollection["RoomOutDate"]))
                troom.RoomOutDate = Convert.ToDateTime(string.Format("{0:dd-MMM-yyyy} {1}", Trans.TransDate, formCollection["RoomOutDate"]));
            troom.RoomStatus = EnumTransRoomStatus.In.ToString();
            if (!string.IsNullOrEmpty(formCollection["RoomId"]))
            {
                troom.RoomId = _mRoomRepository.Get(formCollection["RoomId"]);
            }
            troom.CreatedDate = DateTime.Now;
            troom.CreatedBy = User.Identity.Name;
            troom.DataStatus = EnumDataStatus.New.ToString();
            _tTransRoomRepository.Save(troom);

            string Message = string.Empty;
            bool Success = true;
            try
            {
                _tTransRepository.DbContext.CommitTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Success;
            }
            catch (Exception ex)
            {
                Success = false;
                Message = ex.GetBaseException().Message;
                _tTransRepository.DbContext.RollbackTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Failed;
            }
            var e = new
            {
                Success,
                Message,
                RoomStatus = EnumTransRoomStatus.In.ToString()
            };
            return Json(e, JsonRequestBehavior.AllowGet);
            //return View("Status");
        }

        public virtual ActionResult GetJsonTransRoom(string roomId)
        {
            MRoom room = _mRoomRepository.Get(roomId);
            TTransRoom troom = _tTransRoomRepository.GetByRoom(room);
            DateTime? TransDate = DateTime.Today;
            if (troom == null)
            {
                troom = new TTransRoom();
                troom.SetAssignedIdTo(Guid.NewGuid().ToString());
                troom.RoomStatus = EnumTransRoomStatus.New.ToString();
                troom.RoomInDate = DateTime.Now;
            }
            else
            {
                TransDate = troom.TransId.TransDate;
            }
            var t = new
            {
                troom.Id,
                troom.RoomInDate,
                troom.RoomOutDate,
                troom.RoomStatus,
                troom.RoomBookDate,
                troom.RoomCommissionService,
                troom.RoomCommissionProduct,
                TransDate,
                IsVipRoom = room.RoomStatus.Equals(EnumRoomStatus.VIP.ToString()).ToString()
            };
            return Json(t, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GetJsonTrans(string transId)
        {
            TTrans trans = null;
            if (!string.IsNullOrEmpty(transId))
            {
                trans = _tTransRepository.Get(transId);
            }
            if (trans == null)
            {
                trans = new TTrans();
                trans.SetAssignedIdTo(Guid.NewGuid().ToString());

            }
            string CustomerName = string.Empty;
            if (!string.IsNullOrEmpty(trans.TransBy))
            {
                CustomerName = GetCustomerName(trans.TransBy);
            }
            var j = new
            {
                trans.Id,
                TransDiscount = trans.TransDiscount.HasValue ? trans.TransDiscount.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                trans.TransBy,
                CustomerName
            };
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult DetailItem(string detailId, string packetId, decimal? transDetQty)
        {
            DetailItemFormViewModel viewModel = DetailItemFormViewModel.Create(packetId, _mPacketItemCatRepository);
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult DetailItem(FormCollection formCollection, string detailId, string packetId, decimal? transDetQty)
        {
            string Message = "Data berhasil disimpan";
            bool Success = true;

            _tTransDetItemRepository.DbContext.BeginTransaction();
            TTransDet det = (TTransDet)Session[CONST_TRANSDET];
            _tTransDetRepository.Save(det);

            TTransDetItem detItem;

            //loop item cat packet
            MPacketItemCat packetItemCat;
            IList<MPacketItemCat> list = _mPacketItemCatRepository.GetByPacketId(packetId);

            bool isStockValid = true;
            MItem item;
            for (int i = 0; i < list.Count; i++)
            {
                packetItemCat = list[i];
                item = _mItemRepository.Get(formCollection["txtItemId_" + packetItemCat.ItemCatId.Id]);
                isStockValid = Helper.CommonHelper.CheckStock(Helper.CommonHelper.GetDefaultWarehouse(), item, transDetQty * packetItemCat.ItemCatQty);
                if (!isStockValid)
                {
                    Success = false;
                    Message = "Data tidak berhasi disimpan, kuantitas tidak mencukupi.";
                    break;
                }
                detItem = new TTransDetItem(det);
                detItem.SetAssignedIdTo(Guid.NewGuid().ToString());
                detItem.ItemCatId = packetItemCat.ItemCatId;
                detItem.ItemId = _mItemRepository.Get(formCollection["txtItemId_" + packetItemCat.ItemCatId.Id]);
                detItem.ItemQty = transDetQty * packetItemCat.ItemCatQty;
                detItem.CreatedBy = User.Identity.Name;
                detItem.CreatedDate = DateTime.Now;
                _tTransDetItemRepository.Save(detItem);
            }

            if (isStockValid)
            {
                try
                {
                    _tTransDetItemRepository.DbContext.CommitTransaction();
                    TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Success;
                }
                catch (Exception ex)
                {
                    Success = false;
                    Message = ex.Message;
                    _tTransDetItemRepository.DbContext.RollbackTransaction();
                    TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Failed;
                }
            }
            else
            {
                _tTransDetItemRepository.DbContext.RollbackTransaction();
            }

            var e = new
            {
                Success,
                Message
            };
            return Json(e, JsonRequestBehavior.AllowGet);
        }

        #region Detail Billing

        [Transaction]
        public virtual ActionResult ListBill(string sidx, string sord, int page, int rows, string TransId)
        {
            int totalRecords = 0;
            IList<TTransDet> transDets = new List<TTransDet>();
            if (!string.IsNullOrEmpty(TransId))
            {
                transDets = _tTransDetRepository.GetListByTransId(TransId, EnumTransactionStatus.Service);
            }

            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var result = (
                           from det in transDets
                           select new
                           {
                               i = det.Id.ToString(),
                               cell = new string[]
                                                     {
                                                         det.Id,
                                                         det.PacketId != null ? det.PacketId.Id : null,
                                                         det.PacketId != null ? det.PacketId.PacketName : null,
                                                         det.TransDetQty.HasValue ?  det.TransDetQty.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                                                        det.TransDetPrice.HasValue ?  det.TransDetPrice.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                                                        //det.TransDetDisc.HasValue ?   det.TransDetDisc.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                                                        det.TransDetTotal.HasValue ?   det.TransDetTotal.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                                                     det.EmployeeId != null ? det.EmployeeId.Id : null,
                                                      det.EmployeeId != null ? det.EmployeeId.PersonId.PersonName : null
                                                     }
                           });

            decimal? transDetTotal = transDets.Sum(det => det.TransDetTotal);
            decimal? transDetQty = transDets.Sum(det => det.TransDetQty);
            var userdata = new
            {
                PacketName = "Total",
                TransDetQty = transDetQty.Value.ToString(Helper.CommonHelper.NumberFormat),
                TransDetTotal = transDetTotal.Value.ToString(Helper.CommonHelper.NumberFormat)
            };
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = result.ToArray(),
                userdata
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InsertBill(TTransDet viewModel, FormCollection formCollection, string transId)
        {
            //_tTransDetRepository.DbContext.BeginTransaction();
            TTrans trans = _tTransRepository.Get(transId);

            UpdateNumericData(viewModel, formCollection);
            TTransDet transDetToInsert = new TTransDet(trans);
            MEmployee emp = _mEmployeeRepository.Get(formCollection["EmployeeId"]);
            MPacket packet = _mPacketRepository.Get(formCollection["PacketId"]);
            TransferFormValuesTo(transDetToInsert, viewModel);
            transDetToInsert.SetAssignedIdTo(Guid.NewGuid().ToString());
            transDetToInsert.PacketId = packet;
            transDetToInsert.EmployeeId = emp;

            transDetToInsert.TransDetCommissionService = CalculateCommission(emp, packet, transDetToInsert.TransDetTotal);

            transDetToInsert.CreatedDate = DateTime.Now;
            transDetToInsert.CreatedBy = User.Identity.Name;
            transDetToInsert.DataStatus = EnumDataStatus.New.ToString();
            //_tTransDetRepository.Save(transDetToInsert);
            //try
            //{
            //    _tTransDetRepository.DbContext.CommitTransaction();
            //    TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Success;
            //    return Content("success");
            //}
            //catch (Exception ex)
            //{
            //    _tTransDetRepository.DbContext.RollbackTransaction();
            //    TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Failed;
            //    return Content(ex.Message);
            //}

            //save temporary to session, then display transdetitem form
            Session[CONST_TRANSDET] = transDetToInsert;
            var e = new
            {
                transDetToInsert.Id,
                PacketId = transDetToInsert.PacketId.Id,
                transDetToInsert.TransDetQty
            };
            return Json(e, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// calculate commission, 
        /// if packet commission available, use it, 
        /// else use global commission
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="packet"></param>
        /// <param name="totalTrans"></param>
        /// <returns></returns>
        private decimal? CalculateCommission(MEmployee emp, MPacket packet, decimal? totalTrans)
        {
            decimal? commission = 0;
            string typeCommission = emp.EmployeeCommissionServiceType;
            decimal? commissionVal = emp.EmployeeCommissionServiceVal;
            MPacketComm packetComm = _mPacketCommRepository.GetByEmployeeAndPacket(emp, packet);
            if (packetComm != null)
            {
                typeCommission = packetComm.PacketCommType;
                commissionVal = packetComm.PacketCommVal;
            }
            if (typeCommission == EnumCommissionType.Percent.ToString())
            {
                commission = totalTrans * (commissionVal / 100);
            }
            else
            {
                commission = commissionVal;
            }
            return commission;
        }

        public ActionResult DeleteBill(TTransDet viewModel, FormCollection formCollection)
        {
            _tTransDetRepository.DbContext.BeginTransaction();
            TTransDet det = _tTransDetRepository.Get(viewModel.Id);

            if (det != null)
            {
                _tTransDetRepository.Delete(det);
            }
            try
            {
                _tTransDetRepository.DbContext.CommitTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Success;
                return Content("success");
            }
            catch (Exception ex)
            {
                _tTransDetRepository.DbContext.RollbackTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Failed;
                return Content(ex.Message);
            }
        }

        #endregion

        public ActionResult ListBilling()
        {
            return View();
        }

        public ActionResult ListBillingForEdit(string sidx, string sord, int page, int rows, string searchBy, string searchText)
        {
            int totalRecords = 0;
            var bills = _tTransRoomRepository.GetPagedTransRoomList(sidx, sord, page, rows, ref totalRecords, searchBy, searchText);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from troom in bills
                    select new
                    {
                        i = troom.Id,
                        cell = new string[] {
                            troom.Id,  
                            troom.Id,  
                            troom.TransId.TransFactur,
                        troom.TransId.TransDate.HasValue ?  troom.TransId.TransDate.Value.ToString(Helper.CommonHelper.DateFormat) : null,
                      GetCustomerName(troom.TransId.TransBy),
                          troom.RoomId.RoomName,
                        troom.RoomInDate.HasValue ?  troom.RoomInDate.Value.ToString(Helper.CommonHelper.TimeFormat) : null,
                        troom.RoomOutDate.HasValue ?  troom.RoomOutDate.Value.ToString(Helper.CommonHelper.TimeFormat) : null,
                        troom.TransId.TransSubTotal.HasValue ?  troom.TransId.TransSubTotal.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                        troom.TransId.TransDiscount.HasValue ?  troom.TransId.TransDiscount.Value.ToString(Helper.CommonHelper.NumberFormat) : null
                        }
                    }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteTransRoom(TTrans Trans)
        {
            _tTransRoomRepository.DbContext.BeginTransaction();

            TTrans t = _tTransRepository.Get(Trans.Id);
            if (t != null)
            {
                //delete trans room
                TTransRoom troom = _tTransRoomRepository.Get(t.Id);
                if (troom != null)
                {
                    _tTransRoomRepository.Delete(troom);
                }
                TTransDet det;
                for (int i = 0; i < t.TransDets.Count; i++)
                {
                    det = t.TransDets[i];
                    _tTransDetRepository.Delete(det);
                }
                _tTransRepository.Delete(t);
            }

            string Message = string.Empty;
            bool Success = true;
            try
            {
                _tTransRoomRepository.DbContext.CommitTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Success;
            }
            catch (Exception ex)
            {
                Success = false;
                Message = ex.GetBaseException().Message;
                _tTransRoomRepository.DbContext.RollbackTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Failed;
                return Content(ex.GetBaseException().Message);
            }
            var e = new
            {
                Success,
                Message,
                RoomStatus = EnumTransRoomStatus.New.ToString()
            };
            //return Json(e, JsonRequestBehavior.AllowGet);
            return Content("Data Billing berhasil dihapus.");
        }
    }
}
