using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Working_WIth_XML_Files {
    class Program {
        static void Main(string[] args) {

            string foldername = "XML";
            string filename = "data.xml";
            var current = Directory.GetCurrentDirectory();
            var dirPath = $"{Directory.GetParent(Directory.GetParent(current).ToString())} {Path.DirectorySeparatorChar}{foldername}";

            // check if directory exists.
            if (Directory.Exists(dirPath)) {
                // in side directory
                // check if file exists 

                // set file name 
                var filePath = $"{GetPermissions(dirPath)}{Path.DirectorySeparatorChar}{filename}";

                // check if file exists
                if (File.Exists(filePath)) {
                    Console.WriteLine("open file show contents");
                    /// There are two methods to read xml data
                    /// 1. using xmlreader
                    /// 2. using xmldocument 

                    // create a XMLreader object
                    //XmlReader xmlreader = XmlReader.Create(new StreamReader(filePath));
                    //while (xmlreader.Read()) {
                    //    if ((xmlreader.NodeType==XmlNodeType.Element)&&(xmlreader.Name=="user")) {
                    //        if (xmlreader.HasAttributes)
                    //             Console.WriteLine($"<{xmlreader.Name} name={xmlreader.GetAttribute("name")}></{xmlreader.Name}>");
                    //    }
                    //}

                    // using xmldocument things become easy
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(filePath);
                    Console.Write($"<{xmldoc.DocumentElement.Name}>");
                    ParseElement(xmldoc.DocumentElement.ChildNodes);
                    Console.Write($"\n</{xmldoc.DocumentElement.Name}>");
                    Console.ReadKey();
                }
                else {
                    // create file
                    DumpXMLData($"{GetPermissions(dirPath)}{Path.DirectorySeparatorChar}{filename}", GetUsersList());
                    Console.WriteLine("File Created and Data Dumpped Successfully!!!");
                }
            }
            else {
                // create directory 
                Console.WriteLine("Directory doesn't exist");
                Directory.CreateDirectory(dirPath);
                Console.WriteLine("Directory Successfully Created");
                // create file 
                DumpXMLData($"{GetPermissions(dirPath)}{Path.DirectorySeparatorChar}{filename}", GetUsersList());
                Console.WriteLine("File Created and Data Dumpped Successfully!!!");


            }
        }

        public static DirectoryInfo GetPermissions(string directoryPath) {
            // set access permissions
            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            DirectorySecurity security = dir.GetAccessControl();
            security.AddAccessRule(new FileSystemAccessRule(Environment.UserName, FileSystemRights.FullControl, AccessControlType.Allow));
            dir.SetAccessControl(security);
            return dir;
        }

        // users list
        public static List<User> GetUsersList() {
            List<User> userslst = new List<User>();
            for (int i = 1; i <= 50; i++) {
                userslst.Add(new User {
                    Name = $"user_{i}",
                    Email = $"email_{i}",
                    Password = $"password_{i}",
                    Address = new Address {
                        City = $"city_{i}",
                        Country = $"country_{i}",
                        Phone = $"phone_{i}",
                        Street = $"street_{i}"
                    }
                });
            }
            return userslst;
        }
        // dump data after file creationg
        public static void DumpXMLData(string filePath, List<User> userslst) {
            if (!File.Exists(filePath)) {

                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineOnAttributes = true;
                using (XmlWriter xmlWriter = XmlWriter.Create($"{filePath}", xmlWriterSettings)) {
                    xmlWriter.WriteStartDocument();
                    // users
                    xmlWriter.WriteStartElement("users");

                    foreach (User user in userslst) {
                        // user
                        xmlWriter.WriteStartElement("user");
                        xmlWriter.WriteAttributeString("name", user.Name);
                        xmlWriter.WriteAttributeString("email", user.Email);
                        xmlWriter.WriteAttributeString("password", user.Password);
                        // address
                        xmlWriter.WriteStartElement("address");
                        xmlWriter.WriteAttributeString("street", user.Address.Street);
                        xmlWriter.WriteAttributeString("phone", user.Address.Phone);
                        xmlWriter.WriteAttributeString("city", user.Address.City);
                        xmlWriter.WriteAttributeString("country", user.Address.Country);
                        // close addr
                        xmlWriter.WriteEndElement();
                        // close user
                        xmlWriter.WriteEndElement();
                    }
                    // end users
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
            }
            else {
                XDocument xDocument = XDocument.Load(filePath);
                XElement root = xDocument.Element("users");
                IEnumerable<XElement> rows = root.Descendants("user");
                XElement firstRow = rows.First();
                foreach (var user in userslst) {

                    firstRow.AddBeforeSelf(
                       new XElement("user",
                        new XAttribute("name", user.Name),
                        new XAttribute("email", user.Email),
                        new XAttribute("password", user.Password),
                            new XElement("address",
                             new XAttribute("street", user.Address.Street),
                             new XAttribute("phone", user.Address.Phone),
                             new XAttribute("city", user.Address.City),
                             new XAttribute("country", user.Address.Country)
                       )
                       )
                    );
                }
                xDocument.Save(filePath);
            }

        }
        // parse xml document tree
        public static void ParseElement(XmlNodeList xmlNodeList, string tab = "") {
            foreach (XmlNode node in xmlNodeList) {
                // opening tag
                Console.Write($"\n{tab}<{node.Name}");
                // traverse through all the attributes
                foreach (XmlAttribute attr in node.Attributes) {
                    Console.Write($"\n{tab}   {attr.Name} = \"{node.Attributes[attr.Name].Value}\"");
                }
                Console.Write(">");
                // traverse through all the children
                if (node.HasChildNodes) {
                    // recursion
                    ParseElement(node.ChildNodes, "\t");
                }
                // closing tag
                Console.Write($"\n{tab}</{node.Name}>");
            }
        }

    }
}