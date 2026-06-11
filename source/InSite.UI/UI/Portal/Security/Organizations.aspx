<%@ Page Language="C#" CodeBehind="Organizations.aspx.cs" Inherits="InSite.UI.Portal.Security.Organizations" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <h3 runat="server" id="HelpHeading" class="d-block bg-secondary fs-sm fw-semibold text-body-secondary mb-0 px-4 py-3"><%= Translate("Recent Sessions") %></h3>

    <div class="d-block p-4">
        
        <asp:Repeater runat="server" ID="SessionRepeater">
            <ItemTemplate>
                <div class="organization">
                    <asp:HyperLink runat="server" ID="StartLink" />
                </div>
                <div class="form-text mb-2">
                    <%# GetTimestampHtml(Eval("SessionStarted")) %>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        
    </div>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div runat="server" id="ApiOfflineAlert" visible="false" class="alert alert-warning" role="alert">
        <%= Translate("The Engine API is offline, so partitions cannot be displayed.") %>
    </div>

    <div runat="server" id="NoPartitionsAlert" visible="false" class="alert alert-warning" role="alert">
        <%= Translate("The Engine API returned no partitions.") %>
    </div>

    <asp:Panel runat="server" ID="OrganizationPanel">
        <div class="card">
            <div class="card-body">

                <h2 class="h4 mb-3">Organizations</h2>

                <asp:Repeater runat="server" ID="OrganizationRepeater">
                    <HeaderTemplate>
                        <div class="row">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                    <ItemTemplate>
                        <div class="col-6">
                            <insite:RadioButton runat="server" ID="Select" OnClientChange='<%# $"window.location.replace(\"{Eval("RedirectUrl")}\");" %>'
                                Text='<%# Eval("CompanyTitle") %>' GroupName=<%# ClientID + "_Orgs" %> />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </div>
    </asp:Panel>

    <asp:Panel runat="server" ID="PartitionsPanel" Visible="false">

        <style>
            /* Logos hidden: two partition groups per row to reduce vertical scrolling. */
            #PartitionsList:not(.show-logos) {
                display: grid;
                grid-template-columns: repeat(2, 1fr);
                gap: 1rem;
                align-items: start;
            }
            #PartitionsList:not(.show-logos) .list-group {
                margin-bottom: 0 !important;
            }
            @media (max-width: 767.98px) {
                #PartitionsList:not(.show-logos) {
                    grid-template-columns: 1fr;
                }
            }

            #PartitionsList .org-logo { display: none; }
            #PartitionsList.show-logos .org-logo {
                display: inline-block;
                width: 10rem;
                text-align: center;
                vertical-align: middle;
            }
            #PartitionsList.show-logos .org-logo img {
                max-width: 10rem;
                max-height: 8rem;
                vertical-align: middle;
            }
            #PartitionsList.show-logos .list-group-item {
                
            }
        </style>

        <div id="PartitionsList">
            <asp:Repeater runat="server" ID="PartitionRepeater">
                <ItemTemplate>
                    <ul class="list-group mb-3">
                        <li class="list-group-item active m-0">

                            <div class="float-end"><span class="badge bg-info"><%# Eval("Slug") %></span></div>

                            <strong title='<%# Eval("Domain") %>'><%# Eval("Heading") %></strong>
                            
                        </li>
                        <asp:Repeater runat="server" DataSource='<%# Eval("Organizations") %>'>
                            <ItemTemplate>
                                <li class="list-group-item">
                                    <span class="org-logo me-4">
                                        <asp:Image runat="server" ImageUrl='<%# Eval("LogoUrl") %>'
                                            Visible='<%# !string.IsNullOrEmpty((string)Eval("LogoUrl")) %>' AlternateText="" />
                                    </span>
                                    <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Url") %>' Text='<%# Eval("Name") %>' />
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div id="ShowLogosControl" class="form-check form-switch align-self-md-center mb-0">
            <input class="form-check-input" type="checkbox" id="ShowLogosToggle"
                onclick="document.getElementById('PartitionsList').classList.toggle('show-logos', this.checked);">
            <label class="form-check-label" for="ShowLogosToggle"><%= Translate("Show logos") %></label>
        </div>

        <script>
            // Relocate the toggle into the shared TitlePanel so it floats top-right beside the title.
            // ClientID is namespaced by the user control, so match on the id suffix.
            (function () {
                var control = document.getElementById('ShowLogosControl');
                var title = document.querySelector("[id$='TitlePanel']");
                if (control && title)
                    title.appendChild(control);
            })();
        </script>

    </asp:Panel>

</asp:Content>
