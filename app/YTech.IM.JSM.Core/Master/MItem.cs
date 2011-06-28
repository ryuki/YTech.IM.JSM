using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;

namespace YTech.IM.JSM.Core.Master
{
    public class MItem : EntityWithTypedId<string>, IHasAssignedId<string>
    {
        public MItem()
        {
            InitMembers();
        }

        /// <summary>
        /// Since we want to leverage automatic properties, init appropriate members here.
        /// </summary>
        private void InitMembers()
        {
            ItemUoms = new List<MItemUom>();
        }

        [DomainSignature]
        [NotNull, NotEmpty]

        public virtual MItemCat ItemCatId { get; set; }
        public virtual MBrand BrandId { get; set; }
        public virtual string ItemName { get; set; }
        public virtual string ItemStatus { get; set; }
        public virtual byte[] ItemPhoto { get; set; }
        public virtual string ItemDesc { get; set; }

        public virtual string DataStatus { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual byte[] RowVersion { get; set; }

        public virtual IList<MItemUom> ItemUoms { get; protected set; }

        #region Implementation of IHasAssignedId<string>

        public virtual void SetAssignedIdTo(string assignedId)
        {
            Check.Require(!string.IsNullOrEmpty(assignedId), "Assigned Id may not be null or empty");
            Id = assignedId.Trim();
        }

        #endregion
    }
}
