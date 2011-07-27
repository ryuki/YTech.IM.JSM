using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using SharpArch.Core;
using SharpArch.Core.PersistenceSupport;
using SharpArch.Web.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;
using YTech.IM.JSM.Data.Repository;
using YTech.IM.JSM.Enums;

namespace YTech.IM.JSM.Web.Controllers.ViewModel
{
    public class TransactionFormViewModel
    {
        public static TransactionFormViewModel Create(EnumTransactionStatus enumTransactionStatus, ITTransRepository transRepository, IMWarehouseRepository mWarehouseRepository, IMSupplierRepository mSupplierRepository, IMCustomerRepository mCustomerRepository)
        {
            TransactionFormViewModel viewModel = new TransactionFormViewModel();

            viewModel.Trans = SetNewTrans(enumTransactionStatus);

            switch (enumTransactionStatus)
            {
                case EnumTransactionStatus.PurchaseOrder:
                    viewModel.ViewWarehouse = true;
                    viewModel.Title = "Order Pembelian";
                    viewModel.ViewWarehouseTo = false;
                    viewModel.ViewTransBy = true;
                    viewModel.ViewDate = true;
                    viewModel.ViewFactur = true;
                    viewModel.ViewPrice = true;
                    viewModel.ViewPaymentMethod = false;
                    viewModel.TransByText = "Supplier :";
                    viewModel.TransByList = GetSupplierList();
                    viewModel.UsePrice = EnumPrice.Purchase;
                    viewModel.ViewTransDueDate = false;
                    break;
                case EnumTransactionStatus.Purchase:
                    viewModel.ViewWarehouse = true;
                    viewModel.Title = "Pembelian";
                    viewModel.ViewWarehouseTo = false;
                    viewModel.ViewTransBy = true;
                    viewModel.ViewDate = true;
                    viewModel.ViewFactur = true;
                    viewModel.ViewPrice = true;
                    viewModel.ViewPaymentMethod = true;
                    viewModel.TransByText = "Supplier :";
                    viewModel.TransByList = GetSupplierList();
                    viewModel.UsePrice = EnumPrice.Purchase;
                    viewModel.ViewTransDueDate = true;
                    break;
                case EnumTransactionStatus.ReturPurchase:
                    viewModel.ViewWarehouse = true;
                    viewModel.Title = "Retur Pembelian";
                    viewModel.ViewWarehouseTo = false;
                    viewModel.ViewTransBy = true;
                    viewModel.ViewDate = true;
                    viewModel.ViewFactur = true;
                    viewModel.ViewPrice = true;
                    viewModel.ViewPaymentMethod = true;
                    viewModel.TransByText = "Supplier :";
                    viewModel.TransByList = GetSupplierList();
                    viewModel.UsePrice = EnumPrice.Purchase;
                    viewModel.ViewTransDueDate = true;
                    break;
                case EnumTransactionStatus.Sales:
                    viewModel.ViewWarehouse = true;
                    viewModel.Title = "Penjualan";
                    viewModel.ViewWarehouseTo = false;
                    viewModel.ViewTransBy = true;
                    viewModel.ViewDate = true;
                    viewModel.ViewFactur = true;
                    viewModel.ViewPrice = true;
                    viewModel.ViewPaymentMethod = true;
                    viewModel.TransByText = "Konsumen :";
                    viewModel.TransByList = GetCustomerList();
                    viewModel.UsePrice = EnumPrice.Sale;
                    viewModel.ViewTransDueDate = true;
                    break;
                case EnumTransactionStatus.ReturSales:
                    viewModel.ViewWarehouse = true;
                    viewModel.Title = "Retur Penjualan";
                    viewModel.ViewWarehouseTo = false;
                    viewModel.ViewTransBy = true;
                    viewModel.ViewDate = true;
                    viewModel.ViewFactur = true;
                    viewModel.ViewPrice = true;
                    viewModel.ViewPaymentMethod = true;
                    viewModel.TransByText = "Konsumen :";
                    viewModel.TransByList = GetCustomerList();
                    viewModel.UsePrice = EnumPrice.Sale;
                    viewModel.ViewTransDueDate = true;
                    break;
                case EnumTransactionStatus.Using:
                    viewModel.ViewWarehouse = true;
                    viewModel.Title = "Pemakaian Material";
                    viewModel.ViewWarehouseTo = false;
                    viewModel.ViewTransBy = false;
                    viewModel.ViewDate = true;
                    viewModel.ViewFactur = true;
                    viewModel.ViewPrice = false;
                    viewModel.ViewPaymentMethod = false;
                    viewModel.UsePrice = EnumPrice.None;
                    viewModel.ViewTransDueDate = false;
                    break;
                case EnumTransactionStatus.Mutation:
                    viewModel.ViewWarehouse = true;
                    viewModel.Title = "Mutasi Stok";
                    viewModel.ViewWarehouseTo = true;
                    viewModel.ViewTransBy = false;
                    viewModel.ViewDate = true;
                    viewModel.ViewFactur = true;
                    viewModel.ViewPrice = false;
                    viewModel.ViewPaymentMethod = false;
                    viewModel.UsePrice = EnumPrice.None;
                    viewModel.ViewTransDueDate = false;
                    break;
                case EnumTransactionStatus.Adjusment:
                    viewModel.ViewWarehouse = true;
                    viewModel.Title = "Penyesuaian Stok";
                    viewModel.ViewWarehouseTo = false;
                    viewModel.ViewTransBy = false;
                    viewModel.ViewDate = true;
                    viewModel.ViewFactur = true;
                    viewModel.ViewPrice = false;
                    viewModel.ViewPaymentMethod = false;
                    viewModel.UsePrice = EnumPrice.None;
                    viewModel.ViewTransDueDate = false;
                    break;
                case EnumTransactionStatus.Received:
                    viewModel.ViewWarehouse = true;
                    viewModel.Title = "Penerimaan Stok";
                    viewModel.ViewWarehouseTo = false;
                    viewModel.ViewTransBy = true;
                    viewModel.ViewDate = true;
                    viewModel.ViewFactur = true;
                    viewModel.ViewPrice = true;
                    viewModel.ViewPaymentMethod = false;
                    viewModel.TransByText = "Supplier :";
                    viewModel.TransByList = GetSupplierList();
                    viewModel.UsePrice = EnumPrice.Purchase;
                    viewModel.ViewTransDueDate = false;
                    break;
                case EnumTransactionStatus.Budgeting:
                    viewModel.ViewWarehouse = true;
                    viewModel.Title = "Rencana Anggaran Belanja";
                    viewModel.ViewWarehouseTo = false;
                    viewModel.ViewTransBy = false;
                    viewModel.ViewDate = false;
                    viewModel.ViewFactur = false;
                    viewModel.ViewPrice = true;
                    viewModel.ViewPaymentMethod = false;
                    viewModel.UsePrice = EnumPrice.Purchase;
                    viewModel.ViewTransDueDate = false;
                    break;
            }

            //get if not add stock
            bool calculateStock = false;
            bool addStock = false;
            GetIsCalculateStock(enumTransactionStatus, out addStock, out calculateStock);
            viewModel.IsAddStock = addStock;

            //fill warehouse if it visible
            if (viewModel.ViewWarehouse || viewModel.ViewWarehouseTo)
            {
                IList<MWarehouse> list = mWarehouseRepository.GetAll();
                MWarehouse mWarehouse = new MWarehouse();
                mWarehouse.WarehouseName = "-Pilih Gudang-";
                list.Insert(0, mWarehouse);
                viewModel.WarehouseList = new SelectList(list, "Id", "WarehouseName");
                if (viewModel.ViewWarehouseTo)
                    viewModel.WarehouseToList = new SelectList(list, "Id", "WarehouseName");
            }


            //IList<MSupplier> listSupplier = mSupplierRepository.GetAll();
            //MSupplier mSupplier = new MSupplier();
            //mSupplier.SupplierName = "-Pilih Supplier-";
            //listSupplier.Insert(0, mSupplier);
            //viewModel.SupplierList = new SelectList(listSupplier, "Id", "SupplierName");

            //var listCustomer = mCustomerRepository.GetAll();
            //MCustomer mCustomer = new MCustomer();
            ////mCustomer.SupplierName = "-Pilih Supplier-";
            //listCustomer.Insert(0, mCustomer);
            //var custs = from cust in listCustomer
            //            select new { Id = cust.Id, Name =cust.PersonId.PersonName };
            //viewModel.TransByList = new SelectList(custs, "Id", "Name");

            //fill payment method
            var values = from EnumPaymentMethod e in Enum.GetValues(typeof(EnumPaymentMethod))
                         select new { ID = e, Name = e.ToString() };

            viewModel.PaymentMethodList = new SelectList(values, "Id", "Name");

            //viewModel.MinusStock = GetIsCalculateStock(sta)
            return viewModel;
        }

        private static TTrans SetNewTrans(EnumTransactionStatus enumTransactionStatus)
        {
            TTrans trans = new TTrans();
            trans.TransDate = DateTime.Today;
            trans.TransFactur = Helper.CommonHelper.GetFacturNo(enumTransactionStatus);
            trans.SetAssignedIdTo(Guid.NewGuid().ToString());
            trans.TransStatus = enumTransactionStatus.ToString();
            return trans;
        }

        private static SelectList GetSupplierList()
        {
            IMSupplierRepository mSupplierRepository = new MSupplierRepository();
            IList<MSupplier> listSupplier = mSupplierRepository.GetAll();
            MSupplier mSupplier = new MSupplier();
            mSupplier.SupplierName = "-Pilih Supplier-";
            listSupplier.Insert(0, mSupplier);
            return new SelectList(listSupplier, "Id", "SupplierName");
        }

        private static SelectList GetCustomerList()
        {
            IMCustomerRepository mCustomerRepository = new MCustomerRepository();
            var listCustomer = mCustomerRepository.GetAll();
            MCustomer mCustomer = new MCustomer();
            //mCustomer.SupplierName = "-Pilih Supplier-";
            listCustomer.Insert(0, mCustomer);
            var custs = from cust in listCustomer
                        select new { Id = cust.Id, Name = cust.PersonId != null ? cust.PersonId.PersonName : "-Pilih Konsumen-" };
            return new SelectList(custs, "Id", "Name");
        }

        public TTrans Trans { get; internal set; }
        public IList<TTransDet> ListOfTransDet { get; internal set; }

        public SelectList WarehouseList { get; internal set; }
        public SelectList WarehouseToList { get; internal set; }
        //public SelectList SupplierList { get; internal set; }
        public SelectList TransByList { get; internal set; }
        public SelectList PaymentMethodList { get; internal set; }
        public bool ViewWarehouse { get; internal set; }
        public bool ViewWarehouseTo { get; internal set; }
        public bool ViewTransBy { get; internal set; }
        public bool ViewCustomer { get; internal set; }
        public bool ViewDate { get; internal set; }
        public bool ViewFactur { get; internal set; }
        public bool ViewPrice { get; internal set; }
        public bool ViewPaymentMethod { get; internal set; }
        public bool ViewTransDueDate { get; internal set; }
        public string Title { get; internal set; }
        public string TransByText { get; internal set; }
        public EnumPrice UsePrice { get; internal set; }
        public bool IsAddStock { get; internal set; }


        internal static void GetIsCalculateStock(EnumTransactionStatus status, out bool addStock, out bool calculateStock)
        {
            addStock = true;
            calculateStock = false;
            switch (status)
            {
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
                case EnumTransactionStatus.Purchase:
                    addStock = true;
                    calculateStock = true;
                    break;
            }
        }
    }
}
