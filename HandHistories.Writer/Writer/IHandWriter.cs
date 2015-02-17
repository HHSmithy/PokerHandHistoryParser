using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandHistories.Writer.Writer
{
    public interface IHandWriter
    {
        SiteName SiteName { get; }

        string Write(HandHistory hand);
    }
}
