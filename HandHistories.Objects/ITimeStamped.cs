using System;

namespace HandHistories.Objects
{
    public interface ITimeStamped
    {
        DateTime TimeStampUtc { get; }
    }
}