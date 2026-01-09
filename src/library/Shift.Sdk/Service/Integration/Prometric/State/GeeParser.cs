using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Shift.Common;

namespace InSite.Domain.Integrations.Prometric
{
    public class GeeParser
    {
        public SaveEligibilityOutput ParseSaveEligibilityOutput(string xml)
        {
            var result = new SaveEligibilityOutput();
            string pattern = "<response result=\"(-?\\d+)\">(.*?)</response>";
            Match match = Regex.Match(xml, pattern);
            if (match.Success)
            {
                result.Code = int.Parse(match.Groups[1].Value);
                result.Message = match.Groups[2].Value;
            }
            return result;
        }

        public GetEligibilityOutput ParseGetEligibilityOutput(string xml)
        {
            var result = new GetEligibilityOutput();
            string pattern = "<response result=\"(true|false)\">(.*?)</response>";
            Match match = Regex.Match(xml, pattern);
            if (match.Success)
            {
                result.Success = bool.Parse(match.Groups[1].Value);
                result.Message = match.Groups[2].Value;
            }
            return result;
        }

        public string Base64Encode(SaveEligibilityInput input)
        {
            string plainText = Serialize(input);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private string Serialize(SaveEligibilityInput input)
        {
            return BuildXmlDocument(input).OuterXml;
        }

        public string ParseGetTokenOutput(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsMgr.AddNamespace("ns", "https://gee.prometric.com/");

            XmlNode node = xmlDoc.SelectSingleNode("//ns:string", nsMgr);
            return node.InnerText;
        }

        public XmlDocument BuildXmlDocument(SaveEligibilityInput input)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("XELIG");
            root.SetAttribute("version", "2.00");
            xmlDoc.AppendChild(root);

            XmlElement client = xmlDoc.CreateElement("client");
            client.InnerText = input.Client;
            root.AppendChild(client);

            XmlElement eligibility = xmlDoc.CreateElement("eligibility");
            root.AppendChild(eligibility);

            XmlElement action = xmlDoc.CreateElement("action");
            action.InnerText = input.Action;
            eligibility.AppendChild(action);

            XmlElement demographics = xmlDoc.CreateElement("demographics");
            eligibility.AppendChild(demographics);

            XmlElement firstname = xmlDoc.CreateElement("firstname");
            firstname.InnerText = input.LearnerFirstName;
            demographics.AppendChild(firstname);

            XmlElement lastname = xmlDoc.CreateElement("lastname");
            lastname.InnerText = input.LearnerLastName;
            demographics.AppendChild(lastname);

            XmlElement ids = xmlDoc.CreateElement("ids");
            demographics.AppendChild(ids);

            XmlElement candidateid1 = xmlDoc.CreateElement("candidateid1");
            candidateid1.InnerText = input.LearnerCode;
            ids.AppendChild(candidateid1);

            XmlElement candidateid2 = xmlDoc.CreateElement("candidateid2");
            candidateid2.InnerText = input.ExamEventPassword;
            ids.AppendChild(candidateid2);

            XmlElement emailaddress = xmlDoc.CreateElement("emailaddress");
            emailaddress.InnerText = input.LearnerEmail;
            demographics.AppendChild(emailaddress);

            XmlElement events = xmlDoc.CreateElement("events");
            eligibility.AppendChild(events);

            XmlElement eventElement = xmlDoc.CreateElement("event");
            events.AppendChild(eventElement);

            XmlElement exam = xmlDoc.CreateElement("exam");
            exam.SetAttribute("programid", input.AssessmentFormProgram);
            exam.SetAttribute("examid", $"{input.AssessmentFormCode}");
            eventElement.AppendChild(exam);

            XmlElement eligibilityid1 = xmlDoc.CreateElement("eligibilityid1");
            eligibilityid1.InnerText = input.LearnerCode;
            eventElement.AppendChild(eligibilityid1);

            AppendAccommodations(input, xmlDoc, eventElement);

            return xmlDoc;
        }

        private static void AppendAccommodations(SaveEligibilityInput input, XmlDocument xmlDoc, XmlElement root)
        {
            if (input.Accommodations.IsEmpty())
                return;

            XmlElement specialconditions = xmlDoc.CreateElement("specialconditions");
            root.AppendChild(specialconditions);

            foreach (var accommodation in input.Accommodations)
            {
                XmlElement specialcondition = xmlDoc.CreateElement("specialcondition");
                specialcondition.InnerText = accommodation.Name;
                specialconditions.AppendChild(specialcondition);

                if (accommodation.Multiplier < 1)
                    continue;

                XmlElement specialconditionattributes = xmlDoc.CreateElement("specialconditionattributes");
                specialcondition.AppendChild(specialconditionattributes);

                XmlElement specialconditionattribute = xmlDoc.CreateElement("specialconditionattribute");
                specialconditionattribute.SetAttribute("name", "Adjust Duration");
                specialconditionattributes.AppendChild(specialconditionattribute);

                XmlElement specialconditionparameters = xmlDoc.CreateElement("specialconditionparameters");
                specialconditionattribute.AppendChild(specialconditionparameters);

                XmlElement specialconditionparameter1 = xmlDoc.CreateElement("specialconditionparameter");
                specialconditionparameter1.SetAttribute("name", "Value");
                specialconditionparameter1.SetAttribute("value", accommodation.Multiplier.ToString("n2"));
                specialconditionparameters.AppendChild(specialconditionparameter1);

                XmlElement specialconditionparameter2 = xmlDoc.CreateElement("specialconditionparameter");
                specialconditionparameter2.SetAttribute("name", "Scope");
                specialconditionparameter2.SetAttribute("value", "Content");
                specialconditionparameters.AppendChild(specialconditionparameter2);
            }
        }
    }
}
