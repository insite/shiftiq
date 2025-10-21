<%@ Page Language="C#" CodeBehind="LockSurveyForm.aspx.cs" Inherits="InSite.Admin.Surveys.Forms.Forms.LockSurveyForm" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/SurveyFormInfo.ascx" TagName="SurveyDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="LockStatus" />

    <section runat="server" id="SurveyInformationSection" class="mb-3">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="row settings">
                    <div class="col-md-6">
                        <div class="settings">
                            <uc:SurveyDetails runat="server" ID="SurveyDetail" />
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </section>
    <insite:Alert runat="server" ID="ConfirmAlert" Indicator="Information">
        Are you sure you want to lock this survey?
    </insite:Alert>
    <div>
        <insite:Button runat="server" ID="LockButton" Text="Lock" Icon="fas fa-lock" ButtonStyle="Danger" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>
</asp:Content>
