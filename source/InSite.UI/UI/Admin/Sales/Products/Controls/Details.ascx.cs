using System;
using System.Web.UI;

using InSite.Application.Files.Read;
using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;
using InSite.UI;

using Shift.Common;

namespace InSite.Admin.Invoices.Products.Controls
{
    public partial class Details : UserControl
    {
        public event EventHandler Updated;

        public Guid? ObjectIdentifier
        {
            get => (Guid?)ViewState[nameof(ObjectIdentifier)];
            set => ViewState[nameof(ObjectIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProductType.AutoPostBack = true;
            ProductType.ValueChanged += ProductType_ValueChanged;

            CourseIdentifier.AutoPostBack = true;
            CourseIdentifier.ValueChanged += CourseIdentifier_ValueChanged;
        }

        public void SetInputValues(TProduct entity)
        {
            ProductName.Text = entity.ProductName;
            ProductSummary.Text = entity.ProductSummary;
            ProductDescription.Text = entity.ProductDescription;
            IndustryItemIdentifier.ValueAsGuid = entity.IndustryItemIdentifier;
            OccupationItemIdentifier.ValueAsGuid = entity.OccupationItemIdentifier;
            LevelItemIdentifier.ValueAsGuid = entity.LevelItemIdentifier;
            ProductType.Value = entity.ProductType;
            ProductPrice.ValueAsDecimal = entity.ProductPrice;
            ProductUrl.Text = entity.ProductUrl;
            IsFeatured.Checked = entity.IsFeatured;
            IsTaxable.Checked = entity.IsTaxable;
            ObjectIdentifier = entity.ObjectIdentifier;

            SetObjectTypeValues(entity);

            if (entity.ProductImageUrl.HasValue())
                ProductImage.Text = $"<img src='{entity.ProductImageUrl}' alt='Product Image' />";

        }

        private void SetObjectTypeValues(TProduct entity)
        {
            CoursePanel.Visible = entity.ProductType == "Online Course";
            if (CoursePanel.Visible & entity.ObjectIdentifier.HasValue)
                CourseIdentifier.Value = entity.ObjectIdentifier.Value;

            AssessmentPanel.Visible = entity.ProductType == "Online Assessment";
            if (AssessmentPanel.Visible & entity.ObjectIdentifier.HasValue)
                AssessmentIdentifier.Value = entity.ObjectIdentifier.Value;
        }

        public void GetInputValues(TProduct entity)
        {
            entity.ProductName = ProductName.Text;
            entity.ProductSummary = ProductSummary.Text;
            entity.ProductDescription = ProductDescription.Text;
            entity.IndustryItemIdentifier = IndustryItemIdentifier.ValueAsGuid;
            entity.OccupationItemIdentifier = OccupationItemIdentifier.ValueAsGuid;
            entity.LevelItemIdentifier = LevelItemIdentifier.ValueAsGuid;
            entity.ProductType = ProductType.Value;
            entity.ProductPrice = ProductPrice.ValueAsDecimal;
            entity.ProductUrl = ProductUrl.Text;
            entity.IsFeatured = IsFeatured.Checked;
            entity.IsTaxable = IsTaxable.Checked;
            entity.ObjectIdentifier = GetObjectIdentifier(ProductType.Value);
            entity.ObjectType = GetObjectType(ProductType.Value);

            if (ProductImageUpload.HasFile)
            {
                var model = ProductImageUpload.SaveFile(entity.ProductIdentifier, FileObjectType.Product);
                var files = ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName, false);
                var imageUrl = PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, CurrentSessionState.Identity.Organization.OrganizationCode, files);

                if (!string.IsNullOrWhiteSpace(entity.ProductImageUrl))
                    FileUploadV2.DeleteFileByUrl(entity.ProductImageUrl);

                entity.ProductImageUrl = imageUrl;
            }
        }

        private void ProductType_ValueChanged(object sender, EventArgs e)
        {
            ProductTypeSelected();
            Updated?.Invoke(this, new EventArgs());
        }

        private void CourseIdentifier_ValueChanged(object sender, EventArgs e)
        {
            ObjectIdentifier = CourseIdentifier.Value;
            Updated?.Invoke(this, new EventArgs());
        }

        private void ProductTypeSelected()
        {
            ObjectIdentifier = null;

            CoursePanel.Visible = ProductType.Value == "Online Course";
            if (!CoursePanel.Visible)
                CourseIdentifier.Value = null;

            AssessmentPanel.Visible = ProductType.Value == "Online Assessment";
            if (!AssessmentPanel.Visible)
                AssessmentIdentifier.Value = null;
        }

        private string GetObjectType(string productType)
        {
            switch (productType)
            {
                case "Online Course":
                    return "Course";
                case "Online Assessment":
                    return "Assessment";
                default:
                    return null;
            }
        }

        private Guid? GetObjectIdentifier(string productType)
        {
            switch (productType)
            {
                case "Online Course":
                    return CourseIdentifier.Value;
                case "Online Assessment":
                    return AssessmentIdentifier.Value;
                default:
                    return null;
            }
        }
    }
}