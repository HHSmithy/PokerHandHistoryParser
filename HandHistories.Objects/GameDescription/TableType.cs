using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace HandHistories.Objects.GameDescription
{
    [DataContract]
    [Serializable]
    public struct TableType : IEnumerable<TableTypeDescription>
    {
        #region Statics
        private static readonly Regex ParseRegex = new Regex("[-,;_]");

        public static TableType Parse(string tableType)
        {
            List<string> tableTypeDescriptionStrings = ParseRegex.Split(tableType).ToList();

            return new TableType(
                tableTypeDescriptionStrings
                    .Select(t => (TableTypeDescription)Enum.Parse(typeof(TableTypeDescription), t, true))
                    .Distinct()
                    .ToArray()
                );           
        }

        public static TableType FromTableTypeDescriptions(params TableTypeDescription[] tableTypeDescriptions)
        {
            return new TableType(tableTypeDescriptions);
        }
        #endregion

        [DataMember]
        private readonly TableTypeDescription _tableTypeDescriptions;

        public TableType(params TableTypeDescription[] tableTypeDescriptions)
        {
            _tableTypeDescriptions = TableTypeDescription.Unknown;
            foreach (var item in tableTypeDescriptions)
            {
                _tableTypeDescriptions |= item;
            }
        }

        public TableType(IEnumerable<TableTypeDescription> tableTypeDescriptions)
        {
            _tableTypeDescriptions = TableTypeDescription.Unknown;
            foreach (var item in tableTypeDescriptions)
            {
                _tableTypeDescriptions |= item;
            }
        }

        public IEnumerator<TableTypeDescription> GetEnumerator()
        {
            return GetTableTypeDescriptions().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetTableTypeDescriptions().GetEnumerator();
        }

        public IEnumerable<TableTypeDescription> GetTableTypeDescriptions()
        {
            return _tableTypeDescriptions.ToString()
                  .Split(new[] { ", " }, StringSplitOptions.None)
                  .Select(v => (TableTypeDescription)Enum.Parse(typeof(TableTypeDescription), v));
        }

        public override string ToString()
        {
            return _tableTypeDescriptions.ToString().Replace(", ", "-");
        }
    }
}
