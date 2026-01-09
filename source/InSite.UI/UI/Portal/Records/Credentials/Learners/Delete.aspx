<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Portal.Records.Credentials.Learners.Delete" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <div class="row">

        <div class="col-md-6">

            <h3 style="border-radius: 15px 15px; background: #F7F7FC; padding: 15px;">Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle me-1"></i><strong>Warning:</strong>
                This is a permanent change that cannot be undone.
            </div>

        </div>

        <div class="col-md-6">
            
            <h3 style="border-radius: 15px 15px; background: #F7F7FC; padding: 15px;">Summary</h3>

            <div class="mb-3">
                <dl>
                    <dt>Type</dt>
                    <dd><asp:Literal runat="server" ID="AchievementType" /></dd>
                    <dt>Granted</dt>
                    <dd><asp:Literal runat="server" ID="AchievementDate" Text="-" /></dd>
                    <dt>Name</dt>
                    <dd><asp:Literal runat="server" ID="LearnerName" /></dd>
                    <dt>Attachment</dt>
                    <dd><asp:Literal runat="server" ID="AchievementFile" Text="-" /></dd>
                </dl>
            </div>

        </div>

    </div>

    <div class="pt-3 alert alert-danger" role="alert">
        <i class="fas fa-stop-circle me-1"></i><strong>Please confirm:</strong>
        Are you sure you want to delete it? You can choose to delete only the attachment, or both the the attachment and the achievement.
    </div>

    <div class="text-danger">
        <insite:CheckBox runat="server" ID="DeleteAttachment" Text="Yes, I want to delete the attachment for my achievement" />
    </div>
    <div>
        <insite:CheckBox runat="server" ID="DeleteCredential" Text="Yes, I want to delete my achievement" />
    </div>

    <div class="pt-3">
        <insite:DeleteButton runat="server" ID="DeleteButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
