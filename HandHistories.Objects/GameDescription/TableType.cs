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

            return FromTableTypeDescriptions(
                tableTypeDescriptionStrings
                    .Select(t => (TableTypeDescription)Enum.Parse(typeof(TableTypeDescription), t))
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
        public readonly TableTypeDescription _tableTypeDescriptions;

        private TableType(params TableTypeDescription [] tableTypeDescriptions)
        {
            // Alphabetize them and take the distincts
            _tableTypeDescriptions = 0;
            foreach (var item in tableTypeDescriptions)
	        {
                _tableTypeDescriptions |= item;
            }
        }

        public override string ToString()
        {
            return _tableTypeDescriptions.ToString();
        }

        public bool HasTypeDescription(TableTypeDescription description)
        {
            return _tableTypeDescriptions.HasFlag(description);
        }

        #region IEnumerable Implementation
        public IEnumerable<TableTypeDescription> GetTableTypeDescriptions()
        {
            foreach (TableTypeDescription value in TableTypeDescription.GetValues(_tableTypeDescriptions.GetType()))
                if (_tableTypeDescriptions.HasFlag(value))
                    yield return value;
        }

        public IEnumerator<TableTypeDescription> GetEnumerator()
        {
            return GetTableTypeDescriptions().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetTableTypeDescriptions().GetEnumerator();
        } 
        #endregion
    }
}
