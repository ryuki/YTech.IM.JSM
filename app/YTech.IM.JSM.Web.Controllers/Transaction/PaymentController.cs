using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using SharpArch.Core;
using SharpArch.Web.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;
using YTech.IM.JSM.Core.Transaction;

using YTech.IM.JSM.Core.Transaction.Inventory;
using YTech.IM.JSM.Core.Transaction.Payment;
using YTech.IM.JSM.Data.Repository;
using YTech.IM.JSM.Enums;
using YTech.IM.JSM.Web.Controllers.ViewModel;
using Microsoft.Reporting.WebForms;
using YTech.IM.JSM.Web.Controllers.ViewModel.Reports;

namespace YTech.IM.JSM.Web.Controllers.Transaction
{
    [HandleError]
    public class PaymentController : Controller
    {
        private readonly IMBrandRepository _mBrandRepository;
        private readonly IMSupplierRepository _mSupplierRepository;
        private readonly IMWarehouseRepository _mWarehouseRepository;
        private readonly IMItemRepository _mItemRepository;
        private readonly ITStockCardRepository _tStockCardRepository;
        private readonly ITStockItemRepository _tStockItemRepository;
        private readonly ITTransDetRepository _tTransDetRepository;
        private readonly ITTransRepository _tTransRepository;
        private readonly ITPaymentRepository _tPaymentRepository;
        private readonly ITPaymentDetRepository _tPaymentDetRepository;

        public PaymentController(IMBrandRepository mBrandRepository, IMSupplierRepository mSupplierRepository, IMWarehouseRepository mWarehouseRepository, IMItemRepository mItemRepository, ITStockCardRepository tStockCardRepository, ITStockItemRepository tStockItemRepository, ITTransDetRepository tTransDetRepository, ITTransRepository tTransRepository, ITPaymentRepository tPaymentRepository, ITPaymentDetRepository tPaymentDetRepository)
        {
            Check.Require(mBrandRepository != null, "mBrandRepository may not be null");
            Check.Require(mSupplierRepository != null, "mSupplierRepository may not be null");
            Check.Require(mWarehouseRepository != null, "mBrandRepository may not be null");
            Check.Require(mItemRepository != null, "mItemRepository may not be null");
            Check.Require(tStockCardRepository != null, "tStockCardRepository may not be null");
            Check.Require(tStockItemRepository != null, "tStockItemRepository may not be null");
            Check.Require(tTransDetRepository != null, "tTransDetRepository may not be null");
            Check.Require(tTransRepository != null, "tTransRepository may not be null");
            Check.Require(tPaymentRepository != null, "tPaymentRepository may not be null");
            Check.Require(tPaymentDetRepository != null, "tPaymentDetRepository may not be null");

            this._mBrandRepository = mBrandRepository;
            this._mSupplierRepository = mSupplierRepository;
            this._mWarehouseRepository = mWarehouseRepository;
            this._mItemRepository = mItemRepository;
            this._tStockCardRepository = tStockCardRepository;
            this._tStockItemRepository = tStockItemRepository;
            this._tTransDetRepository = tTransDetRepository;
            this._tTransRepository = tTransRepository;
            this._tPaymentRepository = tPaymentRepository;
            this._tPaymentDetRepository = tPaymentDetRepository;
        }

        [Transaction]
        public ActionResult Index(EnumPaymentType paymentType)
        {
            PaymentViewModel viewModel = PaymentViewModel.Create(paymentType, _tPaymentRepository, _tPaymentDetRepository);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(EnumPaymentType paymentType, PaymentViewModel viewModel, TPayment paymentVM, FormCollection formCollection)
        {
            if (formCollection["btnSave"] != null)
                return SavePayment(paymentType, paymentVM, formCollection, false);
            else if (formCollection["btnDelete"] != null)
                return SavePayment(paymentType, paymentVM, formCollection, true);

            return View();
        }

        private ActionResult SavePayment(EnumPaymentType paymentType, TPayment paymentVM, FormCollection formCollection, bool isDelete)
        {
            string Message = string.Empty;
            bool Success = true;
            try
            {
                _tPaymentRepository.DbContext.BeginTransaction();


                //check first
                TPayment payment = _tPaymentRepository.Get(formCollection["Id"]);
                if (!isDelete)
                {
                    bool isEdit = false;
                    if (payment == null)
                    {
                        isEdit = false;
                        //if 
                        payment = new TPayment();
                        payment.SetAssignedIdTo(Guid.NewGuid().ToString());
                        payment.CreatedDate = DateTime.Now;
                        payment.CreatedBy = User.Identity.Name;
                        payment.DataStatus = Enums.EnumDataStatus.New.ToString();
                    }
                    else
                    {
                        isEdit = true;
                        payment.ModifiedDate = DateTime.Now;
                        payment.ModifiedBy = User.Identity.Name;
                        payment.DataStatus = Enums.EnumDataStatus.Updated.ToString();
                    }
                    payment.PaymentDate = paymentVM.PaymentDate;
                    payment.PaymentDesc = paymentVM.PaymentDesc;
                    payment.PaymentStatus = paymentVM.PaymentStatus;
                    payment.PaymentType = paymentType.ToString();
                    SavePayment(payment, formCollection, isEdit);
                }
                else
                {
                    //if (tr != null)
                    //{
                    //    //do delete
                    //    DeleteTransaction(tr, addStock, calculateStock);
                    //}
                }


                _tPaymentRepository.DbContext.CommitTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Success;
                if (!isDelete)
                    Message = "Data berhasil disimpan.";
                else
                    Message = "Data berhasil dihapus.";
            }
            catch (Exception ex)
            {
                Success = false;
                if (!isDelete)
                    Message = "Data gagal disimpan.";
                else
                    Message = "Data gagal dihapus.";
                Message += "Error : " + ex.GetBaseException().Message;
                _tPaymentRepository.DbContext.RollbackTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Failed;
            }
            var e = new
            {
                Success,
                Message
            };
            return Json(e, JsonRequestBehavior.AllowGet);
        }

        private void SavePayment(TPayment payment, FormCollection formCollection, bool isEdit)
        {
            //if (isEdit)
            //{
            //    if (ListDeleteDetailTrans != null)
            //        if (ListDeleteDetailTrans.Count > 0)
            //            DeleteTransactionDetail(Trans, addStock, calculateStock, ListDeleteDetailTrans.ToArray());
            //}
            
            payment.PaymentDets.Clear();

            //save detail
            string splitter = ",";
            string[] selectedTransId = formCollection["SelectedTransId"].Split(splitter.ToCharArray());

            TPaymentDet detToInsert;
            TTrans trans;
            decimal total = 0;
            for (int i = 0; i < selectedTransId.Length; i++)
            {
                trans = _tTransRepository.Get(selectedTransId[i]);
                if (trans != null)
                {
                    detToInsert = new TPaymentDet(payment);
                    detToInsert.SetAssignedIdTo(Guid.NewGuid().ToString());
                    detToInsert.TransId = trans;
                    detToInsert.PaymentDetValue = trans.TransGrandTotal;
                    detToInsert.CreatedDate = DateTime.Now;
                    detToInsert.CreatedBy = User.Identity.Name;
                    detToInsert.DataStatus = Enums.EnumDataStatus.New.ToString();

                    payment.PaymentDets.Add(detToInsert);
                    total += detToInsert.PaymentDetValue.HasValue ? detToInsert.PaymentDetValue.Value : 0;
                }
            }
            payment.PaymentTotal = total;
            if (isEdit)
                _tPaymentRepository.Update(payment);
            else
                _tPaymentRepository.Save(payment);

            ////save journal
            //SaveJournal(payment);
        }

        //private void SaveJournal(TPayment payment)
        //{
        //    TJournal journal = new TJournal();

        //    journal.SetAssignedIdTo(Guid.NewGuid().ToString());
        //    //journal.CostCenterId = _mCostCenterRepository.Get(formCollection["Journal.CostCenterId"]);
        //    journal.CreatedDate = DateTime.Now;
        //    journal.CreatedBy = User.Identity.Name;
        //    journal.DataStatus = Enums.EnumDataStatus.New.ToString();
        //    journal.JournalDets.Clear();

        //    TJournalDet detToInsert;
        //    decimal total = 0;
        //    foreach (TJournalDet det in ListJournalDet)
        //    {
        //        detToInsert = new TJournalDet(journal);
        //        detToInsert.SetAssignedIdTo(Guid.NewGuid().ToString());
        //        detToInsert.AccountId = det.AccountId;

        //        if (journal.JournalType == EnumJournalType.CashIn.ToString())
        //        {
        //            detToInsert.JournalDetStatus = EnumJournalStatus.K.ToString();
        //        }
        //        else if (journal.JournalType == EnumJournalType.CashOut.ToString())
        //        {
        //            detToInsert.JournalDetStatus = EnumJournalStatus.D.ToString();
        //        }
        //        else if (journal.JournalType == EnumJournalType.GeneralLedger.ToString())
        //        {
        //            detToInsert.JournalDetStatus = det.JournalDetStatus;
        //        }

        //        detToInsert.JournalDetAmmount = det.JournalDetAmmount;
        //        detToInsert.JournalDetNo = det.JournalDetNo;
        //        detToInsert.JournalDetDesc = det.JournalDetDesc;
        //        detToInsert.CreatedBy = User.Identity.Name;
        //        detToInsert.CreatedDate = DateTime.Now;
        //        detToInsert.DataStatus = Enums.EnumDataStatus.New.ToString();
        //        journal.JournalDets.Add(detToInsert);

        //        total += det.JournalDetAmmount.Value;
        //    }
        //    _tJournalRepository.Save(journal);
        //}
    }
}
