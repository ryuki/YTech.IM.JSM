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
using YTech.IM.JSM.Data.Repository;
using YTech.IM.JSM.Enums;
using YTech.IM.JSM.Web.Controllers.ViewModel;
using Microsoft.Reporting.WebForms;
using YTech.IM.JSM.Web.Controllers.ViewModel.Reports;

namespace YTech.IM.JSM.Web.Controllers.Transaction
{
    [HandleError]
    public class ReportController : Controller
    {
        //public ReportController()
        //    : this(new TJournalRepository(), new TJournalDetRepository(), new MCostCenterRepository(), new MAccountRepository(), new TRecAccountRepository(), new TRecPeriodRepository(), new MBrandRepository(), new MSupplierRepository(), new MWarehouseRepository(), new MItemRepository(), new TStockCardRepository(), new TStockItemRepository(), new TTransDetRepository())
        //{ }

        private readonly IMBrandRepository _mBrandRepository;
        private readonly IMSupplierRepository _mSupplierRepository;
        private readonly IMWarehouseRepository _mWarehouseRepository;
        private readonly IMItemRepository _mItemRepository;
        private readonly ITStockCardRepository _tStockCardRepository;
        private readonly ITStockItemRepository _tStockItemRepository;
        private readonly ITTransDetRepository _tTransDetRepository;
        private readonly ITTransRepository _tTransRepository;

        public ReportController(IMBrandRepository mBrandRepository, IMSupplierRepository mSupplierRepository, IMWarehouseRepository mWarehouseRepository, IMItemRepository mItemRepository, ITStockCardRepository tStockCardRepository, ITStockItemRepository tStockItemRepository, ITTransDetRepository tTransDetRepository, ITTransRepository tTransRepository)
        {
            Check.Require(mBrandRepository != null, "mBrandRepository may not be null");
            Check.Require(mSupplierRepository != null, "mSupplierRepository may not be null");
            Check.Require(mWarehouseRepository != null, "mBrandRepository may not be null");
            Check.Require(mItemRepository != null, "mItemRepository may not be null");
            Check.Require(tStockCardRepository != null, "tStockCardRepository may not be null");
            Check.Require(tStockItemRepository != null, "tStockItemRepository may not be null");
            Check.Require(tTransDetRepository != null, "tTransDetRepository may not be null");
            Check.Require(tTransRepository != null, "tTransRepository may not be null");

            this._mBrandRepository = mBrandRepository;
            this._mSupplierRepository = mSupplierRepository;
            this._mWarehouseRepository = mWarehouseRepository;
            this._mItemRepository = mItemRepository;
            this._tStockCardRepository = tStockCardRepository;
            this._tStockItemRepository = tStockItemRepository;
            this._tTransDetRepository = tTransDetRepository;
            this._tTransRepository = tTransRepository;
        }

        [Transaction]
        public ActionResult ReportTrans(EnumReports reports, EnumTransactionStatus TransStatus)
        {
            ReportParamViewModel viewModel = ReportParamViewModel.CreateReportParamViewModel(_mWarehouseRepository, _mSupplierRepository, _mItemRepository);
            string title = string.Empty;
            switch (reports)
            {
                case EnumReports.RptBrand:
                    title = "Daftar Master Merek";

                    break;
                case EnumReports.RptStockCard:
                    title = "Kartu Stok";
                    viewModel.ShowDateFrom = true;
                    viewModel.ShowDateTo = true;
                    viewModel.ShowItem = true;
                    viewModel.ShowWarehouse = true;
                    break;
                case EnumReports.RptStockItem:
                    title = "Lap. Stok Per Gudang";
                    viewModel.ShowItem = true;
                    viewModel.ShowWarehouse = true;
                    break;
                case EnumReports.RptAnalyzeBudgetDetail:
                    title = "Lap. Analisa Budget";
                    viewModel.ShowItem = true;
                    viewModel.ShowWarehouse = true;
                    break;
                case EnumReports.RptTransDetail:
                    title = "Lap. Detail";
                    viewModel.ShowDateFrom = true;
                    viewModel.ShowDateTo = true;
                    viewModel.ShowWarehouse = true;
                    break;
                case EnumReports.RptTransDetailByTransBy:
                    title = "Lap. Rekap Detail Transaksi";
                    viewModel.ShowDateFrom = true;
                    viewModel.ShowDateTo = true;
                    viewModel.ShowSupplier = true;
                    break;
                case EnumReports.RptLRDetailSales:
                    title = "Lap. Detail Laba Kotor";
                    viewModel.ShowDateFrom = true;
                    viewModel.ShowDateTo = true;
                    viewModel.ShowCustomer = true;
                    break;
                case EnumReports.RptMostItemSales:
                    title = "Lap. Penjualan Terbanyak";
                    viewModel.ShowDateFrom = true;
                    viewModel.ShowDateTo = true;
                    viewModel.ShowWarehouse = true;
                    break;
            }
            ViewData["CurrentItem"] = title;


            ViewData["ExportFormat"] = new SelectList(Enum.GetValues(typeof(EnumExportFormat)));

            return View(viewModel);
        }

        [Transaction]
        public ActionResult Report(EnumReports reports)
        {
            return ReportTrans(reports, EnumTransactionStatus.None);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReportTrans(EnumReports reports, ReportParamViewModel viewModel, FormCollection formCollection)
        {
            return Report(reports, viewModel, formCollection);
        }

        [ValidateAntiForgeryToken]      // Helps avoid CSRF attacks
        [Transaction]                   // Wraps a transaction around the action
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Report(EnumReports reports, ReportParamViewModel viewModel, FormCollection formCollection)
        {
            bool Success = true;
            string Message = "redirect";
            try
            {
                EnumTransactionStatus stat =
                               (EnumTransactionStatus)Enum.Parse(typeof(EnumTransactionStatus), formCollection["TransStatus"]);
                ReportDataSource[] repCol = new ReportDataSource[1];
                switch (reports)
                {
                    case EnumReports.RptBrand:
                        repCol[0] = GetBrand();
                        break;
                    case EnumReports.RptStockCard:
                        repCol[0] = GetStockCard(viewModel.DateFrom, viewModel.DateTo, viewModel.ItemId, viewModel.WarehouseId);
                        break;
                    case EnumReports.RptStockItem:
                        repCol[0] = GetStockItem(viewModel.ItemId, viewModel.WarehouseId);
                        break;
                    case EnumReports.RptAnalyzeBudgetDetail:
                        repCol[0] = GetTransDetForBudget(viewModel.ItemId, viewModel.WarehouseId);
                        break;
                    case EnumReports.RptTransDetail:
                        repCol[0] = GetTransTotal(viewModel.DateFrom, viewModel.DateTo, viewModel.WarehouseId, stat);
                        break;
                    case EnumReports.RptTransDetailByTransBy:
                        repCol[0] = GetTransTotal(viewModel.DateFrom, viewModel.DateTo, viewModel.WarehouseId, viewModel.SupplierId, stat);
                        break;
                    case EnumReports.RptLRDetailSales:
                        repCol[0] = GetTransTotalAndRef(viewModel.DateFrom, viewModel.DateTo, null, viewModel.CustomerId, stat);
                        break;
                    case EnumReports.RptMostItemSales:
                        repCol[0] = GetTransDetail(viewModel.DateFrom, viewModel.DateTo, viewModel.WarehouseId, stat);
                        break;
                }
                Session["ReportData"] = repCol;
            }
            catch (Exception ex)
            {
                Success = false;
                Message = "Error :\n " + ex.GetBaseException().Message;
            }


            var e = new
            {
                Success,
                Message,
                UrlReport = string.Format("{0}", reports.ToString())
            };
            return Json(e, JsonRequestBehavior.AllowGet);
        }

        private ReportDataSource GetTransDetail(DateTime? dateFrom, DateTime? dateTo, string warehouseId, EnumTransactionStatus transStatus)
        {
            Check.Require(transStatus != EnumTransactionStatus.None, "transStatus may not be None");
            IList<TTransDet> dets = _tTransDetRepository.GetByDateWarehouseTransBy(dateFrom, dateTo, string.Empty, warehouseId, transStatus.ToString());

            var list = from det in dets
                       select new
                       {
                           det.TransDetNo,
                           det.TransDetQty,
                           det.TransDetDesc,
                           det.TransDetTotal,
                           det.TransDetPrice,
                           det.TransDetDisc,
                           ItemId = det.ItemId.Id,
                           det.ItemId.ItemName
                       }
            ;

            ReportDataSource reportDataSource = new ReportDataSource("TransDetViewModel", list.ToList());
            return reportDataSource;
        }

        private ReportDataSource ConvertToReportDatasource(IList<TTransDet> dets, EnumTransactionStatus transStatus)
        {
            var list = from det in dets
                       select new
                       {
                           det.TransDetNo,
                           det.TransDetQty,
                           det.TransDetDesc,
                           det.TransDetTotal,
                           det.TransDetPrice,
                           det.TransDetDisc,
                           ItemId = det.ItemId.Id,
                           det.ItemId.ItemName,
                           SupplierName = det.TransId.TransBy,
                           det.TransId.TransFactur,
                           det.TransId.TransDate,
                           WarehouseId = det.TransId.WarehouseId.Id,
                           det.TransId.WarehouseId.WarehouseName,
                           WarehouseToName =
                det.TransId.WarehouseIdTo != null ? det.TransId.WarehouseIdTo.WarehouseName : null,
                           det.TransId.TransStatus,
                           det.TransId.TransDesc,
                           det.TransId.TransSubTotal,
                           det.TransId.TransPaymentMethod,
                           TransId = det.TransId.Id,
                           ViewWarehouse = SetView(det.TransId.TransStatus, EnumViewTrans.ViewWarehouse),
                           ViewWarehouseTo = SetView(det.TransId.TransStatus, EnumViewTrans.ViewWarehouseTo),
                           ViewSupplier = SetView(det.TransId.TransStatus, EnumViewTrans.ViewSupplier),
                           ViewDate = SetView(det.TransId.TransStatus, EnumViewTrans.ViewDate),
                           ViewFactur = SetView(det.TransId.TransStatus, EnumViewTrans.ViewFactur),
                           ViewPrice = SetView(det.TransId.TransStatus, EnumViewTrans.ViewPrice),
                           ViewPaymentMethod =
                SetView(det.TransId.TransStatus, EnumViewTrans.ViewPaymentMethod),
                           TransName = Helper.CommonHelper.GetStringValue(transStatus)
                       }
           ;

            ReportDataSource reportDataSource = new ReportDataSource("TransTotalViewModel", list.ToList());
            return reportDataSource;
        }

        private ReportDataSource GetTransTotal(DateTime? dateFrom, DateTime? dateTo, string supplierId, string warehouseId, EnumTransactionStatus transStatus)
        {
            Check.Require(transStatus != EnumTransactionStatus.None, "transStatus may not be None");
            IList<TTransDet> dets = _tTransDetRepository.GetByDateWarehouseTransBy(dateFrom, dateTo, supplierId, warehouseId, transStatus.ToString());

            return ConvertToReportDatasource(dets, transStatus);
        }

        private ReportDataSource GetTransTotalAndRef(DateTime? dateFrom, DateTime? dateTo, string supplierId, string warehouseId, EnumTransactionStatus transStatus)
        {
            Check.Require(transStatus != EnumTransactionStatus.None, "transStatus may not be None");
            IList dets = _tTransDetRepository.GetRefStockByDateWarehouseTransBy(dateFrom, dateTo, supplierId, warehouseId, transStatus.ToString());

            Array arr;
            IList<TransDetWithRef> listResult = new List<TransDetWithRef>();
            TransDetWithRef detWithRef = new TransDetWithRef();
            for (int i = 0; i < dets.Count; i++)
            {
                arr = dets[i] as Array;
                if (arr != null)
                {
                    detWithRef = new TransDetWithRef();
                    detWithRef.TransDet = arr.GetValue(0) as TTransDet;
                    detWithRef.StockPrice = arr.GetValue(1) as decimal?;
                    detWithRef.StockRefQty = arr.GetValue(2) as decimal?;
                    listResult.Add(detWithRef);
                }


            }

            string TransName = Helper.CommonHelper.GetStringValue(transStatus);
            var list = from det in listResult
                       select new
                       {
                           det.TransDet.Id,
                           det.TransDet.TransDetNo,
                           det.TransDet.TransDetQty,
                           det.TransDet.TransDetDesc,
                           det.TransDet.TransDetTotal,
                           det.TransDet.TransDetPrice,
                           det.TransDet.TransDetDisc,
                           ItemId = det.TransDet.ItemId.Id,
                           det.TransDet.ItemId.ItemName,
                           SupplierName = det.TransDet.TransId.TransBy,
                           CustomerName = det.TransDet.TransId.TransBy,
                           det.TransDet.TransId.TransFactur,
                           det.TransDet.TransId.TransDate,
                           WarehouseId = det.TransDet.TransId.WarehouseId.Id,
                           det.TransDet.TransId.WarehouseId.WarehouseName,
                           WarehouseToName =
                det.TransDet.TransId.WarehouseIdTo != null ? det.TransDet.TransId.WarehouseIdTo.WarehouseName : null,
                           det.TransDet.TransId.TransStatus,
                           det.TransDet.TransId.TransDesc,
                           det.TransDet.TransId.TransSubTotal,
                           det.TransDet.TransId.TransPaymentMethod,
                           TransId = det.TransDet.TransId.Id,
                           ViewWarehouse = SetView(det.TransDet.TransId.TransStatus, EnumViewTrans.ViewWarehouse),
                           ViewWarehouseTo = SetView(det.TransDet.TransId.TransStatus, EnumViewTrans.ViewWarehouseTo),
                           ViewSupplier = SetView(det.TransDet.TransId.TransStatus, EnumViewTrans.ViewSupplier),
                           ViewDate = SetView(det.TransDet.TransId.TransStatus, EnumViewTrans.ViewDate),
                           ViewFactur = SetView(det.TransDet.TransId.TransStatus, EnumViewTrans.ViewFactur),
                           ViewPrice = SetView(det.TransDet.TransId.TransStatus, EnumViewTrans.ViewPrice),
                           ViewPaymentMethod =
                SetView(det.TransDet.TransId.TransStatus, EnumViewTrans.ViewPaymentMethod),
                           TransName,
                           StockRefPrice = det.StockPrice,
                           det.StockRefQty
                       }
            ;

            ReportDataSource reportDataSource = new ReportDataSource("TransTotalViewModel", list.ToList());
            return reportDataSource;
        }

        //private ReportDataSource GetTransDetByDate(DateTime? dateFrom, DateTime? dateTo)
        //{
        //    IList<TTransDet> dets = _tTransDetRepository.GetListByDate(EnumTransactionStatus.Service, dateFrom, dateTo);

        //    var list = from det in dets
        //               select new
        //               {
        //                   EmployeeId = det.EmployeeId.Id,
        //                   EmployeeName = det.EmployeeId.PersonId.PersonName,
        //                   det.TransDetCommissionService,
        //                   det.TransId.TransFactur,
        //                   det.TransId.TransDate,
        //                   det.TransDetQty,
        //                   det.TransDetTotal
        //               }
        //    ;

        //    ReportDataSource reportDataSource = new ReportDataSource("TransDetViewModel", list.ToList());
        //    return reportDataSource;
        //}

        //  private ReportDataSource GetShiftViewModel(TShift s)
        //  {
        //      IList<TShift> listShift = new List<TShift>();
        //      listShift.Add(s);
        //      var listRoom = from shift in listShift
        //                     select new
        //                     {
        //                         shift.ShiftNo,
        //                         shift.ShiftDate,
        //                         shift.ShiftDateFrom,
        //                         shift.ShiftDateTo
        //                     }
        //;
        //      ReportDataSource reportDataSource = new ReportDataSource("ShiftViewModel", listRoom.ToList());
        //      return reportDataSource;
        //  }

        private ReportDataSource GetTransTotal(DateTime? dateFrom, DateTime? dateTo, string warehouseId, EnumTransactionStatus transStatus)
        {
            return GetTransTotal(dateFrom, dateTo, string.Empty, warehouseId, transStatus);
            //Check.Require(transStatus != EnumTransactionStatus.None, "transStatus may not be None");
            //IList<TTransDet> dets;
            //MWarehouse warehouse = null;
            //if (!string.IsNullOrEmpty(warehouseId))
            //    warehouse = _mWarehouseRepository.Get(warehouseId);
            //dets = _tTransDetRepository.GetByDateWarehouse(dateFrom, dateTo, warehouse, transStatus.ToString());

            //var list = from det in dets
            //           select new
            //           {
            //               det.TransDetNo,
            //               det.TransDetQty,
            //               det.TransDetDesc,
            //               det.TransDetTotal,
            //               det.TransDetPrice,
            //               det.TransDetDisc,
            //               ItemId = det.ItemId.Id,
            //               det.ItemId.ItemName,
            //               SupplierName = det.TransId.TransBy,
            //               det.TransId.TransFactur,
            //               det.TransId.TransDate,
            //               WarehouseId = det.TransId.WarehouseId.Id,
            //               det.TransId.WarehouseId.WarehouseName,
            //               WarehouseToName =
            //    det.TransId.WarehouseIdTo != null ? det.TransId.WarehouseIdTo.WarehouseName : null,
            //               det.TransId.TransStatus,
            //               det.TransId.TransDesc,
            //               det.TransId.TransSubTotal,
            //               det.TransId.TransPaymentMethod,
            //               TransId = det.TransId.Id,
            //               ViewWarehouse = SetView(det.TransId.TransStatus, EnumViewTrans.ViewWarehouse),
            //               ViewWarehouseTo = SetView(det.TransId.TransStatus, EnumViewTrans.ViewWarehouseTo),
            //               ViewSupplier = SetView(det.TransId.TransStatus, EnumViewTrans.ViewSupplier),
            //               ViewDate = SetView(det.TransId.TransStatus, EnumViewTrans.ViewDate),
            //               ViewFactur = SetView(det.TransId.TransStatus, EnumViewTrans.ViewFactur),
            //               ViewPrice = SetView(det.TransId.TransStatus, EnumViewTrans.ViewPrice),
            //               ViewPaymentMethod =
            //    SetView(det.TransId.TransStatus, EnumViewTrans.ViewPaymentMethod),
            //               TransName = Helper.CommonHelper.GetStringValue(transStatus)
            //           }
            //;

            //ReportDataSource reportDataSource = new ReportDataSource("TransTotalViewModel", list.ToList());
            //return reportDataSource;
        }

        private bool SetView(string TransStatus, EnumViewTrans viewTrans)
        {
            return true;
        }

        private ReportDataSource GetTransDetForBudget(string itemId, string warehouseId)
        {
            IList<TTransDet> dets;
            MItem item = null;
            MWarehouse warehouse = null;
            if (!string.IsNullOrEmpty(itemId))
                item = _mItemRepository.Get(itemId);
            if (!string.IsNullOrEmpty(warehouseId))
                warehouse = _mWarehouseRepository.Get(warehouseId);
            dets = _tTransDetRepository.GetByItemWarehouse(item, warehouse);

            var list = from det in dets
                       select new
                                  {
                                      det.TransDetNo,
                                      det.TransDetQty,
                                      det.TransDetDesc,
                                      det.TransDetTotal,
                                      det.TransDetPrice,
                                      det.TransDetDisc,
                                      ItemId = det.ItemId.Id,
                                      det.ItemId.ItemName,
                                      WarehouseId = det.TransId.WarehouseId.Id,
                                      det.TransId.WarehouseId.WarehouseName,
                                      TotalUsed = _tTransDetRepository.GetTotalUsed(det.ItemId, det.TransId.WarehouseId)
                                  }
            ;

            ReportDataSource reportDataSource = new ReportDataSource("TransDetViewModel", list.ToList());
            return reportDataSource;
        }

        private ReportDataSource GetStockItem(string itemId, string warehouseId)
        {
            IList<TStockItem> stockItems;
            MItem item = null;
            MWarehouse warehouse = null;
            if (!string.IsNullOrEmpty(itemId))
                item = _mItemRepository.Get(itemId);
            if (!string.IsNullOrEmpty(warehouseId))
                warehouse = _mWarehouseRepository.Get(warehouseId);
            stockItems = _tStockItemRepository.GetByItemWarehouse(item, warehouse);

            var list = from stock in stockItems
                       select new
                       {
                           stock.ItemStock,
                           stock.ItemStockMax,
                           stock.ItemStockMin,
                           ItemId = stock.ItemId.Id,
                           stock.ItemId.ItemName,
                           WarehouseId = stock.WarehouseId.Id,
                           stock.WarehouseId.WarehouseName,
                           stock.ItemStockRack
                       }
             ;
            //return list.ToList();
            ReportDataSource reportDataSource = new ReportDataSource("StockItemViewModel", list.ToList());
            return reportDataSource;
        }

        private ReportDataSource GetStockCard(DateTime? dateFrom, DateTime? dateTo, string itemId, string warehouseId)
        {
            IList<TStockCard> cards;
            MItem item = null;
            MWarehouse warehouse = null;
            if (!string.IsNullOrEmpty(itemId))
                item = _mItemRepository.Get(itemId);
            if (!string.IsNullOrEmpty(warehouseId))
                warehouse = _mWarehouseRepository.Get(warehouseId);
            cards = _tStockCardRepository.GetByDateItemWarehouse(dateFrom, dateTo, item, warehouse);

            var list = from card in cards
                       select new
                       {
                           card.StockCardQty,
                           card.StockCardDate,
                           card.StockCardStatus,
                           ItemId = card.ItemId.Id,
                           card.ItemId.ItemName,
                           WarehouseId = card.WarehouseId.Id,
                           card.WarehouseId.WarehouseName,
                           card.StockCardSaldo,
                           card.StockCardDesc
                       }
            ;

            ReportDataSource reportDataSource = new ReportDataSource("StockCardViewModel", list.ToList());
            return reportDataSource;
        }

        private ReportDataSource GetBrand()
        {
            ReportDataSource reportDataSource = new ReportDataSource("Brand", _mBrandRepository.GetAll());
            return reportDataSource;
        }
    }

    public enum EnumViewTrans
    {
        ViewWarehouse,
        ViewWarehouseTo,
        ViewSupplier,
        ViewDate,
        ViewFactur,
        ViewPrice,
        ViewPaymentMethod
    }
}
