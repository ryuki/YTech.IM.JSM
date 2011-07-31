using System.Collections;
using System.Collections.Generic;
using SharpArch.Core.PersistenceSupport.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Core.RepositoryInterfaces
{
    public interface ITTransDetRepository : INHibernateRepositoryWithTypedId<TTransDet, string>
    {
        IList<TTransDet> GetByItemWarehouse(MItem item, MWarehouse warehouse);

        decimal? GetTotalUsed(MItem item, MWarehouse warehouse);


        IList<TTransDet> GetListByTransId(string transId, Enums.EnumTransactionStatus enumTransactionStatus);

        IList<TTransDet> GetListByTrans(TTrans trans);

        IList<TTransDet> GetListByDate(Enums.EnumTransactionStatus enumTransactionStatus, System.DateTime? dateFrom, System.DateTime? dateTo);

        IList<TTransDet> GetListById(object[] detailIdList);

        void DeleteById(object[] detailIdList);

        IList<TTransDet> GetByDateWarehouse(System.DateTime? dateFrom, System.DateTime? dateTo, MWarehouse warehouse, string transStatus);

        IList<TTransDet> GetByDateWarehouseTransBy(System.DateTime? dateFrom, System.DateTime? dateTo, string transBy, string warehouseId, string transStatus);

        IList GetRefStockByDateWarehouseTransBy(System.DateTime? dateFrom, System.DateTime? dateTo, string transBy, string warehouseId, string transStatus);
    }
}
