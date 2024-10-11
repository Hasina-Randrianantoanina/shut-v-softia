
using System.Xml;

namespace IsodaqOperations.Utils
{
    public static class XMLUtils
    {
        public static XmlDocument? GetDocument(string filefullName) 
        {
            try
            { 
                XmlDocument document = new XmlDocument();
                document.Load(filefullName);
                return document;
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $@"Error when reading xml file : {filefullName}");
                throw; // To break main
            }
        }
        public static string? GetInnerText(XmlNode parent, string tag)
        {
            try
            {
                XmlNode? child = parent.SelectSingleNode(@$"{tag}");
                return child == null ? null : child.InnerText;
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $@"Error when looking for innerText in {tag}");
                throw; // To break main
            }
        }

        public static XmlNodeList? GetVoies(XmlDocument document)
        {
            try
            {
                return document.DocumentElement == null ? null : document.DocumentElement.SelectNodes(@"/xdq/system/channel");
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $@"Error when looking for voies in xml file");
                throw; // To break main
            }
        }

    }
}
