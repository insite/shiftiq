<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubmitSearchResults.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.SubmitSearchResults" %>

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid" Translation="Header">
        <Columns>
            <asp:TemplateField HeaderText="Form">
                <ItemTemplate>
                    <%# GetSurveyTitle() %>
                    <div class="form-text">
                        <%# GetFirstCommentAnswer() %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Date and Time">
                <ItemTemplate>
                    <%# FormatTime() %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Status" ItemStyle-Wrap="False" >
                <ItemTemplate>
                    <%# Translate((string)Eval("ResponseSessionStatus")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" HeaderStyle-Width="40px">
                <ItemTemplate>
                    <div class="mb-2">
                        <insite:Button runat="server" ID="RestartButton" ButtonStyle="Default" Icon="fas fa-undo-alt" ToolTip="Start Again" />
                        <insite:Button runat="server" ID="DeleteButton" ButtonStyle="Default" Icon="fas fa-trash-alt" ToolTip="Delete Submission" />
                    </div>
                    <insite:Button runat="server" ID="StartButton" ButtonStyle="Default" Width="130px" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>
</div>