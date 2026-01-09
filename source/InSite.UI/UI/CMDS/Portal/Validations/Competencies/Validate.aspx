<%@ Page CodeBehind="Validate.aspx.cs" Inherits="InSite.Cmds.User.Competencies.Forms.Validate" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/CompetencyAchievementAndDownloadViewer.ascx" TagName="CompetencyAchievementAndDownloadViewer" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <style>
        a.validation { min-width: 70px; background-color: white; color: #343434 !important; }
        a.validation.disabled { background-color: white; }
        a.validation-selected { min-width: 70px; }
        div.knowledge-and-skills h1 { font-size: 22px; }
        table.status-history tr td { padding-bottom: 10px; }
        table.validator-comment tr td { padding-bottom: 0px; }
    </style>

    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Competency" />

    <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

        <insite:NavItem runat="server" ID="CompetencySection" Title="Competency" Icon="far fa-balance-scale" IconPosition="BeforeText">
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
                        <asp:Literal ID="Knowledge" runat="server" />
                    </div>
                </div>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <h3>Skills</h3>
                        <asp:Literal ID="Skills" runat="server" />
                    </div>
                </div>
            </section>

            <section class="mb-3">
                <h2 class="h4 mt-4 mb-3">
                    <i class="far fa-balance-scale me-1"></i>
                    Validation
                </h2>

                <div class="card border-0 shadow-lg mb-3">
                    <div class="card-body">
                        <h3>Self Assessment</h3>
                        <p><strong><asp:Literal ID="SelfAssessmentQuestion" runat="server" /></strong></p>
                        <div><cmds:SelfAssessmentStatusInput ID="SelfAssessmentStatus" runat="server" Enabled="false" /></div>
                    </div>
                </div>

                <asp:Panel ID="ValidationPanel" runat="server" CssClass="mb-3">

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <h3>Validation</h3>
                            <p><strong>Do you agree with the candidate's self assessment?</strong></p>
        
                            <div class="mb-3">
                                <asp:RadioButtonList ID="ValidatorSelection" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" style="display:none;">
                                    <asp:ListItem Value="Yes" />
                                    <asp:ListItem Value="No" />
                                </asp:RadioButtonList>
            
                                <insite:Button ID="YesButton" runat="server" Icon="fas fa-check" Text="Yes" ButtonStyle="Success" OnClientClick="return selectButton(0);" />
                                &nbsp;
                                <insite:Button ID="NoButton" runat="server" Icon="fas fa-times" Text="No" ButtonStyle="Danger" OnClientClick="return selectButton(1);" />
                            </div>

                            <div runat="server" id="ValidatorComments" style="display:none;">

                                <h3>Validator Comments</h3>
                                <table class="validator-comment mb-3">
                                <asp:Repeater ID="PredefinedComments" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="pe-3 pb-1 text-nowrap align-top" style="padding-right: 10px;">
                                            <insite:Button runat="server" ButtonStyle="Default" Icon="far fa-plus-circle" Text="Add Comment" OnClientClick='<%# string.Format(@"addComment(""{0}""); return false;", Container.DataItem) %>' />
                                        </td>
                                        <td><%# Container.DataItem %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                </table>
        
                                <insite:TextBox ID="ValidatorComment" runat="server" CssClass="w-75 d-inline" Rows="10" TextMode="MultiLine" />
                                <insite:RequiredValidator ID="ValidatorCommentRequired" runat="server" Enabled="false" ControlToValidate="ValidatorComment" ValidationGroup="Competency" FieldName="Validator Comment" />
                            </div>
                        </div>
                    </div>

                    <p id="ValidatorCommentsReminder" runat="server" style="display:none;">
                        <strong>REMINDER</strong>: You must enter a comment in order to finish your validation of this competency.
                    </p>

                </asp:Panel>

                <div runat="server" id="NoValidationPanel" Visible="false" class="card border-0 shadow-lg">
                    <div class="card-body">
                        <h3>Validation Access Denied</h3>
                        <p>
                            You do not have permission to validate this competency because
                            <strong><asp:Literal ID="NoValidationReason" runat="server" />.</strong>
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
                        <table class="status-history">
                        <asp:Repeater runat="server" ID="StatusHistory">
                            <ItemTemplate>
                            <tr>
                                <td class="pe-3 pb-1 text-nowrap align-top">
                                    <%# GetStatusDate(Eval("ChangePosted")) %>
                                </td>
                                <td>
                                    <%# Eval("ChangeStatus") %> 
                                    <span class="form-text">
                                        <%# GetUserFullName(Eval("ValidatorName")) %>
                                    </span>
                                    <div class="form-text">
                                        <%# Eval("ChangeComment") %>
                                    </div>
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

    <insite:Button ID="PrevButton" runat="server" Icon="fas fa-arrow-alt-left" Text="Previous" TabIndex="3" ButtonStyle="OutlinePrimary" DisableAfterClick="true" ValidationGroup="Competency" />
    <insite:Button ID="NextButton" runat="server" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" Text="Next" TabIndex="4" ButtonStyle="OutlinePrimary" DisableAfterClick="true" ValidationGroup="Competency" />
    <insite:CancelButton ID="FindButton" runat="server" NavigateUrl="/ui/cmds/design/validations/competencies/search" />

    <div class="float-end">
        <asp:Literal ID="Position" runat="server" />
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            function addComment(text) {
                const input = $get("<%= ValidatorComment.ClientID %>");

                if (input.value == "")
                    input.value = text;
                else
                    input.value += "\n" + text;
            }
    
            function selectButton(index) {
                const selector = $get("<%= ValidatorSelection.ClientID %>");
                const list = selector.getElementsByTagName("input");

                list[index].checked = true;

                const yesButton = document.getElementById("<%= YesButton.ClientID %>");
                const noButton = document.getElementById("<%= NoButton.ClientID %>");

                switch (index) {
                    case 0:
                        yesButton.classList.replace("btn-default", "btn-success");
                        noButton.classList.replace("btn-success", "btn-default");
                        break;
                    case 1:
                        yesButton.classList.replace("btn-success", "btn-default");
                        noButton.classList.replace("btn-default", "btn-success");
                        break;
                }

                $get("<%= ValidatorComments.ClientID %>").style.display = "";
                $get("<%= ValidatorCommentsReminder.ClientID %>").style.display = "";

                ValidatorEnable($get("<%= ValidatorCommentRequired.ClientID %>"), true);

                return false;
            }
        </script>
    </insite:PageFooterContent>
</asp:Content>
