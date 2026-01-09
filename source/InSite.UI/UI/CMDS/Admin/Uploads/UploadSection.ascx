<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadSection.ascx.cs" Inherits="InSite.Cmds.Admin.Uploads.Controls.UploadSection" %>

<div class="settings">
<asp:ListView ID="ListOfAchievementTypes" runat="server">
    <LayoutTemplate>
        <div class="row">
            <asp:PlaceHolder id="groupPlaceholder" runat="server"></asp:PlaceHolder>
        </div>
    </LayoutTemplate>
    <GroupTemplate>
        <div class="col-lg-12">
            <asp:PlaceHolder id="itemPlaceholder" runat="server"></asp:PlaceHolder>
        </div>
    </GroupTemplate>
    <ItemTemplate>

        <asp:Repeater ID="AchievementTypes" runat="server">
            <ItemTemplate>
                <h3><%# Eval("TitleDisplay") %></h3>

                <asp:Repeater ID="Achievements" runat="server">
                    <ItemTemplate>
                        <div>
                            <strong>
                                <asp:PlaceHolder runat="server" Visible='<%# Eval("Number") != null %>'>
                                    <%# Eval("Number") %>:
                                </asp:PlaceHolder>

                                <%# Eval("Title") %>
                            </strong>
                        </div>
                        <div>
                            <ul>
                                <asp:Repeater ID="Downloads" runat="server">
                                    <ItemTemplate>
                                        <li>
                                            <asp:HyperLink runat="server" Target="_blank" NavigateUrl='<%# Eval("Url") %>'>
                                                <%# Eval("Title") %>
                                            </asp:HyperLink>

                                            <asp:Repeater ID="Competencies" runat="server">
                                                <HeaderTemplate><br /></HeaderTemplate>
                                                <SeparatorTemplate>, </SeparatorTemplate>
                                                <ItemTemplate><span class="form-text"><%# Eval("Number") %></span></ItemTemplate>
                                            </asp:Repeater>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

            </ItemTemplate>
        </asp:Repeater>
            
    </ItemTemplate>
</asp:ListView>
</div>