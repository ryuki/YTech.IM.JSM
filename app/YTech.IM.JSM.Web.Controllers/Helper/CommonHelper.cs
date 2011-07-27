using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;
using YTech.IM.JSM.Enums;
using YTech.IM.JSM.Data.Repository;
using YTech.IM.JSM.Core.RepositoryInterfaces;
using YTech.IM.JSM.Core;

namespace YTech.IM.JSM.Web.Controllers.Helper
{
    public class CommonHelper
    {
        private const string CONST_FACTURFORMAT = "JSM/[TRANS]/[YEAR]/[MONTH]/[DAY]/[XXX]";
        public const string CONST_VOUCHERNO = "JSM/VOUCHER/[YEAR]/[MONTH]/[DAY]/[XXX]";

        public static string DateFormat
        {
            get { return "dd-MMM-yyyy"; }
        }
        public static string DateTimeFormat
        {
            get { return "dd-MMM-yyyy HH:mm"; }
        }
        public static string TimeFormat
        {
            get { return "HH:mm"; }
        }
        public static string NumberFormat
        {
            get { return "N2"; }
        }

        public static TReference GetReference(EnumReferenceType referenceType)
        {
            //check in cache first
            object obj = System.Web.HttpContext.Current.Cache[referenceType.ToString()];
            //if not available, set it first
            if (obj == null)
            {
                ITReferenceRepository referenceRepository = new TReferenceRepository();
                TReference reference = referenceRepository.GetByReferenceType(referenceType);
                if (reference == null)
                {
                    referenceRepository.DbContext.BeginTransaction();
                    reference = new TReference();
                    reference.SetAssignedIdTo(Guid.NewGuid().ToString());
                    reference.ReferenceType = referenceType.ToString();
                    reference.ReferenceValue = "0";
                    reference.CreatedDate = DateTime.Now;
                    reference.DataStatus = EnumDataStatus.New.ToString();
                    referenceRepository.Save(reference);
                    referenceRepository.DbContext.CommitTransaction();
                }
                //save to cache
                System.Web.HttpContext.Current.Cache[referenceType.ToString()] = reference;
            }

            //return cache
            return System.Web.HttpContext.Current.Cache[referenceType.ToString()] as TReference;
        }

        public static string GetFacturNo(EnumTransactionStatus transactionStatus)
        {
            return GetFacturNo(transactionStatus, true);
        }

        public static string GetFacturNo(EnumTransactionStatus transactionStatus, bool automatedIncrease)
        {
            TReference refer = GetReference((EnumReferenceType)Enum.Parse(typeof(EnumReferenceType), transactionStatus.ToString()));
            decimal no = Convert.ToDecimal(refer.ReferenceValue) + 1;
            refer.ReferenceValue = no.ToString();
            if (automatedIncrease)
            {
                ITReferenceRepository referenceRepository = new TReferenceRepository();
                referenceRepository.Update(refer);
                referenceRepository.DbContext.CommitChanges();
            }

            string tipeTrans = string.Empty;
            char[] charTransArray = transactionStatus.ToString().ToCharArray();
            char charTrans;

            for (int i = 0; i < transactionStatus.ToString().Length; i++)
            {
                charTrans = charTransArray[i];
                if (char.IsUpper(transactionStatus.ToString(), i))
                    tipeTrans += transactionStatus.ToString().Substring(i, 1);
            }

            StringBuilder result = new StringBuilder();
            result.Append(CONST_FACTURFORMAT);
            result.Replace("[TRANS]", tipeTrans);
            result.Replace("[XXX]", GetFactur(5, no));
            result.Replace("[DAY]", DateTime.Today.Day.ToString());
            result.Replace("[MONTH]", DateTime.Today.ToString("MMM").ToUpper());
            result.Replace("[YEAR]", DateTime.Today.Year.ToString());
            return result.ToString();
        }

        public static string GetVoucherNo()
        {
            return GetVoucherNo(false);
        }

        public static string GetVoucherNo(bool automatedIncrease)
        {
            TReference refer = GetReference(EnumReferenceType.VoucherNo);
            decimal no = Convert.ToDecimal(refer.ReferenceValue) + 1;
            refer.ReferenceValue = no.ToString();
            if (automatedIncrease)
            {
                ITReferenceRepository referenceRepository = new TReferenceRepository();
                referenceRepository.DbContext.BeginTransaction();
                referenceRepository.Update(refer);
                referenceRepository.DbContext.CommitTransaction();
            }

            StringBuilder result = new StringBuilder();
            result.Append(CONST_VOUCHERNO);
            result.Replace("[XXX]", GetFactur(5, no));
            result.Replace("[DAY]", DateTime.Today.Day.ToString());
            result.Replace("[MONTH]", DateTime.Today.ToString("MMM").ToUpper());
            result.Replace("[YEAR]", DateTime.Today.Year.ToString());
            return result.ToString();
        }

        private static string GetFactur(int maxLength, decimal no)
        {
            int len = maxLength - no.ToString().Length;
            string factur = no.ToString();
            for (int i = 0; i < len; i++)
            {
                factur = "0" + factur;
            }
            return factur;
        }

        /// <summary>
        /// get list of enum for jqgrid combobox
        /// </summary>
        /// <typeparam name="T">type of enum</typeparam>
        /// <param name="defaultText">default text for display</param>
        /// <returns>string</returns>
        public static string GetEnumListForGrid<T>(string defaultText)
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("Type object must enum");
            }
            var lists = from T e in Enum.GetValues(typeof(T))
                        select new { ID = e, Name = e.ToString() };
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}:{1}", string.Empty, defaultText);

            for (int i = 0; i < lists.Count(); i++)
            {
                var obj = lists.ElementAt(i);
                sb.AppendFormat(";{0}:{1}", obj.ID, obj.Name);
            }
            return (sb.ToString());
        }

        /// <summary>
        /// get default warehouse
        /// </summary>
        /// <returns></returns>
        internal static MWarehouse GetDefaultWarehouse()
        {
            object obj = System.Web.HttpContext.Current.Cache[EnumReferenceType.DefaultWarehouse.ToString()];
            if (obj == null)
            {
                TReference refer = GetReference(EnumReferenceType.DefaultWarehouse);
                if (!string.IsNullOrEmpty(refer.ReferenceValue))
                {
                    IMWarehouseRepository warehouseRepository = new MWarehouseRepository();
                    System.Web.HttpContext.Current.Cache[EnumReferenceType.DefaultWarehouse.ToString()] = warehouseRepository.Get(refer.ReferenceValue);
                }
            }

            return System.Web.HttpContext.Current.Cache[EnumReferenceType.DefaultWarehouse.ToString()] as MWarehouse;
        }

        internal static bool CheckStock(MWarehouse mWarehouse, MItem item, decimal? qty)
        {
            ITStockItemRepository stockItemRepository = new TStockItemRepository();
            TStockItem stockItem = stockItemRepository.GetByItemAndWarehouse(item, mWarehouse);
            if (stockItem != null)
            {
                if (stockItem.ItemStock > qty)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the StringValue attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : value.ToString();
        }

        internal static object GetCustomerName(IMCustomerRepository _mCustomerRepository, string customerId)
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
    }
}
