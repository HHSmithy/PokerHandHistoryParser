using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HandHistories.CommonObjects
{
    [DataContract]
    [System.Flags()]
    public enum TableTypes : int 
    {
        [EnumMember] Regular = 1,
        [EnumMember] Anonymous = 2,
        [EnumMember] SuperSpeed = 4,
        [EnumMember] Deep = 8,
        [EnumMember] Ante = 16,
        [EnumMember] Cap = 32,
        [EnumMember] Speed = 64,
        [EnumMember] Jackpot = 128,
        [EnumMember] SevenDeuceGame = 256,
        [EnumMember] FiftyBigBlindsMin = 512,
        [EnumMember] Shallow = 1024,
        [EnumMember] ALL = 2048,
        [EnumMember] Unknown = 2048,
    }

    [Serializable]
    [DataContract]
    public class TableType
    {
        [DataMember]
        private TableTypes _type;
        
        private TableType()
        {
            
        }

        public TableType(TableTypes type)
        {
            _type = type;
        }       

        public bool IsTableTypeIncluding(TableTypes tableType)
        {
             var types = Enum.GetValues(typeof(TableTypes)).Cast<TableTypes>().ToArray();
             for (int i = 0; i < types.Count(); i++)
             {
                if (types[i] == tableType)
                {
                    bool isFlipped = ((long)_type & (1 << i)) != 0;

                    return isFlipped;                    
                }
             }

            return false;
        }        

        public bool IsTableTypeCovered(List<TableType> filters)
        {
            bool isTableType = true;

            var types = Enum.GetValues(typeof(TableTypes)).Cast<TableTypes>().ToArray();
            for (int i = 0; i < types.Count(); i++)
            {
                bool isFlipped = ((long)_type & (1 << i)) != 0;

                if (isFlipped)
                {
                    isTableType = isTableType && (filters.Any(f => f.Type == types[i]));
                }
            }

            return isTableType;
        }

        public static bool IsTableTypeIncluding(TableType toCheck, TableTypes tableType)
        {
            var types = Enum.GetValues(typeof(TableTypes)).Cast<TableTypes>().ToArray();
            for (int i = 0; i < types.Count(); i++)
            {
                if (types[i] == tableType)
                {
                    bool isFlipped = ((long)toCheck.Type & (1 << i)) != 0;

                    return isFlipped;
                }
            }

            return false;
        }

        public static bool IsTableTypeCovered(List<TableType> filters, TableType toCheck)
        {
            bool isTableType = true;

            var types = Enum.GetValues(typeof(TableTypes)).Cast<TableTypes>().ToArray();
            for (int i = 0; i < types.Count(); i++)
            {
                bool isFlipped = ((long)toCheck.Type & (1 << i)) != 0;

                if (isFlipped)
                {
                    isTableType = isTableType && (filters.Any(f => f.Type == types[i]));
                }
            }

            return isTableType;
        }

        public TableTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public override string ToString()
        {
            List<string> typesList = new List<string>();

            int tableInt = (int)_type;

            var types = Enum.GetValues(typeof(TableTypes)).Cast<TableTypes>().ToArray();

            for (int i = 0; i < types.Count(); i++)
            {
                if ((tableInt & (1 << i)) != 0)
                    typesList.Add(types[i].ToString());
            }

            return string.Join(" ", typesList.ToArray());
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            return obj.ToString().Equals(ToString());
        }        

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
}
