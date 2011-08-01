﻿using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SharpArch.Data.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;

namespace YTech.IM.JSM.Data.Repository
{
    public class MCustomerPriceRepository : NHibernateRepositoryWithTypedId<MCustomerPrice, string>, IMCustomerPriceRepository
    {
        #region Implementation of IMItemUomRepository

        public MCustomerPrice GetByItem(MItem item)
        {
            ICriteria criteria = Session.CreateCriteria(typeof(MCustomerPrice));
            criteria.Add(Expression.Eq("ItemId", item));
            return criteria.UniqueResult() as MCustomerPrice;
        }

        #endregion
    }
}
