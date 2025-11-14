using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Jobs.Candidates.Controls
{
    public partial class LanguageAbilitySection : BaseUserControl
    {
        private class LanguageInputs
        {
            public FindCollectionItem Language { get; set; }
            public LanguageLevelComboBox Level { get; set; }
            public Common.Web.UI.CustomValidator Validator { get; set; }
        }

        private LanguageInputs[] _languageInputs;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _languageInputs = new[]
            {
                new LanguageInputs { Language = Language1, Level = LanguageLevel1 },
                new LanguageInputs { Language = Language2, Level = LanguageLevel2, Validator = Validator2 },
                new LanguageInputs { Language = Language3, Level = LanguageLevel3, Validator = Validator3 },
                new LanguageInputs { Language = Language4, Level = LanguageLevel4, Validator = Validator4 },
            };

            foreach (var inputs in _languageInputs)
            {
                if (inputs.Validator != null)
                    inputs.Validator.ServerValidate += Validator_ServerValidate;
            }
        }

        private void Validator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var inputs = _languageInputs.First(x => x.Validator == source);
            args.IsValid = !inputs.Language.Value.HasValue || args.Value.IsNotEmpty();

            if (args.IsValid)
                return;

            var item = TCollectionItemCache.Select(inputs.Language.Value.Value);

            inputs.Validator.ErrorMessage = $"The proficiency level for language '{item.ItemName}' is required";
        }

        public void SaveData(QPerson person)
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

        public void LoadData(Person person)
        {
            foreach (var input in _languageInputs)
                input.Language.Filter.CollectionName = CollectionName.Contacts_Settings_Languages_Name;

            foreach (var inputs in _languageInputs)
            {
                inputs.Language.Value = null;
                inputs.Level.ValueAsInt = null;
            }

            var proficiencies = TCandidateLanguageProficiencySearch.SelectByUser(person.UserIdentifier, person.OrganizationIdentifier);

            if (proficiencies.Count > 0)
            {
                var sharedCount = Math.Min(_languageInputs.Length, proficiencies.Count);

                for (var i = 0; i < sharedCount; i++)
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
                    CollectionName = CollectionName.Contacts_Settings_Languages_Name,
                    ItemName = "English"
                });

                Language1.Value = englishItem.ItemIdentifier;
            }
        }
    }
}