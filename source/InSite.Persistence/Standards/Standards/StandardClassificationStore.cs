using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Write;

using Shift.Common;

namespace InSite.Persistence
{
    public static class StandardClassificationStore
    {
        #region Initialization

        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        #endregion

        #region INSERT

        public static void Insert(StandardClassification category)
        {
            _sendCommand(new AddStandardCategory(category.StandardIdentifier, category.CategoryIdentifier, category.ClassificationSequence));
        }

        #endregion

        #region UPDATE

        public static void ReplaceCategory(Guid assetIdentifier, Guid? categoryIdentifier)
        {
            DeleteByAssetIdentifier(assetIdentifier);

            if (!categoryIdentifier.HasValue) return;

            var category = new StandardClassification
            {
                StandardIdentifier = assetIdentifier,
                CategoryIdentifier = categoryIdentifier.Value,
                ClassificationSequence = 0
            };

            Insert(category);
        }

        #endregion

        #region DELETE

        public static void DeleteByAssetIdentifier(Guid assetIdentifier)
        {
            Guid[] categoryIds;
            using (var db = new InternalDbContext())
                categoryIds = db.QStandardCategories.Where(x => x.StandardIdentifier == assetIdentifier).Select(x => x.CategoryIdentifier).ToArray();

            if (categoryIds.IsNotEmpty())
                _sendCommand(new RemoveStandardCategory(assetIdentifier, categoryIds));
        }

        public static void Delete(StandardClassification classification)
        {
            _sendCommand(new RemoveStandardCategory(classification.StandardIdentifier, classification.CategoryIdentifier));
        }

        #endregion
    }
}
