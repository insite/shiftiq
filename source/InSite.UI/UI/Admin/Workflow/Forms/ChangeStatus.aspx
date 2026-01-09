<%@ Page Language="C#" CodeBehind="ChangeStatus.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.ChangeStatus" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Survey" />

    <section runat="server" id="NameSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-check-square me-1"></i>
            Change Status
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">
                        <div>
                            <uc:FormDetails runat="server" ID="SurveyDetail" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div>

                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
                            <insite:UpdatePanel runat="server" ID="UpdatePanel">
                                <ContentTemplate>
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Status
                                        </label>
                                        <div>
                                            <insite:ComboBox runat="server" ID="CurrentStatus" CssClass="w-50">
                                                <Items>
                                                    <insite:ComboBoxOption Value="Drafted" Text="Draft" />
                                                    <insite:ComboBoxOption Value="Opened" Text="Open (Published)" />
                                                    <insite:ComboBoxOption Value="Closed" Text="Closed" />
                                                    <insite:ComboBoxOption Value="Archived" Text="Archived" />
                                                </Items>
                                            </insite:ComboBox>
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Opened Date</label>
                                        <div>
                                            <asp:Literal runat="server" ID="OpenedDate" />
                                        </div>
                                        <div class="form-text">
                                            A date/time when a form was first opened for submissions.    
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Close Date</label>
                                        <div>
                                            <asp:Literal runat="server" ID="ClosedDate" />
                                            <insite:DateTimeOffsetSelector runat="server" ID="ClosedDatePicker" CssClass="w-50" />
                                        </div>
                                        <div class="form-text">
                                            The date/time when the form is intended to close, or when it is actually closed, for submissions.
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </insite:UpdatePanel>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Survey" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>
</asp:Content>
