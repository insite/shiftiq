<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Portal.Records.Credentials.Learners.Controls.SearchResults" %>

<insite:Literal runat="server" ID="Instructions" />

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid">
        <Columns>
            <asp:TemplateField HeaderText="Achievement">
                <ItemTemplate>
                    <%# Eval("AchievementTitle") %>
                    <div class="form-text">
                        <%# ( (bool)Eval("IsSelfDeclared") ? "<span class='badge bg-info'>Self-Declared</span>" : "" ) %>
                        <%# Eval("AchievementType") %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Issued">
                <ItemTemplate>
                    <%# Eval("CredentialIssued") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Expired">
                <ItemTemplate>
                    <%# Eval("CredentialExpiry") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <%# Eval("CredentialStatus") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Attachment">
                <ItemTemplate>
                    <%# Eval("DownloadLink") %>
                    <%# Eval("DeleteLink") %>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </insite:Grid>
</div>
