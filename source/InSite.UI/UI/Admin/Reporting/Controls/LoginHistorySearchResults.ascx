<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginHistorySearchResults.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.LoginHistorySearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Date and Time">
            <ItemTemplate>
                <%# LocalizeTime((DateTimeOffset)Eval("SessionStarted")) %>
                <div class="fs-sm text-muted">
                    <%# (int?)Eval("SessionMinutes") != null && (int)Eval("SessionMinutes") != 0 ? string.Format("Session duration = {0:n0} minutes", Eval("SessionMinutes")) : "" %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="User" DataField="UserEmail" />

        <asp:BoundField HeaderText="Language" DataField="UserLanguage" />

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# (bool)Eval("SessionIsAuthenticated") ? "Succeeded" : "Failed" %>
                <div class="fs-sm text-danger">
                    <%# Eval("AuthenticationErrorType") %>
                    <div class="form-text"><%# Eval("AuthenticationErrorMessage") %></div>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField HeaderText="IP Address" DataField="UserHostAddress" />

        <asp:TemplateField HeaderText="Browser">
            <ItemTemplate>
                <span title="<%# Eval("UserAgent") %>">
                    <%# Eval("UserBrowser") %>
                    v<%# Eval("UserBrowserVersion") %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>