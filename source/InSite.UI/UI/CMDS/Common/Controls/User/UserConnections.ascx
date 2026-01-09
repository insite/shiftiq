<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserConnections.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.People.Controls.UserConnections" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>

        <div class="row">
            <div class="col-lg-4 mb-3 mb-lg-0">
                <div class="card">
                    <div class="card-body">

                        <h3 class="mb-0">Assign reporting line</h3>
                        <div class="mt-0 mb-3 form-text">
                            Select a person from the list, indicate their relationship to 
                            <asp:Literal runat="server" ID="ToUserName" />, 
                            then click Assign
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Person
                                <insite:RequiredValidator runat="server" ControlToValidate="PersonIdentifier" FieldName="Person" ValidationGroup="PersonRelationship" />
                            </label>
                            <cmds:FindPerson ID="PersonIdentifier" runat="server" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Relationship
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="RelationshipType">
                                    <asp:ListItem Value="Leader" />
                                    <asp:ListItem Value="Manager" />
                                    <asp:ListItem Value="Supervisor" />
                                    <asp:ListItem Value="Validator" />
                                </asp:RadioButtonList>
                            </div>
                        </div>

                        <div class="mt-3">
                            <insite:Button ID="AddRelationship" runat="server" Icon="fas fa-plus-circle" Text="Assign" ButtonStyle="Primary" ValidationGroup="PersonRelationship" />
                        </div>

                    </div>
                </div>
            </div>
            <div runat="server" id="GridColumn" class="col-lg-8">
                <div>
                    
                        <insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier" EnablePaging="false">
                            <Columns>

                                <asp:TemplateField HeaderText="Person">
                                    <ItemTemplate>
                                        <a href='<%# string.Format("/ui/cmds/admin/users/edit?userID={0}", Eval("UserIdentifier")) %>'>
                                            <%# Eval("FullName") %>
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Reporting Lines" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <div class="d-inline-block me-2">
                                            <asp:CheckBox ID="IsLeader" runat="server" Text="Leader" AutoPostBack="true" OnCheckedChanged="OnRelationshipChecked" />
                                        </div>
                                        <div class="d-inline-block me-2">
                                            <asp:CheckBox ID="IsManager" runat="server" Text="Manager" AutoPostBack="true" OnCheckedChanged="OnRelationshipChecked" />
                                        </div>
                                        <div class="d-inline-block me-2">
                                            <asp:CheckBox ID="IsSupervisor" runat="server" Text="Supervisor" AutoPostBack="true" OnCheckedChanged="OnRelationshipChecked" />
                                        </div>
                                        <div class="d-inline-block me-2">
                                            <asp:CheckBox ID="IsValidator" runat="server" Text="Validator" AutoPostBack="true" OnCheckedChanged="OnRelationshipChecked" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </insite:Grid>
                    
                </div>

                <div runat="server" id="ReportingLineHelp" class="alert alert-info mt-4" role="alert"></div>

            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>