using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.Cards
{
    public enum SuitEnum : byte
    {
        Clubs = 0x0,
        Diamonds = 0x1,
        Hearts = 0x2,
        Spades = 0x3,
        Unknown = 0xFF,
    }
}
