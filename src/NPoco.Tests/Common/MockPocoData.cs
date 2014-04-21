using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPoco.Tests.Common
{
    class MockPocoData : IPocoData
    {

        private TableInfo _tableInfo;
        private Dictionary<string, PocoColumn> _columns;
        private Dictionary<string, PocoColumn> _queryColumns;

        public MockPocoData(TableInfo tableInfo): this(tableInfo, null) {
           
        }

        public MockPocoData(TableInfo tableInfo, Dictionary<string, PocoColumn> queryColumns)
        {

            _tableInfo = tableInfo;
            _columns = new Dictionary<string,PocoColumn>();

            foreach (var column in _columns)
            {
                column.Value.TableInfo = tableInfo;
            }

            if (queryColumns == null)
            {
                _queryColumns = new Dictionary<string, PocoColumn>();
            }
            else
            {
                _queryColumns = queryColumns;
            }
        }

        public Dictionary<string, PocoColumn> QueryColumns
        {
            get { return _queryColumns; }
        }

        public TableInfo TableInfo
        {
            get { return _tableInfo; }
        }

        public Dictionary<string, PocoColumn> Columns
        {
            get { return _columns; }
        } 
    }
}
