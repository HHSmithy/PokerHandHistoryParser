using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace HandHistories.Objects.UnitTests.Utils.Serialization
{
    public class SerializationHandlerDataContractImpl : ISerializationHandler
    {
        public string Serialize(object objectToSerialize)
        {
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create(sb, settings);

            DataContractSerializer ds = new DataContractSerializer(objectToSerialize.GetType());
            ds.WriteObject(writer, objectToSerialize);

            writer.Flush();

            return sb.ToString();
        }

        public T Deserialize<T>(string serializedString)
        {
            DataContractSerializer ds = new DataContractSerializer(typeof(T));

            byte[] byteArray = Encoding.ASCII.GetBytes(serializedString);
            MemoryStream xmlStream = new MemoryStream(byteArray);

            return (T)ds.ReadObject(xmlStream);         
        }
    }
}