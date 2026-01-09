<%@ Page Language="C#" CodeBehind="EditMembership.aspx.cs" Inherits="InSite.Admin.Contacts.People.Forms.EditMembership" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/Groups/Controls/GroupInfo.ascx" TagName="GroupDetail" TagPrefix="uc" %>	
<%@ Register Src="../Controls/PersonInfo.ascx" TagName="PersonInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Membership" />

    <div class="row mb-3 mt-3">
        <div class="col-lg-3">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">
                    <h3><i class="far fa-users me-2"></i>Group</h3>
                    <uc:GroupDetail runat="server" ID="GroupDetail" />
                </div>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">
                    <h3><i class="far fa-user me-2"></i>Person</h3>
                    <uc:PersonInfo runat="server" ID="PersonInfo" />
                </div>
            </div>
        </div>
        <div class="col-lg-5">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <h3><i class="far fa-id-card-alt me-2"></i>Membership</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Effective
                        </label>
                        <insite:DateTimeOffsetSelector runat="server" ID="AssignedOn" Width="100%" />
                        <div class="form-text">
                            When did <asp:Literal runat="server" ID="PersonName1" /> start membership in this <asp:Literal runat="server" ID="GroupType" />?
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Function
                        </label>
                        <insite:ItemNameComboBox runat="server" ID="MembershipFunction">
                            <Settings UseCurrentOrganization="true" UseGlobalOrganizationIfEmpty="true" CollectionName="Contacts/Memberships/Membership/Type" />
                        </insite:ItemNameComboBox>
                        <div class="form-text">
                            What is <asp:Literal runat="server" ID="PersonName2" />'s purpose within the context of this group?
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">Membership Expiry</label>
                        <div>
                            <insite:DateTimeOffsetSelector runat="server" ID="MembershipExpiry" />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Role" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>