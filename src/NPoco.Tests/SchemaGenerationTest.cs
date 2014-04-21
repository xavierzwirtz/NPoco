using System;
using System.Linq;
using System.Collections.Generic;
using NPoco.Tests.Common;
using NUnit.Framework;

namespace NPoco.Tests
{
    [TestFixture]
    //[NUnit.Framework.Ignore("Appearently the decorated syntax and fluent syntax are some how conflicting.")]
    public class SchemaGenerationTest : BaseDBDecoratedTest
    {
        [Test]
        public void SimpleSchemaTest()
        {

            var tableInfo = new TableInfo();

            tableInfo.TableName = "simple";

            var columns = new Dictionary<String, PocoColumn>();

            var col = new PocoColumn();

            col.ColumnName = "string1";
            var pocoData = new MockPocoData(tableInfo);

            AddColumn(pocoData, "string", typeof(string));
            AddColumn(pocoData, "byte", typeof(byte[]));
            AddColumn(pocoData, "bool", typeof(bool));
            AddColumn(pocoData, "decimal", typeof(decimal));
            AddColumn(pocoData, "datetime", typeof(DateTime));
            AddColumn(pocoData, "double", typeof(double));
            AddColumn(pocoData, "guid", typeof(Guid));
            AddColumn(pocoData, "int", typeof(int));
            AddColumn(pocoData, "short", typeof(short));
            AddColumn(pocoData, "long", typeof(long));
            AddColumn(pocoData, "single", typeof(Single));


            Database.CreateSchema(pocoData);

            EnsureSchemaMatch(Database, pocoData);

        }

        [Test]
        public void AddIdentityColumnTest()
        {

            var tableInfo = new TableInfo();

            tableInfo.TableName = "identited";
            
            var pocoData = new MockPocoData(tableInfo);

            var col = new PocoColumn();

            col.ColumnName = "string1";

            AddColumn(pocoData, "data", typeof(string));

            Database.CreateSchema(pocoData);

            EnsureSchemaMatch(Database, pocoData);


            AddColumn(pocoData, "id", typeof(int), true, 1, 5);

            Assert.Throws<NPoco.UnsafeSchemaModificationException>(() => Database.CreateSchema(pocoData));

            pocoData.Columns.Remove("id");

            EnsureSchemaMatch(Database, pocoData);

        }

        void AddColumn(MockPocoData pocoData, string columnName, Type columnType)
        {
            AddColumn(pocoData, columnName, columnType, false, 1, 1);
        }

        void AddColumn(MockPocoData pocoData, string columnName, Type columnType, bool identityColumn, int identitySeed, int identityIncrement)
        {
            var col = new PocoColumn();
            col.ColumnName = columnName;
            col.ColumnType = columnType;
            col.IdentityColumn = identityColumn;
            col.IdentitySeed = identitySeed;
            col.IdentityIncrement = identityIncrement;

            col.TableInfo = pocoData.TableInfo;

            pocoData.Columns.Add(columnName, col);
        }

        void EnsureSchemaMatch(IDatabase db, IPocoData pocoData)
        {
            var dbType = db.DatabaseType;

            if (dbType is NPoco.DatabaseTypes.SqlServerDatabaseType)
            {
                //var columns = Database.FetchBy<Common.InformationSchema.Column>(y => y.Where(y => (y.TABLE_NAME == "test")));
                var columns = Database.FetchBy<Common.InformationSchema.Column>(y => y.Where(x => x.TABLE_NAME == pocoData.TableInfo.TableName));

                Assert.AreEqual(columns.Count, pocoData.Columns.Count);
                foreach (var pocoColumn in pocoData.Columns)
                {

                    var realColumn = (
                        from col in columns
                        where col.COLUMN_NAME == pocoColumn.Value.ColumnName
                        select col).First();

                    string dataType = realColumn.DATA_TYPE.ToLower();

                    if (pocoColumn.Value.ColumnType == typeof (string))
                    {
                        Assert.AreEqual("varchar", dataType);
                    }
                    else if (pocoColumn.Value.ColumnType == typeof (byte[]))
                    {
                        Assert.AreEqual("image", dataType);
                    }
                    else if (pocoColumn.Value.ColumnType == typeof(bool))
                    {
                        Assert.AreEqual("bit", dataType);
                    }
                    else if (pocoColumn.Value.ColumnType == typeof(decimal))
                    {
                        Assert.AreEqual("decimal", dataType);
                    }
                    else if (pocoColumn.Value.ColumnType == typeof(DateTime))
                    {
                        Assert.AreEqual("datetime", dataType);
                    }
                    else if (pocoColumn.Value.ColumnType == typeof(double))
                    {
                        Assert.AreEqual("float", dataType);
                    }
                    else if (pocoColumn.Value.ColumnType == typeof(Guid))
                    {
                        Assert.AreEqual("uniqueidentifier", dataType);
                    }
                    else if (pocoColumn.Value.ColumnType == typeof(short))
                    {
                        Assert.AreEqual("smallint", dataType);
                    }
                    else if (pocoColumn.Value.ColumnType == typeof(int))
                    {
                        Assert.AreEqual("int", dataType);
                    }
                    else if (pocoColumn.Value.ColumnType == typeof(long))
                    {
                        Assert.AreEqual("bigint", dataType);
                    }
                    else if (pocoColumn.Value.ColumnType == typeof(Single))
                    {
                        Assert.AreEqual("real", dataType);
                    }
                    else
                    {
                        throw new NotSupportedException(pocoColumn.Value.ColumnType.Name);
                    }
                    
                    NPoco.DatabaseTypes.SqlServerDatabaseType sqlType = (NPoco.DatabaseTypes.SqlServerDatabaseType) dbType;

                    bool isIdentited = sqlType.IsIdentityColumn(db, pocoColumn.Value);
                    
                    Assert.AreEqual(pocoColumn.Value.IdentityColumn, isIdentited);
                }
            }
            else
            {
                throw new NotImplementedException(dbType.GetType().Name);
            }

        }
    }
}
