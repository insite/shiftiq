<%@ Page Language="C#" CodeBehind="LearningPlan.aspx.cs" Inherits="InSite.UI.Portal.Home.LearningPlan" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<style type="text/css">
    .badge {
        padding: 0.5em 0.5em 0.5em !important;
    }
</style>

<!-- Title -->
<div class="d-sm-flex align-items-center justify-content-between pb-4 text-center text-sm-start">
    <div class="d-flex align-items-center mb-3">
        <label class="form-label text-nowrap pe-1 me-2 mb-0">Show</label>
        <asp:DropDownList runat="server" ID="ShowWhat" CssClass="form-select form-select-sm">
            <asp:ListItem Selected="True" Text="All" Value="All" />
            <asp:ListItem Text="Pending" Value="Pending" />
            <asp:ListItem Text="Valid" Value="Valid" />
            <asp:ListItem Text="Expired" Value="Expired" />
        </asp:DropDownList>
    </div>
</div>

<!-- Content-->
<div class="row">
    <div class="col-lg-12">
        
        <div runat="server" id="NoAchievements" class="alert alert-info" visible="false"></div>

        <asp:Repeater ID="AchievementTypes" runat="server">
            <ItemTemplate>
                
                <h2 class="h3"><%# Eval("AchievementTypeDisplay") %></h2>

                <div class="accordion mb-5" id='accordion-achievements-<%# Eval("AchievementTypeCode") %>'>

                    <asp:Repeater ID="AchievementItems" runat="server">
                        <ItemTemplate>

                            <div class="accordion-item">
                                <h2 class="accordion-header" id='item-header-<%# Eval("CredentialIdentifier") %>'>
                                  <button class="accordion-button no-indicator justify-content-between pe-4 collapsed" type="button" data-bs-toggle="collapse" data-bs-target='#item-collapse-<%# Eval("CredentialIdentifier") %>' aria-expanded="false" aria-controls='item-collapse-<%# Eval("CredentialIdentifier") %>'>
                                    <div class="d-flex justify-content-start">
                                        <%# GetFlagHtml(Container.DataItem) %>
                                        <%# Eval("AchievementTitle") %>
                                    </div>
                                    <div class="d-flex justify-content-end">
                                        <%# GetExpiryHtml(Container.DataItem) %>
                                    </div>
                                  </button>
                                </h2>
                                <div class="accordion-collapse collapse" id='item-collapse-<%# Eval("CredentialIdentifier") %>' aria-labelledby='item-header-<%# Eval("CredentialIdentifier") %>' data-bs-parent="#orders-accordion" style="">
                                  <div class="accordion-body pt-4 bg-secondary rounded-top-0 rounded-3">
                                    
                                    <div class="pb-4">
                                        <%# Eval("AchievementDescription") %>
                                    </div>

                                    <div class="d-flex flex-wrap align-items-center justify-content-between pt-3 border-top">
                                      <div class="fs-sm my-2 me-2"><span class="text-body-secondary me-1">Current Status:</span><span class="fw-medium"><%# GetStatusHtml(Container.DataItem) %></span></div>
                                      <div class="fs-sm my-2 me-2"><span class="text-body-secondary me-1">Effective:</span><span class="fw-medium"><%# GetGrantedHtml(Container.DataItem) %></span></div>
                                      <div class="fs-sm my-2 me-2"><span class="text-body-secondary me-1">Lifetime:</span><span class="fw-medium"><%# GetLifetimeHtml(Container.DataItem) %></span></div>
                                    </div>

                                    <%# GetDeclarationHtml(Container.DataItem) %>

                                  </div>
                                </div>
                              </div>

                        </ItemTemplate>
                    </asp:Repeater>

                </div>

            </ItemTemplate>
        </asp:Repeater>

    </div>
</div>

</asp:Content>