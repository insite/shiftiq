<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Cmds.Actions.Talent.Employee.Competency.Assessment.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/CompetencyAchievementAndDownloadViewer.ascx" TagName="CompetencyAchievementAndDownloadViewer" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Competency" />

    <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

        <insite:NavItem runat="server" ID="MainSection" Title="Self-Assessment" Icon="far fa-balance-scale" IconPosition="BeforeText">
            <section class="mb-3">
                <h2 class="h4 mt-4 mb-3">
                    <i class="far fa-ruler-triangle me-1"></i>
                    Competency
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Number
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="Number" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Summary
                            </label>
                            <div>
                                <asp:Literal ID="Summary" runat="server" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Profiles
                            </label>
                            <div>
                                <asp:Repeater ID="Profiles" runat="server">
                                    <ItemTemplate>
                                        <%# Eval("ProfileTitle") %>
                                    </ItemTemplate>
                                    <SeparatorTemplate>, </SeparatorTemplate>
                                </asp:Repeater>
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Category
                            </label>
                            <div>
                                <asp:Literal ID="Category" runat="server" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Current Status
                            </label>
                            <div>
                                <asp:Literal ID="StatusLabel" runat="server" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                    </div>
                </div>
            </section>

            <section runat="server" id="CompetencyAchievementsSection" class="mb-3">
                <h2 class="h4 mt-4 mb-3">
                    <i class="far fa-trophy me-1"></i>
                    Achievements and Downloads
                </h2>

                <uc:CompetencyAchievementAndDownloadViewer ID="CompetencyAchievements" runat="server" />
            </section>

            <section class="mb-3">
                <h2 class="h4 mt-4 mb-3">
                    <i class="far fa-lightbulb-on me-1"></i>
                    Knowledge and Skills
                </h2>

                <div class="card border-0 shadow-lg mb-3">
                    <div class="card-body">
                        <h3>Knowledge</h3>
                        <div class="form-group mb-3">
                            <div>
                                <asp:Literal ID="Knowledge" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <h3>Skills</h3>
                        <div class="form-group mb-3">
                            <div>
                                <asp:Literal ID="Skills" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>

            </section>

            <section>
                <h2 class="h4 mt-4 mb-3">
                    <i class="far fa-balance-scale me-1"></i>
                    Self Assessment
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <asp:LinkButton ID="FocusButton" runat="server" TabIndex="1" OnClientClick="return false;" style="outline:none;" />
        
                        <p id="SelfAssessmentAllowedStatusPanel" runat="server"><strong><asp:Literal ID="SelfAssessmentAllowedStatus" runat="server" /></strong></p>

                        <asp:Panel ID="SelfAssessmentPanel" runat="server">
                            <p><strong><asp:Literal ID="SelfAssessmentQuestion" runat="server" /></strong></p>
                            <div><cmds:SelfAssessmentStatusInput ID="SelfAssessmentStatus" runat="server" TabIndex="2" /></div>
                        </asp:Panel>
        
                        <p id="WarningPanel" runat="server" class="warning_panel" visible="false">
                            <div class="alert alert-warning">
                                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                                <strong>You have not yet acquired a profile at the level required to self-assess on this competency.</strong>
                            </div>
                        </p>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="StatusHistorySection" Title="Competency Status History" Icon="far fa-history" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competency Status History
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <table class="table table-striped">
                        <asp:Repeater runat="server" ID="StatusHistory">
                            <ItemTemplate>
                            <tr>
                                <td class="text-nowrap align-top" style="width: 150px;"><%# GetStatusDate(Eval("ChangePosted")) %></td>
                                <td>
                                    <%# Eval("ChangeStatus") %>
                                    <%# GetUserFullName(Eval("ValidatorName")) %>
                                    <span class="form-text"><%# Eval("ChangeComment") == DBNull.Value ? "" : "<br />" + (String)Eval("ChangeComment") %></span>
                                </td>
                            </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        </table>
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <p>
        <strong>WARNING!</strong> After you save your changes, this competency will no longer be considered Validated until you re-submit it for validation (and it is re-validated).
    </p>

    <div class="text-end">
        Click here if you are ready to submit your self assessments:
    </div>

    <div class="row">
        <div class="col-lg-8">
            <insite:Button ID="PrevButton" runat="server" Icon="fas fa-arrow-alt-left" Text="Previous" TabIndex="3" ButtonStyle="OutlinePrimary" DisableAfterClick="true" />
            <insite:Button ID="NextButton" runat="server" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" Text="Next" TabIndex="4" ButtonStyle="OutlinePrimary" DisableAfterClick="true" />
        </div>
        <div class="col-lg-4 text-end">
            <insite:Button ID="SubmitForValidation" runat="server" Icon="fas fa-cloud-upload" Text="Submit for Validation" ButtonStyle="Success" TabIndex="5" DisableAfterClick="true" />
            <insite:CancelButton ID="FindButton" runat="server" TabIndex="6" NavigateUrl="/ui/cmds/portal/validations/competencies/search" />
        </div>
    </div>

    <div class="float-end">
        <asp:Literal ID="Position" runat="server" />
    </div>

</asp:Content>
