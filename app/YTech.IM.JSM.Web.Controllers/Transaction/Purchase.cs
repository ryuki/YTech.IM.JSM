﻿using System;
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
    public class Purchase : AbstractTransaction
    {
        #region Overrides of AbstractTransaction

        public override void SaveJournal(TTrans trans, decimal totalHPP)
        {
            string desc = string.Format("Pembelian dari {0}", trans.TransBy);
            string newVoucher = Helper.CommonHelper.GetVoucherNo(false);
            //save header of journal
            TJournal journal = SaveJournalHeader(newVoucher, trans, desc);
            MAccountRef accountRef = null;

            //save pembelian
            SaveJournalDet(journal, newVoucher, Helper.AccountHelper.GetPurchaseAccount(), EnumJournalStatus.D, trans.TransGrandTotal.Value, trans, desc);
            if (trans.TransPaymentMethod == EnumPaymentMethod.Tunai.ToString())
            {
                //save cash
                SaveJournalDet(journal, newVoucher, Helper.AccountHelper.GetCashAccount(), EnumJournalStatus.K, trans.TransGrandTotal.Value, trans, desc);
            }
            else
            {
                accountRef = AccountRefRepository.GetByRefTableId(EnumReferenceTable.Supplier, trans.TransBy);
                //save hutang
                SaveJournalDet(journal, newVoucher, accountRef.AccountId, EnumJournalStatus.K, trans.TransGrandTotal.Value, trans, desc);
            }

            //save persediaan
            accountRef = AccountRefRepository.GetByRefTableId(EnumReferenceTable.Warehouse, trans.WarehouseId.Id);
            SaveJournalDet(journal, newVoucher, accountRef.AccountId, EnumJournalStatus.D, totalHPP, trans, desc);

            //save ikhtiar LR
            SaveJournalDet(journal, newVoucher, Helper.AccountHelper.GetIkhtiarLRAccount(), EnumJournalStatus.K, totalHPP, trans, desc);

            JournalRepository.Save(journal);

            //save journal ref
            SaveJournalRef(trans, journal);
        }

        #endregion
    }
}
