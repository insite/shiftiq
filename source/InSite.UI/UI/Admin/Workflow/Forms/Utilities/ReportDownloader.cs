using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Domain.Surveys.Forms;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    public class ReportDownloader
    {
        private static readonly HashSet<SurveyQuestionType> ExcludeTypes = new HashSet<SurveyQuestionType>
        {
            SurveyQuestionType.BreakQuestion,
            SurveyQuestionType.Terminate,
            SurveyQuestionType.BreakPage
        };

        private Encoding _exportEncoding;
        private string _optionFormat;
        private SurveyForm _survey;
        private Dictionary<Guid, SurveyQuestion> _qMapping;
        private Dictionary<Guid, SurveyOptionItem> _sOptionMapping;
        private List<ISurveyResponse> _responses;
        private Dictionary<Guid, QResponseAnswer[]> _responseAnswerMapping;
        private Dictionary<Guid, QResponseOption[]> _responseOptionMapping;

        private ZipArchive _archive;

        public (string surveyName, byte[] report) Create(Guid surveyId, string encoding, bool includeSystemFiles, string optionFormat)
        {
            if (!ReadData(surveyId, encoding))
                return (null, null);

            _optionFormat = optionFormat;

            var report = CreateArchive(includeSystemFiles);

            return (_survey.Name, report);
        }

        private bool ReadData(Guid surveyId, string encoding)
        {
            _exportEncoding = new UTF8Encoding(true);
            if (encoding == "Unicode")
                _exportEncoding = Encoding.Unicode;

            _survey = ServiceLocator.SurveySearch.GetSurveyState(surveyId)?.Form;
            if (_survey == null)
                return false;

            _qMapping = _survey.Questions.ToDictionary(x => x.Identifier);
            _sOptionMapping = _survey.Questions.SelectMany(x => x.Options.Lists).SelectMany(x => x.Items).ToDictionary(x => x.Identifier);
            _responses = ServiceLocator.SurveySearch.GetResponseSessions(new QResponseSessionFilter { SurveyFormIdentifier = surveyId });

            var answers = ServiceLocator.SurveySearch.GetAnswersByResponse(surveyId);
            _responseAnswerMapping = answers
                .GroupBy(x => x.ResponseSessionIdentifier)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var options = ServiceLocator.SurveySearch.GetOptionsByResponse(surveyId);
            _responseOptionMapping = options
                .GroupBy(x => x.ResponseSessionIdentifier)
                .ToDictionary(x => x.Key, x => x.ToArray());

            return true;
        }

        private byte[] CreateArchive(bool includeSystemFiles)
        {
            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    _archive = archive;

                    if (includeSystemFiles)
                    {
                        AddQuestions();
                        AddRespondents();
                        AddAnswers();
                        AddChoices();
                    }

                    AddSurveyDataSet();
                }

                return outStream.ToArray();
            }
        }

        private Stream CreateFile(string fileName)
        {
            var fileInArchive = _archive.CreateEntry(fileName, CompressionLevel.Optimal);
            return fileInArchive.Open();
        }

        private void AddQuestions()
        {
            using (var stream = CreateFile("Questions.csv"))
            {
                using (var writer = new StreamWriter(stream, _exportEncoding))
                {
                    writer.WriteLine("QuestionID,QuestionCode,QuestionSequence,QuestionText,QuestionType,OptionSequence,OptionValue,OptionText");

                    foreach (var question in _survey.Questions)
                    {
                        AppendFirstValue(writer, question.Identifier);
                        AppendValue(writer, $"Q{question.Sequence:00}");
                        AppendValue(writer, question.Sequence);
                        AppendValue(writer, question.Content?.Title.GetText());
                        AppendValue(writer, question.Type.GetDescription());
                        AppendValue(writer, null);
                        AppendValue(writer, null);
                        AppendValue(writer, null);
                        writer.WriteLine();

                        if (!question.Options.IsEmpty)
                        {
                            foreach (var option in question.Options.Lists.SelectMany(x => x.Items))
                            {
                                AppendFirstValue(writer, null);
                                AppendValue(writer, null);
                                AppendValue(writer, null);
                                AppendValue(writer, null);
                                AppendValue(writer, null);
                                AppendValue(writer, option.Sequence);
                                AppendValue(writer, option.Letter);
                                AppendValue(writer, option.Content?.Title.GetText());
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }
        }

        private void AddRespondents()
        {
            var columns = "ResponseIdentifier,ResponseCreated,ResponseStarted,ResponseCompleted,ResponseIsCompleted";
            if (!_survey.EnableUserConfidentiality)
                columns += ",ContactIdentifier,ContactEmail,ContactFirstName,ContactLastName,ContactName,ContactLanguage";

            using (var stream = CreateFile("Respondents.csv"))
            {
                using (var writer = new StreamWriter(stream, _exportEncoding))
                {
                    writer.WriteLine(columns);

                    foreach (var response in _responses.OrderBy(x => GetCreated(x)))
                    {
                        AppendFirstValue(writer, response.ResponseSessionIdentifier);
                        AppendValue(writer, response.ResponseSessionCreated);
                        AppendValue(writer, response.ResponseSessionStarted);
                        AppendValue(writer, response.ResponseSessionCompleted);
                        AppendValue(writer, response.ResponseSessionCompleted.HasValue);

                        if (!_survey.EnableUserConfidentiality)
                        {
                            AppendValue(writer, response.RespondentUserIdentifier);
                            AppendValue(writer, response.RespondentEmail);
                            AppendValue(writer, response.RespondentNameFirst);
                            AppendValue(writer, response.RespondentNameLast);
                            AppendValue(writer, response.RespondentName);
                            AppendValue(writer, response.RespondentLanguage);
                        }

                        writer.WriteLine();
                    }
                }
            }
        }

        private void AddAnswers()
        {
            using (var stream = CreateFile("Answers.csv"))
            {
                using (var writer = new StreamWriter(stream, _exportEncoding))
                {
                    writer.WriteLine("ResponseIdentifier,QuestionIdentifier,OptionIdentifier,QuestionSequence,QuestionType,Answer,AnswerCode");

                    foreach (var response in _responses)
                    {
                        if (!_responseAnswerMapping.TryGetValue(response.ResponseSessionIdentifier, out var responseAnswers))
                            responseAnswers = new QResponseAnswer[0];

                        if (!_responseOptionMapping.TryGetValue(response.ResponseSessionIdentifier, out var responseOptions))
                            responseOptions = new QResponseOption[0];

                        var data = responseAnswers
                            .Where(x => _qMapping.ContainsKey(x.SurveyQuestionIdentifier))
                            .Select(x => new
                            {
                                Answer = x,
                                Question = _qMapping[x.SurveyQuestionIdentifier]
                            })
                            .OrderBy(x => x.Question.Sequence);

                        foreach (var item in data)
                        {
                            var test = responseOptions
                                    .Where(x => x.ResponseOptionIsSelected && _sOptionMapping.ContainsKey(x.SurveyOptionIdentifier) && _sOptionMapping[x.SurveyOptionIdentifier].Question.Identifier == item.Question.Identifier)
                                    .ToArray();

                            var option = item.Question.Type == SurveyQuestionType.RadioList || item.Question.Type == SurveyQuestionType.Selection
                                ? responseOptions
                                    .Where(x => x.ResponseOptionIsSelected && _sOptionMapping.ContainsKey(x.SurveyOptionIdentifier) && _sOptionMapping[x.SurveyOptionIdentifier].Question.Identifier == item.Question.Identifier)
                                    .Select(x => _sOptionMapping[x.SurveyOptionIdentifier])
                                    .SingleOrDefault()
                                : null;

                            AppendFirstValue(writer, response.ResponseSessionIdentifier);
                            AppendValue(writer, item.Question.Identifier);
                            AppendValue(writer, option?.Identifier);
                            AppendValue(writer, item.Question.Sequence);
                            AppendValue(writer, item.Question.Type.GetDescription());

                            if (item.Question.Type == SurveyQuestionType.Upload)
                                AppendValue(writer, HttpRequestHelper.CurrentRootUrl + item.Answer.ResponseAnswerText);
                            else
                                AppendValue(writer, item.Answer.ResponseAnswerText);

                            AppendValue(writer, option?.Content?.Title.GetText());

                            writer.WriteLine();
                        }
                    }
                }
            }
        }

        private void AddChoices()
        {
            using (var stream = CreateFile("Choices.csv"))
            {
                using (var writer = new StreamWriter(stream, _exportEncoding))
                {
                    writer.WriteLine("ResponseIdentifier,QuestionIdentifier,OptionIdentifier,AnswerSequence,AnswerValue,AnswerText,AnswerScore,AnswerComment");

                    foreach (var response in _responses)
                    {
                        if (!_responseAnswerMapping.TryGetValue(response.ResponseSessionIdentifier, out var responseAnswers))
                            responseAnswers = new QResponseAnswer[0];

                        if (!_responseOptionMapping.TryGetValue(response.ResponseSessionIdentifier, out var responseOptions))
                            responseOptions = new QResponseOption[0];

                        var data = responseOptions
                            .Where(x => x.ResponseOptionIsSelected && _sOptionMapping.ContainsKey(x.SurveyOptionIdentifier))
                            .Select(x => new
                            {
                                ResponseOption = x,
                                SurveyOption = _sOptionMapping[x.SurveyOptionIdentifier]
                            })
                            .OrderBy(x => x.SurveyOption.Question.Sequence)
                            .ThenBy(x => x.SurveyOption.Sequence);

                        foreach (var item in data)
                        {
                            var answerComment = item.SurveyOption.Question.ListEnableOtherText
                                ? responseAnswers
                                    .SingleOrDefault(x => x.SurveyQuestionIdentifier == item.SurveyOption.Question.Identifier)
                                    ?.ResponseAnswerText
                                : null;

                            AppendFirstValue(writer, response.ResponseSessionIdentifier);
                            AppendValue(writer, item.SurveyOption.Question.Identifier);
                            AppendValue(writer, item.SurveyOption.Identifier);
                            AppendValue(writer, item.SurveyOption.Sequence);
                            AppendValue(writer, item.SurveyOption.Letter);
                            AppendValue(writer, item.SurveyOption.Content?.Title.GetText());
                            AppendValue(writer, item.SurveyOption.Points);
                            AppendValue(writer, StringHelper.StripHtml(answerComment));

                            writer.WriteLine();
                        }
                    }
                }
            }
        }

        private void AddSurveyDataSet()
        {
            using (var stream = CreateFile("SurveyDataSet.csv"))
            {
                using (var writer = new StreamWriter(stream, _exportEncoding))
                {
                    AddSurveyDataSetColumns(writer);
                    AddSurveyDataSetRows(writer);
                }
            }
        }

        private void AddSurveyDataSetColumns(StreamWriter writer)
        {
            writer.Write("LastModified,ResponseIdentifier");

            if (!_survey.EnableUserConfidentiality)
                writer.Write(",ContactEmail,ContactName");

            foreach (var question in _survey.Questions)
            {
                if (ExcludeTypes.Contains(question.Type))
                    continue;

                if (question.Type == SurveyQuestionType.Likert)
                {
                    foreach (var list in question.Options.Lists)
                        writer.Write($",Q{question.Sequence:00}_L{list.Sequence:00}");
                }
                else if (question.Type == SurveyQuestionType.CheckList)
                {
                    foreach (var option in question.Options.Lists.SelectMany(x => x.Items))
                        writer.Write($",Q{question.Sequence:00}_O{option.Sequence:00}");
                }
                else
                    writer.Write($",Q{question.Sequence:00}");

                if (IsQuantitative(question.Type) && question.ListEnableOtherText)
                    writer.Write($",Q{question.Sequence:00}_Other");
            }

            writer.WriteLine();
        }

        private void AddSurveyDataSetRows(StreamWriter writer)
        {
            foreach (var response in _responses)
            {
                if (!_responseAnswerMapping.TryGetValue(response.ResponseSessionIdentifier, out var responseAnswers))
                    responseAnswers = new QResponseAnswer[0];

                if (!_responseOptionMapping.TryGetValue(response.ResponseSessionIdentifier, out var responseOptions))
                    responseOptions = new QResponseOption[0];

                AppendFirstValue(writer, GetModified(response));
                AppendValue(writer, response.ResponseSessionIdentifier);

                if (!_survey.EnableUserConfidentiality)
                {
                    AppendValue(writer, response.RespondentEmail);
                    AppendValue(writer, response.RespondentName);
                }

                var answerMapping = responseAnswers.ToDictionary(x => x.SurveyQuestionIdentifier);
                var optionMapping = responseOptions
                    .Where(x => x.ResponseOptionIsSelected && _sOptionMapping.ContainsKey(x.SurveyOptionIdentifier))
                    .Select(x => _sOptionMapping[x.SurveyOptionIdentifier].Identifier)
                    .ToHashSet();

                AddSurveyDataSetQuestions(writer, answerMapping, optionMapping);

                writer.WriteLine();
            }
        }

        private void AddSurveyDataSetQuestions(StreamWriter writer, Dictionary<Guid, QResponseAnswer> answerMapping, HashSet<Guid> optionMapping)
        {
            foreach (var question in _survey.Questions)
            {
                if (ExcludeTypes.Contains(question.Type))
                    continue;

                if (!answerMapping.TryGetValue(question.Identifier, out var answer))
                    answer = null;

                if (question.Type == SurveyQuestionType.RadioList || question.Type == SurveyQuestionType.Selection || question.Type == SurveyQuestionType.Likert)
                {
                    var lists = question.Options.Lists.AsQueryable();
                    if (question.Type != SurveyQuestionType.Likert)
                        lists = lists.Take(1);

                    foreach (var list in lists)
                    {
                        var option = list.Items.SingleOrDefault(x => optionMapping.Contains(x.Identifier));
                        if (option == null)
                            AppendValue(writer, null);
                        else if (_optionFormat == "Code")
                            AppendValue(writer, option.Letter);
                        else if (_optionFormat == "Number")
                            AppendValue(writer, option.Sequence);
                        else
                            AppendValue(writer, option.Content?.Title.GetText());
                    }
                }
                else if (question.Type == SurveyQuestionType.CheckList)
                {
                    foreach (var option in question.Options.Lists.SelectMany(x => x.Items))
                    {
                        var isSelected = optionMapping.Contains(option.Identifier);
                        if (option.Points != 0)
                            AppendValue(writer, $"{(isSelected ? option.Points : 0):n2}");
                        else
                            AppendValue(writer, isSelected ? "Yes" : "No");
                    }
                }
                else
                {
                    var text = answer?.ResponseAnswerText;
                    if (question.Type == SurveyQuestionType.Upload)
                        AppendValue(writer, HttpRequestHelper.CurrentRootUrl + text ?? String.Empty);
                    else
                        AppendValue(writer, text);
                }

                if (IsQuantitative(question.Type) && question.ListEnableOtherText)
                    AppendValue(writer, answer?.ResponseAnswerText);
            }
        }

        private static void AppendFirstValue(StreamWriter writer, object value)
        {
            if (value != null)
                writer.Write(GetCsvValue(value));
        }

        private static void AppendValue(StreamWriter writer, object value)
        {
            writer.Write(",");

            if (value != null)
                writer.Write(GetCsvValue(value));
        }

        private static string GetCsvValue(object value)
        {
            var s = value.ToString();
            s = s.Replace("\"", "\"\"");
            s = s.Replace("\r\n", " | ");

            return $"\"{s}\"";
        }

        private static bool IsQuantitative(SurveyQuestionType t) =>
            t == SurveyQuestionType.CheckList
            || t == SurveyQuestionType.RadioList
            || t == SurveyQuestionType.Selection
            || t == SurveyQuestionType.Likert;

        private static DateTimeOffset? GetCreated(ISurveyResponse r) =>
            r.ResponseSessionCreated ?? r.ResponseSessionStarted ?? r.ResponseSessionCompleted;

        private static DateTimeOffset? GetModified(ISurveyResponse r) =>
            r.ResponseSessionCompleted ?? r.ResponseSessionStarted ?? r.ResponseSessionCreated;
    }
}