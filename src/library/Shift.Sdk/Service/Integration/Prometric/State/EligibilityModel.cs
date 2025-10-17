using System.IO;
using System.Xml.Serialization;

namespace InSite.Domain.Integrations.Prometric
{
    [XmlRoot("eligibility")]
    public class EligibilityModel
    {
        [XmlElement("action")]
        public string Action { get; set; }

        [XmlElement("demographics")]
        public Demographics Demographics { get; set; }

        [XmlElement("events")]
        public Events Events { get; set; }

        public static EligibilityModel DeserializeXml(string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EligibilityModel));
            using (StringReader reader = new StringReader(xmlString))
            {
                return (EligibilityModel)serializer.Deserialize(reader);
            }
        }
    }

    public class Demographics
    {
        [XmlElement("firstname")]
        public string FirstName { get; set; }

        [XmlElement("middlename")]
        public string MiddleName { get; set; }

        [XmlElement("lastname")]
        public string LastName { get; set; }

        [XmlElement("dateofbirth")]
        public string DateOfBirth { get; set; }

        [XmlElement("ids")]
        public Ids Ids { get; set; }

        [XmlElement("address")]
        public Address Address { get; set; }

        [XmlElement("phones")]
        public Phones Phones { get; set; }

        [XmlElement("emailaddress")]
        public string EmailAddress { get; set; }
    }

    public class Ids
    {
        [XmlElement("ssn")]
        public string Ssn { get; set; }

        [XmlElement("mothersmaidenname")]
        public string MothersMaidenName { get; set; }

        [XmlElement("candidateid1")]
        public string CandidateId1 { get; set; }

        [XmlElement("candidateid2")]
        public string CandidateId2 { get; set; }
    }

    public class Address
    {
        [XmlElement("address1")]
        public string Address1 { get; set; }

        [XmlElement("address2")]
        public string Address2 { get; set; }

        [XmlElement("address3")]
        public string Address3 { get; set; }

        [XmlElement("address4")]
        public string Address4 { get; set; }

        [XmlElement("city")]
        public string City { get; set; }

        [XmlElement("province")]
        public string Province { get; set; }

        [XmlElement("postalcode")]
        public string PostalCode { get; set; }

        [XmlElement("countrycode")]
        public string CountryCode { get; set; }
    }

    public class Phones
    {
        [XmlElement("work")]
        public string Work { get; set; }

        [XmlElement("home")]
        public string Home { get; set; }
    }

    public class Events
    {
        [XmlElement("event")]
        public Event Event { get; set; }
    }

    public class Event
    {
        [XmlElement("exam")]
        public Exam Exam { get; set; }

        [XmlElement("sessions")]
        public Sessions Sessions { get; set; }

        [XmlElement("eligibilityid1")]
        public string EligibilityId1 { get; set; }

        [XmlElement("restrictions")]
        public Restrictions Restrictions { get; set; }

        [XmlElement("specialconditions")]
        public string SpecialConditions { get; set; }

        [XmlElement("eventspecificfiles")]
        public EventSpecificFiles EventSpecificFiles { get; set; }
    }

    public class Exam
    {
        [XmlAttribute("programid")]
        public string ProgramId { get; set; }

        [XmlAttribute("examid")]
        public string ExamId { get; set; }

        [XmlAttribute("examtype")]
        public string ExamType { get; set; }

        [XmlAttribute("examsessions")]
        public string ExamSessions { get; set; }

        [XmlAttribute("maxhours")]
        public string MaxHours { get; set; }

        [XmlAttribute("maxdays")]
        public string MaxDays { get; set; }

        [XmlAttribute("consecutivedays")]
        public string ConsecutiveDays { get; set; }
    }

    public class Sessions
    {
    }

    public class Restrictions
    {
        [XmlElement("startdate")]
        public string StartDate { get; set; }

        [XmlElement("enddate")]
        public string EndDate { get; set; }

        [XmlElement("sites")]
        public string Sites { get; set; }

        [XmlElement("rrcs")]
        public string Rrcs { get; set; }

        [XmlElement("countries")]
        public string Countries { get; set; }
    }

    public class EventSpecificFiles
    {
        [XmlAttribute("numberrequired")]
        public string NumberRequired { get; set; }

        [XmlElement("eventspecificfile")]
        public EventSpecificFile EventSpecificFile { get; set; }
    }

    public class EventSpecificFile
    {
        [XmlAttribute("filename")]
        public string FileName { get; set; }

        [XmlAttribute("encoding")]
        public string Encoding { get; set; }

        [XmlElement("contents")]
        public string Contents { get; set; }
    }
}
