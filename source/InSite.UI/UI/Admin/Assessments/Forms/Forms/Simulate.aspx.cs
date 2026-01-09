using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Simulate : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private static readonly FileFormatInfo[] FileFormats = new[]
        {
            new FileFormatInfo(
                "csv",
                "Shift iQ Comma Separated Values (*.csv)",
                null),

            new FileFormatInfo(
                "scantron",
                "Scantron Answer Sheet - Template 994 (*.txt)",
                BuildScantron),

            new FileFormatInfo(
                "lxr",
                "LXR Merge (*.LXRMerge)",
                null),
        };

        #endregion

        #region Classes

        private class AttemptUserInfo
        {
            public string Name { get; set; }
            public string Code { get; set; }
        }

        private class FileFormatInfo
        {
            #region Properties

            public string ID { get; }
            public string Title { get; }
            public Func<FileFormatSettings, byte[]> Build { get; }

            #endregion

            #region Construction

            public FileFormatInfo(string id, string title, Func<FileFormatSettings, byte[]> build)
            {
                ID = id;
                Title = title;
                Build = build;
            }

            #endregion
        }

        private class FileFormatSettings
        {
            public Question[] Questions { get; internal set; }

            public int ScoreFrom { get; set; }
            public int ScoreThru { get; set; }

            public int UsersNumber { get; set; }

            public int AttemptsNumberFrom { get; set; }
            public int AttemptsNumberThru { get; set; }
        }

        [Serializable]
        private class ControlData
        {
            #region Properties

            public string FormName { get; }

            #endregion

            #region Construction

            public ControlData(Form form)
            {
                FormName = form.Name;
            }

            #endregion
        }

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.Parse(Request.QueryString["form"]);

        private ControlData CurrentData
        {
            get => (ControlData)ViewState[nameof(CurrentData)];
            set => ViewState[nameof(CurrentData)] = value;
        }

        #endregion

        #region Methods (initialization and loading)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BuildButton.Click += BuildButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (CurrentData == null)
                Open();
        }

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var form = bank.FindForm(FormID);
            if (form == null)
                RedirectToReader();

            var title = $"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)} <span class=\"fw-normal form-text\">Asset #{form.Asset}</span>";
            PageHelper.AutoBindHeader(this, null, title);

            FormDetails.BindForm(form, BankID, form.Specification.Bank.IsAdvanced);

            FileFormat.LoadItems(FileFormats.Where(x => x.Build != null), "ID", "Title");

            CurrentData = new ControlData(form);

            ScoreFrom.ValueAsInt = 50;
            ScoreThru.ValueAsInt = 100;

            UsersNumber.ValueAsInt = 1;

            AttemptsNumberFrom.ValueAsInt = 5;
            AttemptsNumberThru.ValueAsInt = 5;

            CloseButton.NavigateUrl = GetReaderUrl(FormID);
        }

        #endregion

        #region Event handlers

        private void BuildButton_Click(object sender, EventArgs e)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var form = bank.FindForm(FormID);

            var settings = new FileFormatSettings
            {
                Questions = form.Sections
                    .SelectMany(x => x.Fields)
                    .Select(x => x.Question)
                    .Where(x => x.Type == QuestionItemType.SingleCorrect)
                    .ToArray(),

                ScoreFrom = ScoreFrom.ValueAsInt.Value,
                ScoreThru = ScoreThru.ValueAsInt.Value,

                UsersNumber = UsersNumber.ValueAsInt.Value,

                AttemptsNumberFrom = AttemptsNumberFrom.ValueAsInt.Value,
                AttemptsNumberThru = AttemptsNumberThru.ValueAsInt.Value,
            };

            try
            {
                var formatId = FileFormat.Value;
                var formatInfo = FileFormats.Single(x => x.ID == formatId);
                var data = formatInfo.Build(settings);
                var fileName = string.Format(
                    "scantron-{0:yyyyMMdd}-{0:HHmmss}-{1}.txt",
                    DateTime.UtcNow,
                    FileHelper.AdjustFileName(CurrentData.FormName));

                Response.SendFile(fileName, data);
            }
            catch (ApplicationError err)
            {
                EditorStatus.AddMessage(AlertType.Error, err.Message);
            }
        }

        #endregion

        #region Methods (build)

        private static byte[] BuildScantron(FileFormatSettings settings)
        {
            var pointsSum = settings.Questions.Sum(x => x.Options.Max(y => y.Points));
            var attemptUsers = GetAttemptUsers(settings.UsersNumber, settings.AttemptsNumberFrom, settings.AttemptsNumberThru);

            var referenceGenerator = new RandomStringGenerator(RandomStringType.Numeric, 22);
            var date = DateTime.Now.ToString("yyyy/MM/dd");

            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms, Encoding.UTF8))
                {
                    foreach (var user in attemptUsers)
                    {
                        var targetScore = settings.ScoreFrom >= settings.ScoreThru
                            ? settings.ScoreFrom
                            : RandomNumberGenerator.Instance.Next(settings.ScoreFrom, settings.ScoreThru + 1);
                        var targetPoints = pointsSum * targetScore / 100m;
                        var answers = GetAnswers(settings.Questions, targetPoints);

                        writer.Write("SCANTRONSIMULATION" + referenceGenerator.Next().ToUpper());
                        writer.Write(user.Code?.PadRight(9));
                        writer.Write(date);
                        writer.Write(user.Name.ToUpper().PadRight(23));

                        for (var i = 0; i < settings.Questions.Length; i++)
                        {
                            var q = settings.Questions[i];
                            var o = answers[q.Identifier];

                            writer.Write(o);
                        }

                        if (settings.Questions.Length < 150)
                            writer.Write(new string(' ', 150 - settings.Questions.Length));

                        writer.Write(Environment.NewLine);
                    }
                }

                return ms.ToArray();
            }
        }

        private static AttemptUserInfo[] GetAttemptUsers(int usersNumber, int attemptsNumberFrom, int attemptsNumberThru)
        {
            var numberGenerator = RandomNumberGenerator.Instance;
            var nameGenerator = RandomPersonNameGenerator.Instance;

            IEnumerable<AttemptUserInfo> users;

            if (usersNumber > 1)
            {
                users = Enumerable.Range(0, usersNumber).Select(x =>
                {
                    var gender = numberGenerator.Next() % 2 == 0 ? GenderType.Male : GenderType.Female;
                    var firstName = nameGenerator.GetFirstName(gender, out var firstNameIndex);
                    var lastName = nameGenerator.GetLastName(out var lastNameIndex);

                    return new AttemptUserInfo
                    {
                        Name = $"{firstName} {lastName}",
                        Code = $"{(gender == GenderType.Male ? "M" : "F")}.{Calculator.ToBase26(firstNameIndex + 1)}.{Calculator.ToBase26(lastNameIndex + 1)}"
                    };
                });
            }
            else
            {
                var user = PersonCriteria.BindFirst(
                    x => new
                    {
                        x.User.FirstName,
                        x.User.LastName,
                        x.PersonCode
                    },
                    new PersonFilter
                    {
                        OrganizationIdentifier = Organization.Identifier,
                        UserIdentifier = User.UserIdentifier
                    });

                users = new[]
                {
                    new AttemptUserInfo
                    {
                        Name = $"{user.FirstName} {user.LastName}",
                        Code = user.PersonCode
                    }
                };
            }

            var result = users.SelectMany(x =>
            {

                var count = attemptsNumberFrom >= attemptsNumberThru
                    ? attemptsNumberFrom
                    : numberGenerator.Next(attemptsNumberFrom, attemptsNumberThru + 1);

                return Enumerable.Range(0, count).Select(y => x);
            }).ToArray();

            result.Shuffle();

            return result;
        }

        private static Dictionary<Guid, string> GetAnswers(IEnumerable<Question> questions, decimal targetPoints)
        {
            var shuffledArray = questions.ToArray();
            shuffledArray.Shuffle();

            var totalPoints = 0m;
            var result = new Dictionary<Guid, string>();

            foreach (var q in shuffledArray)
            {
                var optionPoints = 0m;
                var optionLetter = "?";

                var shuffledOptions = q.Options.ToArray();
                shuffledOptions.Shuffle();

                var o = totalPoints < targetPoints
                    ? shuffledOptions.Where(x => x.Points > 0).FirstOrDefault()
                    : shuffledOptions.Where(x => x.Points == 0).FirstOrDefault();

                if (o != null)
                {
                    optionPoints = o.Points;
                    optionLetter = o.Letter;
                }

                totalPoints += optionPoints;

                result.Add(q.Identifier, optionLetter);
            }

            return result;
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? formId = null)
        {
            var url = GetReaderUrl(formId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? formId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (formId.HasValue)
                url += $"&form={formId.Value}";

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}
