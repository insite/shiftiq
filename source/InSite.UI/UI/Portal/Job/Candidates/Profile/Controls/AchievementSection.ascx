<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.AchievementSection" %>

<div class="card mb-3">
    <div class="card-body">

        <h4 id="achievement-section" class="card-title">Shift iQ Achievements</h4>

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
        <insite:UpdatePanel runat="server" ID="UpdatePanel">
            <ContentTemplate>

                <div runat="server" id="NoAchievements" class="alert alert-warning" role="alert">
                    There are no achievements
                </div>

                <div class="table-responsive">
                    <insite:Grid runat="server" ID="AchievementsGrid" DataKeyNames="CredentialIdentifier">
                        <Columns>
                            <asp:TemplateField HeaderText="Achievement">
                                <ItemTemplate>
                                    <%# Eval("AchievementTitle") %>
                                    <div class="form-text">
                                        <%# Eval("AchievementLabel") %>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Granted">
                                <ItemTemplate>
                                    <%# GetDateString("CredentialGranted") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Expired">
                                <ItemTemplate>
                                    <%# GetDateString("CredentialExpirationExpected") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <%# GetStatusHtml() %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <%# GetDownloadCertificateButton() %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </insite:Grid>
                </div>

            </ContentTemplate>
        </insite:UpdatePanel>

    </div>
</div>
