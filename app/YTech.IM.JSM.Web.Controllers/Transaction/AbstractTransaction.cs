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
    public abstract class AbstractTransaction
    {
        public abstract void SaveJournal(TTrans trans, decimal totalHPP);

        public string UserName;
        public IMAccountRefRepository AccountRefRepository;
        public ITJournalRepository JournalRepository;
        public ITJournalRefRepository JournalRefRepository;

        protected string Desc = string.Empty;
        protected string NewVoucher = string.Empty;

        protected TJournal SaveJournalHeader(string newVoucher, TTrans trans, string desc)
        {
            //delete journal first
            DeleteJournal(trans);

            TJournal j = new TJournal();
            j.SetAssignedIdTo(Guid.NewGuid().ToString());
            j.CostCenterId = trans.WarehouseId.CostCenterId;
            j.JournalType = EnumJournalType.GeneralLedger.ToString();
            j.JournalVoucherNo = newVoucher;
            j.JournalPic = trans.TransBy;
            j.JournalDate = trans.TransDate;
            j.JournalEvidenceNo = trans.TransFactur;
            //j.JournalAmmount = ammount;
            j.JournalDesc = desc;

            j.DataStatus = EnumDataStatus.New.ToString();
            j.CreatedBy = UserName;
            j.CreatedDate = DateTime.Now;
            j.JournalDets.Clear();

            return j;
        }

        private void DeleteJournal(TTrans trans)
        {
            TJournalRef journalRef = JournalRefRepository.GetByReference(EnumReferenceTable.Transaction,
                                                                         trans.TransStatus, trans.Id);
            if (journalRef != null)
            {
                TJournal journalToDelete = journalRef.JournalId;
                JournalRefRepository.Delete(journalRef);
                JournalRepository.Delete(journalToDelete);
            }
        }

        protected void SaveJournalRef(TTrans trans, TJournal j)
        {
            TJournalRef journalRef = new TJournalRef();
            journalRef.SetAssignedIdTo(Guid.NewGuid().ToString());
            journalRef.JournalId = j; ;
            journalRef.ReferenceTable = EnumReferenceTable.Transaction.ToString();
            journalRef.ReferenceType = trans.TransStatus;
            journalRef.ReferenceId = trans.Id;
            journalRef.JournalRefDesc = trans.TransDesc;
            journalRef.CreatedBy = UserName;
            journalRef.CreatedDate = DateTime.Now;
            journalRef.DataStatus = EnumDataStatus.New.ToString();
            JournalRefRepository.Save(journalRef);
        }

        protected void SaveJournalDet(TJournal journal, string newVoucher, MAccount accountId, EnumJournalStatus journalStatus, decimal ammount, TTrans trans, string desc)
        {
            TJournalDet detToInsert = new TJournalDet(journal);
            detToInsert.SetAssignedIdTo(Guid.NewGuid().ToString());
            detToInsert.AccountId = accountId;
            detToInsert.JournalDetStatus = journalStatus.ToString();
            detToInsert.JournalDetEvidenceNo = trans.TransFactur;
            detToInsert.JournalDetAmmount = ammount;
            detToInsert.JournalDetNo = 0;
            detToInsert.JournalDetDesc = desc;
            detToInsert.CreatedBy = UserName;
            detToInsert.CreatedDate = DateTime.Now;
            detToInsert.DataStatus = Enums.EnumDataStatus.New.ToString();
            journal.JournalDets.Add(detToInsert);
        }
    }
}
