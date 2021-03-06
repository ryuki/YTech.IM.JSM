﻿using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SharpArch.Data.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;
using YTech.IM.JSM.Core.Transaction.Payment;

namespace YTech.IM.JSM.Data.Repository
{
    public class TPaymentDetRepository : NHibernateRepositoryWithTypedId<TPaymentDet, string>, ITPaymentDetRepository
    {
    }
}
