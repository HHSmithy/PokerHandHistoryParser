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
    public class TableType : IEnumerable<TableTypeDescription>
    {
        [DataMember]
        private readonly List<TableTypeDescription> _tableTypeDescriptions;

        private TableType(params TableTypeDescription [] tableTypeDescriptions)
        {
            // Alphabetize them and take the distincts
            _tableTypeDescriptions = new List<TableTypeDescription>(tableTypeDescriptions.OrderBy(t => t.ToString()).Distinct());
        }

        private static readonly Regex ParseRegex = new Regex("[-,;_]");
        public static TableType Parse(string tableType)
        {
            List<string> tableTypeDescriptionStrings = ParseRegex.Split(tableType).ToList();

            return FromTableTypeDescriptions(
                tableTypeDescriptionStrings
                    .Select(t => (TableTypeDescription)Enum.Parse(typeof(TableTypeDescription), t))
                    .ToArray()
                );           
        }

        public static TableType FromTableTypeDescriptions(params TableTypeDescription [] tableTypeDescriptions)
        {
            return new TableType(tableTypeDescriptions);
        }

        public IEnumerable<TableTypeDescription> GetTableTypeDescriptions()
        {
            return _tableTypeDescriptions;
        }

        public IEnumerator<TableTypeDescription> GetEnumerator()
        {
            return _tableTypeDescriptions.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join("-", _tableTypeDescriptions);
        }

        public override bool Equals(object obj)
        {
            TableType tableType = obj as TableType;
            if (tableType == null)
            {
                return false;
            }

            // See if contains all the same elements
            //  http://stackoverflow.com/questions/1673347/linq-determine-if-two-sequences-contains-exactly-the-same-elements
            return new HashSet<TableTypeDescription>(tableType.GetTableTypeDescriptions()).SetEquals(this.GetTableTypeDescriptions());
        }        

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
