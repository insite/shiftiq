using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class ViewLanguageAbilitySection : InSite.Common.Web.UI.BaseUserControl
    {
        private class LanguageInputs
        {
            public Literal Language { get; set; }
            public Panel LanguagePanel { get; set; }
            public Literal Level { get; set; }
            public Panel LanguageLevelPanel { get; set; }
        }

        private List<LanguageInputs> _languageInputs;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitLanguageInputs();
        }

        public void BindModelToControls(Person person)
        {
            BindLanguageInputs();

            var proficiencies = TCandidateLanguageProficiencySearch.SelectByUser(person.UserIdentifier, person.OrganizationIdentifier);

            if (proficiencies.Count > 0)
                BindProficiencies(person, proficiencies);
            else
                BindDefaultLanguageLevel();
        }

        private void BindLanguageInputs()
        {
            foreach (var inputs in _languageInputs)
            {
                inputs.Language.Text = null;
                inputs.Level.Text = null;
                inputs.LanguagePanel.Visible = false;
                inputs.LanguageLevelPanel.Visible = false;
            }
        }

        private void BindDefaultLanguageLevel()
        {
            var hasEnglish = TCollectionItemCache.Exists(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                CollectionName = CollectionName.Contacts_Settings_Languages_Name,
                ItemName = "English"
            });

            if (hasEnglish)
            {
                Language1.Text = "English";
                LanguageLevel1.Text = "<p class=\"fw-light text-secondary\">Information not disclosed</p>";
                LanguagePanel1.Visible = true;
                LanguageLevelPanel1.Visible = true;
            }
            else
                LanguageAbilityCard.Visible = false;
        }

        private void BindProficiencies(Person person, IReadOnlyList<TCandidateLanguageProficiency> proficiencies)
        {
            var sharedCount = Math.Min(_languageInputs.Count, proficiencies.Count);

            var langCollection = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = person.OrganizationIdentifier,
                CollectionName = CollectionName.Contacts_Settings_Languages_Name
            });

            var langLevelCollection = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                CollectionName = CollectionName.Contacts_Settings_Languages_Proficiency
            });

            if (langLevelCollection.Count == 0)
                langLevelCollection = TCollectionItemCache.Select(new TCollectionItemFilter
                {
                    OrganizationIdentifier = OrganizationIdentifiers.Global,
                    CollectionName = CollectionName.Contacts_Settings_Languages_Proficiency
                });

            for (int i = 0; i < sharedCount; i++)
            {
                var level = proficiencies[i];
                var inputs = _languageInputs[i];

                inputs.Language.Text = langCollection.FirstOrDefault(x => x.ItemIdentifier == level.LanguageItemIdentifier)?.ItemName;
                inputs.Level.Text = langLevelCollection.FirstOrDefault(x => x.ItemSequence == level.ProficiencyLevel)?.ItemName;

                if (inputs.Language.Text.HasValue())
                {
                    inputs.LanguagePanel.Visible = true;
                    inputs.LanguageLevelPanel.Visible = true;
                    if (inputs.Level.Text.HasNoValue())
                        inputs.Level.Text = "<p class=\"fw-light text-secondary\">Information not disclosed</p>";
                }
            }
        }

        private void InitLanguageInputs()
        {
            if (_languageInputs != null)
                return;

            _languageInputs = new List<LanguageInputs>
            {
                new LanguageInputs { Language = Language1, Level = LanguageLevel1, LanguagePanel=LanguagePanel1, LanguageLevelPanel = LanguageLevelPanel1 },
                new LanguageInputs { Language = Language2, Level = LanguageLevel2, LanguagePanel=LanguagePanel2, LanguageLevelPanel = LanguageLevelPanel2 },
                new LanguageInputs { Language = Language3, Level = LanguageLevel3, LanguagePanel=LanguagePanel3, LanguageLevelPanel = LanguageLevelPanel3 },
                new LanguageInputs { Language = Language4, Level = LanguageLevel4, LanguagePanel=LanguagePanel4, LanguageLevelPanel = LanguageLevelPanel4 },
            };
        }
    }
}