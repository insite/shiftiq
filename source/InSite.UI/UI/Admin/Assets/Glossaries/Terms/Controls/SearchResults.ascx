<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Assets.Glossaries.Terms.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="TermIdentifier" CssClass="table table-striped table-searchresults" ShowHeader="false">
    <Columns>

        <asp:TemplateField ItemStyle-CssClass="cell-1">
            <ItemTemplate>
                <div class="header-1">
                    <%# Eval("Term.TermName") %>
                    <span runat="server" class="fas fa-globe" visible='<%# (bool)Eval("IsTranslated") %>'></span>
                </div>
                <div style="padding-right: 15px;">
                    <div runat="server" style="margin:10px 0;font-weight:bold;" visible='<%# Eval("ShowTitle") %>'><%# Eval("TermTitleText") %></div>
                    <%# Eval("TermDefinitionHtml") %>
                </div>
                <div class="form-text">
                    <%# GetTimestamp() %>
                </div>
                <div style="margin-bottom:40px;">
                    <%# GetTermStatusHtml() %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField ItemStyle-CssClass="cell-2">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" Type="Regular" CommandName="Edit" ToolTip='Revise Term'
                    NavigateUrl='<%# Eval("TermIdentifier", "/ui/admin/assets/glossaries/terms/revise?term={0}") %>' />
                <insite:IconLink runat="server" Name="thumbs-up" Type="Regular" ToolTip='Approve Term'
                    NavigateUrl='<%# Eval("TermIdentifier", "/ui/admin/assets/glossaries/terms/approve?term={0}") %>' />
                <insite:IconLink runat="server" Name="thumbs-down" Type="Regular" ToolTip='Reject Term'
                    NavigateUrl='<%# Eval("TermIdentifier", "/ui/admin/assets/glossaries/terms/reject?term={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<insite:PageFooterContent runat="server">
    <style type="text/css">
        a {
            text-decoration: none!important;
        }

        .table-searchresults {
        }

            .table-searchresults > tbody > tr > td {
            }

            .table-searchresults > tbody > tr > td.cell-1 {
                padding: 10px 20px 10px 20px;
                border: 0px;
            }

                .table-searchresults > tbody > tr > td.cell-1 .header-1 {
                    font-size: 24px;
                    background-color: #444;
                    padding: 5px 10px 5px 10px;
                    color: white;
                    margin: 10px 0 20px 0;
                    line-height: 1.1;
                }

                    .table-searchresults > tbody > tr > td.cell-1 .header-1 .fas {
                        float: right;
                        font-size: 0.8em;
                        padding: 3px 0;
                    }

            .table-searchresults > tbody > tr > td.cell-2 {
                width: 40px;
                white-space: nowrap;
                padding-top: 20px;
                vertical-align: top;
                border: 0px;
            }
    </style>
</insite:PageFooterContent>
