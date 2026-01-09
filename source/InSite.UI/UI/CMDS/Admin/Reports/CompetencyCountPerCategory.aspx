<%@ Page Language="C#" CodeBehind="CompetencyCountPerCategory.aspx.cs" Inherits="InSite.Cmds.Actions.Reports.CompetencyCountPerCategory" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <section runat="server" ID="PreviewSection" class="mb-3">

        <h2 class="h4 mb-3">
            <i class="far fa-chart-bar me-1"></i>
            Report
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div runat="server" ID="DownloadCommandsPanel" class="mb-3">
                    <insite:Button runat="server" ID="DownloadXlsx" ButtonStyle="Primary" Text="Download XLSX" Icon="far fa-download" />
                </div>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                <insite:UpdatePanel runat="server" ID="UpdatePanel">
                    <ContentTemplate>
                        <div class="row search">
                            <div class="col-lg-12">
                                <div class="input-group">
                                    <button class="btn btn-outline-primary dropdown-toggle" type="button" id="filters-menu-button" data-bs-toggle="dropdown" aria-expanded="false">
                                        Filters
                                    </button>
                                    <ul class="dropdown-menu" aria-labelledby="filters-menu-button">
                                        <li><a class="dropdown-item" href="#" onclick="competencyCounts.showAdvancedSearch(); return false;">Advanced Search</a></li>
                                    </ul>
                                    <asp:TextBox runat="server" ID="SearchText" CssClass="form-control" autocomplete="off" onkeypress="return competencyCounts.searchKeyPress();" />
                                    <asp:LinkButton runat="server" ID="SearchButton" CssClass="btn btn-outline-primary" Text='<i class="fa fa-search me-2"></i>Search' />
                                </div>
                            </div>
                        </div>

                        <div class="my-3">
                            <asp:PlaceHolder runat="server" ID="ReportOutput"></asp:PlaceHolder>
                        </div>

                        <insite:Modal runat="server" ID="AdvancedSearchWindow" Title="Advanced Search">
                            <ContentTemplate>
                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AdvancedSearchUpdatePanel" />

                                <insite:UpdatePanel runat="server" ID="AdvancedSearchUpdatePanel">
                                    <ContentTemplate>
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Keyword
                                            </label>
                                            <insite:TextBox runat="server" ID="KeywordText" />
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Category
                                            </label>
                                            <insite:ComboBox runat="server" ID="CategorySelector" EmptyMessage="All Categories" />
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Profile
                                            </label>
                                             <cmds:FindProfile ID="ProfileSelector" runat="server" EmptyMessage="All Profiles" />
                                        </div>
                                    </ContentTemplate>
                                </insite:UpdatePanel>

                                <div class="mt-3">
                                    <insite:FilterButton runat="server" ID="SearchAdvancedButton" OnClientClick="competencyCounts.closeAdvancedSearch();" />
                                    <insite:CancelButton runat="server" OnClientClick="competencyCounts.closeAdvancedSearch(); return false;" />
                                </div>
                            </ContentTemplate>
                        </insite:Modal>
                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>

    </section>

    <insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (() => {
            Sys.Application.add_load(() => {
                $('table.table-report td h1').addClass('h4');
            });            
        })();

        var competencyCounts = {
            searchKeyPress: function (e) {
                if (window.event) {
                    e = window.event;
                }

                if (e.keyCode == 13) {
                    __doPostBack("<%= SearchButton.UniqueID %>", "");
                    return false;
                }

                return true;
            },

            showAdvancedSearch: function () {
                modalManager.show("<%= AdvancedSearchWindow.ClientID %>");

                document.getElementById('<%= AdvancedSearchUpdatePanel.ClientID %>').ajaxRequest('refresh');
            },

            closeAdvancedSearch: function () {
                modalManager.close("<%= AdvancedSearchWindow.ClientID %>");
            }
        };
    </script>
    </insite:PageFooterContent>
</asp:Content>