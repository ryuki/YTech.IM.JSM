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
    [HandleError]
    public partial class InventoryController : Controller
    {
        private readonly ITTransRepository _tTransRepository;
        private readonly IMWarehouseRepository _mWarehouseRepository;
        private readonly IMSupplierRepository _mSupplierRepository;
        private readonly IMItemRepository _mItemRepository;
        private readonly ITStockCardRepository _tStockCardRepository;
        private readonly ITStockItemRepository _tStockItemRepository;
        private readonly ITTransRefRepository _tTransRefRepository;
        private readonly ITStockRepository _tStockRepository;
        private readonly ITStockRefRepository _tStockRefRepository;
        private readonly IMCustomerRepository _mCustomerRepository;
        private readonly IMEmployeeRepository _mEmployeeRepository;
        private readonly ITTransDetRepository _tTransDetRepository;
        //private readonly ITTransDetItemRepository _tTransDetItemRepository;
        //private readonly IMAccountRefRepository _mAccountRefRepository;
        //private readonly ITJournalRepository _tJournalRepository;
        //private readonly ITJournalDetRepository _tJournalDetRepository;
        //private readonly IMAccountRepository _mAccountRepository;
        //private readonly ITJournalRefRepository _tJournalRefRepository;

        public InventoryController(ITTransRepository tTransRepository, IMWarehouseRepository mWarehouseRepository, IMSupplierRepository mSupplierRepository, IMItemRepository mItemRepository, ITStockCardRepository tStockCardRepository, ITStockItemRepository tStockItemRepository, ITTransRefRepository tTransRefRepository, ITStockRepository tStockRepository, ITStockRefRepository tStockRefRepository, IMCustomerRepository mCustomerRepository, IMEmployeeRepository mEmployeeRepository, ITTransDetRepository tTransDetRepository)
        {
            Check.Require(tTransRepository != null, "tTransRepository may not be null");
            Check.Require(mWarehouseRepository != null, "mWarehouseRepository may not be null");
            Check.Require(mSupplierRepository != null, "mSupplierRepository may not be null");
            Check.Require(mItemRepository != null, "mItemRepository may not be null");
            Check.Require(tStockCardRepository != null, "tStockCardRepository may not be null");
            Check.Require(tStockItemRepository != null, "tStockItemRepository may not be null");
            Check.Require(tTransRefRepository != null, "tTransRefRepository may not be null");
            Check.Require(tStockRepository != null, "tStockRepository may not be null");
            Check.Require(tStockRefRepository != null, "tStockRefRepository may not be null");
            Check.Require(mCustomerRepository != null, "mCustomerRepository may not be null");
            Check.Require(mEmployeeRepository != null, "mEmployeeRepository may not be null");
            Check.Require(tTransDetRepository != null, "tTransDetRepository may not be null");

            this._tTransRepository = tTransRepository;
            this._mWarehouseRepository = mWarehouseRepository;
            this._mSupplierRepository = mSupplierRepository;
            this._mItemRepository = mItemRepository;
            this._tStockCardRepository = tStockCardRepository;
            this._tStockItemRepository = tStockItemRepository;
            this._tTransRefRepository = tTransRefRepository;
            this._tStockRepository = tStockRepository;
            this._tStockRefRepository = tStockRefRepository;
            this._mCustomerRepository = mCustomerRepository;
            this._mEmployeeRepository = mEmployeeRepository;
            this._tTransDetRepository = tTransDetRepository;

        }

        public ActionResult Index()
        {
            TransactionFormViewModel viewModel = SetViewModelByStatus(EnumTransactionStatus.PurchaseOrder);

            return View(viewModel);
        }

        private TransactionFormViewModel SetViewModelByStatus(EnumTransactionStatus enumTransactionStatus)
        {
            TransactionFormViewModel viewModel = TransactionFormViewModel.Create(enumTransactionStatus, _tTransRepository, _mWarehouseRepository, _mSupplierRepository, _mCustomerRepository);

            ViewData["CurrentItem"] = viewModel.Title;
            //ViewData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.NotSaved;

            ListDetTrans = new List<TransDetViewModel>();
            ListDeleteDetailTrans = new ArrayList();

            return viewModel;
        }

        public ActionResult Purchase()
        {
            TransactionFormViewModel viewModel = SetViewModelByStatus(EnumTransactionStatus.Purchase);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase(TTrans Trans, FormCollection formCollection)
        {
            return SaveTransactionInterface(Trans, formCollection);
        }

        private ActionResult SaveTransactionRef(TTrans Trans, FormCollection formCollection)
        {
            _tTransRepository.DbContext.BeginTransaction();
            if (Trans == null)
            {
                Trans = new TTrans();
            }
            Trans.SetAssignedIdTo(formCollection["Trans.Id"]);
            Trans.CreatedDate = DateTime.Now;
            Trans.CreatedBy = User.Identity.Name;
            Trans.DataStatus = Enums.EnumDataStatus.New.ToString();
            Trans.TransSubTotal = ListTransRef.Sum(x => x.TransIdRef.TransSubTotal);
            _tTransRepository.Save(Trans);
            _tTransRepository.DbContext.CommitTransaction();

            _tTransRefRepository.DbContext.BeginTransaction();
            TTransRef detToInsert;
            foreach (TTransRef det in ListTransRef)
            {
                detToInsert = new TTransRef();
                detToInsert.SetAssignedIdTo(det.Id);
                detToInsert.TransId = Trans;
                detToInsert.TransIdRef = det.TransIdRef;
                detToInsert.TransRefDesc = det.TransRefDesc;
                detToInsert.TransRefStatus = det.TransRefStatus;

                detToInsert.CreatedBy = User.Identity.Name;
                detToInsert.CreatedDate = DateTime.Now;
                detToInsert.DataStatus = EnumDataStatus.New.ToString();
                _tTransRefRepository.Save(detToInsert);
            }
            try
            {
                _tTransRefRepository.DbContext.CommitTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Success;
            }
            catch (Exception)
            {
                _tTransRefRepository.DbContext.RollbackTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Failed;
            }
            if (!Trans.TransStatus.Equals(EnumTransactionStatus.PurchaseOrder.ToString()))
            {
                return RedirectToAction(Trans.TransStatus);
            }
            return RedirectToAction("Index");
        }

        public ActionResult ReturPurchase()
        {
            TransactionFormViewModel viewModel = SetViewModelByStatus(EnumTransactionStatus.ReturPurchase);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReturPurchase(TTrans Trans, FormCollection formCollection)
        {
            return SaveTransactionInterface(Trans, formCollection);
        }

        public ActionResult Using()
        {
            TransactionFormViewModel viewModel = SetViewModelByStatus(EnumTransactionStatus.Using);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Using(TTrans Trans, FormCollection formCollection)
        {
            return SaveTransactionInterface(Trans, formCollection);
        }

        public ActionResult Received()
        {
            TransactionFormViewModel viewModel = SetViewModelByStatus(EnumTransactionStatus.Received);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Received(TTrans Trans, FormCollection formCollection)
        {
            return SaveTransactionInterface(Trans, formCollection);
        }

        public ActionResult Mutation()
        {
            TransactionFormViewModel viewModel = SetViewModelByStatus(EnumTransactionStatus.Mutation);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Mutation(TTrans Trans, FormCollection formCollection)
        {
            return SaveTransactionInterface(Trans, formCollection);
        }

        [Transaction]
        public ActionResult Adjusment()
        {
            TransactionFormViewModel viewModel = SetViewModelByStatus(EnumTransactionStatus.Adjusment);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Adjusment(TTrans Trans, FormCollection formCollection)
        {
            return SaveTransactionInterface(Trans, formCollection);
        }

        private ArrayList ListDeleteDetailTrans
        {
            get
            {
                if (Session["ListDeleteDetailTrans"] == null)
                {
                    Session["ListDeleteDetailTrans"] = new ArrayList();
                }
                return Session["ListDeleteDetailTrans"] as ArrayList;
            }
            set
            {
                Session["ListDeleteDetailTrans"] = value;
            }
        }

        private List<TransDetViewModel> ListDetTrans
        {
            get
            {
                if (Session["listDetTrans"] == null)
                {
                    Session["listDetTrans"] = new List<TransDetViewModel>();
                }
                return Session["listDetTrans"] as List<TransDetViewModel>;
            }
            set
            {
                Session["listDetTrans"] = value;
            }
        }

        private List<TTransRef> ListTransRef
        {
            get
            {
                if (Session["ListTransRef"] == null)
                {
                    Session["ListTransRef"] = new List<TTransRef>();
                }
                return Session["ListTransRef"] as List<TTransRef>;
            }
            set
            {
                Session["ListTransRef"] = value;
            }
        }

        [Transaction]
        public virtual ActionResult List(string sidx, string sord, int page, int rows, string usePrice)
        {
            int totalRecords = 0;
            var transDets = ListDetTrans;
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var result = (
                           from det in transDets
                           select new
                                      {
                                          i = det.TransDet.Id.ToString(),
                                          cell = new string[]
                                                     {
                                                         det.TransDet.Id,
                                                         det.TransDet.ItemId != null ? det.TransDet.ItemId.Id : null,
                                                         det.TransDet.ItemId != null ? det.TransDet.ItemId.ItemName : null,
                                                         det.TransDet.TransDetQty.HasValue ?  det.TransDet.TransDetQty.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                                                        det.TransDet.TransDetPrice.HasValue ?  det.TransDet.TransDetPrice.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                                                        det.TransDet.TransDetDisc.HasValue ?   det.TransDet.TransDetDisc.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                                                        det.TransDet.TransDetTotal.HasValue ?   det.TransDet.TransDetTotal.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                                                         det.TransDet.TransDetDesc
                                                     }
                                      });

            decimal? transDetTotal = transDets.Sum(det => det.TransDet.TransDetTotal);
            decimal? transDetQty = transDets.Sum(det => det.TransDet.TransDetQty);
            var userdata = new
            {
                ItemName = "Total",
                TransDetQty = transDetQty.Value.ToString(Helper.CommonHelper.NumberFormat),
                TransDetTotal = transDetTotal.Value.ToString(Helper.CommonHelper.NumberFormat)
            };
            if (usePrice.Equals(false.ToString()))
            {
                result = (
                           from det in transDets
                           select new
                           {
                               i = det.TransDet.Id.ToString(),
                               cell = new string[]
                                                     {
                                                         det.TransDet.Id,
                                                         det.TransDet.ItemId != null ? det.TransDet.ItemId.Id : null,
                                                         det.TransDet.ItemId != null ? det.TransDet.ItemId.ItemName : null,
                                                       det.TransDet.TransDetQty.HasValue ?    det.TransDet.TransDetQty.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                                                         det.TransDet.TransDetDesc
                                                     }
                           });
            }

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

        [Transaction]
        public virtual ActionResult GetListTransRef(string sidx, string sord, int page, int rows)
        {
            int totalRecords = 0;
            var transRefs = ListTransRef;
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from det in transRefs
                    select new
                    {
                        i = det.Id.ToString(),
                        cell = new string[] {
                             det.TransIdRef.Id,
                             det.TransIdRef.Id,
                            det.TransIdRef.TransFactur, 
                            det.TransIdRef.TransDate.HasValue ? det.TransIdRef.TransDate.Value.ToString(Helper.CommonHelper.DateFormat) : null,
                           det.TransIdRef.TransSubTotal.HasValue ?  det.TransIdRef.TransSubTotal.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                            det.TransIdRef.TransDesc
                        }
                    }).ToArray()
                //userdata: {price:1240.00} 
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(TTransDet viewModel, FormCollection formCollection)
        {
            TTransDet transDetToInsert = new TTransDet();
            TransferFormValuesTo(transDetToInsert, viewModel);
            transDetToInsert.SetAssignedIdTo(viewModel.Id);
            transDetToInsert.CreatedDate = DateTime.Now;
            transDetToInsert.CreatedBy = User.Identity.Name;
            transDetToInsert.DataStatus = EnumDataStatus.New.ToString();

            TransDetViewModel detViewModel = new TransDetViewModel();
            detViewModel.TransDet = transDetToInsert;
            detViewModel.IsNew = false;
            ListDetTrans.Add(detViewModel);
            return Content("success");
        }

        static Predicate<TransDetViewModel> ByTransDetId(string detId)
        {
            return delegate(TransDetViewModel detViewModel)
            {
                return detViewModel.TransDet.Id == detId;
            };
        }


        public ActionResult Delete(TTransDet viewModel, FormCollection formCollection)
        {
            //remove use native predicate function from list, awesome, no need foreach anymore
            ListDetTrans.Remove(ListDetTrans.Find(ByTransDetId(viewModel.Id)));


            if (ListDeleteDetailTrans == null)
            {
                ListDeleteDetailTrans = new ArrayList();
            }
            ListDeleteDetailTrans.Add(viewModel.Id);
            return Content("success");
        }

        public ActionResult DeleteTransRef(TTransRef viewModel, FormCollection formCollection)
        {
            ListTransRef.Remove(viewModel);
            return Content("success");
        }

        public ActionResult Insert(TTransDet viewModel, FormCollection formCollection, bool IsAddStock, string WarehouseId)
        {
            //format numeric 
            UpdateNumericData(viewModel, formCollection);
            //
            MItem item = _mItemRepository.Get(formCollection["ItemId"]);
            //check stock is enough or not if no add stock 
            //return Content(IsAddStock.ToString());
            if (!IsAddStock)
            {
                MWarehouse warehouse = _mWarehouseRepository.Get(WarehouseId);
                bool isStockValid = Helper.CommonHelper.CheckStock(warehouse, item, viewModel.TransDetQty);
                if (!isStockValid)
                {
                    return Content("Kuantitas barang tidak cukup");
                }
            }

            TTransDet transDetToInsert = new TTransDet();
            TransferFormValuesTo(transDetToInsert, viewModel);
            transDetToInsert.SetAssignedIdTo(Guid.NewGuid().ToString());
            transDetToInsert.ItemId = item;
            transDetToInsert.SetAssignedIdTo(viewModel.Id);
            transDetToInsert.CreatedDate = DateTime.Now;
            transDetToInsert.CreatedBy = User.Identity.Name;
            transDetToInsert.DataStatus = EnumDataStatus.New.ToString();

            TransDetViewModel detViewModel = new TransDetViewModel();
            detViewModel.TransDet = transDetToInsert;
            detViewModel.IsNew = true;
            ListDetTrans.Add(detViewModel);
            return Content("Detail transaksi berhasil disimpan.");
        }

        private static void UpdateNumericData(TTransDet viewModel, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["TransDetQty"]))
            {
                string wide = formCollection["TransDetQty"].Replace(",", "");
                decimal? qty = Convert.ToDecimal(wide);
                viewModel.TransDetQty = qty;
            }
            else
            {
                viewModel.TransDetQty = null;
            }
            if (!string.IsNullOrEmpty(formCollection["TransDetPrice"]))
            {
                string wide = formCollection["TransDetPrice"].Replace(",", "");
                decimal? price = Convert.ToDecimal(wide);
                viewModel.TransDetPrice = price;
            }
            else
            {
                viewModel.TransDetPrice = null;
            }
            if (!string.IsNullOrEmpty(formCollection["TransDetDisc"]))
            {
                string wide = formCollection["TransDetDisc"].Replace(",", "");
                decimal? disc = Convert.ToDecimal(wide);
                viewModel.TransDetDisc = disc;
            }
            else
            {
                viewModel.TransDetDisc = null;
            }
            if (!string.IsNullOrEmpty(formCollection["TransDetTotal"]))
            {
                string wide = formCollection["TransDetTotal"].Replace(",", "");
                decimal? total = Convert.ToDecimal(wide);
                viewModel.TransDetTotal = total;
            }
            else
            {
                viewModel.TransDetTotal = null;
            }
        }

        public ActionResult InsertTransRef(TTransRef viewModel, FormCollection formCollection)
        {
            TTransRef transDetToInsert = new TTransRef();

            transDetToInsert.SetAssignedIdTo(Guid.NewGuid().ToString());
            transDetToInsert.TransIdRef = _tTransRepository.Get(formCollection["TransIdRef"]);
            //transDetToInsert.TransId = _tTransRepository.Get(formCollection["TransId"]);
            transDetToInsert.CreatedDate = DateTime.Now;
            transDetToInsert.CreatedBy = User.Identity.Name;
            transDetToInsert.DataStatus = EnumDataStatus.New.ToString();

            ListTransRef.Add(transDetToInsert);
            return Content("success");
        }

        private void TransferFormValuesTo(TTransDet transDet, TTransDet viewModel)
        {
            transDet.TransDetNo = ListDetTrans.Count + 1;
            transDet.TransDetQty = viewModel.TransDetQty;
            transDet.TransDetPrice = viewModel.TransDetPrice;
            transDet.TransDetDisc = viewModel.TransDetDisc;
            transDet.TransDetTotal = viewModel.TransDetTotal;
            transDet.TransDetDesc = viewModel.TransDetDesc;
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(TTrans Trans, FormCollection formCollection)
        {
            return SaveTransactionInterface(Trans, formCollection);
        }

        private ActionResult SaveTransactionInterface(TTrans Trans, FormCollection formCollection)
        {
            if (formCollection["btnSave"] != null)
                return SaveTransaction(Trans, formCollection, false);
            else if (formCollection["btnDelete"] != null)
                return SaveTransaction(Trans, formCollection, true);
            else if (formCollection["btnPrint"] != null)
            {
                SetReportDataForPrint(formCollection["Trans.Id"]);

                var e = new
                {
                    Success = false,
                    Message = "redirect"
                };
                return Json(e, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        private ActionResult SaveTransaction(TTrans Trans, FormCollection formCollection, bool isDelete)
        {
            string Message = string.Empty;
            bool Success = true;
            try
            {
                _tTransRepository.DbContext.BeginTransaction();

                //get stock add or calculated
                bool addStock = true;
                bool calculateStock = false;
                EnumTransactionStatus status = (EnumTransactionStatus)Enum.Parse(typeof(EnumTransactionStatus), Trans.TransStatus);
                switch (status)
                {
                    case EnumTransactionStatus.Purchase:
                        addStock = true;
                        calculateStock = true;
                        break;
                    case EnumTransactionStatus.Received:
                        addStock = true;
                        calculateStock = true;
                        break;
                    case EnumTransactionStatus.Adjusment:
                        addStock = true;
                        calculateStock = true;
                        break;
                    case EnumTransactionStatus.ReturPurchase:
                        addStock = false;
                        calculateStock = true;
                        break;
                    case EnumTransactionStatus.ReturSales:
                        addStock = true;
                        calculateStock = true;
                        break;
                    case EnumTransactionStatus.Sales:
                        addStock = false;
                        calculateStock = true;
                        break;
                    case EnumTransactionStatus.Using:
                        addStock = false;
                        calculateStock = true;
                        break;
                    case EnumTransactionStatus.Mutation:
                        addStock = false;
                        calculateStock = true;
                        break;
                }

                //check first
                TTrans tr = _tTransRepository.Get(formCollection["Trans.Id"]);
                if (!isDelete)
                {
                    bool isEdit = false;
                    if (tr == null)
                    {
                        isEdit = false;
                        //if 
                        tr = new TTrans();
                        tr.SetAssignedIdTo(formCollection["Trans.Id"]);
                        tr.CreatedDate = DateTime.Now;
                        tr.CreatedBy = User.Identity.Name;
                        tr.DataStatus = Enums.EnumDataStatus.New.ToString();
                    }
                    else
                    {
                        isEdit = true;
                        tr.ModifiedDate = DateTime.Now;
                        tr.ModifiedBy = User.Identity.Name;
                        tr.DataStatus = Enums.EnumDataStatus.Updated.ToString();
                    }
                    tr.WarehouseId = _mWarehouseRepository.Get(formCollection["Trans.WarehouseId"]);
                    if (!string.IsNullOrEmpty(formCollection["Trans.WarehouseIdTo"]))
                    {
                        tr.WarehouseIdTo = _mWarehouseRepository.Get(formCollection["Trans.WarehouseIdTo"]);
                    }
                    tr.TransStatus = Trans.TransStatus;
                    tr.TransFactur = Trans.TransFactur;
                    tr.TransDate = Trans.TransDate;
                    tr.TransDueDate = Trans.TransDueDate;
                    tr.TransBy = Trans.TransBy;
                    tr.TransPaymentMethod = Trans.TransPaymentMethod;
                    SaveTransaction(tr, formCollection, addStock, calculateStock, isEdit);
                }
                else
                {
                    if (tr != null)
                    {
                        //do delete
                        DeleteTransaction(tr, addStock, calculateStock);
                    }
                }


                _tTransRepository.DbContext.CommitTransaction();
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
                _tTransRepository.DbContext.RollbackTransaction();
                TempData[EnumCommonViewData.SaveState.ToString()] = EnumSaveState.Failed;
            }
            var e = new
            {
                Success,
                Message
            };
            return Json(e, JsonRequestBehavior.AllowGet);
        }

        private void DeleteTransaction(TTrans tr, bool addStock, bool calculateStock)
        {
            IList<TTransDet> listDet = tr.TransDets;

            IEnumerable<string> listDetailId = from det in listDet
                                               select det.Id;

            DeleteTransactionDetail(tr, addStock, calculateStock, listDetailId.ToArray());

            _tTransRepository.Delete(tr);
        }

        private void SaveTransaction(TTrans Trans, FormCollection formCollection, bool addStock, bool calculateStock, bool isEdit)
        {
            if (isEdit)
            {
                if (ListDeleteDetailTrans != null)
                    if (ListDeleteDetailTrans.Count > 0)
                        DeleteTransactionDetail(Trans, addStock, calculateStock, ListDeleteDetailTrans.ToArray());
            }


            Trans.TransDets.Clear();

            //save stock card

            TTransDet detToInsert;
            decimal total = 0;
            IList<TransDetViewModel> listDetToSave = new List<TransDetViewModel>();
            TransDetViewModel detToSave = new TransDetViewModel();

            foreach (TransDetViewModel det in ListDetTrans)
            {
                detToSave = new TransDetViewModel();
                detToSave.IsNew = det.IsNew;

                if (det.IsNew)
                {
                    detToInsert = new TTransDet(Trans);
                    detToInsert.SetAssignedIdTo(Guid.NewGuid().ToString());
                    detToInsert.ItemId = det.TransDet.ItemId;
                    detToInsert.ItemUomId = det.TransDet.ItemUomId;
                    detToInsert.TransDetQty = det.TransDet.TransDetQty;
                    detToInsert.TransDetPrice = det.TransDet.TransDetPrice;
                    detToInsert.TransDetDisc = det.TransDet.TransDetDisc;
                    detToInsert.TransDetTotal = det.TransDet.TransDetTotal;
                    detToInsert.CreatedBy = User.Identity.Name;
                    detToInsert.CreatedDate = DateTime.Now;
                    detToInsert.DataStatus = Enums.EnumDataStatus.New.ToString();
                    Trans.TransDets.Add(detToInsert);

                    detToSave.TransDet = detToInsert;
                }
                else
                {
                    detToSave.TransDet = det.TransDet;
                }
                listDetToSave.Add(detToSave);
                total += det.TransDet.TransDetTotal.HasValue ? det.TransDet.TransDetTotal.Value : 0;

            }
            Trans.TransSubTotal = total;
            if (isEdit)
                _tTransRepository.Update(Trans);
            else
                _tTransRepository.Save(Trans);
            //_tTransRepository.DbContext.CommitTransaction();

            //_tTransRepository.DbContext.BeginTransaction();
            if (calculateStock)
            {
                decimal totalHpp = 0;
                foreach (TransDetViewModel det in listDetToSave)
                {
                    if (det.IsNew)
                    {
                        //save stock
                        if (Trans.TransStatus.Equals(EnumTransactionStatus.Mutation.ToString()))
                        {
                            SaveStockItem(Trans.TransDate, Trans.TransDesc, det.TransDet.ItemId, det.TransDet.TransDetQty, false, Trans.WarehouseId);
                            SaveStockItem(Trans.TransDate, Trans.TransDesc, det.TransDet.ItemId, det.TransDet.TransDetQty, true, Trans.WarehouseIdTo);

                            //still to do, for mutation, price of stock must recalculate per stock, 
                            //sum hpp for each stock for stock out
                            totalHpp += UpdateStock(Trans.TransDate, Trans.TransDesc, Trans.TransStatus, det.TransDet.ItemId, det.TransDet.TransDetPrice, det.TransDet.TransDetQty, det.TransDet, false, Trans.WarehouseId);
                            UpdateStock(Trans.TransDate, Trans.TransDesc, Trans.TransStatus, det.TransDet.ItemId, det.TransDet.TransDetPrice, det.TransDet.TransDetQty, det.TransDet, true, Trans.WarehouseIdTo);
                        }
                        else
                        {
                            SaveStockItem(Trans.TransDate, Trans.TransDesc, det.TransDet.ItemId, det.TransDet.TransDetQty, addStock, Trans.WarehouseId);

                            //sum hpp for each stock
                            totalHpp += UpdateStock(Trans.TransDate, Trans.TransDesc, Trans.TransStatus, det.TransDet.ItemId, det.TransDet.TransDetPrice, det.TransDet.TransDetQty, det.TransDet, addStock, Trans.WarehouseId);
                        }
                    }
                    else
                    {
                        //get HPP from existing detail
                        // totalHpp +=
                    }
                }
                ////save journal
                //SaveJournal(Trans, totalHpp);
            }
        }

        private void DeleteTransactionDetail(TTrans Trans, bool addStock, bool calculateStock, object[] detailIdList)
        {
            if (calculateStock)
            {
                IList<TTransDet> listDet = _tTransDetRepository.GetListById(detailIdList);
                foreach (TTransDet det in listDet)
                {
                    //undo stock
                    if (Trans.TransStatus.Equals(EnumTransactionStatus.Mutation.ToString()))
                    {
                        SaveStockItem(Trans.TransDate, "Hapus Detail" + Trans.TransFactur, det.ItemId, det.TransDetQty, !false, Trans.WarehouseId);
                        SaveStockItem(Trans.TransDate, "Hapus Detail" + Trans.TransFactur, det.ItemId, det.TransDetQty, !true, Trans.WarehouseIdTo);

                        ////still to do, for mutation, price of stock must recalculate per stock, 
                        ////sum hpp for each stock for stock out
                        //UpdateStock(Trans.TransDate, Trans.TransDesc, Trans.TransStatus, det.ItemId, det.TransDetPrice, det.TransDetQty, det, false, Trans.WarehouseId);
                        //UpdateStock(Trans.TransDate, Trans.TransDesc, Trans.TransStatus, det.ItemId, det.TransDetPrice, det.TransDetQty, det, true, Trans.WarehouseIdTo);
                    }
                    else
                    {
                        SaveStockItem(Trans.TransDate, "Hapus Detail" + Trans.TransFactur, det.ItemId, det.TransDetQty, !addStock, Trans.WarehouseId);

                        if (addStock)
                        {
                            //set stock as out stock
                            UpdateStock(Trans.TransDate, "Hapus Detail" + Trans.TransFactur, Trans.TransStatus, det.ItemId, det.TransDetPrice, det.TransDetQty, det, false, Trans.WarehouseId);
                        }
                    }
                }

                //delete stock ref for not add stock
                if (!addStock)
                {
                    _tStockRefRepository.DeleteByTransDetId(detailIdList);
                }
            }

            //delete detail
            _tTransDetRepository.DeleteById(detailIdList);
        }

        public ActionResult Budgeting()
        {
            TransactionFormViewModel viewModel = SetViewModelByStatus(EnumTransactionStatus.Budgeting);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Budgeting(TTrans Trans, FormCollection formCollection)
        {
            return SaveTransactionInterface(Trans, formCollection);
        }

        public ActionResult Sales()
        {
            TransactionFormViewModel viewModel = SetViewModelByStatus(EnumTransactionStatus.Sales);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sales(TTrans Trans, FormCollection formCollection)
        {
            return SaveTransactionInterface(Trans, formCollection);
        }

        public ActionResult ReturSales()
        {
            TransactionFormViewModel viewModel = SetViewModelByStatus(EnumTransactionStatus.ReturSales);

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReturSales(TTrans Trans, FormCollection formCollection)
        {
            return SaveTransactionInterface(Trans, formCollection);
        }

        [Transaction]
        public virtual ActionResult GetListTrans(string transStatus, string warehouseId, string transBy)
        {
            IList<TTrans> transes;
            //if (!string.IsNullOrEmpty(transStatus))
            MWarehouse warehouse = _mWarehouseRepository.Get(warehouseId);
            {
                transes = _tTransRepository.GetByWarehouseStatusTransBy(warehouse, transStatus, transBy);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}:{1}", string.Empty, "-Pilih Faktur-");
            foreach (TTrans trans in transes)
            {
                sb.AppendFormat(";{0}:{1}", trans.Id, trans.TransFactur);
            }
            return Content(sb.ToString());
        }

        [Transaction]
        public ActionResult ListTransaction()
        {
            return View();
        }

        [Transaction]
        public ActionResult ListSearchTrans(string sidx, string sord, int page, int rows, string searchBy, string searchText, string transStatus)
        {
            int totalRecords = 0;
            var transList = _tTransRepository.GetPagedTransList(sidx, sord, page, rows, ref totalRecords, searchBy, searchText, transStatus);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from trans in transList
                    select new
                    {
                        i = trans.Id.ToString(),
                        cell = new string[] {
                            trans.Id, 
                            trans.TransFactur, 
                            trans.TransDate.HasValue ? trans.TransDate.Value.ToString(Helper.CommonHelper.DateFormat): null,
                            trans.TransDesc,
                            trans.TransGrandTotal.HasValue ? trans.TransGrandTotal.Value.ToString(Helper.CommonHelper.NumberFormat): null
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public ActionResult GetJsonTrans(string transId)
        {
            TTrans trans = _tTransRepository.Get(transId);
            ConvertListDet(trans.TransDets.ToList());

            //set new array list for delete id 
            //to prevent deleted id before
            ListDeleteDetailTrans = new ArrayList();

            var t = new
            {
                TransId = trans.Id,
                trans.TransDate,
                trans.TransDueDate,
                trans.TransFactur,
                WarehouseId = trans.WarehouseId != null ? trans.WarehouseId.Id : null,
                trans.TransPaymentMethod,
                trans.TransBy,
                WarehouseIdTo = trans.WarehouseIdTo != null ? trans.WarehouseIdTo.Id : null

            };
            return Json(t, JsonRequestBehavior.AllowGet);
        }

        private void ConvertListDet(List<TTransDet> listDet)
        {
            TTransDet det;
            TransDetViewModel detViewModel;
            ListDetTrans = new List<TransDetViewModel>();
            for (int i = 0; i < listDet.Count; i++)
            {
                det = listDet[i] as TTransDet;
                detViewModel = new TransDetViewModel();
                detViewModel.TransDet = det;
                detViewModel.IsNew = false;
                ListDetTrans.Add(detViewModel);
            }
        }



        public ActionResult PrintFactur(string TransId)
        {
            bool Success = true;
            string Message = string.Empty;
            try
            {
                SetReportDataForPrint(TransId);
                Success = true;
            }
            catch (Exception ex)
            {
                Success = false;
                Message = ex.GetBaseException().Message;
            }
            var e = new
            {
                Success,
                Message
            };

            return Json(e, JsonRequestBehavior.AllowGet);
        }

        private void SetReportDataForPrint(string TransId)
        {
            ReportDataSource[] repCol = new ReportDataSource[1];
            TTrans trans = _tTransRepository.Get(TransId);

            IList<TTransDet> listDetail = trans.TransDets;
            var listDet = from det in listDetail
                          select new
                          {
                              ItemId = det.ItemId != null ? det.ItemId.Id : null,
                              ItemName = det.ItemId != null ? det.ItemId.ItemName : null,
                              det.TransDetPrice,
                              det.TransDetQty,
                              det.TransDetDisc,
                              det.TransDetNo,
                              det.TransDetTotal,
                              CustomerName = Helper.CommonHelper.GetCustomerName(_mCustomerRepository, trans.TransBy),
                              trans.TransFactur,
                              trans.TransDate,
                              trans.TransBy,
                              trans.TransGrandTotal
                          }
      ;
            ReportDataSource reportDataSource = new ReportDataSource("TransTotalViewModel", listDet.ToList());
            repCol[0] = reportDataSource;

            Session["ReportData"] = repCol;
        }
    }
}
