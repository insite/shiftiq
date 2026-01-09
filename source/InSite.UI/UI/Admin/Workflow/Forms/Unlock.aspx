<%@ Page Language="C#" CodeBehind="Unlock.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Unlock" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="LockStatus" />

    <section runat="server" id="SurveyInformationSection" class="mb-3">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h2 class="h4 mb-3">
                    <i class="far fa-check-square me-1"></i>
                    Unlock Form
                </h2>

                <div class="row">
                    <div class="col-md-6">
                        <div>
                            <uc:FormDetails runat="server" ID="SurveyDetail" />
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <insite:Alert runat="server" ID="ConfirmAlert" Indicator="Information">
        Are you sure you want to unlock this form?
    </insite:Alert>

    <div>
        <insite:Button runat="server" ID="UnlockButton" Text="Unlock" Icon="fas fa-lock-open" ButtonStyle="Info" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
