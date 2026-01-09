<%@ Page Language="C#" CodeBehind="ChangeGradebook.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Forms.ChangeGradebook" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Change Gradebook</h3>

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
                        <insite:UpdatePanel runat="server" ID="UpdatePanel">
                            <ContentTemplate>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Mode
                                    </label>
                                    <div>
                                        <insite:RadioButton runat="server" ID="CreateGradebookMode" Text="Create Gradebook" GroupName="Mode" />
                                    </div>
                                    <div>
                                        <insite:RadioButton runat="server" ID="SelectGradebookMode" Text="Select Gradebook" GroupName="Mode" />
                                    </div>
                                    <insite:Alert runat="server" ID="Warning" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Gradebook
                                    </label>
                                    <div>
                                        <insite:GradebookComboBox runat="server" ID="Gradebook" DropDown-Size="10" />
                                    </div>
                                    <div class="form-text">
                                        The gradebook where results are stored for answers to the questions in this form.
                                    </div>
                                </div>
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:FormDetails ID="FormDetails" runat="server" />
                    </div>
                </div>
            </div>

        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>