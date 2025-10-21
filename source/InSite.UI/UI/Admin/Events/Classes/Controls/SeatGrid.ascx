<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeatGrid.ascx.cs" Inherits="InSite.Admin.Events.Classes.Controls.SeatGrid" %>

<insite:Grid runat="server" ID="Grid">

    <Columns>

        <asp:TemplateField HeaderText="Seat Name">
            <ItemTemplate>
                <a href="<%# InSite.ReturnUrlHelper.GetRedirectUrl((string)Eval("SeatIdentifier", "/ui/admin/events/seats/edit?id={0}"), "panel=seats") %>"><%# Eval("SeatTitle") %></a>
                <p><%# GetDescription(Container.DataItem) %></p>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Seat Availability">
            <ItemTemplate>
                <%# (bool)Eval("IsAvailable") ? "Available" : "Hide" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Seat Taxes">
            <ItemTemplate>
                <%# (bool)Eval("IsTaxable") ? "Yes" : "No" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <asp:Literal runat="server" ID="FreePrice" Text="Free" />
                <asp:Literal runat="server" ID="SinglePrice" />

                <asp:Repeater runat="server" ID="MultiplePrice">
                    <ItemTemplate>
                        <div>
                            <span runat="server" visible='<%# Eval("GroupStatus") != null %>' class="form-text">
                                <%# Eval("GroupStatus") %>
                            </span>

                            <%# Eval("Name") %>: <%# Eval("Amount", "{0:c2}") %>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="100px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" Type="Regular" Visible="<%# CanWrite %>" ToolTip='Edit Seat'
                    NavigateUrl='<%# InSite.ReturnUrlHelper.GetRedirectUrl((string)Eval("SeatIdentifier", "/ui/admin/events/seats/edit?id={0}&back=outline"), "panel=seats") %>' />
                <insite:IconLink runat="server" Visible="<%# CanWrite %>" Name="trash-alt" Type="Regular" ToolTip="Delete Seat"
                    NavigateUrl='<%# Eval("SeatIdentifier", "/ui/admin/events/seats/delete?id={0}&back=outline") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>

</insite:Grid>
