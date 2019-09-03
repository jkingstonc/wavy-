using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using wavynet.file.wcformat;

namespace wavynet.file
{
    class WCWriter
    {
        public static char BLOCK_SEPERATOR_CHAR = '?';
        public static char NEWLINE_CHAR = '\n';

        public static void write(WC wc)
        {

        }

        public static void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }


            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, serializableObject);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(fileName);
            }
        }
    }
}
