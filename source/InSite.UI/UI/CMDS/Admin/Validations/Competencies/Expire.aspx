<%@ Page Language="C#" CodeBehind="Expire.aspx.cs" Inherits="InSite.Cmds.Actions.BulkTool.Expiry.Competency" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Competency
                        <insite:RequiredValidator runat="server" ControlToValidate="CompetencySelector" ValidationGroup="Bulk" />
                    </label>
                    <div>
                        <cmds:FindCompetency ID="CompetencySelector" runat="server" CssClass="w-50" />
                    </div>
                    <div class="form-text">
                        Which competency would you like to expire?
                    </div>
                </div>
            
                <div class="form-group mb-3">
                    <label class="form-label">
                        Scope
                    </label>
                    <div>
                        <insite:ComboBox ID="EntityType" runat="server" CssClass="w-50">
                            <Items>
                                <insite:ComboBoxOption Value="Person" Text="Person" />
                                <insite:ComboBoxOption Value="Department" Text="Department" />
                                <insite:ComboBoxOption Value="Organization" Text="Organization" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                    <div class="form-text">
                        Do you want to force the expiry of this competency for a specific person or department or organization?
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        <asp:Literal runat="server" ID="Subject" />
                        <insite:RequiredValidator ID="PersonRequired" runat="server" ControlToValidate="Person" ValidationGroup="Bulk" Display="Dynamic" />
                        <insite:RequiredValidator ID="DepartmentRequired" runat="server" ControlToValidate="Department" ValidationGroup="Bulk" Display="Dynamic" />
                        <insite:RequiredValidator ID="CompanyRequired" runat="server" ControlToValidate="Company" ValidationGroup="Bulk" Display="Dynamic" />
                    </label>
                    <div>
                        <cmds:FindPerson ID="Person" runat="server" CssClass="w-50" />
                        <cmds:FindDepartment ID="Department" runat="server" CssClass="w-50" />
                        <cmds:FindCompany ID="Company" runat="server" CssClass="w-50" />
                    </div>
                    <div class="form-text">
                        Specify the one for which you want to expire this competency.
                    </div>
                </div>

            </div>
        </div>
    </section>

    <insite:SaveButton ID="ExpireButton" runat="server" Text="Expire" ValidationGroup="Bulk" ConfirmText="Are you sure you want to expire competency by selected criterias" />

</asp:Content>
