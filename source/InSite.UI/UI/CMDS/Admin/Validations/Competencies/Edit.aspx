<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Cmds.Actions.Talent.Employee.Competency.Admin.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/CompetencyAchievementAndDownloadViewer.ascx" TagName="CompetencyAchievementAndDownloadViewer" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Competency" />

    <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

        <insite:NavItem runat="server" ID="CompetencySection" Title="Competency" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
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
                                Old Competency #
                            </label>
                            <div>
                                <asp:Literal ID="NumberOld" runat="server" />
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
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="AchievementPanel" Title="Achievements and Downloads" Icon="far fa-trophy" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Achievements and Downloads
                </h2>

                <uc:CompetencyAchievementAndDownloadViewer ID="CompetencyAchievements" runat="server" />
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="DetailSection" Title="Knowledge and Skills" Icon="far fa-lightbulb-on" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
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
        </insite:NavItem>
        <insite:NavItem runat="server" ID="StatusSection" Title="Status" Icon="far fa-balance-scale" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Status
                </h2>

                <div class="card border-0 shadow-lg mb-3">
                    <div class="card-body">
                        <h3>Self Assessment</h3>
                        <div class="form-group mb-3">
                            <div>
                                <p><strong><asp:Literal ID="SelfAssessmentQuestion" runat="server" /></strong></p>
                                <div><cmds:SelfAssessmentStatusInput ID="SelfAssessmentStatus" runat="server" /></div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card border-0 shadow-lg mb-3">
                    <div class="card-body">

                        <h3>Validation</h3>
                        <div class="form-group mb-3">
                            <div>
                                <p><strong><asp:Literal ID="Literal1" runat="server" /></strong></p>
                                <div><cmds:SelfAssessmentStatusInput ID="SelfAssessmentStatusInput1" runat="server" /></div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <h3>Self Assessment</h3>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Validation Status
                                <insite:RequiredValidator runat="server" ControlToValidate="ValidationStatus" FieldName="Validation Status" />
                            </label>
                            <div>
                                <asp:Literal ID="ValidationStatusLabel" runat="server" />
                                <cmds:CompetencyStatusSelector ID="ValidationStatus" runat="server" CssClass="d-inline-block w-25" />
                                <asp:CheckBox runat="server" ID="IsValidated" Text="Validated" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Last Self-Assessed
                            </label>
                            <div>
                                <asp:Literal ID="SelfAssessmentDate" runat="server" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Last Validated
                            </label>
                            <div>
                                <asp:Literal ID="ValidationDate" runat="server" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Expiration Date
                            </label>
                            <div>
                                <asp:Literal ID="ExpirationDate" runat="server" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div runat="server" id="ValidatorField" class="form-group mb-3">
                            <label class="form-label">
                                Validator
                            </label>
                            <div>
                                <asp:Literal ID="ValidatorName" runat="server" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Validator Comment
                            </label>
                            <div>
                                <table>
                                <asp:Repeater ID="PredefinedComments" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-nowrap align-top" style="padding-bottom:10px;">
                                                <insite:Button runat="server" ButtonStyle="Success" Icon="fas fa-plus-circle" Text="Add Comment" OnClientClick='<%# string.Format(@"addComment(""{0}""); return false;", Container.DataItem) %>' />:
                                            </td>
                                            <td style="padding-bottom:10px;">
                                                <%# Container.DataItem %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                </table>
                                <asp:Literal ID="ValidationCommentLabel" runat="server" />
                                <asp:TextBox ID="ValidationComment" runat="server" Width="97%" Rows="10" TextMode="MultiLine" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                    </div>
                </div>

            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="StatusHistorySection" Title="Competency Status History" Icon="far fa-history" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competency Status History
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                    <table>
                    <asp:Repeater runat="server" ID="StatusHistory">
                        <ItemTemplate>
                        <tr>
                            <td style="width:200px;"><%# GetStatusDate(Eval("ChangePosted")) %>:</td>
                            <td>
                                <%# Eval("ChangeStatus") %> <%# GetUserFullName(Eval("ValidatorName")) %>
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

    <div class="row mb-3">
        <div class="col-lg-8">
            <insite:Button ID="PrevButton" runat="server" Icon="fas fa-arrow-alt-left" Text="Previous" TabIndex="3" ButtonStyle="OutlinePrimary" DisableAfterClick="true" />
            <insite:Button ID="NextButton" runat="server" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" Text="Next" TabIndex="4" ButtonStyle="OutlinePrimary" DisableAfterClick="true" />
        </div>
        <div class="col-lg-4 text-end">
            <insite:CancelButton ID="FindButton" runat="server" TabIndex="5" NavigateUrl="/ui/cmds/portal/validations/competencies/search" />
        </div>
    </div>

    <div class="float-end">
        <asp:Literal ID="Position" runat="server" />
    </div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        function addComment(text) {
            const input = $get("<%= ValidationComment.ClientID %>");

            if (input.value == "")
                input.value = text;
            else
                input.value += "\n" + text;
        }
    </script>
</insite:PageFooterContent>
</asp:Content>
