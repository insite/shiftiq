<%@ Page Language="C#" CodeBehind="History.aspx.cs" Inherits="InSite.Admin.Assessments.Attachments.Forms.History" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ChangeRepeater" Src="../../../Reports/Changes/Controls/ChangeRepeater.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ViewerStatus" />

    <section>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-list-ul me-1"></i>
                    Changes
                </h4>

                <uc:ChangeRepeater runat="server" ID="ChangeRepeater" />

            </div>
        </div>

    </section>

    <div class="mt-3">
        <insite:CloseButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
