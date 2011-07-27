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

using YTech.IM.JSM.Core.Transaction.Payment;
using YTech.IM.JSM.Enums;

namespace YTech.IM.JSM.Web.Controllers.ViewModel
{
    public class PaymentViewModel
    {
        public static PaymentViewModel Create(EnumPaymentType paymentType,ITPaymentRepository tPaymentRepository, ITPaymentDetRepository tPaymentDetRepository)
        {
            PaymentViewModel viewModel = new PaymentViewModel();

            TPayment p = new TPayment();
            p.SetAssignedIdTo(Guid.NewGuid().ToString());
            p.PaymentDate = DateTime.Today;
            p.PaymentDesc = string.Empty;
            p.PaymentType = paymentType.ToString();

            viewModel.Payment = p;

            viewModel.Title = string.Format("Pembayaran {0}", paymentType.ToString());

            //get label text
            switch (paymentType)
            {
                case EnumPaymentType.Piutang :
                    viewModel.CashAccountLabel = "Deposit ke : ";
                    break;
                case EnumPaymentType.Hutang :
                    viewModel.CashAccountLabel = "Deposit dari : ";
                    break;
            }

            return viewModel;
        }

        public TPayment Payment { get; internal set; }
        public string CashAccountId { get; internal set; }
        public string CashAccountName { get; internal set; }
        public string CashAccountLabel { get; internal set; }
        public string SelectedTransId { get; internal set; }

        public string Title { get; internal set; }
    }
}
