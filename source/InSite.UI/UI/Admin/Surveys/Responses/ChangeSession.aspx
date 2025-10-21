<%@ Page Language="C#" CodeBehind="ChangeSession.aspx.cs" Inherits="InSite.UI.Admin.Surveys.Responses.ChangeSession" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Surveys/Forms/Controls/SurveyFormInfo.ascx" TagName="SurveyDetails" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ChangeStatus" />

    <section runat="server" id="SurveyInformationSection" class="mb-3">

        <h2 class="h4 mb-3">
            <i class="far fa-poll-people me-1"></i>
            Change Survey Response
        </h2>

        <div class="row">

            <div class="col-md-4">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Response</h3>
                        <div class="form-group mb-3">

                            <label class="form-label">Status</label>
                            <div>
                                <asp:Literal runat="server" ID="ResponseStatus" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Started</label>
                            <div>
                                <asp:Literal runat="server" ID="ResponseStarted" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Completed</label>
                            <div>
                                <asp:Literal runat="server" ID="ResponseCompleted" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Group</label>
                            <div>
                                <insite:FindGroup runat="server" ID="GroupIdentifier" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Period</label>
                            <div>
                                <insite:FindPeriod runat="server" ID="PeriodIdentifier" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Respondent</h3>
                        <insite:Container runat="server" ID="RespondentFields" Visible="false">
                            <uc:PersonDetail runat="server" ID="PersonDetail" />
                        </insite:Container>
                    </div>
                </div>

            </div>

            <div class="col-md-5">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Survey</h3>
                        <uc:SurveyDetails runat="server" ID="SurveyDetail" />
                    </div>
                </div>

            </div>

        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>
</asp:Content>
