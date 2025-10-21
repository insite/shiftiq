using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using InSite.Application.Attempts.Read;
using InSite.Persistence;

namespace InSite.Admin.Attempts.Reports.Downloads
{
    public static class QTIWriter
    {
        public static byte[] Create(QAttemptFilter filter)
        {
            var attempts = ServiceLocator.AttemptSearch.GetAttempts(filter, x => x.Form.Bank);

            var registrationIdentifiers = attempts.Where(x => x.RegistrationIdentifier != null).Select(x => x.RegistrationIdentifier.Value).Distinct().ToList();
            var learnerTypes = ServiceLocator.RegistrationSearch.GetLearnerTypes(registrationIdentifiers);
            var learners = PersonCriteria.Bind(
                x => new { x.UserIdentifier, x.PersonCode },
                new PersonFilter
                {
                    OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                    IncludeUserIdentifiers = attempts.Select(x => x.LearnerUserIdentifier).Distinct().ToArray()
                });

            byte[] xml;

            using (var stream = new MemoryStream())
            {
                var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

                using (var writer = XmlWriter.Create(stream, settings))
                {
                    writer.WriteStartDocument();

                    writer.WriteStartElement("ExamResults");
                    writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                    writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");

                    writer.WriteElementString("Jurisdiction", "BC");

                    writer.WriteStartElement("ExamsTaken");

                    foreach (var attempt in attempts)
                    {
                        var code = learners.FirstOrDefault(x => x.UserIdentifier == attempt.LearnerUserIdentifier)?.PersonCode;
                        var type = learnerTypes.FirstOrDefault(x => x.RegistrationIdentifier == attempt.RegistrationIdentifier)?.LearnerType;
                        WriteAttempt(code, type, attempt, writer);
                    }

                    writer.WriteEndElement();

                    writer.WriteEndElement();

                    writer.WriteEndDocument();
                }

                stream.Position = 0;

                xml = new byte[stream.Length];
                stream.Read(xml, 0, (int)stream.Length);
            }

            return xml;
        }

        private static void WriteAttempt(string personCode, string clientType, QAttempt attempt, XmlWriter writer)
        {
            var filter = new QAttemptFilter { AttemptIdentifier = attempt.AttemptIdentifier };

            var questions = ServiceLocator.AttemptSearch
                .BindAttemptQuestions(x => new { x.AnswerOptionKey, x.QuestionSequence }, filter)
                .OrderBy(x => x.QuestionSequence)
                .ToList();

            var options = ServiceLocator.AttemptSearch
                .BindAttemptOptions(x => new AttemptOptionModel { OptionKey = x.OptionKey, OptionSequence = x.OptionSequence }, attempt.AttemptIdentifier)
                .ToList();

            var edition = !string.IsNullOrEmpty(attempt.Form.Bank.BankEdition) ? attempt.Form.Bank.BankEdition.Split(new[] { '.' }) : null;
            var reference = edition != null ? edition[0] : "Unknown";
            var version = edition != null && edition.Length > 1 ? edition[1] : "Unknown";

            writer.WriteStartElement("ExamTaken");

            writer.WriteElementString("ClientType", clientType);
            writer.WriteElementString("ClientId", personCode);
            writer.WriteElementString("SittingIdentifier", attempt.FormIdentifier.ToString());
            writer.WriteElementString("Reference", reference);
            writer.WriteElementString("Version", version);
            writer.WriteElementString("WrittenDate", $"{attempt.AttemptSubmitted:yyyy-MM-dd}");
            writer.WriteElementString("Score", $"{attempt.AttemptScore * 100:n0}");
            writer.WriteElementString("Pass", attempt.AttemptIsPassing ? "true" : "false");

            writer.WriteStartElement("Responses");

            foreach (var question in questions)
            {
                string response = GetResponse(question.AnswerOptionKey, options);

                writer.WriteStartElement("Question");
                writer.WriteAttributeString("number", question.QuestionSequence.ToString());

                writer.WriteString(response);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        class AttemptOptionModel
        {
            public int OptionKey { get; set; }
            public int OptionSequence { get; set; }
        }

        private static string GetResponse(int? answerOptionKey, List<AttemptOptionModel> options)
        {
            string response = "";

            if (answerOptionKey.HasValue)
            {
                var answerOption = options.Find(x => x.OptionKey == answerOptionKey);
                if (answerOption != null)
                {
                    switch (answerOption.OptionSequence)
                    {
                        case 1:
                            response = "A";
                            break;
                        case 2:
                            response = "B";
                            break;
                        case 3:
                            response = "C";
                            break;
                        case 4:
                            response = "D";
                            break;
                    }
                }
            }

            return response;
        }
    }
}