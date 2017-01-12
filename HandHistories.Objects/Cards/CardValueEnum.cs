using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.Cards
{
    public enum CardValueEnum : byte
    {
        _2 = 0,
        _3 = 1,
        _4 = 2,
        _5 = 3,
        _6 = 4,
        _7 = 5,
        _8 = 6,
        _9 = 7,
        _T = 8,
        _J = 9,
        _Q = 0xA,
        _K = 0xB,
        _A = 0xC,
        Unknown = 0xFF,
    }
}
