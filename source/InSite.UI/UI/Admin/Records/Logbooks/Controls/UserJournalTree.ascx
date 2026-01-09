<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserJournalTree.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Controls.UserJournalTree" %>

<insite:PageHeadContent runat="server">
    <style>
        .journal-item {
            background-color: #f5f5f5;
        }
        .experience-item {
            background-color: #fafafa;
        }
    </style>
</insite:PageHeadContent>


<div id="journal-tree-accordion-panel">
    <div class="row">
        <div class="col-md-6">
            <button id="grade-expand-all" type="button" class="btn btn-default"><i class="fas fa-chevron-down me-1"></i>Expand All</button>
            <button id="grade-collapse-all" type="button" class="btn btn-default"><i class="fas fa-chevron-up me-1"></i>Collapse All</button>
        </div>
    </div>

    <div class="tree-view-container mt-3">
        <ul class='tree-view' data-init="code">

            <asp:Repeater runat="server" ID="JournalRepeater">
                <ItemTemplate>
                    <li class="outline-item" data-key='<%# Eval("JournalIdentifier") %>'>
                        <div class="journal-item">
                            <div>
                                <div class="node-title">
                                    <div>
                                        <a href='<%# Eval("JournalUrl") %>'>
                                            <%# Eval("JournalSetupName") %>
                                        </a>
                                    </div>
                                    <div class="form-text">
                                        Total Hours: <%# Eval("Hours") %>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <ul class='tree-view'>

                            <asp:Repeater runat="server" ID="ExperienceRepeater">
                                <ItemTemplate>
                                    <li class="outline-item" data-key='<%# Eval("ExperienceIdentifier") %>'>
                                        <div class="experience-item">
                                            <div>
                                                <div class="node-title">
                                                    <table style="width:100%;">
                                                        <tr>
                                                            <asp:Repeater runat="server" ID="LabelRepeater">
                                                                <ItemTemplate>
                                                                    <th style="<%# ParentColumnStyle %>">
                                                                        <%# Eval("LabelText") %>
                                                                    </th>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                            <th style='<%# Eval("ColumnStyle") + "text-align:right;" %>'>
                                                                Hours
                                                            </th>
                                                            <th style='<%# Eval("ColumnStyle") + "text-align:center;" %>'>
                                                                Skill Rating
                                                            </th>
                                                        </tr>
                                                        <tr>
                                                            <asp:Repeater runat="server" ID="ValueRepeater">
                                                                <ItemTemplate>
                                                                    <td style="<%# ParentColumnStyle %>">
                                                                        <insite:DynamicControl runat="server" ID="Value" />
                                                                    </td>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                            <td style='<%# Eval("ColumnStyle") + "text-align:right;" %>'>
                                                                <%# Eval("Hours") %>
                                                            </td>
                                                            <td style='<%# Eval("ColumnStyle") + "text-align:center;" %>'>
                                                                <%# Eval("SkillRating") %>
                                                            </td>
                                                        </tr>
                                                    </table>                        
                                                </div>
                                            </div>
                                        </div>

                                        <ul class='tree-view'>

                                            <asp:Repeater runat="server" ID="CompetencyRepeater">
                                                <ItemTemplate>
                                                    <li class="outline-item" data-key='<%# Eval("StandardIdentifier") %>'>
                                                        <div>
                                                            <div>
                                                                <div class="node-title">
                                                                    <table style="width:100%;">
                                                                        <tr>
                                                                            <td>
                                                                                <div><%# Eval("CompetencyName") %></div>
                                                                                <div class="form-text">
                                                                                    Satisfaction Level: <%# Eval("SatisfactionLevel") %>
                                                                                </div>
                                                                            </td>
                                                                            <td style='<%# ParentColumnStyle + "text-align:right;" %>'>
                                                                                <%# Eval("Hours") %>
                                                                            </td>
                                                                            <td style='<%# ParentColumnStyle + "text-align:center;" %>'>
                                                                                <%# Eval("SkillRating") %>
                                                                            </td>
                                                                        </tr>
                                                                    </table>                        
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>

                                        </ul>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>

                        </ul>
                    </li>
                </ItemTemplate>
            </asp:Repeater>

        </ul>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        function initUserJournalTreeView() {
            inSite.common.treeView.init($('#journal-tree-accordion-panel .tree-view-container > .tree-view'), {
                expand: $('#journal-tree-accordion-panel #grade-expand-all'),
                collapse: $('#journal-tree-accordion-panel #grade-collapse-all'),
                state: 'admin.records.logbooks.userJournals.state.<%= UserIdentifier %>',
                defaultLevel: 2
            });
        };
    </script>
</insite:PageFooterContent>
