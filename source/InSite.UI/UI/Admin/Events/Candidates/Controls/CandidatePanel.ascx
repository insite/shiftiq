<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CandidatePanel.ascx.cs" Inherits="InSite.Admin.Events.Candidates.Controls.CandidatePanel" %>

<style type="text/css">
    div.workflow-status span {
        margin-left: 3px;
    }

    div.workflow-status {
        text-align: right;
    }

    div.workflow-status .accommodation {
        font-size: 13px;
        color: #a94442;
    }

    div.workflow-status .instructor {
        font-size: 13px;
        color: #594442;
    }

    .insite-combobox.exam-asset-key .dropdown-menu a {
        clear:both;
    }

        .insite-combobox.exam-asset-key .dropdown-menu a span small {
            float: left;
            width: 100%;
            font-size: 0.85em;
            color: #14334e;
            font-weight: 600;
        }

</style>

<div runat="server" loadingpanelid="LoadingPanel">

    <div class="mb-3">
        <div class="float-end">
            <insite:Button runat="server" ID="AddCandidates" Text="Add Candidate(s)" Icon="fas fa-plus-circle" ButtonStyle="Default" />
            <insite:Button runat="server" ID="VerifyCandidates" Text="Verify All" Icon="fas fa-shield-check" ButtonStyle="Warning" />
            <insite:Button runat="server" ID="DownloadScrapPaperPre" Text="Scrap Paper" Icon="fas fa-download" ButtonStyle="Primary" />
        </div>
        <div class="row">
            <div class="col-md-5 ps-0 pe-0">
                <insite:TextBox runat="server" ID="FilterTextBox" Width="260" EmptyMessage="Filter" CssClass="d-inline-block form-control-sm" />
                <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />
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
            </div>
            <div class="col-md-4">
                <insite:ComboBox runat="server" ID="SortSelector" Width="160px" CssClass="d-inline" ButtonSize="Small">
                    <Items>
                        <insite:ComboBoxOption Text="Sort by Full Name" Value="full" />
                        <insite:ComboBoxOption Text="Sort by Last Name" Value="last" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>
    </div>

    <insite:Grid runat="server" ID="Grid" CssClass="candidate-panel-grid table table-striped table-bordered" DataKeyNames="RegistrationIdentifier">
        <Columns>

            <asp:TemplateField HeaderText="Exam Candidate" ItemStyle-Wrap="False">
                <ItemTemplate>

                    <%# GetCandidateStatus(Container.DataItem) %>

                    <asp:HyperLink runat="server" ID="PersonLink" NavigateUrl='<%# Eval("CandidateIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'>
                        <%# Eval("CandidateName") %>
                    </asp:HyperLink>
                    <div class="form-text">
                        Exam Candidate Code: <%# Eval("CandidateCode") %>
                    </div>
                    <div>
                        <insite:IconLink runat="server" ID="DeleteButton"
                            Visible="<%# CanWrite %>" Name="trash-alt" Type="Regular" ToolTip="Delete Exam Candidate"
                        />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Assessment Form">
                <ItemTemplate>

                    <div runat="server" id="AssetLink">
                        <%# GetFormTitle(Container.DataItem) %>
                    </div>

                    <div class="form-text" runat="server" id="AssetNumber">Code: <%# GetFormCode(Container.DataItem) %></div>
                    <div class="form-text" runat="server" id="Div1">Name: <%# GetFormName(Container.DataItem) %></div>

                    <asp:PlaceHolder runat="server" ID="UnassignedExamPanel">
                        <insite:ComboBox runat="server" ID="ExamAssetKey" CssClass="exam-asset-key" Width="100%" />
                        <div style="padding-top: 3px;">
                            <insite:IconButton runat="server" ID="SelectExamFormButton" Name="layer-plus" ToolTip="Complete Selection" CommandName="CompleteSelection" />
                        </div>
                    </asp:PlaceHolder>

                    <div runat="server" id="AssignedExamPanel">
                        <insite:IconButton runat="server" ID="CancelRegistrationButton" ToolTip="Cancel Selection" CommandName="CancelSelection" Name="ban" ConfirmText="Are you sure you want to cancel the exam form selection for this exam candidate?" />
                    </div>

                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Exam Registration">
                <ItemTemplate>

                    <%# GetRegistrationStatus(Eval("RegistrationIdentifier")) %>

                    <div runat="server" id="RegistrationPanel">

                        <insite:IconLink runat="server" ID="RegistrationLink" ToolTip="Change Candidate Registration" Name="pencil" />

                        <insite:IconLink runat="server" ID="HistoryLink" ToolTip="View Change History" Name="history" />

                        <insite:IconButton runat="server" ToolTip="Start Verification" CommandName="StartVerification" Name="play" Visible="<%# CanWrite %>" />

                        <insite:IconLink runat="server" ToolTip="Start Attempt" Name="rocket"
                            NavigateUrl="/ui/lobby/events/login" Target="_blank" />

                        <insite:IconLink runat="server" ID="ViewAttemptButton" ToolTip="View Attempt" Name="search"
                            Visible='<%# Eval("AttemptScore") != null %>'
                            NavigateUrl='<%# Eval("RegistrationIdentifier", "/ui/admin/assessments/attempts/view?attempt={0}") %>' />

                    </div>

                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>

</div>

<asp:Button runat="server" ID="DownloadScrapPaper" Style="display: none;" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {

            var instance = window.candidatePanel = window.candidatePanel || {};

            instance.scrollToRegistration = function (id) {
                var $registrationRow = $('div.candidate-panel-grid:visible > table > tbody > tr[data-registration="' + String(id) + '"]');
                if ($registrationRow.length !== 1)
                    return;

                var headerHeight = $('header.navbar:first').outerHeight();
                var scrollTo = $registrationRow.offset().top - headerHeight;

                if (scrollTo < 0)
                    scrollTo = 0;

                $('html, body').animate({ scrollTop: scrollTo }, 250);
            };

            $("#<%= DownloadScrapPaperPre.ClientID %>").on("click", function () {
                __doPostBack("<%= DownloadScrapPaper.UniqueID %>", "");
            });

            Sys.Application.remove_load(onAppLoad);
            Sys.Application.add_load(onAppLoad);

            function onAppLoad() {
                $('[data-toggle="popover"]').each(function () {
                    var $this = $(this);
                    if ($this.data('bs.popover'))
                        return;

                    $this.popover();
                });
            }

        })();
    </script>
</insite:PageFooterContent>
