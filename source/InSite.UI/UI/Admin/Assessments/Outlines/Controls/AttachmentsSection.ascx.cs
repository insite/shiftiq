using System;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Constant;

namespace InSite.Admin.Assessments.Outlines.Controls
{
    public partial class AttachmentsSection : BaseUserControl
    {
        #region Data binding

        public void LoadData(BankState bank, bool canWrite, out bool isSelected)
        {
            AddAttachmentButton.NavigateUrl = $"/ui/admin/assessments/attachments/add?bank={bank.Identifier}";
            AddAttachmentButton.Visible = canWrite;
            ScanImagesLink.NavigateUrl = $"/ui/admin/assessments/attachments/scan-images?bank={bank.Identifier}";

            isSelected = false;

            if (!IsPostBack)
            {
                if (Guid.TryParse(Request.QueryString["attachment"], out var attachmentId))
                {
                    var attachment = bank.FindAttachment(attachmentId);
                    if (attachment != null)
                    {
                        isSelected = true;

                        if (attachment.Type == AttachmentType.Image)
                            AttachmentsNav.SelectTab(TabType.Image);
                        else if (attachment.Type == AttachmentType.Document)
                            AttachmentsNav.SelectTab(TabType.Document);
                        else
                            AttachmentsNav.SelectTab(TabType.Other);
                    }
                }

                if (!isSelected && Request.QueryString["panel"] == "attachments")
                {
                    isSelected = true;

                    var tab = Request.QueryString["tab"];
                    if (!string.IsNullOrEmpty(tab))
                    {
                        if (tab == "images")
                            AttachmentsNav.SelectTab(TabType.Image);
                        else if (tab == "documents")
                            AttachmentsNav.SelectTab(TabType.Document);
                        else if (tab == "other")
                            AttachmentsNav.SelectTab(TabType.Other);
                    }
                }
            }

            var hasData = bank.Attachments.Count > 0;
            var returnUrl = new ReturnUrl("bank&attachment&panel=attachments");

            FilterAttachmentsTextBox.Visible = hasData;

            AttachmentsNav.Visible = hasData;
            AttachmentsNav.LoadData(bank.Identifier, bank.Attachments, returnUrl, canWrite);
        }

        #endregion
    }
}