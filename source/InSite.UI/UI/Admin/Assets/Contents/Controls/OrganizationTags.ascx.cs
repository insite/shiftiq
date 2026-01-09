using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assets.Contents.Controls
{
    public partial class OrganizationTags : BaseUserControl
    {
        #region Classes

        private class Tag
        {
            public string Color { get; set; }
            public string Text { get; set; }
            public bool Selected { get; set; }
        }

        private class TagCollection
        {
            public string Name { get; set; }
            public List<Tag> Tags { get; set; }
        }

        #endregion

        #region Public methods

        public bool LoadData(OrganizationState organization, List<Tuple<string, List<string>>> tags)
        {
            var tagCollections = TCollectionItemCache
                .Query(new TCollectionItemFilter
                {
                    OrganizationIdentifier = organization.OrganizationIdentifier,
                    CollectionName = CollectionName.Standards_Organizations_Classification_Flag
                })
                .GroupBy(x => x.ItemFolder ?? "Common")
                .Select(x => new TagCollection
                {
                    Name = x.Key,
                    Tags = x
                        .OrderBy(y => y.ItemSequence)
                        .ThenBy(y => y.ItemName)
                        .Select(y => new Tag { Color = y.ItemColor, Text = y.ItemName })
                        .ToList()
                }).ToList();

            if (tags != null)
            {
                var questionTagCollections = tags.Select(x => new TagCollection
                {
                    Name = x.Item1,
                    Tags = x.Item2.Select(y => new Tag
                    {
                        Text = y,
                        Selected = true
                    }).ToList()
                }).ToList();

                Merge(tagCollections, questionTagCollections);
            }

            tagCollections.Sort((x1, x2) => x1.Name.CompareTo(x2.Name));

            TagCollectionRepeater.ItemDataBound += TagCollectionRepeater_ItemDataBound;
            TagCollectionRepeater.DataSource = tagCollections;
            TagCollectionRepeater.DataBind();

            return tagCollections.Count > 0;
        }

        public List<Tuple<string, List<string>>> SaveData()
        {
            var tags = new List<Tuple<string, List<string>>>();

            foreach (RepeaterItem tagCollectionItem in TagCollectionRepeater.Items)
            {
                var collectionName = ((ITextControl)tagCollectionItem.FindControl("Name")).Text;
                var tagRepeater = (Repeater)tagCollectionItem.FindControl("TagRepeater");

                Tuple<string, List<string>> tagCollection = null;

                foreach (RepeaterItem tagItem in tagRepeater.Items)
                {
                    var selectedCheckBox = (ICheckBoxControl)tagItem.FindControl("Selected");
                    if (!selectedCheckBox.Checked)
                        continue;

                    var tagText = ((ITextControl)tagItem.FindControl("TagText")).Text;

                    if (tagCollection == null)
                        tags.Add(tagCollection = new Tuple<string, List<string>>(collectionName, new List<string>()));

                    tagCollection.Item2.Add(tagText);
                }
            }

            return tags;
        }

        #endregion

        #region Event handlers

        private void TagCollectionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var tagCollection = (TagCollection)e.Item.DataItem;

            var tagRepeater = (Repeater)e.Item.FindControl("TagRepeater");
            tagRepeater.DataSource = tagCollection.Tags;
            tagRepeater.DataBind();
        }

        #endregion

        #region Helper methods

        protected string GetTagText()
        {
            var tag = (Tag)Page.GetDataItem();
            var color = tag.Color.IsNotEmpty() ? $" text-{tag.Color.ToLower()}" : string.Empty;

            return $"<i class='fas fa-flag {color}'></i> " + tag.Text;
        }

        private static void Merge(List<TagCollection> tagCollections, List<TagCollection> questionTagCollections)
        {
            foreach (var questionTagCollection in questionTagCollections)
            {
                var tagCollection = tagCollections.Find(x => x.Name.Equals(questionTagCollection.Name, StringComparison.OrdinalIgnoreCase));

                if (tagCollection == null)
                {
                    tagCollections.Add(questionTagCollection);
                }
                else
                {
                    foreach (var tag in questionTagCollection.Tags)
                    {
                        var existingTag = tagCollection.Tags.Find(x => x.Text.Equals(tag.Text, StringComparison.OrdinalIgnoreCase));

                        if (existingTag == null)
                            tagCollection.Tags.Add(tag);
                        else
                            existingTag.Selected = true;
                    }
                }
            }
        }

        #endregion
    }
}