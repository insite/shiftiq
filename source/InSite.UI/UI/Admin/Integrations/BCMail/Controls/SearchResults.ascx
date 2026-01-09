<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Desktops.Custom.SkilledTradesBC.Distributions.Controls.SearchResults" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="EventIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Examination Scheduled">
            <ItemTemplate>
                <strong>
                    <a href="/ui/admin/events/exams/outline?event=<%# Eval("EventIdentifier") %>"><%# Eval("EventType") %> <%# Eval("EventNumber") %>-<%# Eval("EventBillingType") %></a>
                </strong>
                <div>
                    <%# GetLocalTime((DateTimeOffset)Eval("EventScheduledStart")) %>
                </div>
                <%# Eval("VenueLocation.GroupName") %>
                <div>
                    <span class="badge bg-primary"><%# Eval("FormCountText") %></span>
                    <span class="badge bg-success"><%# Eval("CandidateCountText") %></span>
                </div>
                <span class="form-text">
                    <%# Eval("DistributionCodeHtml") %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Distribution Ordered" ItemStyle-Width="190px">
            <ItemTemplate>
                <%# GetLocalTime((DateTimeOffset?)Eval("DistributionOrdered")) %>
                <div style="margin-top: 10px;">
                    <%# GetLocalTime((DateTimeOffset?)Eval("DistributionOrdered")) == null ? "" : "RECORD_RECEIVED" %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Distribution Expected" ItemStyle-Width="190px">
            <ItemTemplate>
                <%# GetLocalTime((DateTimeOffset?)Eval("DistributionExpected")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Distribution Shipped" ItemStyle-Width="190px">
            <ItemTemplate>
                <%# GetLocalTime((DateTimeOffset?)Eval("DistributionShipped")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Distribution Returned" ItemStyle-Width="190px">
            <ItemTemplate>
                <%# GetLocalTime((DateTimeOffset?)Eval("ExamMaterialReturnShipmentReceived")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<div>

    <div style="margin-top: 30px;">
        <insite:Button runat="server" ID="PlaceDistributionOrderButton" ButtonStyle="Success" Text="Place Distribution Order" Icon="fas fa-mail-bulk" />
    </div>

    <asp:Repeater runat="server" ID="RequestRepeater">
        <HeaderTemplate>
            <div class="form-text" style="margin-top: 10px;">
                Here is a snapshot of the data to be submitted to the Web API at BC Mail:
            </div>
        </HeaderTemplate>
        <ItemTemplate>
            <h2 style="margin: 5px 0px 10px 0px;"><%# Eval("Title") %></h2>
            <pre><code><%# Eval("Json") %></code></pre>
        </ItemTemplate>
    </asp:Repeater>

    <asp:Literal runat="server" ID="RequestMessage" />

</div>
