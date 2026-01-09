<%@ Page Language="C#" CodeBehind="Assign.aspx.cs" Inherits="InSite.Cmds.Admin.Records.Programs.Assign" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Assign" />

    <asp:CustomValidator ID="EmployeeRequired" runat="server" Display="None" ValidationGroup="Assign" ErrorMessage="There are no learners selected." />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="Step1" Title="Learners" Icon="far fa-users" IconPosition="BeforeText">

            <section>

                <h2 class="h4 mt-4 mb-3">Select Learners</h2>

                        <div class="card border-0 shadow-lg">

                            <div class="card-body">

                                <div class="row">

                                    <div class="col-6">

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Department
                                                <insite:RequiredValidator runat="server" ControlToValidate="DepartmentIdentifier" FieldName="Department" ValidationGroup="Assign" />
                                            </label>
                                            <div>
                                                <div>
                                                    <cmds:FindDepartment ID="DepartmentIdentifier" runat="server" />
                                                </div>
                                            </div>
                                            <div class="form-text"></div>
                                        </div>

                                        <div runat="server" id="TemplateField" class="form-group mb-3" visible="false">
                                            <label class="form-label">
                                                Program
                                                <insite:RequiredValidator runat="server" ControlToValidate="ProgramIdentifier" FieldName="Program" ValidationGroup="Assign" />
                                            </label>
                                            <div>
                                                <insite:FindAchievementList runat="server" ID="ProgramIdentifier" />
                                            </div>
                                            <div class="form-text"></div>
                                        </div>

                                        <div runat="server" id="LearnersPanel" visible="false" class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:Button ID="SelectAllLearnersButton" runat="server" Text="Select All" Icon="far fa-check-square" ButtonStyle="Link" />
                                                <insite:Button ID="UnselectAllLearnersButton" runat="server" Text="Deselect All" Icon="far fa-square" ButtonStyle="Link" />
                                            </div>
                                            <label class="form-label">
                                                Learners
                                            </label>
                                            <div>

                                                <div id="NoLearnersPanel" runat="server"><strong>No Learners</strong></div>

                                                <asp:Repeater ID="LearnersRepeater" runat="server">
                                                    <HeaderTemplate>
                                                        <table class="table table-striped">
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td>
                                                                <insite:CheckBox ID="IsSelected" runat="server" Checked="true" Text='<%# Eval("FullName") %>' />
                                                                <asp:Literal ID="UserIdentifier" runat="server" Visible="false" Text='<%# Eval("UserIdentifier") %>' />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>

                                            </div>
                                        </div>

                                    </div>

                                    <div class="col-6">

                                        <div class="form-group mb-3">

                                            <label class="form-label">How to handle previously assigned achievements</label>

                                            <div class="alert alert-info fs-sm" role="alert">
                                                <p>
                                                    When assigning a new program, the system automatically applies the settings 
defined within the program to <strong>all achievements included in that 
program</strong>.
   
                                                </p>
                                                <p>
                                                    If the learner already has those achievements assigned 
with different settings, then <strong>the program's settings will override 
the existing ones</strong>.
                                                </p>
                                                <p>
                                                    For <strong>achievements outside the program 
assigned previously</strong>, select one of these options to 
determine how they should be handled.
                                                </p>
                                            </div>

                                            <insite:RadioButton runat="server" ID="AssignStrategy_NoChange" GroupName="ModifyStrategy" Text="<strong>Do nothing</strong>. No changes will be made to any previously assigned achievements. Their existing Planned and Required statuses will remain as-is. <small>(Use this if you want to preserve each learner's current setup without overriding anything.)</small>"
                                                Checked="true" />

                                            <insite:RadioButton runat="server" ID="AssignStrategy_Unplan" GroupName="ModifyStrategy" Text="<strong>Make all unplanned and optional</strong>. All previously assigned achievements will be set to Planned = No and Required = No. <small>(Use this if you want to remove any previous assignments from the learner's plan and mark them as not required.)</small>" />

                                            <insite:RadioButton runat="server" ID="AssignStrategy_Delete" GroupName="ModifyStrategy" Text="<strong>Delete all previously assigned achievements (outside the program)</strong>. All existing achievements outside this program will be removed from the learner's record. Only the achievements included in the new program will remain. <small>(This action cannot be undone.)</small>" />

                                        </div>

                                    </div>

                                </div>

                                <insite:NextButton runat="server" ID="Step1NextButton" ValidationGroup="Assign" />

                            </div>
                        </div>

            </section>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="Step2" Title="Pending Changes" Icon="far fa-users" IconPosition="BeforeText" Visible="false">

            <section>

                <div class="d-flex justify-content-between mt-4 mb-3">
                    <h2 class="h4">Pending Changes</h2>
                    <div>
                        
                    </div>
                </div>
                

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div>

                            <p>
                                Review these pending changes to your learners' training plans. 
                                Verify everything is correct, then click Save to apply the changes. 
                                See the legend below to understand the Change column indicators.
                            </p>

                            <dl class="row">
                                <dt class="col-sm-2 text-end"><span class='badge bg-success'><i class='fa-solid fa-plus me-1'></i>New</span></dt>
                                <dd class="col-sm-10">A program achievement that is new to the learner</dd>
                                <dt class="col-sm-2 text-end"><span class='badge bg-info'><i class='fa-solid fa-edit me-1'></i>Update</span></dt>
                                <dd class="col-sm-10">A program achievement that is already assigned to the learner</dd>
                                <dt class="col-sm-2 text-end"><span class='badge bg-warning'><i class='fa-solid fa-plus me-1'></i>Add</span></dt>
                                <dd class="col-sm-10">An achievement outside the program, already assigned to the learner, to be added to their training plan</dd>
                                <dt class="col-sm-2 text-end"><span class='badge bg-danger'><i class='fa-solid fa-times me-1'></i>Unassign</span></dt>
                                <dd class="col-sm-10">An achievement outside the program, already assigned to the learner, to be removed from their training plan</dd>
                                <dt class="col-sm-2 text-end"><span class='badge bg-danger'><i class='fa-solid fa-trash-alt me-1'></i>Delete</span></dt>
                                <dd class="col-sm-10">An achievement outside the program, already assigned to the learner, to be deleted from both their education and training plan</dd>
                            </dl>

                            <div runat="server" id="DownloadAlert" class="alert alert-danger">
                                <span runat="server" id="DownloadHelp">
                                    Before you proceed, you may want to download a copy of the learners' achievements 
                                    that will be modified or deleted, in case you need a snapshot of the information as 
                                    it was prior to this bulk update.
                                </span>
                                <asp:LinkButton runat="server" ID="DownloadButton" CssClass="btn btn-sm btn-light"><i class="fa-solid fa-download ms-1 me-1"></i>Download</asp:LinkButton>
                                <asp:HiddenField runat="server" ID="DownloadFile" />
                            </div>

                        </div>

                        <asp:Repeater ID="LearnerSettings" runat="server">
                            <HeaderTemplate>
                                <table id="LearnerSettingsTable" class="table table-striped">
                                    <tr>
                                        <th>Learner</th>
                                        <th>Achievement</th>
                                        <th class="text-center">Planned</th>
                                        <th class="text-center">Required</th>
                                        <th class="text-center text-nowrap">Time-Sensitive</th>
                                        <th class="text-center">Change</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="text-nowrap">
                                        <%# Eval("FullName") %>
                                    </td>
                                    <td>
                                        <%# Eval("AchievementTitle") %>
                                        <div class="text-muted fs-sm">
                                            <%# Eval("AchievementLabel") %>
                                        </div>
                                    </td>
                                    <td class="text-center">
                                        <insite:CheckBox runat="server" ID="IsPlanned" Checked='<%# Eval("IsPlanned") %>' RenderMode="Input" />
                                    </td>
                                    <td class="text-center">
                                        <insite:CheckBox runat="server" ID="IsRequired" Checked='<%# Eval("IsRequired") %>' RenderMode="Input" />
                                    </td>
                                    <td class="text-center">
                                        <asp:Literal runat="server" Text='<%# Eval("LifetimeMonths") %>' />
                                        <asp:Literal runat="server" Text="months" Visible='<%# Eval("LifetimeMonths") != null %>' />
                                        <asp:Literal runat="server" Text="No" Visible='<%# Eval("LifetimeMonths") == null %>' />
                                    </td>
                                    <td class="text-center text-nowrap">
                                        <asp:Literal runat="server" Text='<%# Eval("ActionHtml") %>' />
                                        <asp:Literal ID="Data" runat="server" Text='<%# Eval("UserIdentifier") + ":" + Eval("AchievementIdentifier") + ":" + Eval("LifetimeMonths") + ":" + Eval("Action") %>' Visible="false" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>

                        <div class="mt-3">
                            <insite:Button runat="server" ID="ConfirmButton" ButtonStyle="Success" Icon="fas fa-cloud-upload" Text="Save" ValidationGroup="Assign" />
                        </div>

                    </div>
                </div>

            </section>

        </insite:NavItem>

    </insite:Nav>

</asp:Content>
