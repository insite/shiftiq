<%@ Page Language="C#" CodeBehind="Configure.aspx.cs" Inherits="InSite.Admin.Achievements.Credentials.Forms.Configure" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/CredentialDetails.ascx" TagName="Details" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Records/Achievements/Controls/AchievementExpirationField.ascx" TagName="ExpirationField" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Credential" />

    <section runat="server" ID="CredentialSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-award me-1"></i>
            Configure Credential
        </h2>

        <div class="row">
            <div class="col-md-7">
                <uc:Details runat="server" ID="CredentialDetails" />    
            </div>

            <div class="col-md-5">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <uc:ExpirationField runat="server" ID="ExpirationField" ValidationGroup="Credential" />
                    
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Necessity
                            </label>
                            <div>
                                <insite:ItemNameComboBox runat="server" ID="Necessity" Settings-CollectionName="Achievements/Credentials/Necessity/Status" Width="300" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Priority
                            </label>
                            <div>
                                <insite:ItemNameComboBox runat="server" ID="Priority" Settings-CollectionName="Achievements/Credentials/Priority/Status" Width="300" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Credential" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
