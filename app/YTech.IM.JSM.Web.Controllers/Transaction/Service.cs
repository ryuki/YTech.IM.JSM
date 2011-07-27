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
    public class Service : AbstractTransaction
    {
        #region Overrides of AbstractTransaction

        public override void SaveJournal(TTrans trans, decimal totalHPP)
        {
            string desc = string.Format("Penjualan paket jasa kepada {0}", trans.TransBy);
            string newVoucher = Helper.CommonHelper.GetVoucherNo(false);
            //save header of journal
            TJournal journal = SaveJournalHeader(newVoucher, trans, desc);
            MAccountRef accountRef = null;

            if (trans.TransPaymentMethod == EnumPaymentMethod.Tunai.ToString())
            {
                //save cash
                SaveJournalDet(journal, newVoucher, Helper.AccountHelper.GetCashAccount(), EnumJournalStatus.D, trans.TransGrandTotal.Value, trans, desc);
            }
            else
            {
                accountRef = AccountRefRepository.GetByRefTableId(EnumReferenceTable.Customer, trans.TransBy);
                //save piutang
                SaveJournalDet(journal, newVoucher, accountRef.AccountId, EnumJournalStatus.D, trans.TransGrandTotal.Value, trans, desc);
            }
            //save penjualan
            SaveJournalDet(journal, newVoucher, Helper.AccountHelper.GetSalesAccount(), EnumJournalStatus.K, trans.TransGrandTotal.Value, trans, desc);

            //save ikhtiar LR
            SaveJournalDet(journal, newVoucher, Helper.AccountHelper.GetIkhtiarLRAccount(), EnumJournalStatus.D, totalHPP, trans, desc);

            //save persediaan
            accountRef = AccountRefRepository.GetByRefTableId(EnumReferenceTable.Warehouse, trans.WarehouseId.Id);
            SaveJournalDet(journal, newVoucher, accountRef.AccountId, EnumJournalStatus.K, totalHPP, trans, desc);

            JournalRepository.Save(journal);

            //save journal ref
            SaveJournalRef(trans, journal);
        }

        #endregion
    }
}
