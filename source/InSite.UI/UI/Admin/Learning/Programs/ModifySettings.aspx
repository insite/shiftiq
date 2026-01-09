<%@ Page Language="C#" CodeBehind="ModifySettings.aspx.cs" Inherits="InSite.Admin.Records.Programs.ModifySettings" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/TaskGridEdit.ascx" TagName="TaskGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Settings" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <uc:TaskGrid runat="server" ID="TaskGrid" />

            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Settings" />
        <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
    </div>

</asp:Content>