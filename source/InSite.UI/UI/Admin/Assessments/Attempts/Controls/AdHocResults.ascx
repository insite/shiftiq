<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdHocResults.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.AdHocResults" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="SubmissionIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Framework">
            <itemtemplate>
                <%# Eval("FrameworkTitle") %>
                <div class="form-text"><%# Eval("ProfileTitle") %></div>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Form">
            <itemtemplate>
                <%# Eval("FormName") %>
                <div class="form-text">
                    Asset #<%# Eval("FormAsset") %>
                            /
                            v<%# GetFormAssetVersion() %>
                </div>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Candidate">
            <itemtemplate>
                <%# Eval("CandidateName") %>
                <div class="form-text"><%# Eval("CandidateCode") %></div>
            </itemtemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Format" DataField="EventFormat" ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="Tag" DataField="AttemptTag" />

        <asp:TemplateField HeaderText="Completed">
            <itemtemplate>
                <%# Eval("AttemptDuration") %>
                <div class="form-text"><%# Eval("AttemptCompletedText") %></div>
            </itemtemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Score" DataField="AttemptScore" DataFormatString="{0:p0}" ItemStyle-CssClass="text-end" />
        <asp:BoundField HeaderText="Grade" DataField="AttemptGrade" ItemStyle-CssClass="text-end" />

    </Columns>
</insite:Grid>
