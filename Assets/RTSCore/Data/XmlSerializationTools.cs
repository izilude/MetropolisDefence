using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Assets.RTSCore.Data
{
    public enum XmlType { Element, Attribute}

    /// <summary>
    ///     Class to handle .NET serializing and deserializing operations.
    /// </summary>
    public static class XmlSerializationTools
    {
        private static readonly Hashtable Serializers = new Hashtable();

        public static TObject ReadFromString<TObject>(string xmlData)
        {
            if (string.IsNullOrEmpty(xmlData))
            {
                return default(TObject);
            }

            try
            {
                var serializer = GetSerializer(typeof(TObject));
                var xmlReader = XmlReader.Create(new StringReader(xmlData));
                var created = ((TObject)serializer.Deserialize(xmlReader));
                return created;
            }
            catch (Exception ex)
            {
                return default(TObject);
            }
        }

        public static TObject ReadFromString<TObject>(Type serializerType, string xmlData)
        {
            if (string.IsNullOrEmpty(xmlData))
            {
                return default(TObject);
            }

            try
            {
                var serializer = GetSerializer(serializerType);
                var xmlReader = XmlReader.Create(new StringReader(xmlData));
                var created = (TObject)serializer.Deserialize(xmlReader);
                return created;
            }
            catch (Exception ex)
            {
                return default(TObject);
            }
        }

        public static void WriteToFile(string filePath, string data, bool overwriteReadOnly)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.Write(data);
            }
        }

        public static void WriteToFile<TObject>(TObject obj, string filePath) where TObject : class
        {
            WriteToFile<TObject>(obj, filePath, true);
        }

        public static void WriteToFile<TObject>(TObject obj, string filePath, bool direct) where TObject : class
        {
            if (direct)
            {
                WriteToFileDirect(obj, filePath);                
            }
            else
            {
                string contents = WriteToString<TObject>(obj);
                WriteToFile(filePath, contents, true);
            }
        }

        public static string ReadFromFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }

        public static TObject ReadFromFile<TObject>(string filePath, bool direct)
        {
            if (!direct) return ReadFromFile<TObject>(filePath);

            var serializer = GetSerializer(typeof(TObject));

            using (var sr = new StreamReader(filePath))
            {
                return (TObject)serializer.Deserialize(sr);
            }
        }

        public static TObject ReadFromFile<TObject>(string filePath)
        {
            try
            {
                string data = ReadFromFile(filePath);
                return ReadFromString<TObject>(data);
            }
            catch (Exception ex)
            {
                return default(TObject);
            }
        }

        private static void WriteToFileDirect<TObject>(TObject obj, string filePath) where TObject : class
        {
            if (obj == null)
            {
                return;
            }


            var settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false, true); //  new UnicodeEncoding(false, false);
            settings.Indent = true;
            settings.OmitXmlDeclaration = false;
            var serializer = GetSerializer(typeof(TObject));

            using (var stringWriter = File.CreateText(filePath))
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, obj);
                }
            }
        }

        public static TObject Clone<TObject>(TObject original) where TObject : class
        {
            string data = WriteToString(original);
            var cloned = ReadFromString<TObject>(data);
            return cloned;
        }

        public static string WriteToString<TObject>(TObject obj) where TObject : class
        {
            if (obj == null)
            {
                return string.Empty;
            }

            var serializer = GetSerializer(typeof (TObject));

            using (var sw = new Utf8StringWriter())
            {
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }

        private static XmlSerializer GetSerializer(Type type)
        {
            var key = new { Type = type };
            var serializer = (XmlSerializer)Serializers[key];
            if (null == serializer)
            {
                lock (Serializers)
                {
                    serializer = new XmlSerializer(type);

                    serializer.UnknownAttribute += SerializerOnUnknownAttribute;
                    serializer.UnknownElement += SerializerOnUnknownElement;
                    serializer.UnknownNode += SerializerOnUnknownNode;
                    serializer.UnreferencedObject += SerializerOnUnreferencedObject;

                    Serializers.Add(key, serializer);
                }
            }
            return serializer;
        }

        private static void SerializerOnUnreferencedObject(object sender, UnreferencedObjectEventArgs unreferencedObjectEventArgs)
        {
            
        }

        private static void SerializerOnUnknownNode(object sender, XmlNodeEventArgs xmlNodeEventArgs)
        {
            
        }

        private static void SerializerOnUnknownElement(object sender, XmlElementEventArgs xmlElementEventArgs)
        {
            var deserializedObject = (xmlElementEventArgs.ObjectBeingDeserialized as IBackwardCompatibilitySerializer);
            if (deserializedObject == null) return;
            deserializedObject.OnUnknownElementFound(xmlElementEventArgs.Element.Name, xmlElementEventArgs.Element.InnerText);
        }

        private static void SerializerOnUnknownAttribute(object sender, XmlAttributeEventArgs xmlAttributeEventArgs)
        {
            var deserializedObject = (xmlAttributeEventArgs.ObjectBeingDeserialized as IBackwardCompatibilitySerializer);
            if (deserializedObject == null) return;
            deserializedObject.OnUnknownElementFound(xmlAttributeEventArgs.Attr.Name, xmlAttributeEventArgs.Attr.InnerText);
        }
    }

    /// <summary>
    ///     Class to handle writing strings in UTF8 format
    /// </summary>
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
