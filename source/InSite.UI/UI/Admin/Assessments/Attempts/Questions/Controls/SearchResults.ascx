<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Attempts.Questions.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SearchResultsUpdatePanel" />
<insite:UpdatePanel runat="server" ID="SearchResultsUpdatePanel">
    <ContentTemplate>
        <insite:Grid runat="server" ID="Grid" DataKeyNames="QuestionIdentifier, AttemptIdentifier">
            <Columns>
                
                <asp:TemplateField HeaderText="Attempted">
                    <ItemTemplate>
                        <%# Eval("AttemptGraded") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Candidate">
                    <ItemTemplate>
                        <%# Eval("AttemptCandidateName") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Answer">
                    <ItemTemplate>
                        <%# Eval("AnswerOptionSequence") %> <%# Eval("AnswerText") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Points">
                    <ItemTemplate>
                        <%# GetDecimalString(Eval("AnswerPoints")) %> out of <%# GetDecimalString(Eval("QuestionPoints")) %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>
    </ContentTemplate>
</insite:UpdatePanel>
