using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace YTech.IM.JSM.Data.NHibernateMaps.Conventions
{
    public class HasManyToManyConvention : IHasManyToManyConvention
    {
        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Cascade.SaveUpdate();
        }
    }
}
