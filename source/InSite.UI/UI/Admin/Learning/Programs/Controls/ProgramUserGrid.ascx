<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramUserGrid.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.ProgramUserGrid" %>

<div runat="server" id="FilterSection">

    <div class="row mt-4 mb-4">
        <div class="col-lg-6">
            <insite:Button runat="server" ID="AddButton" Text="Add Learners" Icon="fas fa-plus-circle" ButtonStyle="Success" />
            <insite:DropDownButton runat="server" ID="DownloadDropDown" />
            <insite:Button runat="server" ID="RefreshButton" Text="Refresh" Icon="fas fa-redo" ButtonStyle="Default" />
        </div>
        <asp:Panel runat="server" DefaultButton="SearchInput" CssClass="col-lg-6">
            <insite:InputSearch runat="server" ID="SearchInput" />
        </asp:Panel>
    </div>

</div>

<div class="row">
    <div class="col-lg-12">

        <div class="card border-0 shadow-lg">
            <div class="card-body">

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

    </div>
</div>
