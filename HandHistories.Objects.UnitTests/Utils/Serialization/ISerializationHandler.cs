using System.Text;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HandHistories.Objects.UnitTests.Utils.Serialization
{
    public interface ISerializationHandler
    {
        string Serialize(object obj);

        T Deserialize<T>(string serializedString);
    }
}

