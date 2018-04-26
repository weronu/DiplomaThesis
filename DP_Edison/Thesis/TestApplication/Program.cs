using System.Collections.Generic;
using Common;
using Domain.DomainClasses;
using FileHelper;

namespace TestApplication
{
    public class Program
    {
        static void Main(string[] args)
        {
            ParseXml();
        }

        public static void ParseXml()
        {
            XmlParser xmlParser = new XmlParser();
            HashSet<EmailMessage> emailsFromXml = xmlParser.GetEmailsFromXML(CommonMethods.GetAppSetting("datasetPath"));
        }

        
    }
}
