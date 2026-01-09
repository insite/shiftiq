<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.Cmds.Actions.Profile.Employee.Profile.Search" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/EmploymentGrid.ascx" TagName="EmploymentGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

    <insite:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <insite:Nav runat="server" ID="NavPanel">

                <insite:NavItem runat="server" ID="EmploymentSection" Title="Primary Profiles" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
                    <section>
                        <h2 class="h4 mt-4 mb-3">Primary Profiles
                        </h2>

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <uc:EmploymentGrid ID="EmploymentGrid" runat="server" />
                            </div>
                        </div>
                    </section>
                </insite:NavItem>
                <insite:NavItem runat="server" ID="CompanyProfilesSection" Title="Organization Profiles" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
                    <section>
                        <h2 class="h4 mt-4 mb-3">Organization Profiles
                        </h2>

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <h3 runat="server" id="ActiveOrganizationName" style="margin-top: 10px"></h3>
                                <p id="ActiveOrganizationHelp" runat="server"></p>

                                <asp:PlaceHolder ID="GridPanel" runat="server">

                                    <insite:Grid runat="server" ID="Grid" DataKeyNames="DepartmentIdentifier,ProfileStandardIdentifier">
                                        <Columns>

                                            <asp:TemplateField ItemStyle-Width="16">
                                                <ItemTemplate>
                                                    <insite:Icon runat="server" ID="Flag" Type="Regular" />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Type">
                                                <ItemTemplate>
                                                    <%# (Boolean)Eval("IsPrimary") ? "Primary" : "Secondary" %>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField HeaderText="Department" DataField="DepartmentName" />

                                            <asp:TemplateField HeaderText="Profile" ItemStyle-Wrap="false">
                                                <ItemTemplate>
                                                    <a runat="server" href='<%# "/ui/cmds/portal/validations/competencies/search?profile=" + Eval("ProfileStandardIdentifier") + "&userID=" + Eval("UserIdentifier") + "&department=" + Eval("DepartmentIdentifier") %>'>
                                                        <%# Eval("ProfileNumber") %>: <%# Eval("ProfileTitle") %>
                                                    </a>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Compliance Required" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# (Boolean)Eval("IsComplianceRequired")  ? "Yes": "" %>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Critical" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# GetCriticalValue(Container.DataItem) %>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Non-Critical" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# GetNonCriticalValue(Container.DataItem) %>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Total" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# GetTotalValue(Container.DataItem) %>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <insite:TemplateField ItemStyle-Width="15px" FieldName="DeleteColumn">
                                                <ItemTemplate>
                                                    <insite:IconButton runat="server" ID="DeleteButton" Name="trash-alt" ToolTip="Remove" CommandName="Delete" />
                                                </ItemTemplate>
                                            </insite:TemplateField>
                                        </Columns>
                                    </insite:Grid>

                                    <div class="mt-3">
                                        <cmds:EmployeeProfileFilterSelector ID="FilterSelector" runat="server" AllowBlank="false" />
                                    </div>

                                    <div class="mt-3">
                                        <asp:RadioButtonList ID="DisplaySelector" runat="server" CssClass="display_selector" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                            <asp:ListItem Value="DisplayAsScore" Text="Display as score (x/y)" />
                                            <asp:ListItem Value="DisplayAsPercentage" Text="Display as percentage" />
                                        </asp:RadioButtonList>
                                    </div>

                                </asp:PlaceHolder>

                                <asp:Panel runat="server" ID="OtherOrganizationPanel">

                                    <h3 class="mt-5">Other Organizations</h3>
                                    <p id="OtherOrganizationHelp" runat="server"></p>

                                    <insite:Button ID="OtherProfilesButton" ButtonStyle="Default" runat="server" Icon="fas fa-search" Text="View other profiles." Visible="false" />

                                    <insite:Grid runat="server" ID="OtherGrid">
                                        <Columns>
                                            <asp:BoundField HeaderText="Organization" DataField="CompanyName" />
                                            <asp:BoundField HeaderText="Department" DataField="DepartmentName" />
                                            <asp:TemplateField HeaderText="Profile">
                                                <ItemTemplate>
                                                    <%# Eval("ProfileNumber") %>: <%# Eval("ProfileTitle") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </insite:Grid>

                                </asp:Panel>

                            </div>
                        </div>
                    </section>
                </insite:NavItem>
                <insite:NavItem runat="server" ID="AddSection" Title="Assign New Profile" Icon="far fa-file" IconPosition="BeforeText">
                    <section>
                        <h2 class="h4 mt-4 mb-3">Assign New Profile
                        </h2>

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <div style="float: right;">
                                    <insite:Button runat="server" ID="PersonEditorLink" ButtonStyle="Default" Icon="fas fa-pencil" Text="Edit User" />
                                </div>

                                <p style="margin-bottom: 10px;">
                                    <asp:Label runat="server" ID="AcquireInstruction">
                            Select the profile you want to acquire from the dropdown list and then click the Assign Profile button.
                                    </asp:Label>
                                </p>

                                <div class="row">
                                    <div class="col-lg-4">
                                        
                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Department
                                        <insite:RequiredValidator runat="server" ControlToValidate="NewDepartment" ValidationGroup="NewProfile" />
                                                </label>
                                                <div>
                                                    <cmds:FindDepartment ID="NewDepartment" runat="server" />
                                                </div>
                                                <div class="form-text"></div>
                                            </div>
                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Profile
                                        <insite:RequiredValidator runat="server" ControlToValidate="NewProfile" ValidationGroup="NewProfile" />
                                                </label>
                                                <div>
                                                    <cmds:FindProfile ID="NewProfile" runat="server" />
                                                </div>
                                                <div class="form-text"></div>
                                            </div>
                                            <div class="form-group mb-3">
                                                <div>
                                                    <cmds:CmdsButton ID="AddPosition" runat="server" Text="<i class='fas fa-plus-circle me-1'></i> Assign Profile" ValidationGroup="NewProfile" />
                                                </div>
                                                <div class="form-text"></div>
                                            </div>
                                        
                                    </div>
                                    <div class="col-lg-4">
                                        
                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Type
                                                    
                                        <insite:RequiredValidator runat="server" ControlToValidate="NewProfileType" ValidationGroup="NewProfile" />
                                                </label>
                                                <div>
                                                    <insite:ComboBox ID="NewProfileType" runat="server">
                                                        <Items>
                                                            <insite:ComboBoxOption Text="" Value="" />
                                                            <insite:ComboBoxOption Text="Primary" Value="Primary" />
                                                            <insite:ComboBoxOption Text="Secondary" Value="Secondary" />
                                                        </Items>
                                                    </insite:ComboBox>
                                                </div>
                                                <div class="form-text"></div>
                                            </div>
                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Status
                                                </label>
                                                <div>
                                                    <cmds:EmployeeProfileStatusSelector ID="NewProfileStatus" runat="server" />
                                                </div>
                                                <div class="form-text"></div>
                                            </div>
                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Compliance
                                                </label>
                                                <div>
                                                    <asp:CheckBox ID="IsComplianceRequired" runat="server" Text="Yes, this profile requires compliance" />
                                                </div>
                                                <div class="form-text"></div>
                                            </div>
                                        
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
                </insite:NavItem>

            </insite:Nav>
        </ContentTemplate>
    </insite:UpdatePanel>

</asp:Content>