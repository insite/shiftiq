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

</asp:Content>