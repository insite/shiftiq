using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Persistence;

using Shift.Constant;
using Shift.Sdk.UI;

using CustomValidator = InSite.Common.Web.UI.CustomValidator;
using FindCollectionItem = InSite.Common.Web.UI.FindCollectionItem;
using LanguageLevelComboBox = InSite.Common.Web.UI.LanguageLevelComboBox;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class LanguageAbilitySection : InSite.Common.Web.UI.BaseUserControl
    {
        private class LanguageInputs
        {
            public FindCollectionItem Language { get; set; }
            public LanguageLevelComboBox Level { get; set; }
            public CustomValidator Validator { get; set; }
        }

        private List<LanguageInputs> _languageInputs;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitLanguageInputs();

            foreach (var inputs in _languageInputs)
            {
                if (inputs.Validator != null)
                    inputs.Validator.ServerValidate += Validator_ServerValidate;
            }
        }

        private void Validator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var inputs = _languageInputs.Find(x => x.Validator == source);
            args.IsValid = !string.IsNullOrEmpty(args.Value) || inputs.Language.Value == null;

            if (!args.IsValid)
            {
                var item = TCollectionItemCache.Select(inputs.Language.Value.Value);

                inputs.Validator.ErrorMessage = $"The proficiency level for language '{item.ItemName}' is required";
            }
        }

        public void SaveProficiencies(QPerson person)
        {
            var proficiencies = new List<(Guid Language, int Level)>();

            foreach (var inputs in _languageInputs)
            {
                if (inputs.Language.Value == null || !inputs.Level.ValueAsInt.HasValue)
                    continue;

                proficiencies.Add((inputs.Language.Value.Value, inputs.Level.ValueAsInt.Value));
            }

            TCandidateLanguageProficiencyStore.Update(person.UserIdentifier, person.OrganizationIdentifier, proficiencies);
        }

        public void BindModelToControls(Person person)
        {
            ProficiencyDescriptionRepeater.DataSource = ProficiencyDescriptions.List;
            ProficiencyDescriptionRepeater.DataBind();

            foreach (var input in _languageInputs)
            {
                input.Language.Filter.CollectionName = CollectionName.Contacts_Settings_Languages_Name;
                input.Language.Filter.OrganizationIdentifier = Organization.Identifier;
            }

            foreach (var inputs in _languageInputs)
            {
                inputs.Language.Value = null;
                inputs.Level.ValueAsInt = null;
            }

            var proficiencies = TCandidateLanguageProficiencySearch.SelectByUser(person.UserIdentifier, person.OrganizationIdentifier);

            if (proficiencies.Count > 0)
            {
                var sharedCount = Math.Min(_languageInputs.Count, proficiencies.Count);

                for (int i = 0; i < sharedCount; i++)
                {
                    var level = proficiencies[i];
                    var inputs = _languageInputs[i];

                    inputs.Language.Value = level.LanguageItemIdentifier;
                    inputs.Level.ValueAsInt = level.ProficiencyLevel;
                }
            }
            else
            {
                var englishItem = TCollectionItemCache.SelectFirst(new TCollectionItemFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    CollectionName = CollectionName.Contacts_Settings_Languages_Name,
                    ItemName = "English"
                });

                if (englishItem != null)
                    Language1.Value = englishItem.ItemIdentifier;
            }
        }

        private void InitLanguageInputs()
        {
            if (_languageInputs != null)
                return;

            _languageInputs = new List<LanguageInputs>
            {
                new LanguageInputs { Language = Language1, Level = LanguageLevel1 },
                new LanguageInputs { Language = Language2, Level = LanguageLevel2, Validator = Validator2 },
                new LanguageInputs { Language = Language3, Level = LanguageLevel3, Validator = Validator3 },
                new LanguageInputs { Language = Language4, Level = LanguageLevel4, Validator = Validator4 },
            };
        }
    }
}