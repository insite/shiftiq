<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminHeader.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.Navigation.AdminHeaderControl" %>

<asp:Literal runat="server" ID="HiddenTitleLiteral" Visible="false">
    <style>
        .form-header.border-bottom {
            border-bottom: none !important;
        }
    </style>
</asp:Literal>

<div class="form-header border-bottom">
    <div runat="server" id="EnvironmentReminder" />
    <nav runat="server" id="Breadcrumbs" aria-label="breadcrumb">
        <ol class="breadcrumb">
            <li runat="server" id="HomeItem" visible="false" class="breadcrumb-item"><a runat="server" id="HomeLink">Home</a></li>
            <li runat="server" id="AdminItem" visible="false" class="breadcrumb-item"><a runat="server" id="AdminLink">Admin</a></li>
            <asp:Repeater runat="server" ID="BreadcrumbRepeater">
                <ItemTemplate>
                    <li class='breadcrumb-item <%# Eval("Active") %>' aria-current="page"><%# Eval("Anchor") %></li>
                </ItemTemplate>
            </asp:Repeater>
            <li runat="server" id="GoToCalendar" visible="false" class="ms-5"><a runat="server" id="Calendar" href="/ui/portal/events/calendar"><i class="fas fal fa-calendar-alt me-1"></i>Calendar</a></li>
            <li runat="server" id="AddNewItem" visible="false" class="ms-5"><a runat="server" id="AddNewAnchor" href="#"></a></li>
            <li runat="server" id="AddNewList" visible="false" class="ms-5">
                <div class="btn-group" role="group" aria-label="button group with nested dropdown">
                  <div class="btn-group" role="group">
                    <span type="button" class="dropdown-toggle" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <i class="fas fa-plus-circle me-1"></i>
                        <insite:Literal runat="server" Text="Add New" />
                    </span>
                    <div class="dropdown-menu">
                        <asp:Repeater runat="server" ID="AddNewAnchors">
                            <ItemTemplate>
                                <a href='<%# Eval("Href") %>' class="dropdown-item"><%# Eval("Text") %></a>
                            </ItemTemplate>
                        </asp:Repeater>
                      
                    </div>
                  </div>
                </div>
            </li>
        </ol>
    </nav>
    <div runat="server" id="ActionTitlePanel" class="row pb-2">
        <div class="col-lg-12">
            <h1 runat="server" id="ActionTitle" class="mb-1"></h1>
            <div runat="server" id="ActionSubtitle" class="d-none"></div>
        </div>
    </div>
</div>