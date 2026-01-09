using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using Shift.Common.Timeline.Changes;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Forms
{
    public partial class Analysis : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private class QuestionSnapshot : Question
        {
            #region Properties

            public DateTimeOffset SnapshotDate { get; private set; }

            public int SnapshotVersion { get; private set; }

            public Guid SnapshotUser { get; private set; }

            public override int BankIndex
            {
                get => _bankIndex;
                set => throw new NotImplementedException();
            }

            public override int Sequence => _sequence;

            #endregion

            #region Fields

            private int _bankIndex;
            private int _sequence;

            #endregion

            #region Construction

            private QuestionSnapshot()
            {

            }

            #endregion

            #region Initialization

            public static QuestionSnapshot Create(IChange change, Question question)
            {
                var json = JsonConvert.SerializeObject(question);
                var snapshot = JsonConvert.DeserializeObject<QuestionSnapshot>(json);
                snapshot.Identifier = Guid.NewGuid();
                snapshot.SnapshotDate = change.ChangeTime;
                snapshot.SnapshotVersion = change.AggregateVersion;
                snapshot.SnapshotUser = change.OriginUser;
                snapshot._bankIndex = question.BankIndex;
                snapshot._sequence = question.Sequence;
                snapshot.Set = question.Set;

                return snapshot;
            }

            #endregion
        }

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request["bank"], out var value) ? value : Guid.Empty;

        private Guid QuestionID => Guid.TryParse(Request["question"], out var value) ? value : Guid.Empty;

        private Guid? FormID => Guid.TryParse(Request["form"], out var value) ? value : (Guid?)null;

        private int VersionCount
        {
            get => ViewState[nameof(VersionCount)] as int? ?? 0;
            set => ViewState[nameof(VersionCount)] = value;
        }

        #endregion

        #region Fields

        private static readonly string[] _questionDiffExclusions = new[]
        {
            nameof(Question.Identifier),
            nameof(QuestionSnapshot.SnapshotDate),
            nameof(QuestionSnapshot.SnapshotVersion)
        };

        #endregion

        #region Methods (initialization)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SnapshotLoad.Click += SnapshotLoad_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        protected override void CreateChildControls()
        {
            if (AnalysisNav.ItemsCount == 0)
            {
                for (var i = 0; i < VersionCount; i++)
                    AddAnalysisTab(out _, out _);
            }

            if (CommentsNav.ItemsCount == 0)
            {
                for (var i = 0; i < VersionCount; i++)
                    AddCommentTab(out _, out _);
            }

            base.CreateChildControls();
        }

        #endregion

        #region Event handlers

        private void SnapshotLoad_Click(object sender, EventArgs e)
        {
            var snapshots = new List<QuestionSnapshot>();
            var bankState = new BankState();
            var changes = ServiceLocator.ChangeStore.GetChanges("Bank", BankID, null);

            IChange prevChange = null;
            QuestionSnapshot prevSnapshot = null;
            var isFlushed = true;

            foreach (var c in changes)
            {
                bankState.Apply(c);

                var questionState = bankState.FindQuestion(QuestionID);

                if (prevSnapshot == null)
                {
                    if (questionState != null)
                        snapshots.Add(prevSnapshot = QuestionSnapshot.Create(c, questionState));
                }
                else if (c.OriginUser != c.OriginUser || (c.ChangeTime - prevChange.ChangeTime).TotalMilliseconds > 100)
                {
                    if (questionState == null)
                        throw new ApplicationException($"bankState.FindQuestion({QuestionID}) returned null during applying changes.");

                    if (IsQuestionChanged(prevSnapshot, questionState))
                        snapshots.Add(prevSnapshot = QuestionSnapshot.Create(c, questionState));

                    isFlushed = true;
                }
                else
                {
                    isFlushed = false;
                }

                prevChange = c;
            }

            if (!isFlushed)
            {
                var questionState = bankState.FindQuestion(QuestionID)
                    ?? throw new ApplicationException($"bankState.FindQuestion({QuestionID}) returned null after changes applied.");

                if (prevSnapshot == null)
                    throw new ApplicationException($"prevSnapshot for {QuestionID} is null after changes applied.");

                if (IsQuestionChanged(prevSnapshot, questionState))
                    snapshots.Add(prevSnapshot = QuestionSnapshot.Create(prevChange, questionState));
            }

            SnapshotRepeater.LoadData(snapshots, new QuestionRepeater.BindSettings
            {
                BankIdentifier = bankState.Identifier,
                OrganizationIdentifier = bankState.Tenant
            });

            var users = UserSearch
                .Bind(
                    x => new { x.UserIdentifier, x.FullName },
                    new UserFilter { IncludeUserIdentifiers = snapshots.Select(x => x.SnapshotUser).Distinct().ToArray() })
                .ToDictionary(x => x.UserIdentifier, x => x.FullName);
            var jsData = JsonHelper.SerializeJsObject(snapshots.Select(x => new
            {
                id = x.Identifier.ToString().ToLower(),
                date = x.SnapshotDate.Format(User.TimeZone),
                user = users.TryGetValue(x.SnapshotUser, out var userName) ? userName : UserNames.Someone,
                version = x.SnapshotVersion
            }));

            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "init_snapshots",
                $"questionAnalysis.afterSnapshotInit({jsData});",
                true);
        }

        private bool IsQuestionChanged(Question q1, Question q2)
        {
            if (q1 == null) throw new ArgumentNullException("q1");
            if (q1.Classification == null) throw new ArgumentNullException($"q1.Classification: {q1.Identifier}");
            if (q1.Layout == null) throw new ArgumentNullException($"q1.Layout: {q1.Identifier}");
            if (q1.Randomization == null) throw new ArgumentNullException($"q1.Randomization: {q1.Identifier}");
            if (q1.Matches == null) throw new ArgumentNullException($"q1.Matches: {q1.Identifier}");
            if (q1.Options == null) throw new ArgumentNullException($"q1.Options: {q1.Identifier}");

            if (q2 == null) throw new ArgumentNullException("q2");
            if (q2.Classification == null) throw new ArgumentNullException($"q2.Classification: {q2.Identifier}");
            if (q2.Layout == null) throw new ArgumentNullException($"q2.Layout: {q2.Identifier}");
            if (q2.Randomization == null) throw new ArgumentNullException($"q2.Randomization: {q2.Identifier}");
            if (q2.Matches == null) throw new ArgumentNullException($"q2.Matches: {q2.Identifier}");
            if (q2.Options == null) throw new ArgumentNullException($"q2.Options: {q2.Identifier}");

            return !q1.Content.IsEqual(q2.Content)
                || Shift.Common.ObjectComparer.IsChanged(q1, q2, _questionDiffExclusions)
                || !q1.Classification.Equals(q2.Classification)
                || !q1.Layout.Equals(q2.Layout)
                || !q1.Randomization.Equals(q2.Randomization)
                || !q1.Matches.Equals(q2.Matches)
                || q1.Options.Count != q2.Options.Count
                || q1.Options.Count > 0 && q1.Options.Zip(q2.Options, (o1, o2) => o1.Equals(o2)).Any(x => !x)
                ;
        }

        #endregion

        #region Methods (Binding)

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var question = bank.FindQuestion(QuestionID);
            if (question == null)
                RedirectToOutline();

            SetInputValues(question);
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Question question)
        {
            var bank = question.Set.Bank;

            PageHelper.AutoBindHeader(this, null, $"{bank} <span class='form-text'>Bank Asset #{bank.Asset}</span>");

            QuestionSection.Title = $"Question {question.BankIndex + 1}";

            VersionCount = 0;
            var questionFilter = new HashSet<Guid>();

            foreach (var version in question.EnumerateAllVersions(SortOrder.Descending))
            {
                var isCurrent = version.Identifier == question.Identifier;
                var hasPilotVersion = version.FirstPublished.HasValue && version.IsFirstVersion();

                {
                    AddAnalysisTab(out var navItem, out var content);

                    navItem.SetTitle(isCurrent ? $"<b>v{version.AssetVersion}</b>" : $"v{version.AssetVersion}", 0);
                    navItem.IsSelected = isCurrent;

                    content.LoadData(version, hasPilotVersion ? InclusionType.Exclude : InclusionType.Include);

                    if (hasPilotVersion)
                    {
                        AddAnalysisTab(out var pilotNavItem, out var pilotContent);

                        pilotNavItem.SetTitle("Pilot", 0);

                        pilotNavItem.Visible = pilotContent.LoadData(version, InclusionType.Only);
                    }
                }

                {
                    AddCommentTab(out var navItem, out var content);

                    var dataCount = content.LoadData(
                        version,
                        hasPilotVersion ? new DateTimeOffsetRange { Since = version.FirstPublished, } : null);

                    navItem.SetTitle(isCurrent ? $"<b>v{version.AssetVersion}</b>" : $"v{version.AssetVersion}", dataCount);
                    navItem.IsSelected = isCurrent;

                    if (hasPilotVersion)
                    {
                        AddCommentTab(out var pilotNavItem, out var pilotContent);

                        var pilotDataCount = pilotContent.LoadData(
                            version,
                            new DateTimeOffsetRange { Before = version.FirstPublished, });

                        pilotNavItem.Visible = pilotDataCount > 0;
                        pilotNavItem.SetTitle("Pilot", pilotDataCount);
                    }
                }

                VersionCount++;
                questionFilter.Add(version.Identifier);
            }

            {
                var questionChanges = QuestionChangeHelper.GetChanges(bank.Identifier, questionFilter).ToArray();

                ChangeRepeater.LoadData(bank.Identifier, questionChanges);

                HistorySection.SetTitle("History", questionChanges.Length);
            }

            CloseButton.NavigateUrl = GetOutlineUrl(question.Identifier);
        }

        #endregion

        #region Methods (redirect)

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToOutline(Guid? questionId = null)
        {
            var url = GetOutlineUrl(questionId);
            HttpResponseHelper.Redirect(url, true);
        }

        private string GetOutlineUrl(Guid? questionId)
        {
            var returnUrl = new ReturnUrl();
            var url = returnUrl.GetReturnUrl();

            if (url == null)
            {
                url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

                if (questionId.HasValue)
                    url += $"&question={questionId.Value}";

                if (FormID.HasValue)
                    url += $"&form={FormID.Value}&tab=fields";
            }

            return url;
        }

        #endregion

        #region Methods (helpers)

        private void AddAnalysisTab(out NavItem navItem, out AnalysisQuestionTabContent content)
        {
            AnalysisNav.AddItem(navItem = new NavItem());
            navItem.Controls.Add(content = (AnalysisQuestionTabContent)LoadControl("~/UI/Admin/Assessments/Questions/Controls/AnalysisQuestionTabContent.ascx"));
        }

        private void AddCommentTab(out NavItem navItem, out AnalysisCommentTabContent content)
        {
            CommentsNav.AddItem(navItem = new NavItem());
            navItem.Controls.Add(content = (AnalysisCommentTabContent)LoadControl("~/UI/Admin/Assessments/Questions/Controls/AnalysisCommentTabContent.ascx"));
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
