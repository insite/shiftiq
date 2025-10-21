<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Admin.Records.Instructors.Search" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Alert runat="server" ID="SearchAlert" />
    <insite:Alert runat="server" ID="ScreenStatus" />

    <!-- Nav tabs -->
    <ul class="nav nav-tabs" role="tablist">
      <li class="nav-item">
        <a href="#results" class="nav-link active" data-bs-toggle="tab" role="tab">
          <i class="far fa-spell-check me-2"></i>
          <asp:Literal runat="server" ID="GradebooksTitle" />
        </a>
      </li>
    </ul>

    <!-- Tabs content -->
    <div class="tab-content">
      <div class="tab-pane fade show active" id="results" role="tabpanel">
        <p><asp:Literal runat="server" ID="GradebooksGuide" /></p>
        <div  id="NoGradebooks" runat="server">
            No gradebooks are available.
        </div>
        <asp:Repeater runat="server" ID="GradebookRepeater">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Gradebook</th>
                            <th>Class</th>
                            <th>Scheduled</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <a href='/ui/admin/records/gradebooks/instructors/gradebook-outline?<%# Eval("GradebookIdentifier", "id={0}") %>'><%# Eval("GradebookTitle") %></a>
                    </td>
                    <td>
                        <div runat="server" visible='<%# Eval("EventIdentifier") != null %>'>
                            <%# Eval("Event.EventTitle") %>
                        </div>
                        <div class="form-text"><%# Eval("Event.EventDescription") %></div>
                    </td>
                    <td>
                        <%# GetLocalTime(Eval("Event.EventScheduledStart")) %> - <%# GetLocalTime(Eval("Event.EventScheduledEnd")) %>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>
      </div>     
    </div>
</asp:Content>
