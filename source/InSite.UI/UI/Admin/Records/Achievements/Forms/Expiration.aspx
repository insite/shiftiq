<%@ Page Language="C#" CodeBehind="Expiration.aspx.cs" Inherits="InSite.Admin.Achievements.Achievements.Forms.Expiration" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/AchievementDetailsV2.ascx" TagName="Details" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Records/Achievements/Controls/AchievementExpirationField.ascx" TagName="ExpirationField" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Achievement" />

    <section runat="server" id="NameSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-trophy me-1"></i>
            Achievement Expiration
        </h2>

        <div class="row">
            <div class="col-md-5">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <uc:Details runat="server" ID="AchievementDetails" />

                    </div>
                </div>
            </div>
            <div class="col-md-7">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <uc:ExpirationField runat="server" ID="ExpirationField" ValidationGroup="Achievement" />

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Bulk Update Credentials
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="BulkUpdateCredentials" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                    <asp:ListItem Value="Yes" Text="Yes" />
                                    <asp:ListItem Value="No" Text="No" Selected="true" />
                                </asp:RadioButtonList>
                            </div>
                            <div class="form-text">Do you want to bulk-update the expiration settings for all existing credentials to match the settings above?</div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Achievement" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
