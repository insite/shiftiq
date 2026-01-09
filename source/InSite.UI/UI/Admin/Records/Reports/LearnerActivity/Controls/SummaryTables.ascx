<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SummaryTables.ascx.cs" Inherits="InSite.Admin.Records.Reports.LearnerActivity.Controls.SummaryTables" %>

<div class="row settings">
    <insite:Container runat="server" ID="SummariesContainer">
        <div class="row settings my-4">

            <div class="col-lg-4">

                <h3>Program Summaries</h3>

                <insite:Grid runat="server" ID="ProgramNames" HeaderStyle-CssClass="d-none" Caption="Program Name">
                    <Columns>
                        <asp:BoundField DataField="Name" />
                        <asp:BoundField DataField="Value" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </insite:Grid>

                <insite:Grid runat="server" ID="GradebookNames" HeaderStyle-CssClass="d-none" Caption="Gradebook Name">
                    <Columns>
                        <asp:BoundField DataField="Name" />
                        <asp:BoundField DataField="Value" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </insite:Grid>

                <insite:Grid runat="server" ID="EnrollmentStatuses" HeaderStyle-CssClass="d-none" Caption="Gradebook Enrollment Status">
                    <Columns>
                        <asp:BoundField DataField="Name" />
                        <asp:BoundField DataField="Value" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </insite:Grid>

                <h3>Engagement Summaries</h3>

                <insite:Grid runat="server" ID="EngagementStatuses" HeaderStyle-CssClass="d-none" Caption="Sign In Activity">
                    <Columns>
                        <asp:BoundField DataField="Name" HtmlEncode="False" />
                        <asp:BoundField DataField="Value" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </insite:Grid>

            </div>

            <div class="col-lg-4">

                <h3>Learner Summaries</h3>

                <insite:Grid runat="server" ID="LearnerGenders" HeaderStyle-CssClass="d-none" Caption="Gender">
                    <Columns>
                        <asp:BoundField DataField="Name" />
                        <asp:BoundField DataField="Value" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </insite:Grid>

                <insite:Grid runat="server" ID="LearnerEmployers" HeaderStyle-CssClass="d-none" Caption="Employed By / Belongs To" EnablePaging="false">
                    <Columns>
                        <asp:BoundField DataField="Name" />
                        <asp:BoundField DataField="Value" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </insite:Grid>

                <insite:Grid runat="server" ID="LearnerCitizenships" HeaderStyle-CssClass="d-none" Caption="Citizenship" EnablePaging="false">
                    <Columns>
                        <asp:BoundField DataField="Name" />
                        <asp:BoundField DataField="Value" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </insite:Grid>

                <insite:Grid runat="server" ID="LearnerMembershipStatuses" HeaderStyle-CssClass="d-none" Caption="Membership Status" EnablePaging="false">
                    <Columns>
                        <asp:BoundField DataField="Name" />
                        <asp:BoundField DataField="Value" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </insite:Grid>

            </div>

        </div>

        <div class="mt-3 sticky-buttons">
            <insite:Button runat="server" ID="DownloadSummaryTables" Text="Download Summaries" Icon="fas fa-download" />
            <insite:Button runat="server" ID="SendEngagementPrompt" Text="Send Engagement Prompt" Icon="fas fa-paper-plane" NavigateUrl="/ui/admin/records/reports/engagement-prompt" NavigateTarget="_blank" />
        </div>
    </insite:Container>
</div>
