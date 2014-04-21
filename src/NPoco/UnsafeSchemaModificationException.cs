using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPoco
{
    public class UnsafeSchemaModificationException : Exception
    {
        private PocoColumn _pocoColumn;
        public UnsafeSchemaModificationException(PocoColumn pocoColumn)
        {
            _pocoColumn = pocoColumn;
        }

        public PocoColumn PocoColumn
        {
            get { return _pocoColumn; }
        }

    }
}
