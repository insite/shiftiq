using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.UI
{
    public class QTIReader
    {
        #region Classes

        private static class QuestionTypeNames
        {
            public const string MultipleChoiceQuestion = "multiple_choice_question";
            public const string MultipleAnswersQuestion = "multiple_answers_question";
            public const string EssayQuestion = "essay_question";
            public const string MatchingQuestion = "matching_question";
            public const string ShortAnswerQuestion = "short_answer_question";
            public const string TrueFalseQuestion = "true_false_question";
        }

        public class Choice
        {
            public string Identifier { get; set; }
            public string Label { get; set; }
            public double Score { get; set; }
        }

        public class Response
        {
            public string Identifier { get; set; }
            public string ResponseText { get; set; }
            public List<Choice> Choices { get; set; }
        }

        public class Question
        {
            public string Identifier { get; set; }
            public string QuestionText { get; set; }
            public QuestionType QuestionType { get; set; }
            public double PointsPossible { get; set; }
            public List<Response> Responses { get; set; }
        }

        public class Section
        {
            public string Identifier { get; set; }
            public string Title { get; set; }
            public double PointsPerItem { get; set; }
            public bool IsCustom { get; set; }
            public List<Question> Questions { get; set; }
        }

        public class Assessment
        {
            public string Identifier { get; set; }
            public string Title { get; set; }
            public int MaxAttempts { get; set; }
            public List<Section> Sections { get; set; }
        }

        public class Result
        {
            public List<Assessment> Assessments { get; set; }
            public string Error { get; set; }
            public int ErrorLine { get; set; }
        }

        #endregion

        static readonly XNamespace NS = XNamespace.Get("http://www.imsglobal.org/xsd/ims_qtiasiv1p2");

        private Result _result;

        #region Public methods

        public Result Read(Stream file)
        {
            _result = new Result { Assessments = new List<Assessment>() };

            XDocument xml;

            try
            {
                xml = XDocument.Load(file, LoadOptions.SetLineInfo);
            }
            catch (Exception ex)
            {
                _result.Error = ex.Message;
                _result.ErrorLine = -1;
                return _result;
            }

            foreach (var assessmentElement in xml.Root.Elements(NS + "assessment"))
            {
                var assessment = ReadAssessment(assessmentElement);
                if (assessment == null)
                    return _result;

                _result.Assessments.Add(assessment);

                var itemElements = ReadElements(assessmentElement, "section", "item");
                if (itemElements == null)
                    return _result;

                foreach (var section in assessment.Sections)
                {
                    if (!ReadQuestions(section, itemElements))
                        return _result;
                }

                if (itemElements.Count > 0)
                {
                    var customSection = new Section
                    {
                        Identifier = null,
                        Title = CreateSectionTitle(assessment),
                        PointsPerItem = 1,
                        IsCustom = true,
                        Questions = new List<Question>()
                    };
                    assessment.Sections.Add(customSection);

                    if (!ReadQuestions(customSection, itemElements))
                        return _result;
                }
            }

            _result.Error = null;

            return _result;
        }

        #endregion

        #region Helper methods

        private static string CreateSectionTitle(Assessment assessment)
        {
            var index = 1;
            string title;

            do
            {
                title = "Set " + index;
                index++;
            }
            while (assessment.Sections.Find(x => x.Title.Equals(title, StringComparison.OrdinalIgnoreCase)) != null);

            return title;
        }

        private Assessment ReadAssessment(XElement assessmentElement)
        {
            var assessmentTitle = GetAttributeValue(assessmentElement, "title");
            if (assessmentTitle == null)
                return null;

            var assessmentIdentifier = GetAttributeValue(assessmentElement, "ident");
            if (assessmentIdentifier == null)
                return null;

            var sections = new List<Section>();

            var sectionElements = ReadElements(assessmentElement, "section", "section");
            if (sectionElements != null)
            {
                foreach (var sectionElement in sectionElements)
                {
                    var sectionTitle = GetAttributeValue(sectionElement, "title");
                    if (sectionTitle == null)
                        return null;

                    var sectionIdentifier = GetAttributeValue(sectionElement, "ident");
                    if (sectionIdentifier == null)
                        return null;

                    var pointsPerItem = double.TryParse(ReadElement(sectionElement, "selection_ordering", "selection", "selection_extension", "points_per_item")?.Value, out var pointsPerItemTemp)
                        ? pointsPerItemTemp
                        : 1;

                    sections.Add(new Section { Identifier = sectionIdentifier, Title = sectionTitle, PointsPerItem = pointsPerItem, Questions = new List<Question>() });
                }
            }

            var assessment = new Assessment
            {
                Title = assessmentTitle,
                Identifier = assessmentIdentifier,
                MaxAttempts = int.TryParse(GetMetaDataFieldValue(assessmentElement, "cc_maxattempts"), out var maxAttempts) ? maxAttempts : 1,
                Sections = sections
            };

            return assessment;
        }

        private bool ReadQuestions(Section section, List<XElement> itemElements)
        {
            var sectionItemElements = section.IsCustom
                ? itemElements
                : itemElements.Where(x => string.Equals(x.Attribute("title")?.Value, section.Title, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var sectionItemElement in sectionItemElements)
            {
                var questionIdentifier = GetAttributeValue(sectionItemElement, "ident");
                if (questionIdentifier == null)
                    return false;

                var itemMetaDataNode = ReadElement(sectionItemElement, "itemmetadata");
                if (itemMetaDataNode == null)
                    return false;

                var questionType = GetQuestionType(itemMetaDataNode);
                if (questionType == null)
                    return false;

                var questionText = ReadElement(sectionItemElement, "presentation", "material", "mattext")?.Value;
                if (questionText == null)
                    return false;

                var responses = ReadResponses(sectionItemElement, questionType.Value);
                if (responses == null)
                    return false;

                var pointsPossible = double.TryParse(GetMetaDataFieldValue(itemMetaDataNode, "points_possible"), out var pointsPossibleTemp) ? pointsPossibleTemp : 1;

                section.Questions.Add(new Question
                {
                    Identifier = questionIdentifier,
                    QuestionType = questionType.Value,
                    PointsPossible = pointsPossible,
                    QuestionText = questionText,
                    Responses = responses
                });
            }

            itemElements.RemoveAll(x => sectionItemElements.Contains(x));

            return true;
        }

        private QuestionType? GetQuestionType(XElement itemMetaDataElement)
        {
            var questionTypeName = GetMetaDataFieldValue(itemMetaDataElement, "question_type");
            if (questionTypeName == null)
                return null;

            switch (questionTypeName.ToLower())
            {
                case QuestionTypeNames.MultipleChoiceQuestion:
                    return QuestionType.MultipleChoice;
                case QuestionTypeNames.MultipleAnswersQuestion:
                    return QuestionType.MultipleAnswers;
                case QuestionTypeNames.EssayQuestion:
                    return QuestionType.Essay;
                case QuestionTypeNames.MatchingQuestion:
                    return QuestionType.Matching;
                case QuestionTypeNames.ShortAnswerQuestion:
                    return QuestionType.ShortAnswer;
                case QuestionTypeNames.TrueFalseQuestion:
                    return QuestionType.TrueFalse;
            }

            _result.Error = $"Question type '{questionTypeName}' is not supported";
            _result.ErrorLine = ((IXmlLineInfo)itemMetaDataElement).LineNumber;

            return null;
        }

        private List<Response> ReadResponses(XElement questionElement, QuestionType questionType)
        {
            switch (questionType)
            {
                case QuestionType.MultipleChoice:
                case QuestionType.TrueFalse:
                    return ReadMultipleChoiceQuestionResponses(questionElement);
                case QuestionType.MultipleAnswers:
                    return ReadMultipleAnswersQuestionResponses(questionElement);
                case QuestionType.Essay:
                    return new List<Response>();
                case QuestionType.Matching:
                    return ReadMatchingQuestionResponses(questionElement);
                case QuestionType.ShortAnswer:
                    return ReadShortAnswerResponses(questionElement);
            }

            _result.Error = $"Question type '{questionType}' is not supported";
            _result.ErrorLine = ((IXmlLineInfo)questionElement).LineNumber;

            return null;
        }

        private List<Response> ReadMultipleChoiceQuestionResponses(XElement questionElement)
        {
            var correctAnswer = ReadElement(questionElement, "resprocessing", "respcondition", "conditionvar", "varequal")?.Value;
            var choiceElements = ReadElements(questionElement, "presentation", "response_lid", "render_choice", "response_label");

            var choices = new List<Choice>();
            if (choiceElements != null)
            {
                foreach (var choiceElement in choiceElements)
                {
                    var label = ReadElement(choiceElement, "material", "mattext")?.Value;
                    if (label == null)
                        return null;

                    var identifier = GetAttributeValue(choiceElement, "ident");
                    var score = string.Equals(identifier, correctAnswer) ? 1 : 0;

                    choices.Add(new Choice { Identifier = identifier, Label = label, Score = score });
                }
            }

            return new List<Response> { new Response { Choices = choices } };
        }

        private List<Response> ReadMultipleAnswersQuestionResponses(XElement questionElement)
        {
            var correctAnswerElements = ReadElements(questionElement, "resprocessing", "respcondition", "conditionvar", "and", "varequal");
            var correctAnswers = new HashSet<string>((correctAnswerElements?.Select(x => x.Value)).EmptyIfNull());
            var choiceElements = ReadElements(questionElement, "presentation", "response_lid", "render_choice", "response_label");

            var choices = new List<Choice>();
            if (choiceElements != null)
            {
                foreach (var choiceElement in choiceElements)
                {
                    var label = ReadElement(choiceElement, "material", "mattext")?.Value;
                    if (label == null)
                        return null;

                    var identifier = GetAttributeValue(choiceElement, "ident");
                    var score = correctAnswers.Contains(identifier) ? 1 : 0;

                    choices.Add(new Choice { Identifier = identifier, Label = label, Score = score });
                }
            }

            return new List<Response> { new Response { Choices = choices } };
        }

        private List<Response> ReadMatchingQuestionResponses(XElement questionElement)
        {
            var outcomesElement = ReadElement(questionElement, "resprocessing", "outcomes");
            if (outcomesElement == null)
                return null;

            var decvarElements = ReadElements(outcomesElement, "decvar");
            if (decvarElements == null)
                return null;

            var scoreDecvarElement = decvarElements.FirstOrDefault(x => string.Equals(x.Attribute("varname").Value, "SCORE", StringComparison.OrdinalIgnoreCase));
            if (scoreDecvarElement == null)
            {
                _result.Error = $"Element 'decvar' with varname='SCORE' is not found";
                _result.ErrorLine = ((IXmlLineInfo)outcomesElement).LineNumber;
                return null;
            }

            var maxValueAttribute = GetAttributeValue(scoreDecvarElement, "maxvalue");
            if (maxValueAttribute == null)
                return null;

            var maxValue = double.Parse(maxValueAttribute);

            var correctAnswers = questionElement
                .Element(NS + "resprocessing")?
                .Elements(NS + "respcondition")?
                .Select(x => {
                    var varequalElement = x.Element(NS + "conditionvar")?.Element(NS + "varequal");
                    return new
                    {
                        ResponseID = varequalElement?.Attribute("respident")?.Value,
                        ChoiceID = varequalElement?.Value,
                        Score = double.Parse(x.Elements(NS + "setvar").FirstOrDefault(y => string.Equals(y.Attribute("varname")?.Value, "SCORE", StringComparison.OrdinalIgnoreCase)).Value) / maxValue
                    };
                }).ToDictionary(x => x.ResponseID);

            return questionElement
                .Element(NS + "presentation")?
                .Elements(NS + "response_lid")?
                .Select(x =>
                {
                    var identifier = x.Attribute("ident")?.Value;
                    var correctAnswer = correctAnswers[identifier];
                    return new Response
                    {
                        Identifier = identifier,
                        ResponseText = x.Element(NS + "material")?.Element(NS + "mattext")?.Value,
                        Choices = x.Element(NS + "render_choice")?
                                    .Elements(NS + "response_label")?
                                    .Select(y => new Choice
                                    {
                                        Identifier = y.Attribute("ident")?.Value,
                                        Label = y.Element(NS + "material")?.Element(NS + "mattext")?.Value,
                                        Score = string.Equals(correctAnswer.ChoiceID, y.Attribute("ident")?.Value, StringComparison.OrdinalIgnoreCase) ? correctAnswer.Score : 0
                                    }).ToList()
                    };
                }).ToList();
        }

        private List<Response> ReadShortAnswerResponses(XElement questionElement)
        {
            var respconditios = ReadElements(questionElement, "resprocessing", "respcondition");
            if (respconditios == null)
                return null;

            var correctResponse = respconditios.FirstOrDefault(x => x.Element(NS + "setvar") != null);
            if (correctResponse == null)
            {
                _result.Error = $"Correct response node is not found";
                _result.ErrorLine = ((IXmlLineInfo)questionElement).LineNumber;
                return null;
            }

            var textNode = ReadElement(correctResponse, "conditionvar", "varequal");
            if (textNode == null)
                return null;

            return new List<Response> { new Response { ResponseText = textNode.Value } };
        }

        private string GetMetaDataFieldValue(XElement element, string label)
        {
            var elements = ReadElements(element, "qtimetadata", "qtimetadatafield");

            if (elements == null)
                return null;

            return elements
                .FirstOrDefault(x => string.Equals(x.Element(NS + "fieldlabel")?.Value, label, StringComparison.OrdinalIgnoreCase))?
                .Element(NS + "fieldentry")?.Value;
        }

        private string GetAttributeValue(XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);

            if (attribute == null)
            {
                _result.Error = $"Attribute '{attributeName}' is not specified in the node '{element.Name.LocalName}'";
                _result.ErrorLine = ((IXmlLineInfo)element).LineNumber;
                return null;
            }

            return attribute.Value;
        }

        private XElement ReadElement(XElement element, params string[] path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                var nodeName = path[i];
                var child = element.Element(NS + nodeName);

                if (child == null)
                {
                    _result.Error = $"Node '{nodeName}' is not found";
                    _result.ErrorLine = ((IXmlLineInfo)element).LineNumber;
                    return null;
                }

                element = child;
            }

            return element;
        }

        private List<XElement> ReadElements(XElement element, params string[] path)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                var nodeName = path[i];
                var child = element.Element(NS + nodeName);

                if (child == null)
                {
                    _result.Error = $"Node '{nodeName}' is not found";
                    _result.ErrorLine = ((IXmlLineInfo)element).LineNumber;
                    return null;
                }

                element = child;
            }

            var collectionNodeName = path[path.Length - 1];
            var list = element.Elements(NS + collectionNodeName).ToList();

            if (list.Count == 0)
            {
                _result.Error = $"Node '{collectionNodeName}' is not found";
                _result.ErrorLine = ((IXmlLineInfo)element).LineNumber;
                return null;
            }

            return list;
        }

        #endregion
    }
}