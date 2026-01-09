using System;

using InSite.Application.Files.Read;
using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;
using Shift.Common;

namespace InSite.UI.Admin.Sales.Packages.Controls
{
    public partial class Details : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProductQuantity.MaxValue = int.MaxValue;
        }

        public void SetInputValues(TProduct entity)
        {
            ProductName.Text = entity.ProductName;
            ProductDescription.Text = entity.ProductDescription;
            ProductQuantity.ValueAsInt = entity.ProductQuantity;
            ProductPrice.ValueAsDecimal = entity.ProductPrice;
            IsTaxable.Checked = entity.IsTaxable;

            if (entity.ProductImageUrl.HasValue())
                PackageImage.Text = $"<img src='{entity.ProductImageUrl}' alt='Package Image' />";
        }

        public void GetInputValues(TProduct entity)
        {
            entity.ProductName = ProductName.Text;
            entity.ProductDescription = ProductDescription.Text;
            entity.ProductQuantity = ProductQuantity.ValueAsInt ?? 0;
            entity.ProductPrice = ProductPrice.ValueAsDecimal;
            entity.IsTaxable = IsTaxable.Checked;

            if (PackageImageUpload.HasFile)
            {
                var model = PackageImageUpload.SaveFile(entity.ProductIdentifier, FileObjectType.Product);
                var files = ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName, false);
                var imageUrl = PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, CurrentSessionState.Identity.Organization.OrganizationCode, files);

                if (!string.IsNullOrWhiteSpace(entity.ProductImageUrl))
                    FileUploadV2.DeleteFileByUrl(entity.ProductImageUrl);

                entity.ProductImageUrl = imageUrl;
            }
        }
    }
}