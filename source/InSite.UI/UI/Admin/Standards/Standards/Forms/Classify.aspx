<%@ Page Language="C#" CodeBehind="Classify.aspx.cs" Inherits="InSite.Admin.Standards.Standards.Forms.Classify" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ClassifyStatus" />

    <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

        <insite:NavItem runat="server" ID="SearchSection" Title="Search" Icon="far fa-search" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Search</h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Standard
                            </label>
                            <div>
                                <insite:FindStandard runat="server" ID="SelectedAssetID" />
                            </div>
                            <div class="form-text">
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Show
                            </label>
                            <div>
                                <asp:CheckBox runat="server" ID="ShowType" Text="Type" Checked="true" />
                                <asp:CheckBox runat="server" ID="ShowCode" Text="Code" Checked="true" />
                                <asp:CheckBox runat="server" ID="ShowTitle" Text="Title" Checked="true" />
                                <div class="d-inline-block">
                                    <asp:RadioButtonList runat="server" ID="ShowTitleLanguages" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
                                </div>
                            </div>
                        </div>

                        <insite:FilterButton runat="server" ID="SearchButton" />

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="OutlineSection" Title="Outline" Icon="far fa-sitemap" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Outline</h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="row pb-3">
                            <div class="col-md-6">
                                <button id="expand-all" type="button" class="btn btn-sm btn-default"><i class="fas fa-chevron-down me-1"></i>Expand All</button>
                                <select id="expand-level" data-width="100px" class="insite-combobox"></select>
                                <button id="collapse-all" type="button" class="btn btn-sm btn-default"><i class="fas fa-chevron-up me-1"></i>Collapse All</button>
                            </div>

                            <div class="col-md-6 text-end">
                                <insite:Button runat="server" ID="RecodeButton" ButtonStyle="Success" ToolTip="Automatically Code Asset Structure" Text="Recode" Icon="fas fa-sort-numeric-down" />
                            </div>
                        </div>

                        <insite:Container runat="server" id="TreeViewPanel">
                            <div class="tree-view-container">
                                <asp:Repeater runat="server" ID="TreeViewRepeater">
                                    <ItemTemplate>
                                        <%# Eval("HtmlPrefix") %>
                                        <asp:Literal runat="server" ID="AssetID" Visible="false" Text='<%# Eval("NodeID") %>' />
                                        <div>
                                            <div>
                                                <div class="node-title">
                                                    <span class='text'>
                                                        <%# Eval("StandardTypeIcon") == null ? string.Empty : Eval("StandardTypeIcon", "<i class='align-middle {0}'></i>") %>
                                                        <span><%# Eval("CodePath") %>. <%# Eval("TitleDefault") %>&nbsp;</span>
                                                    </span>
                                                </div>
                                                <div class="node-inputs node-inputs-sm">
                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <insite:TextBox runat="server" ID="TitleInput" CssClass="form-control-sm" />
                                                        </div>
                                                        <div class="col-md-3">
                                                            <insite:StandardTypeComboBox runat="server" ID="TypeSelector" AllowBlank="false" ButtonSize="Small" />
                                                        </div>
                                                        <div class="col-md-3">
                                                            <insite:TextBox runat="server" ID="CodeInput" CssClass="form-control-sm" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <%# Eval("HtmlPostfix") %>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </insite:Container>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <div runat="server" id="SavePanel">
        <insite:SaveButton runat="server" ID="SaveButton" />
        <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
    </div>

    <insite:PageHeadContent runat="server">

        <style type="text/css">

            .tree-view > li > div > div .node-inputs textarea {
                height: 34px;
            }

            .bootstrap-select > div.dropdown-menu > ul > li > a > span.text {
                color: #333;
            }

            .bootstrap-select > div.dropdown-menu > ul > li.selected {
                background: #eee;
            }

            ul.tree-view > li > div > div .node-title > span.text {
                display: block;
                text-overflow: ellipsis;
                overflow: hidden;
                white-space: nowrap;
            }

            .node-inputs {
                position: static !important;
                width: 100%;
                padding-right: 0 !important;
                padding-left: 0 !important;
            }

                .node-inputs > .row {
                    --ar-gutter-x: 0.75rem;
                }

            @media (min-width: 1600px) {
                .node-inputs {
                    position: absolute !important;
                    padding-right: var(--in-tree-view-item-toggle-padding-x) !important;
                    width: 775px;
                }
                ul.tree-view > li > div > div .node-title > span.text {
                    max-width: calc(100% - 760px);
                }
            }
        </style>

    </insite:PageHeadContent>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            inSite.common.comboBox.init({
                id: '#expand-level',
                width: '115px',
                style: 'btn-combobox btn-sm'
            });

            inSite.common.treeView.init('<%= TreeViewRepeater.ClientID %>', {
                expand: '#expand-all',
                collapse: '#collapse-all',
                level: '#expand-level',
                defaultLevel: 2
            });
        </script>
    </insite:PageFooterContent>
</asp:Content>
