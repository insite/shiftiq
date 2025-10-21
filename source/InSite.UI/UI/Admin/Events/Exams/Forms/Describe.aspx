<%@ Page Language="C#" CodeBehind="Describe.aspx.cs" Inherits="InSite.UI.Admin.Events.Exams.Forms.Describe"  MasterPageFile="~/UI/Layout/Admin/AdminHome.master"%>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor.ascx" TagName="ContentEditor" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Class" />

    <section runat="server" ID="ContentSection" class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <h4 class="card-title mb-3">
                    <i class="far fa-edit me-1"></i>
                    Describe Exam
                </h4>

                <uc:ContentEditor runat="server" ID="ContentEditor" ValidationGroup="Class" />
            </div>
        </div>
    </section>

    <section>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Class" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

</asp:Content>

