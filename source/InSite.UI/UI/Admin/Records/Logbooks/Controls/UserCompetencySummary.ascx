<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserCompetencySummary.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.UserCompetencySummary" %>

<asp:Repeater runat="server" ID="FrameworkRepeater">
    <ItemTemplate>
        <div>
            <h3>
                <a href='<%# Eval("Identifier", "/ui/admin/standards/edit?id={0}") %>'>
                    <%# Eval("Name") %>
                </a>
            </h3>
        </div>
        
        <asp:Repeater runat="server" ID="AreaRepeater">
            <HeaderTemplate>

                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Competency</th>
                            <th style="text-align:center;"><insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/></th>
                            <th style="text-align:center;">Number of Log Entries</th>
                            <th style="text-align:center;">Required Skill Rating</th>
                        </tr>
                    </thead>
                    <tbody>

            </HeaderTemplate>
            <FooterTemplate>
                    
                    </tbody>
                </table>
                
            </FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td colspan="5">
                        <b><a href='<%# "/ui/admin/standards/edit?id=" + Eval("Identifier") %>'><%# Eval("Name") %> </a> </b>
                    </td>
                </tr>

                <asp:Repeater runat="server" ID="CompetencyRepeater">
                    <ItemTemplate>
                        <tr>
                            <td style="padding-left:45px;"><a href='<%# "/ui/admin/standards/edit?id=" + Eval("Identifier") %>'><%# Eval("Name") %> </a></td>
                            <td style="text-align:center;">
                                <%# Eval("Hours", "{0:n2}") %>
                            </td>
                            <td style="text-align:center;">
                                <%# Eval("JournalItems", "{0:n2}") %>
                            </td>
                            <td style="text-align:center;"><%# Eval("SkillRating") != null ? Eval("SkillRating", "{0:n0}") : "None" %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:Repeater>
        
    </ItemTemplate>
</asp:Repeater>
