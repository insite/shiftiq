<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TermGrid.ascx.cs" Inherits="InSite.Admin.Assets.Glossaries.Terms.Controls.TermGrid" %>

<div class="row mb-3">
    <div runat="server" id="AddPanel" class="col-lg-7">
        <div class="d-inline-block w-50">
            <insite:FindGlossaryTerm runat="server" ID="AddTermIdentifier" EmptyMessage="Glossary Term" />
        </div>
        <insite:RequiredValidator runat="server" FieldName="Glossary Term" Display="Static"
            ControlToValidate="AddTermIdentifier" ValidationGroup="AddTerm" />
        <insite:Button runat="server" ID="AddButton" Icon="fas fa-link" Text="Link Term" ButtonStyle="Default"
            CausesValidation="true" ValidationGroup="AddTerm" />
    </div>

    <div class="col-lg-5 float-end text-end">
        <insite:Container runat="server" ID="FilterContainer" Visible="false">
            <insite:TextBox runat="server" ID="FilterTextBox" EmptyMessage="Filter" CssClass="d-inline w-75" />
            <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" />
        </insite:Container>
    </div>
</div>

<div class="row">
    <div class="col-xs-12">

        <insite:Grid runat="server" ID="Grid" DataKeyNames="RelationshipIdentifier,TermIdentifier">
            <Columns>

                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate>
                        <a runat="server" visible="<%# AllowEdit %>" href='<%# Eval("TermIdentifier", "/ui/admin/assets/glossaries/terms/revise?term={0}") %>' title="Revise Term"><%# Eval("TermName") %></a>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Definition">
                    <ItemTemplate>
                        <div runat="server" style="margin:0 0 10px 0;font-weight:bold;" visible='<%# Eval("ShowTitle") %>'><%# Eval("TermTitleText") %></div>
                        <%# Eval("TermDefinitionHtml") %>
                        <div class="form-text" style="margin-top:8px;">
                            <%# GetTimestamp() %>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Refs">
                    <ItemTemplate>
                        <%# GetReferencesHtml() %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <ItemTemplate>
                        <insite:IconButton runat="server" Visible="<%# AllowEdit %>" CommandName="Remove" ConfirmText='<%# Eval("TermName", "Are you sure you want to unlink &apos;{0}&apos; term?") %>'
                            Name="unlink" ToolTip="Unlink Term" />
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>

    </div>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .table-refs {
            background-color: transparent !important;
        }

            .table-refs td {
                border: none  !important;
            }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            Sys.Application.add_load(function () {
                $('#<%= FilterTextBox.ClientID %>')
                    .off('keydown', onKeyDown)
                    .on('keydown', onKeyDown);
            });

            function onKeyDown(e) {
                if (e.which === 13) {
                    e.preventDefault();
                    $('#<%= FilterButton.ClientID %>')[0].click();
                }
            }
        })();
    </script>
</insite:PageFooterContent>
