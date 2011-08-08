using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using SharpArch.Core;
using SharpArch.Web.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;
using YTech.IM.JSM.Data.Repository;
using YTech.IM.JSM.Enums;

namespace YTech.IM.JSM.Web.Controllers.Master
{
    [HandleError]
    public class CustomerPriceController : Controller
    {
        private readonly IMCustomerPriceRepository _mCustomerPriceRepository;
        private readonly IMItemRepository _mitemRepository;
        public CustomerPriceController(IMCustomerPriceRepository mCustomerPriceRepository, IMItemRepository mItemRepository)
        {
            Check.Require(mCustomerPriceRepository != null, "mCustomerPriceRepository may not be null");
            Check.Require(mItemRepository != null, "mItemRepository may not be null");

            this._mCustomerPriceRepository = mCustomerPriceRepository;
            this._mitemRepository = mItemRepository;
        }

        [Transaction]
        public ActionResult ListSubGrid(string itemId)
        {
            var customerPrices = _mCustomerPriceRepository.GetListByItemId(itemId);

            var jsonData = new
            {
                rows = (
                    from cp in customerPrices
                    select new
                    {
                        i = cp.Id.ToString(),
                        cell = new string[]
                        {
                            cp.Id,
                           cp.CustomerId != null ? cp.CustomerId.Id : null,
                           cp.CustomerId != null ? cp.CustomerId.PersonId.PersonName : null,
                            Helper.CommonHelper.ConvertToString(cp.Price)
                        }
                    }
                ).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertSubGrid(MCustomerPrice viewModel, FormCollection formCollection, string itemId)
        {
            try
            {
                MCustomerPrice cp = new MCustomerPrice();
                cp.CustomerId = viewModel.CustomerId;
                cp.ItemId = _mitemRepository.Get(itemId);
                cp.Price = Helper.CommonHelper.ConvertToDecimal(formCollection["Price"]);
                cp.SetAssignedIdTo(Guid.NewGuid().ToString());
                cp.CreatedDate = DateTime.Now;
                cp.CreatedBy = User.Identity.Name;
                cp.DataStatus = EnumDataStatus.New.ToString();

                _mCustomerPriceRepository.Save(cp);

                _mCustomerPriceRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {
                _mCustomerPriceRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("Data Harga Konsumen berhasil disimpan.");
        }


        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateSubGrid(MCustomerPrice viewModel, FormCollection formCollection, string itemId)
        {
            try
            {
                MCustomerPrice cp = _mCustomerPriceRepository.Get(viewModel.Id);
                cp.CustomerId = viewModel.CustomerId;
                cp.ItemId = _mitemRepository.Get(itemId);
                cp.Price = Helper.CommonHelper.ConvertToDecimal(formCollection["Price"]);
                cp.ModifiedDate = DateTime.Now;
                cp.ModifiedBy = User.Identity.Name;
                cp.DataStatus = EnumDataStatus.Updated.ToString();

                _mCustomerPriceRepository.Save(cp);

                _mCustomerPriceRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {
                _mCustomerPriceRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("Data Harga Konsumen berhasil disimpan");
        }

        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteSubGrid(MCustomerPrice viewModel, FormCollection formCollection, string itemId)
        {
            try
            {
                MCustomerPrice cp = _mCustomerPriceRepository.Get(viewModel.Id);
                _mCustomerPriceRepository.Delete(cp);

                _mCustomerPriceRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {
                _mCustomerPriceRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("Data Harga Konsumen berhasil dihapus.");
        }
    }
}
