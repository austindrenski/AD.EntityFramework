using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using JetBrains.Annotations;

namespace AD.EntityFramework
{
    [PublicAPI]
    public static class FindTableNameExtensions
    {
        public static string FindTableName(this DbContext context, DbSet entity)
        {
            MetadataWorkspace metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;
            ObjectItemCollection objectItemCollection = (ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace);
            EntityType entityType = metadata.GetItems<EntityType>(DataSpace.OSpace)
                                            .Single(e => objectItemCollection.GetClrType(e) == entity.ElementType);
            EntitySet entitySet = metadata.GetItems<EntityContainer>(DataSpace.CSpace)
                                          .Single()
                                          .EntitySets
                                          .Single(x => x.ElementType.Name == entityType.Name);
            EntitySetMapping mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                                               .Single()
                                               .EntitySetMappings
                                               .Single(x => x.EntitySet == entitySet);
            IEnumerable<MappingFragment> tables = mapping.EntityTypeMappings
                                                         .Single()
                                                         .Fragments;
            return tables.Select(x => (string)x.StoreEntitySet.MetadataProperties["Table"].Value)
                         .SingleOrDefault();
        }
    }
}
