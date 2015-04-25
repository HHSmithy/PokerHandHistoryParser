using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.GameDescription
{
    public static class TableTypeUtils
    {

        public static string GetDisplayString(TableTypeDescription tableType)
        {
            switch (tableType)
            {
                case TableTypeDescription.P2:
                    return "2 Players";
                case TableTypeDescription.P4:
                    return "4 Players";
                case TableTypeDescription.P12:
                    return "12 Players";
                case TableTypeDescription.P16:
                    return "16 Players";
                case TableTypeDescription.P18:
                    return "18 Players";
                case TableTypeDescription.P27:
                    return "27 Players";
                case TableTypeDescription.P32:
                    return "32 Players";
                case TableTypeDescription.P45:
                    return "45 Players";
                case TableTypeDescription.P90:
                    return "90 Players";
                case TableTypeDescription.P180:
                    return "180 Players";
                case TableTypeDescription.P240:
                    return "240 Players";
                case TableTypeDescription.P360:
                    return "360 Players";
                case TableTypeDescription.P990:
                    return "990 Players";
                case TableTypeDescription.HyperTurbo:
                    return "HyperTurbo";
                default:
                    return tableType.ToString();
            }
        }

        public static TableTypeDescription ParseTableType(string tableType)
        {
            if (string.IsNullOrEmpty(tableType))
            {
                return TableTypeDescription.Unknown;
            }

            switch (tableType.ToLower())
            {
                case "hyper-turbo":
                case "hyperturbo":
                    return TableTypeDescription.HyperTurbo;
                    
                case "2players":
                case "2 players":
                    return TableTypeDescription.P2;
                case "4players":
                case "4 players":
                    return TableTypeDescription.P4;
                case "12players":
                case "12 players":
                    return TableTypeDescription.P12;
                case "16players":
                case "16 players":
                    return TableTypeDescription.P16;
                case "18players":
                case "18 players":
                    return TableTypeDescription.P18;
                case "27players":
                case "27 players":
                    return TableTypeDescription.P27;
                case "32players":
                case "32 players":
                    return TableTypeDescription.P32;
                case "45players":
                case "45 players":
                    return TableTypeDescription.P45;
                case "90players":
                case "90 players":
                    return TableTypeDescription.P90;
                case "180players":
                case "180 players":
                    return TableTypeDescription.P180;
                case "240players":
                case "240 players":
                    return TableTypeDescription.P240;
                case "360players":
                case "360 players":
                    return TableTypeDescription.P360;
                case "990players":
                case "990 players":
                    return TableTypeDescription.P990;
                default:
                    string match = Enum.GetNames(typeof(TableTypeDescription)).FirstOrDefault(s => s.ToLower().Equals(tableType.ToLower()));
                    return match == null ? TableTypeDescription.Unknown : (TableTypeDescription)Enum.Parse(typeof(TableTypeDescription), match, true);
            }
        }
    }
}
