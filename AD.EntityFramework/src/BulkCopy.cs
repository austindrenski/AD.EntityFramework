using System;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AD.IO;
using JetBrains.Annotations;
using Npgsql;

namespace AD.EntityFramework
{
    /// <summary>
    /// Extension methods to expose the PostgreSQL COPY command for the bulk import of data.
    /// </summary>
    [PublicAPI]
    public static class BulkCopyExtensions 
    {
        /// <summary>
        /// Imports data records from an XDocument to the specified table. 
        /// First, existing rows are deleted, then new records are imported.
        /// This occurs within a transaction.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> that encapsulates the necessary tables and credentials.</param>
        /// <param name="schema">The name of the schema for the table.</param>
        /// <param name="entity">The entity to which records are copied.</param>
        /// <param name="file">An <see cref="XmlFilePath"/> wherein the children of the root element are record elements.</param>
        public static int BulkCopy(this DbContext context, string schema, DbSet entity, XmlFilePath file)
        {
            string table = context.FindTableName(entity);
            return BulkCopy(context, schema, table, file);
        }

        /// <summary>
        /// Imports data records from an XDocument to the specified table. 
        /// First, existing rows are deleted, then new records are imported.
        /// This occurs within a transaction.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> that encapsulates the necessary tables and credentials.</param>
        /// <param name="schema">The name of the schema for the table.</param>
        /// <param name="file">An <see cref="XmlFilePath"/> wherein the children of the root element are record elements.</param>
        /// <param name="table">The name of the table to which records are copied.</param>
        public static int BulkCopy(this DbContext context, string schema, string table, XmlFilePath file)
        {
            XDocument document = XDocument.Load(file);
            int count = document.Root?.Elements().Count() ?? 0;
            if (count == 0)
            {
                Console.WriteLine($@">> {DateTime.Now.TimeOfDay}: No records found in {Path.GetFileName(file)}.");
                Console.WriteLine($@">> {DateTime.Now.TimeOfDay}: No records written to {schema}.{table}.");
                return count;
            }
            context.Database.Connection.Open();
            string fields = document.Root?.Elements().FirstOrDefault()?.Elements().Select(x => x.Name.LocalName).ToDelimited(",").ToLower();
            using (DbTransaction transaction = context.Database.Connection.BeginTransaction())
            {
                try
                {
                    context.Database.ExecuteSqlCommand($"DELETE FROM {schema}.{table}");
                    using (TextWriter writer = ((NpgsqlConnection)context.Database.Connection).BeginTextImport($"COPY {schema}.{table} ({fields}) FROM STDIN"))
                    {
                        foreach (XElement record in document.Root?.Elements() ?? new XElement[0])
                        {
                            writer.WriteLine(record.ToDelimited("\t"));
                        }
                    }
                    //using (NpgsqlBinaryImporter writer = ((NpgsqlConnection)context.Database.Connection).BeginBinaryImport($"COPY {schema}.{table} ({fields}) FROM STDIN (FORMAT BINARY)"))
                    //{
                    //    foreach (XElement record in document.Root?.Elements() ?? new XElement[0])
                    //    {
                    //        writer.StartRow();
                    //        foreach (XElement property in record.Elements())
                    //        {
                    //            switch (property.Name.ToString())
                    //            {
                    //                case "TradeValue":
                    //                {
                    //                    writer.Write(property.ToLong() ?? 0, NpgsqlTypes.NpgsqlDbType.Bigint);
                    //                    break;
                    //                }
                    //                case "Quantity":
                    //                {
                    //                    writer.Write(property.ToLong() ?? 0, NpgsqlTypes.NpgsqlDbType.Bigint);
                    //                    break;
                    //                }
                    //                default:
                    //                {
                    //                    writer.Write(property.Value);
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    transaction.Commit();
                    Console.WriteLine($@">> {DateTime.Now.TimeOfDay}: {count} records written to {schema}.{table}.");
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    Console.WriteLine($@">> {DateTime.Now.TimeOfDay}: An error has occured while writing to {schema}.{table}.");
                    Console.WriteLine($@">> {DateTime.Now.TimeOfDay}: The operation was rolled back. No records were altered.");
                    Console.WriteLine($@">> {DateTime.Now.TimeOfDay}: Exception message: {exception.Message}.");
                }
            }
            context.Database.Connection.Close();
            return count;
        }
    }
}
