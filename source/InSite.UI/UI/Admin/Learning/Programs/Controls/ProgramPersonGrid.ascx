<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramPersonGrid.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.ProgramPersonGrid" %>

<div class="card mt-3">
    <div class="card-body">

        <div class="d-flex justify-content-between align-items-top">
            <h3>People</h3>
            <div>
                <insite:DropDownButton runat="server" ID="DownloadDropDown" />
                <insite:Button runat="server" ID="RefreshButton" Text="Refresh" Icon="fas fa-redo" ButtonStyle="Default" />
            </div>
        </div>

        <insite:Grid runat="server" ID="Grid">
            <Columns>

                <insite:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false" FieldName="ActionColumn">
                    <ItemTemplate>
                        <%# Eval("DeleteEnrollmentLink") %>
                        <%# Eval("EnrollmentToolTip") %>
                    </ItemTemplate>
                </insite:TemplateField>
                        
                <asp:TemplateField HeaderText="Program">
                    <ItemTemplate>
                        <a href='/ui/admin/learning/programs/outline?<%# Eval("ProgramIdentifier", "id={0}") %>'><%# Eval("ProgramName") %></a>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Learner">
                    <ItemTemplate>
                        <%# Eval("LearnerNameLink") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <%# Eval("UserEmail", "<a href='mailto:{0}'>{0}</a>") %>
                        <span class="form-text"><%# Eval("UserEmailAlternate", "<a href='mailto:{0}'>{0}</a>") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Phone">
                    <ItemTemplate>
                        <%# Eval("UserPhone") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Assigned">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("ProgressAssigned")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Completed">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("ProgressCompleted")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Time Taken">
                    <ItemTemplate>
                        <%# Eval("DaysTaken") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Progress" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%# Eval("CompletionCounter") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Completion" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%# Eval("CompletionPercent") %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>

    </div>
</div>