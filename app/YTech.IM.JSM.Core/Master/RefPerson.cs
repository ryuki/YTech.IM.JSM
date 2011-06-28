using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;

namespace YTech.IM.JSM.Core.Master
{
    public class RefPerson : EntityWithTypedId<string>, IHasAssignedId<string>
    {
        [DomainSignature]
        [NotNull, NotEmpty]

        public virtual string PersonFirstName { get; set; }
        public virtual string PersonLastName { get; set; }
        public virtual DateTime? PersonDob { get; set; }
        public virtual string PersonPob { get; set; }
        public virtual string PersonGender { get; set; }
        public virtual string PersonPhone { get; set; }
        public virtual string PersonMobile { get; set; }
        public virtual string PersonEmail { get; set; }
        public virtual string PersonReligion { get; set; }
        public virtual string PersonRace { get; set; }
        public virtual string PersonIdCardType { get; set; }
        public virtual string PersonIdCardNo { get; set; }
        public virtual string PersonDesc { get; set; }

        public virtual string DataStatus { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual byte[] RowVersion { get; set; }

        public virtual string PersonName
        {
            get
            {
                return string.Format("{0} {1}", this.PersonFirstName, this.PersonLastName);
            }
        }

        #region Implementation of IHasAssignedId<string>

        public virtual void SetAssignedIdTo(string assignedId)
        {
            Check.Require(!string.IsNullOrEmpty(assignedId), "Assigned Id may not be null or empty");
            Id = assignedId.Trim();
        }

        #endregion
    }
}
