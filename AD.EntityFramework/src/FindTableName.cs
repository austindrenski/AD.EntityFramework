using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using JetBrains.Annotations;

namespace AD.EntityFramework
{
    /// <summary>
    /// Extension methods to find the table name of a <see cref="DbSet"/> inside of a <see cref="DbContext"/>.
    /// </summary>
    [PublicAPI]
    public static class FindTableNameExtensions
    {
        /// <summary>
        /// Returns the string table name of the <see cref="DbSet"/> from the <see cref="DbContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> to search.</param>
        /// <param name="entity">The <see cref="DbSet"/> for which to search.</param>
        /// <returns>The string table name of the <see cref="DbSet"/>.</returns>
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
