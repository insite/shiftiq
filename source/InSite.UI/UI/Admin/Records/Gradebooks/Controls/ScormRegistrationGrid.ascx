<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScormRegistrationGrid.ascx.cs" Inherits="InSite.Admin.Records.Gradebooks.Controls.ScormRegistrationGrid" %>

<h2 class="h4 mt-4 mb-3">SCORM Registrations</h2>

<div class="card border-0 shadow-lg">

    <div class="card-body">
        
        <insite:Grid runat="server" ID="Grid" DataKeyNames="ScormRegistrationIdentifier">
        
            <Columns>

                <asp:TemplateField HeaderText="Last Accessed">
                    <ItemTemplate>
                        <%# LocalizeTime(Eval("ScormAccessedLast")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="First Accessed">
                    <ItemTemplate>
                        <%# LocalizeTime(Eval("ScormAccessedFirst")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Learner">
                    <ItemTemplate>
                        <%# Eval("LearnerName") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="SCO">
                    <ItemTemplate>
                        <%# Eval("ScormPackageHook") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Launches" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# Eval("ScormLaunchCount") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Completion">
                    <ItemTemplate>
                        <%# Eval("ScormRegistrationCompletion") %>
                        <%# LocalizeTime(Eval("ScormCompleted")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Grade">
                    <ItemTemplate>
                        <%# Eval("ScormRegistrationSuccess") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Score" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%# Eval("ScormRegistrationScore", "{0:p0}") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Time Tracked" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%# Eval("ScormRegistrationTrackedSeconds", "{0:n0} seconds") %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>

        </insite:Grid>

    </div>

</div>