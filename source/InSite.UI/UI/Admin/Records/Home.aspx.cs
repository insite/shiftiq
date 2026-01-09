using System;

using InSite.Application.Records.Read;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

using CounterItem = InSite.UI.Admin.Foundations.Controls.HomeCounterRepeater.DataItem;

namespace InSite.UI.Admin.Records
{
    public partial class Dashboard : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls();

            LearningMastery.Visible = Identity.IsGranted(PermissionIdentifiers.Admin_Integrations_Canvas);
            AcademicYear.Visible = Identity.IsGranted(PermissionIdentifiers.Admin_Integrations_Canvas);

            LoadRecentChanges();
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            BindAchievements();
            BindGradebooks();
            BindLogbooks();
            BindPrograms();
            BindRubrics();
        }

        private void BindAchievements()
        {
            var filter = new QAchievementFilter(Organization.OrganizationIdentifier);

            AchievementsCounterRepeater.LoadData(
                new CounterItem
                {
                    Url = "/ui/admin/records/achievements/search",
                    Count = ServiceLocator.AchievementSearch.CountAchievements(filter),
                    Icon = "far fa-trophy",
                    Title = "Achievement Templates"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/credentials/search",
                    Count = ServiceLocator.AchievementSearch.CountCredentials(new VCredentialFilter
                    {
                        OrganizationIdentifier = Organization.OrganizationIdentifier
                    }),
                    Icon = "far fa-award",
                    Title = "Learner Achievements"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/achievement-layouts/search",
                    Count = TCertificateLayoutSearch.Count(new TCertificateLayoutFilter
                    {
                        OrganizationIdentifier = Organization.OrganizationIdentifier
                    }),
                    Icon = "far fa-ruler-combined",
                    Title = "Achievement Templates"
                }
            );
            AchievementsSection.Visible = AchievementsCounterRepeater.ItemsCount > 0;
        }

        private void BindGradebooks()
        {
            var isInstructor = Identity.IsGranted(PermissionIdentifiers.Portal_Gradebooks);
            var isAdmin = Identity.IsGranted(PermissionIdentifiers.Admin_Records);

            if (!isInstructor && !isAdmin)
            {
                GradebooksSection.Visible = false;
                return;
            }

            GradebookPanelForInstructors.Visible = isInstructor;
            GradebookPanelForAdministrators.Visible = isAdmin;
            HideGradebookTabs.Visible = !isInstructor && !isAdmin;

            BindGradebooksForAdmins(isAdmin);
            BindGradebooksForInstructor(isInstructor);

            if (!isInstructor)
                GradebookTabs.SelectedIndex = 1;
        }

        private void BindGradebooksForAdmins(bool isAdmin)
        {
            int gradebooks = 0, scores = 0, outcomes = 0;

            if (isAdmin)
            {
                gradebooks = ServiceLocator.RecordSearch.CountGradebooks(new QGradebookFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                });
                scores = ServiceLocator.RecordSearch.CountGradebookScores(new QProgressFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier
                });
                outcomes = ServiceLocator.RecordSearch.CountValidations(new QGradebookCompetencyValidationFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier
                });
            }

            GradebookForAdministratorsCounterRepeater.LoadData(
                new CounterItem
                {
                    Url = "/ui/admin/records/gradebooks/search",
                    Count = gradebooks,
                    Icon = "far fa-spell-check",
                    Title = "Gradebooks"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/scores/search",
                    Count = scores,
                    Icon = "far fa-bullseye-arrow",
                    Title = "Learner Scores"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/outcomes/search",
                    Count = outcomes,
                    Icon = "far fa-ballot-check",
                    Title = "Learning Outcomes"
                }
            );
        }

        private void BindGradebooksForInstructor(bool isInstructor)
        {
            int gradebooks = 0;

            if (isInstructor)
            {
                gradebooks = ServiceLocator.RecordSearch.CountGradebooks(new QGradebookFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    EventInstructorIdentifier = User.UserIdentifier,
                    IsLocked = false
                });
            }

            GradebookForInstructorsCounterRepeater.LoadData(
                new CounterItem
                {
                    Url = "/ui/admin/records/gradebooks/instructors/search",
                    Count = gradebooks,
                    Icon = "far fa-spell-check",
                    Title = "Gradebooks"
                }
            );
        }

        private void BindLogbooks()
        {
            var isValidator = Identity.IsGranted(PermissionIdentifiers.Portal_Logbooks);
            var isAdmin = Identity.IsGranted(PermissionIdentifiers.Admin_Records);

            if (!isValidator && !isAdmin)
            {
                LogbooksSection.Visible = false;
                return;
            }

            LogbookPanelForValidators.Visible = isValidator;
            LogbookPanelForAdministrators.Visible = isAdmin;
            HideLogbookTabs.Visible = !isValidator && !isAdmin;

            BindLogbooksForValidator(isValidator);
            BindLogbooksForAdmins(isAdmin);

            if (!isValidator)
                LogbookTabs.SelectedIndex = 1;
        }

        private void BindLogbooksForAdmins(bool isAdmin)
        {
            int logbooks = 0, learnersLogbooks = 0, logEntries = 0, logCompetencies = 0;

            if (isAdmin)
            {
                logbooks = CountLogbooks(null);
                learnersLogbooks = CountLearnersLogbooks(null);
                logEntries = CountLogEntries(null);
                logCompetencies = CountLogCompetencies(null);
            }

            LogbookForAdministratorsCounterRepeater.LoadData(
                new CounterItem
                {
                    Url = "/ui/admin/records/logbooks/search",
                    Count = logbooks,
                    Icon = "far fa-pencil-ruler",
                    Title = "Logbooks"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/logbooks/searchjournal/search",
                    Count = learnersLogbooks,
                    Icon = "far fa-book-open",
                    Title = "Learner Logbooks"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/logbooks/entries/search",
                    Count = logEntries,
                    Icon = "far fa-pencil-ruler",
                    Title = "Logged Entries"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/logbooks/competencies/search",
                    Count = logCompetencies,
                    Icon = "far fa-ruler-triangle",
                    Title = "Logged Competencies"
                }
            );
        }

        private void BindLogbooksForValidator(bool isValidator)
        {
            int validatorLogbooks = 0, validatorLearnerLogbooks = 0, validatorLogEntries = 0, validatorLogCompetencies = 0;

            if (isValidator)
            {
                validatorLogbooks = CountLogbooks(User.Identifier);
                validatorLearnerLogbooks = CountLearnersLogbooks(User.Identifier);
                validatorLogEntries = CountLogEntries(User.Identifier);
                validatorLogCompetencies = CountLogCompetencies(User.Identifier);
            }

            LogbookForValidatorsCounterRepeater.LoadData(
                new CounterItem
                {
                    Url = "/ui/admin/records/logbooks/validators/search",
                    Count = validatorLogbooks,
                    Icon = "far fa-pencil-ruler",
                    Title = "Logbooks"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/logbooks/validators/searchjournal/search",
                    Count = validatorLearnerLogbooks,
                    Icon = "far fa-book-open",
                    Title = "Learner Logbooks"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/logbooks/validators/entries/search",
                    Count = validatorLogEntries,
                    Icon = "far fa-pencil-ruler",
                    Title = "Logged Entries"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/logbooks/validators/competencies/search",
                    Count = validatorLogCompetencies,
                    Icon = "far fa-ruler-triangle",
                    Title = "Logged Competencies"
                }
            );
        }

        private void BindPrograms()
        {
            if (!Identity.IsGranted(PermissionIdentifiers.Admin_Records))
            {
                ProgramsSection.Visible = false;
                return;
            }

            ProgramsCounterRepeater.LoadData(
                new CounterItem
                {
                    Url = InSite.Admin.Records.Programs.Search.NavigateUrl,
                    Count = ProgramSearch.CountPrograms(new TProgramFilter
                    {
                        OrganizationIdentifier = Organization.OrganizationIdentifier
                    }),
                    Icon = "far fa-graduation-cap",
                    Title = "Programs"
                },
                new CounterItem
                {
                    Url = "/ui/admin/records/periods/search",
                    Count = ServiceLocator.PeriodSearch.CountPeriods(new QPeriodFilter
                    {
                        OrganizationIdentifier = Organization.OrganizationIdentifier
                    }),
                    Icon = "far fa-clock",
                    Title = "Periods"
                }
            );
        }

        private void BindRubrics()
        {
            if (!Identity.IsGranted(PermissionIdentifiers.Admin_Records))
            {
                RubricsSection.Visible = false;
                return;
            }

            RubricsCounterRepeater.LoadData(
                new CounterItem
                {
                    Url = Rurbics.Search.NavigateUrl,
                    Count = ServiceLocator.RubricSearch.CountRubrics(new QRubricFilter
                    {
                        OrganizationIdentifier = Organization.OrganizationIdentifier
                    }),
                    Icon = "far fa-table",
                    Title = "Rubrics"
                }
            );
        }

        private int CountLogbooks(Guid? instructor)
        {
            var filter = new QJournalSetupFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                ValidatorUserIdentifier = instructor
            };

            return ServiceLocator.JournalSearch.CountJournalSetups(filter);
        }

        private int CountLearnersLogbooks(Guid? instructor)
        {
            var filter = new VJournalSetupUserFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                ValidatorUserIdentifier = instructor,
                Role = JournalSetupUserRole.Learner
            };

            return ServiceLocator.JournalSearch.CountJournalSetupUsers(filter);
        }

        private int CountLogEntries(Guid? instructor)
        {
            var filter = new QExperienceFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                ValidatorUserIdentifier = instructor
            };

            return ServiceLocator.JournalSearch.CountExperiences(filter);
        }

        private int CountLogCompetencies(Guid? instructor)
        {
            var filter = new QExperienceCompetencyFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                ValidatorUserIdentifier = instructor
            };

            return ServiceLocator.JournalSearch.CountExperienceCompetencies(filter);
        }

        private void LoadRecentChanges()
        {
            if (!Identity.IsGranted(PermissionIdentifiers.Admin_Records))
            {
                HistoryPanel.Visible = false;
                return;
            }

            RecentGradebooks.LoadData(10);
            RecentCredentials.LoadData(10);
            HistoryPanel.Visible = RecentGradebooks.ItemCount > 0;
        }
    }
}