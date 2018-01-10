using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils
{
    public class HandIntegrityException : Exception
    {
        internal HandIntegrityException(string message)
            : base(message)
        {
        }
    }
}
