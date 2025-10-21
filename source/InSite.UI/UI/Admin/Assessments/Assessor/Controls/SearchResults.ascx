<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Assessments.Assessor.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="AttemptIdentifier">
    <Columns>


        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <insite:IconLink runat="server" ID="ViewAttemptLink" Name="search"
                    NavigateUrl='<%# Eval("AttemptIdentifier", "/ui/admin/assessments/assessor/view?attempt={0}") %>' 
                />
            </ItemTemplate>
        </asp:TemplateField>
                        
        <asp:TemplateField HeaderText="Exam Form">
            <ItemTemplate>
                <%# Eval("Form.FormTitle") %>
                <div class="form-text">
                    <%# Eval("Form.FormName") %>
                    &bull;
                    Exam Form Asset #<%# GetFormAsset() %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Date and Time">
            <ItemTemplate>
                <%# FormatTime() %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Score" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end" ItemStyle-Wrap="False" >
            <ItemTemplate>
                <%# FormatScore() %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
