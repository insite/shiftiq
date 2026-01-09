<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExamMarkGrid.ascx.cs" Inherits="InSite.Admin.Events.Exams.Controls.ExamMarkGrid" %>

<insite:Alert runat="server" ID="ErrorPanel" Visible="false" />

<asp:Panel runat="server" ID="NoGradesPanel" CssClass="alert alert-info mb-3" Visible="false">
    There are no grades for this exam event. Click the <b>Upload Attempts</b> button to upload results from CSV, Scantron, or LXR.
</asp:Panel>

<div runat="server" id="CommandPanel" class="mb-3">
    <insite:Button runat="server" ID="GradeLink" Text="Upload Attempts" Icon="fas fa-upload" ButtonStyle="Default" />

    <insite:Button runat="server" ID="ValidateButton" ButtonStyle="Warning" ConfirmText="Are you sure to validate all the scores for this event?" Text="Validate" Icon="fas fa-shield-check" />

    <insite:Button runat="server" ID="ReleaseButton" ButtonStyle="Primary" ConfirmText="Are you sure to release all the grades withheld for this event?" Text="Release" Icon="fas fa-calendar-check" />

    <insite:Button runat="server" ID="PublishButton" ButtonStyle="Success" ConfirmText="Are you sure to publish all the grades for this event?" Text="Push to DA" Icon="fas fa-share-square" />
</div>

<insite:Grid runat="server" ID="Grid" DataKeyNames="RegistrationIdentifier">
    <Columns>
                        
        <asp:TemplateField HeaderText="Exam Form">
            <ItemTemplate>
                <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("Form.BankIdentifier") %>&form=<%# Eval("Form.FormIdentifier") %>">
                    <%# Eval("Form.FormTitle") %>
                </a>
                <div class="form-text">
                    <%# Eval("Form.FormName") %>
                    &bull;
                    Exam Form Asset #<%# Eval("Form.FormAsset") %>.<%# Eval("Form.FormAssetVersion") %>
                </div>
                <div>
                    <%# Logic.GetRegistrationStatus(Container.DataItem) %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Exam Candidate" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a href='/ui/admin/contacts/people/edit?contact=<%# Eval("CandidateIdentifier") %>'><%# Eval("Candidate.UserFullName") %></a>
                <span class="form-text"><%# Eval("Candidate.PersonCode") %></span>
                <div>
                    <a href="mailto:<%# Eval("Candidate.UserEmail") %>">
                        <%# Eval("Candidate.UserEmail") %>
                    </a>
                </div>
                <div><%# Logic.GetSebVersion(Container.DataItem) %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Date and Time">
            <ItemTemplate>
                <%# Logic.FormatTime(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Score" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Logic.FormatScore(Container.DataItem) %>
                <div>
                    <%# Logic.GetGradeStatus(Container.DataItem) %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end">
            <ItemTemplate>

                <insite:IconButton runat="server" ID="WithholdGradeButton" CommandName="Withhold" Name="calendar-times" ToolTip="Withhold Grade" 
                    Visible='<%# CanWrite && Logic.AllowWithhold(Container.DataItem) %>' />

                <insite:IconButton runat="server" ID="ReleaseGradeButton" CommandName="Release" Name="calendar-check" ToolTip="Release Grade"
                    Visible='<%# CanWrite && Logic.AllowRelease(Container.DataItem) %>'/>

                <insite:IconButton runat="server" ID="PublishGradeButton" CommandName="Publish" Name="share-square" ToolTip="Publish"
                    Visible='<%# CanWrite && Logic.AllowPublish(Container.DataItem) %>'
                    ConfirmText="Are you sure you want to publish this grade?" />

                <insite:IconLink runat="server" Name="search" ToolTip="View Exam Result"
                    style='<%# Eval("AttemptIdentifier") != null ? "" : "visibility:hidden;" %>'
                    NavigateUrl='<%# Eval("AttemptIdentifier", "/ui/admin/assessments/attempts/view?attempt={0}") %>' />

            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
