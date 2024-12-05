using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Security.Cryptography;

namespace FP
{
    public class XmlSerializableDictioanry<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region "IXmlSerializable methods"

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keys = new XmlSerializer(typeof(TKey));
            XmlSerializer values = new XmlSerializer(typeof(TValue));

            bool bEmpty = reader.IsEmptyElement;
            reader.Read();

            if (bEmpty)
                return;
            
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keys.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)values.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadEndElement();

                this.Add(key, value);
            }

            reader.ReadEndElement(); // </XmlSerializableDictioanry>
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keys = new XmlSerializer(typeof(TKey));
            XmlSerializer values = new XmlSerializer(typeof(TValue));

            foreach (KeyValuePair<TKey,TValue> pair in this)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keys.Serialize(writer, pair.Key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                values.Serialize(writer, pair.Value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
        #endregion
    }

    public class ScanResult : IXmlSerializable
    {
        /// <summary>
        /// Unique name for this scan result
        /// </summary>
        public string Name;

        public string Value;

        public int Weight;

        public int Uniqueness;

        public object Tag;

        #region "IXmlSerializable methods"

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer strings = new XmlSerializer(typeof(string));
            XmlSerializer ints = new XmlSerializer(typeof(int));
            XmlSerializer objs = new XmlSerializer(typeof(object));

            bool bEmpty = reader.IsEmptyElement;
            reader.Read();

            if (bEmpty)
                return;

            reader.ReadStartElement("name");
            Name = (string)strings.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("value");
            Value = (string)strings.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("weight");
            Weight = (int)ints.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("uniqueness");
            Uniqueness = (int)ints.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("tag");
            Tag = (object)objs.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadEndElement(); // </ScanResult>
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer strings = new XmlSerializer(typeof(string));
            XmlSerializer ints = new XmlSerializer(typeof(int));
            XmlSerializer objs = new XmlSerializer(typeof(object));

            writer.WriteStartElement("name");
            strings.Serialize(writer, Name);
            writer.WriteEndElement();

            writer.WriteStartElement("value");
            strings.Serialize(writer, Value);
            writer.WriteEndElement();

            writer.WriteStartElement("weight");
            ints.Serialize(writer, Weight);
            writer.WriteEndElement();

            writer.WriteStartElement("uniqueness");
            ints.Serialize(writer, Uniqueness);
            writer.WriteEndElement();

            writer.WriteStartElement("tag");
            objs.Serialize(writer, Tag);
            writer.WriteEndElement();
        }
        #endregion
    }

    public class ScanResultCollection : IXmlSerializable
    {
        public string Name;

        public string Hash;

        private XmlSerializableDictioanry<string, ScanResult> _lookupTable = new XmlSerializableDictioanry<string, ScanResult>();

        #region "IXmlSerializable methods"

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer strings = new XmlSerializer(typeof(string));
            XmlSerializer table = new XmlSerializer(typeof(XmlSerializableDictioanry<string, ScanResult>));

            bool bEmpty = reader.IsEmptyElement;
            reader.Read();

            if (bEmpty)
                return;

            reader.ReadStartElement("name");
            Name = (string)strings.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("hash");
            Hash = (string)strings.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("results");
            _lookupTable = (XmlSerializableDictioanry<string, ScanResult>)table.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadEndElement(); // </ScanResultCollection>
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer strings = new XmlSerializer(typeof(string));
            XmlSerializer table = new XmlSerializer(typeof(XmlSerializableDictioanry<string, ScanResult>));

            writer.WriteStartElement("name");
            strings.Serialize(writer, Name);
            writer.WriteEndElement();

            writer.WriteStartElement("hash");
            strings.Serialize(writer, Hash);
            writer.WriteEndElement();

            writer.WriteStartElement("results");
            table.Serialize(writer, _lookupTable);
            writer.WriteEndElement();
        }
        #endregion

        public ScanResultCollection()
        {
        }

        public ScanResultCollection(string theFile)
        {
            Name = Path.GetFileName(theFile);
            Hash = GetMD5Hash(theFile);
        }

        public string GetMD5Hash(string theFile)
        {
            using (FileStream file = File.OpenRead(theFile))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] ret = md5.ComputeHash(file);

                StringBuilder s = new StringBuilder();
                for (int i = 0; i < ret.Length; i++)
                {
                    s.Append(ret[i].ToString("X2"));
                }

                return s.ToString();
            }
        }

        public ScanResultCollection(ScanResultCollection input)
        {
            Name = input.Name;
            Hash = input.Hash;

            foreach (ScanResult aResult in input.GetResultList())
            {
                _lookupTable[aResult.Name] = aResult;
            }
        }

        public void RemoveResult(ScanResult aResult)
        {
            if (ResultExists(aResult.Name))
            {
                _lookupTable.Remove(aResult.Name);
            }
        }

        /// <summary>
        /// If the given name does not exist, it is added
        /// If it exists, the existing value/weight/uniqueness/tag are updated
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="weight"></param>
        /// <param name="uniqueness"></param>
        public void SetResult(string name, string value, int weight, int uniqueness, object Tag)
        {
            if (ResultExists(name, value))
            {
                ScanResult result = _lookupTable[name];
                result.Name = name;
                result.Value = value;
                result.Weight = weight;
                result.Uniqueness = uniqueness;
                result.Tag = Tag;
            }
            else
            {
                AddResult(name, value, weight, uniqueness, Tag);
            }
        }
        public void SetResult(string name, string value, int weight, int uniqueness)
        {
            SetResult(name, value, weight, uniqueness, null);
        }

        public List<ScanResult> GetResultList() { return _lookupTable.Values.ToList(); }

        /// <summary>
        /// Returns the matching result based on Name, or null if no result exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ScanResult GetResult(string name)
        {
            if (ResultExists(name))
            {
                ScanResult result = _lookupTable[name];
                return result;
            }

            return null;
        }

        public string GetResultValue(string name)
        {
            if (ResultExists(name))
            {
                ScanResult result = _lookupTable[name];
                return result.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// Adds a new result to the result set
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="weight"></param>
        /// <param name="uniqueness"></param>
        public bool AddResult(string name, string value, int weight, int uniqueness, object Tag)
        {
            if (ResultExists(name))
            {
                int i = 2;
                string newname = name + " " + i.ToString();
                while (ResultExists(newname))
                {
                    i++;
                    newname = name + " " + i.ToString();
                }

                Console.WriteLine("AddResult: {0} already exists in the result set, renaming to {1}", name, newname);
                name = newname;
            }

            ScanResult result = new ScanResult();
            result.Name = name;
            result.Value = value;
            result.Weight = weight;
            result.Uniqueness = uniqueness;
            result.Tag = Tag;

            // store by name
            _lookupTable[name] = result;

            return true;
        }

        public bool AddResult(string name, string value, int weight, int uniqueness)
        {
            return AddResult(name, value, weight, uniqueness, null);
        }

        public bool ResultExists(string name, string value)
        {
            if (_lookupTable.ContainsKey(name))
            {
                ScanResult result = _lookupTable[name];

                if (result.Value == value)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ResultExists(string name)
        {
            if (_lookupTable.ContainsKey(name))
            {
                return true;
            }
            return false;
        }
    }

    public class ScanResultCollectionList : IXmlSerializable
    {
        #region "IXmlSerializable methods"

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer list = new XmlSerializer(typeof(List<ScanResultCollection>));

            bool bEmpty = reader.IsEmptyElement;
            reader.Read();

            if (bEmpty)
                return;

            reader.MoveToContent();

            reader.ReadStartElement("list");
            Items = (List<ScanResultCollection>)list.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadEndElement(); // </ScanResultCollectionList>
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer list = new XmlSerializer(typeof(List<ScanResultCollection>));

            writer.WriteStartElement("list");
            list.Serialize(writer, Items);
            writer.WriteEndElement();
        }
        #endregion

        public List<ScanResultCollection> Items = new List<ScanResultCollection>();

        public void AddResultCollection(ScanResultCollection collection)
        {
            foreach (ScanResultCollection aCollection in Items)
            {
                if (collection.Hash == aCollection.Hash)
                {
                    // duplicate
                    return;
                }
            }

            Items.Add(collection);

            return;
        }

        static public ScanResultCollectionList SerializeFromXML(string theFile)
        {
            if (File.Exists(theFile))
            {
                using (StreamReader r = new StreamReader(theFile))
                {
                    return SerializeFromXML(r.BaseStream);
                }
            }

            return new ScanResultCollectionList();
        }

        static public ScanResultCollectionList SerializeFromXML(Stream theStream)
        {
            XmlSerializer binary = new XmlSerializer(typeof(ScanResultCollectionList));
            return (ScanResultCollectionList)binary.Deserialize(theStream);
        }

        static public void SerializeToXML(string theFile, ScanResultCollectionList theList)
        {
            using (StreamWriter s = new StreamWriter(theFile))
            {
                SerializeToXML(s.BaseStream, theList);
            }
        }

        static public void SerializeToXML(Stream theStream, ScanResultCollectionList theList)
        {
            XmlSerializer binary = new XmlSerializer(typeof(ScanResultCollectionList));
            binary.Serialize(theStream, theList);
        }
    }
}
