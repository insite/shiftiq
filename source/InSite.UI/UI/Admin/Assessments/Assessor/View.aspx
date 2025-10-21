<%@ Page CodeBehind="View.aspx.cs" Inherits="InSite.Admin.Assessments.Assessor.View" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assessments/Attempts/Controls/RubricGrid.ascx" TagName="RubricGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assessments/Attempts/Controls/ViewComposedEssay.ascx" TagName="ViewComposedEssay" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assessments/Attempts/Controls/ViewComposedVoice.ascx" TagName="ViewComposedVoice" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <insite:ResourceLink runat="server" Type="Css" Url="/UI/Admin/assessments/attempts/forms/view.css" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ViewerStatus" />
    <insite:ValidationSummary runat="server" />
    <insite:ValidationSummary runat="server" ValidationGroup="FileUpload" />

    <section>
        <h2 class="h4 mt-4 mb-3">Grading Items
        </h2>

        <insite:Button runat="server" ID="GradeButton"
            ButtonStyle="Default"
            Icon="fas fa-award"
            Text="Grade"
            CssClass="mb-3" />

        <insite:Button runat="server" ID="RegradeButton"
            ButtonStyle="Danger"
            Icon="fas fa-award"
            Text="Re-grade"
            CssClass="mb-3" />

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:RubricGrid runat="server" ID="Rubrics" />
            </div>
        </div>

    </section>

</asp:Content>
