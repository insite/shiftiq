<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Records.Reports.LearnerActivity.Controls.SearchResults" %>

<insite:PageHeadContent runat="server">
<style type="text/css">
    table { caption-side: top; }
    table.table-bordered caption { font-weight: bold; }
    table.table-bordered td.number { width: 60px; text-align: right; }
    table.table-striped>tbody>tr:nth-of-type(even) { background-color: #ffffff; }
    .radio-count-strategy label + input[type="radio"] { margin-left: 1.5rem; }
</style>
</insite:PageHeadContent>

<div class="d-inline-block">
    <asp:Literal ID="Instructions" runat="server" />

    <insite:Grid runat="server" ID="Grid" CssClass="table table-bordered table-striped table-responsive">
        <Columns>

            <asp:TemplateField HeaderText="Full Name">
                <ItemTemplate>
                    <a href="/ui/admin/contacts/people/edit?contact=<%# Eval("UserIdentifier") %>"><%# Eval("UserFullName") %></a>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:BoundField HeaderText="Email" DataField="UserEmail" />
            <asp:BoundField HeaderText="Gender" DataField="UserGender" />
            <asp:BoundField HeaderText="Phone" DataField="UserPhone" ItemStyle-Wrap="false" />
            
            <asp:TemplateField HeaderText="Birthdate" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# FormatDate(Eval("UserBirthdate")) %>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:BoundField HeaderText="Person Code" DataField="PersonCode" />

            <asp:TemplateField HeaderText="Program">
                <ItemTemplate>
                    <a href="/ui/admin/learning/programs/outline?id=<%# Eval("ProgramIdentifier") %>"><%# Eval("ProgramName") %></a>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Gradebook">
                <ItemTemplate>
                    <asp:Repeater runat="server" ID="GradebookRepeater">
                        <HeaderTemplate><ul></HeaderTemplate>
                        <FooterTemplate></ul></FooterTemplate>
                        <ItemTemplate>
                            <li>
                                <a href="/ui/admin/records/gradebooks/outline?id=<%# Eval("GradebookIdentifier") %>"><%# Eval("GradebookTitle") %></a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Achievement Granted" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Repeater runat="server" ID="AchievementRepeater">
                        <ItemTemplate>
                            <div>
                                <%# LocalizeDate(Eval("CredentialGranted")) %>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>

</div>
