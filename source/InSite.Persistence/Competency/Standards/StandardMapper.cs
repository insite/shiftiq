using System.Linq;

using Shift.Common;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.Persistence
{
    public static class StandardMapper
    {
        public static StandardModel Map(Standard entity)
        {
            if (entity == null)
                return null;

            var model = new StandardModel
            {
                StandardIdentifier = entity.StandardIdentifier,
                Code = entity.Code,
                Hook = entity.StandardHook,
                Icon = entity.Icon,
                IsHidden = entity.IsHidden,
                IsPractical = entity.IsPractical,
                IsPublished = entity.IsPublished,
                IsTheory = entity.IsTheory,
                Name = entity.ContentName,
                AssetNumber = entity.AssetNumber,
                Sequence = entity.Sequence,
                Source = entity.SourceDescriptor,
                StandardType = entity.StandardType,
                OrganizationIdentifier = entity.OrganizationIdentifier,
                Thumbprint = entity.StandardIdentifier,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Created = entity.Created,
                Modified = entity.Modified,
                UtcPublished = entity.UtcPublished
            };

            {
                var contents = TContentSearch.Instance.SelectContainer(entity.StandardIdentifier);
                model.Content = contents
                    .Select(x => new StandardContentModel
                    {
                        Language = x.ContentLanguage,
                        Label = x.ContentLabel,
                        Text = x.ContentText,
                        Html = x.ContentHtml
                    })
                    .ToList();
            }

            return model;
        }
    }
}
