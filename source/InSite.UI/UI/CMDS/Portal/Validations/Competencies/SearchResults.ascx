<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Cmds.Controls.Talents.EmployeeCompetencies.EmployeeCompetencySearchResults" %>

<style type="text/css">
    div.validation-history { padding-top: 10px; font-size: 13px; }
    table.item-comments tr td { vertical-align: top; padding: 5px; font-size: 13px; }
</style>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Number" ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:HyperLink runat="server" ID="NumberLink" Text='<%# Eval("Number") %>' />
            </ItemTemplate>
        </asp:TemplateField>
                
        <asp:TemplateField HeaderText="Summary">
            <ItemTemplate>

                <%# Eval("Summary") %>
                <span runat="server" visible='<%# Eval("EmployeeFullName") != DBNull.Value %>' class="form-text">
                    <br />
                    Worker: <%# Eval("EmployeeFullName") %>
                </span>

                <div runat="server" id="ValidationHistory" class="validation-history">
                    <table class="item-comments">
                        <asp:Repeater runat="server" ID="ValidatorCommentGrid">
                            <HeaderTemplate>
                                <strong>Validation History:</strong>
                            </HeaderTemplate>
                            <ItemTemplate>
                            <tr>
                                <td class="name" style="white-space:nowrap;"><%# Eval("ChangePosted", "{0:MMM d, yyyy}") %>:</td>
                                <td>
                                    <%# Eval("ChangeStatus") %> (<%# Eval("ValidatorName") %>)
                                    <span class="form-text"><%# Eval("ChangeComment") == DBNull.Value ? "" : "<br />" + (String)Eval("ChangeComment") %></span>
                                </td>
                            </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </div>

            </ItemTemplate>
        </asp:TemplateField>
                
        <asp:TemplateField HeaderText="Category">
            <ItemTemplate>
                <%# GetCategories((Guid)Eval("CompetencyStandardIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>
                
        <asp:TemplateField HeaderText="Priority" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("PriorityName") == DBNull.Value ? "Non-Critical": Eval("PriorityName") %>
            </ItemTemplate>
        </asp:TemplateField>
                
        <asp:BoundField HeaderText="Self-Assessment" DataField="SelfAssessmentStatus" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
        <asp:BoundField HeaderText="Validation" DataField="ValidationStatus" ItemStyle-Wrap="false" />
                
        <asp:TemplateField ItemStyle-Width="60">
            <ItemStyle Wrap="false" />
            <ItemTemplate>
                <insite:Icon runat="server" ID="IsValidated" Type="Regular" Name="flag-checkered" Color="Success" />
                <insite:Icon runat="server" ID="IsNotValidated" Type="Solid" Name="flag" Color="Danger" />
                <insite:Icon runat="server" ID="IsTimeSensitive" Type="Regular" Name="history" />
                <insite:Icon runat="server" ID="IsCritical" Type="Regular" Name="exclamation" ToolTip="Critical" />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
