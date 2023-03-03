using System; 
 using System.Collections; 
 using System.Collections.Generic; 
 using System.Data; 
 using System.Linq; 
 using System.IO; 
 using System.Threading.Tasks; 
 using System.Xml; 
  
 public static class FileUtilities 
 { 
  
         public static string ReadAllXlmTagElements(string path, string tag) 
         { 
                XmlDocument xmlDoc = new XmlDocument(); 
                xmlDoc.Load(path); 
                XmlNode root = xmlDoc.DocumentElement; 
                XmlNodeList nodeList; 

                nodeList = xmlDoc.GetElementsByTagName(tag); 


                return nodeList.ToString(); 
         } 
  
         /// <summary> 
         /// Creates a text node for an xml document 
         /// </summary> 
         /// <param name="doc"></param> 
         /// <param name="elem"></param> 
         /// <param name="appendChild"></param> 
         /// <returns></returns> 
         public static XmlNode CreateXmlTextNode(XmlDocument doc, string elemName, string elemValue, bool appendChild = true) 
         { 
                 XmlNode xmlElement = doc.CreateElement(elemName); 
                 if(appendChild) 
                 { 
                         xmlElement.AppendChild(doc.CreateTextNode(elemValue)); 
                 } 
  
                 return xmlElement; 
         } 
  
  
  
         /// <summary> 
         /// Gets the value associated with an xml element tag 
         /// </summary> 
         /// <param name="element"></param> 
         /// <param name="strTagName"></param> 
         /// <returns></returns> 
         public static string GetXmlElementTagValue(XmlElement element, string strTagName) 
         { 
  
                 //Get a list of the child nodes with the tag name passed 
                 XmlNodeList nodeList = element.GetElementsByTagName(strTagName).Item(0).ChildNodes; 
  
                 //Get the node in the node list 
                 XmlNode node = (XmlNode)nodeList.Item(0); 
  
                 //return the nodes value 
                 return node.Value; 
  
         } 
  
  
  
         /// <summary> 
         /// Adds the element to every member of the tag passed 
         /// </summary> 
         /// <param name="document"></param> 
         /// <param name="strTagName"></param> 
         /// <param name="strElementName"></param> 
         /// <param name="strDefaultElementValue"></param> 
         /// <returns></returns> 
         public static bool AddXmlElement(XmlDocument document, string strTagName, string strElementName, string strDefaultElementValue) 
         { 
                 bool success = false; 
                 try { 
  
                         //The Node list to be adding to 
                         XmlNodeList nodeList = document.GetElementsByTagName(strTagName); 
  
  
                         //The new element being added 
                         XmlElement element = null; 
  
                         //Loop to the end of the node list 
                         for (int i = 0; i < nodeList.Count; i++) 
                         { 
  
                                 //Get the current element 
                                 element = (XmlElement)nodeList.Item(i); 
  
                                 XmlNode newElement = CreateXmlTextNode(document, strElementName, strDefaultElementValue); 
  
  
                                 //Append the new element 
                                 element.AppendChild(newElement); 
  
  
                         } 
  
  
                         success = true; 
  
  
                 }  
                 catch (Exception e)  
                 { 
                         success = false; 
                 } 
  
                 return success; 
  
         } 
  
  
  
         /// <summary> 
         /// Adds an element at the ID passed 
         /// </summary> 
         /// <param name="document"></param> 
         /// <param name="strIdToFind"></param> 
         /// <param name="strTagName"></param> 
         /// <param name="strElementName"></param> 
         /// <param name="strDefaultElementValue"></param> 
         public static void AddXmlElementAtID(XmlDocument document, string strIdToFind, string strTagName, string strElementName, string strDefaultElementValue) 
         { 
  
                 try { 
  
                         //The Node list to be adding to 
                         XmlNodeList nodeList = document.GetElementsByTagName(strTagName); 
  
  
                         //The new element being added 
                         XmlElement element = null; 
  
                         //Loop to the end of the node list 
                         for (int i = 0; i < nodeList.Count; i++) 
                         { 
  
                                 //Get the current element 
                                 element = (XmlElement)nodeList.Item(i); 
  
  
                                 if (element.GetAttribute("id") == (strIdToFind)) 
                                 { 
                                         XmlNode newElement = CreateXmlTextNode(document, strElementName, strDefaultElementValue); 
  
                                         //Append the new element 
                                         element.AppendChild(newElement); 
                                 } 
  
  
                         } 
  
  
                 } catch (Exception e) { 
                         //e.printStackTrace(); 
                         //System.out.println("\nCould not add " + strElementName + " to " + strTagName); 
                 } 
                  
         } 
  
  
  
         /// <summary> 
         /// Updates every element of type X 
         /// </summary> 
         /// <param name="document"></param> 
         /// <param name="strTagName"></param> 
         /// <param name="strElementName"></param> 
         /// <param name="strElementValue"></param> 
         public static void UpdateXmlAllElements(XmlDocument document, string strTagName, string strElementName, string strElementValue) 
 { 
                  
                 try { 
  
                         XmlNodeList nodeList = document.GetElementsByTagName(strTagName); 
                         XmlElement element = null; 
  
                         //Loop to the end of the node list 
                         for (int i = 0; i < nodeList.Count; i++) 
                         { 
                                 element = (XmlElement)nodeList.Item(i); 
                                 XmlNode node = element.GetElementsByTagName(strElementName).Item(0).FirstChild; 
                                 node.Value = strElementValue; 
                         } 
  
  
  
                 } catch (Exception e) { 
                         //e.printStackTrace(); 
                         //System.out.println("\nCould not update " + strElementName + " in " + strTagName + ". Are you sure it exists?\n"); 
                 } 
         } 
  
  
  
         /// <summary> 
         /// Deletes every element of type X. Uh oh, Danger! 
         /// </summary> 
         /// <param name="document"></param> 
         /// <param name="strTagName"></param> 
         /// <param name="strElementName"></param> 
         public static void DeleteXmlElement(XmlDocument document, string strTagName, string strElementName) 
         { 
                 try { 
  
                         //The Node list to be accessed 
                         XmlNodeList nodeList = document.GetElementsByTagName(strTagName); 
                         XmlElement element = null; 
  
  
                         //Loop to the end of the node list 
                         for (int i = 0; i < nodeList.Count; i++) 
                         { 
                                 element = (XmlElement)nodeList.Item(i); 
                                 XmlNode node = element.GetElementsByTagName(strElementName).Item(0); 
                                 element.RemoveChild(node); 
  
                         } 
  
                 } catch (Exception e) { 
                         //e.printStackTrace(); 
                         //System.out.println("\nCould not delete " + strElementName + " in " + strTagName + ". Are you sure it exists?\n"); 
                 } 
  
  
  
         } 
  
  
  
         /// <summary> 
         /// Updates the elements value at the passed ID 
         /// </summary> 
         /// <param name="document"></param> 
         /// <param name="strIdToFind"></param> 
         /// <param name="strTagName"></param> 
         /// <param name="strElementName"></param> 
         /// <param name="strElementValue"></param> 
         public static void UpdateXmlElementAtID(XmlDocument document, string strIdToFind, string strTagName, string strElementName, String strElementValue) 
         { 
                  
                 try { 
  
                         XmlNodeList nodeList = document.GetElementsByTagName(strTagName); 
                         XmlElement element = null; 
  
                         //Loop to the end of the node list 
                         for (int i = 0; i < nodeList.Count; i++) 
                         { 
                                 element = (XmlElement)nodeList.Item(i); 
  
  
  
  
                                 XmlNode node = element.GetElementsByTagName(strElementName).Item(0).FirstChild; 
                                 if (element.GetAttribute("id")==(strIdToFind)) 
                                 { 
                                         node.Value = strElementValue; 
                                         break; 
                                 } 
  
  
                         } 
  
  
  
                 } catch (Exception e) { 
                         //e.printStackTrace(); 
                         //System.out.println("\nCould not update " + strElementName + " in " + strTagName + ". Are you sure it exists?\n"); 
                 } 
         } 
  
  
  
         /// <summary> 
         /// Deletes the element at the ID passed 
         /// </summary> 
         /// <param name="document"></param> 
         /// <param name="strIdToFind"></param> 
         /// <param name="strTagName"></param> 
         /// <param name="strElementName"></param> 
         public static void DeleteXmlElementAtID(XmlDocument document, string strIdToFind, string strTagName, string strElementName) 
         { 
                 try { 
  
                         //The Node list to be accessed 
                         XmlNodeList nodeList = document.GetElementsByTagName(strTagName); 
                         XmlElement element = null; 
  
  
                         //Loop to the end of the node list 
                         for (int i = 0; i < nodeList.Count; i++) 
                         { 
                                 element = (XmlElement)nodeList.Item(i); 
  
                                 if (element.GetAttribute("id")==(strIdToFind)) 
                                 { 
                                         XmlNode node = element.GetElementsByTagName(strElementName).Item(0); 
                                         element.RemoveChild(node); 
                                         break; 
                                 } 
  
                         } 
  
                 } catch (Exception e) { 
                         //e.printStackTrace(); 
                         //System.out.println("\nCould not delete " + strElementName + " in " + strTagName + ". Are you sure it exists?\n"); 
                 } 
  
  
  
         } 
  
  
  
         /// <summary> 
         /// Creates a directory at the path passed 
         /// </summary> 
         /// <param name="path"></param> 
         /// <returns></returns> 
         public static DirectoryInfo CreateDirectory(string path) 
         { 
            if(path.Length > 0) 
            { 
                return null; 
            } 
            else 
            { 
                return Directory.CreateDirectory(path); 
            }
         } 
  
  
         /// <summary> 
         /// Returns the current directory path 
         /// </summary> 
         /// <returns></returns> 
         public static string GetCurrentDirectoryPath() => Directory.GetCurrentDirectory();
  
  
  
         /// <summary> 
         /// Reads the lines from the file passed 
         /// </summary> 
         /// <param name="path"></param> 
         /// <returns></returns> 
         public static List<string> ReadFromFile(string path) => System.IO.File.ReadAllLines(path).ToList(); 
  
  
  
         /// <summary> 
         /// Appends the file at path 
         /// </summary> 
         /// <param name="filePath"></param> 
         /// <param name="linesToWrite"></param> 
         /// <returns></returns> 
         public static bool AppendLinesToFile(string filePath, ICollection<string> linesToWrite) 
         { 
  
            bool success = false; 
            var stringsAsArray = linesToWrite.ToArray(); 
            try 
            { 
                if (stringsAsArray.Length > 0) 
                { 
                    File.AppendAllLines(filePath, stringsAsArray); 
                    success = true; 
                } 
            } 
            catch (Exception ex) 
            { 
                success = false; 
            } 

            return success; 
         } 
  
  
  
         /// <summary> 
         /// Asyncrenously appends the file at path 
         /// </summary> 
         /// <param name="filePath"></param> 
         /// <param name="linesToWrite"></param> 
         /// <returns></returns> 
         public static async Task AppendLinesToFileAsync(string filePath, ICollection<string> linesToWrite) 
         { 
  
            var stringsAsArray = linesToWrite.ToArray(); 

            try 
            { 
                if (stringsAsArray.Length > 0) 
                { 
                    await File.AppendAllLinesAsync(filePath, stringsAsArray); 
                } 
            } 
            catch (Exception ex) 
            { 
            } 
  
         } 
  
  
  
         /// <summary> 
         /// Writes to the file at path 
         /// </summary> 
         /// <param name="filePath"></param> 
         /// <param name="linesToWrite"></param> 
         /// <returns></returns> 
         public static bool WriteLinesToFile(string filePath, ICollection<string> linesToWrite) 
         { 
  
            bool success = false; 
            var stringsAsArray = linesToWrite.ToArray(); 
            try 
            { 
                if (stringsAsArray.Length > 0) 
                { 
                    File.WriteAllLines(filePath, stringsAsArray); 
                    success = true; 
                } 
            } 
            catch (Exception ex) 
            { 
                    success = false; 
            } 

            return success; 
         } 
  
  
  
         /// <summary> 
         /// Asynchrounessly writes to the file at path 
         /// </summary> 
         /// <param name="filePath"></param> 
         /// <param name="linesToWrite"></param> 
         /// <returns></returns> 
         public static async Task WriteLinesToFileAsync(string filePath, ICollection<string> linesToWrite) 
         { 
  
                 var stringsAsArray = linesToWrite.ToArray(); 
  
                 try 
                 { 
                         if(stringsAsArray.Length > 0) 
                         { 
                                 await File.WriteAllLinesAsync(filePath, stringsAsArray); 
                         } 
                 } 
                 catch(Exception ex) 
                 { 
                 } 
  
         } 
          
 }
