<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionsManager.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.QuestionsManager" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        #<%= ClientID %>_cmd {
            margin-bottom: 30px;
        }

        #<%= ClientID %> a.delete-page {
            float: right;
            margin-right: 8px;
            display: block;
        }

        #<%= ClientID %> .buttons {
            white-space: nowrap;
        }

        #<%= ClientID %> img {
            max-width: 100%;
        }

        /* Question grid */

        table.question-grid { width:100%; }

        table.question-grid > tbody > tr > td { padding: 20px 0 20px 20px; vertical-align:top; }

        table.question-grid > tbody > tr > td:first-child {
            text-align: center;
            width: 60px;
            border-left: 1px solid transparent;
        }

        table.question-grid > tbody > tr > td:first-child > div.sequence {
            font-size: 27px;
            color: #265F9F;
            white-space: nowrap;
        }

        table.question-grid > tbody > tr > td:nth-child(2) {
            width:100%;
        }

        table.question-grid > tbody > tr > td:last-child {
            padding-right:10px;
        }

        table.question-grid > tbody > tr > td > table { width:100%; }

        table.question-grid > tbody > tr > td > table > tbody > tr > td { vertical-align:top; }

        table.question-grid > tbody > tr > td div.sequence-custom {
            color: #ffffff !important;
            display: block;
            margin-bottom: 10px;
        }

        /* Options */

        table.question-grid .option-grid td {
            vertical-align: top;
            padding: 5px;
            text-align: left;
        }

        table.question-grid .option-grid {
            margin-left: 10px;
        }

        table.question-grid .option-grid td.option-points {
            padding-top: 8px;
            padding-left: 0;
            white-space: nowrap;
        }

        table.question-grid .option-grid tr:first-child td { padding-top:10px; }
        table.question-grid .option-grid tr:first-child td.option-points { padding-top: 13px; }

        tr.extra-option { display:none; }

    </style>
</insite:PageHeadContent>

<div class="row" id="<%= ClientID %>_cmd">
    <div class="d-flex col-lg-12">       
        <div class="col-lg-6 d-flex align-items-center">
            <insite:TextBox runat="server" ID="FilterTextBox" Width="300" CssClass="float-start me-1" EmptyMessage="Question Text" />
            <div class="float-start">
                <insite:ComboBox runat="server" ID="SurveyPage" EmptyMessage="Page" Width="200" />
            </div>           
            <insite:IconButton runat="server" ID="FilterButton" Name="filter" CssClass="float-start ms-1" ToolTip="Filter" />
            <insite:PageFooterContent runat="server">
                <script type="text/javascript"> 
                    (function () {
                        Sys.Application.add_load(function () {
                            $('#<%= FilterTextBox.ClientID %>')
                                .off('keydown', onKeyDown)
                                .on('keydown', onKeyDown);
                        });

                        function onKeyDown(e) {
                            if (e.which === 13) {
                                e.preventDefault();
                                $('#<%= FilterButton.ClientID %>')[0].click();
                            }
                        }
                    })();
                </script>
            </insite:PageFooterContent>
        </div>           
        <div class="col-lg-6">
            <div class="float-end">
                <div class="btn-group add-button" runat="server" id="AddOptionList">
                    <insite:Button runat="server" ID="AddNewQuestion" ButtonStyle="Default" ToolTip="Add question" Icon="fas fa-plus-circle"
                        Text="Add Question" />
                </div>

                <div class="btn-group reorder ms-1" runat="server" id="ReorderField">
                    <insite:Button runat="server" ID="ReorderQuestions" Text="Reorder" Icon="fas fa-sort" ButtonStyle="Default" />
                </div>
            
                <div class="btn-group ms-1">
                    <insite:Button runat="server" ID="DownloadPdfButton" ButtonStyle="Primary" ToolTip="Download PDF" Text="Download" Icon="fas fa-download" />
                </div>
            </div>
        </div>           
    </div>
</div>

<div class="row">
    <div class="col-md-12 col-sm-9 col-xs-8">
        <asp:Repeater runat="server" ID="QuestionRepeater">
            <HeaderTemplate>
                <table id='<%# QuestionRepeater.ClientID %>' class="table question-grid"><tbody>
            </HeaderTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
            <ItemTemplate>
                <tr>                   
                    <td style="width:40px; vertical-align:top;">
                        
                        <h2 style="margin-top:0px;margin-bottom:20px;"><asp:Literal runat="server" ID="SequenceLabel" /></h2>

                        <div style="margin-bottom:5px;">
                            <insite:IconLink runat="server"
                                Visible='<%# CanEdit && (Shift.Constant.SurveyQuestionType)Eval("Type") != Shift.Constant.SurveyQuestionType.BreakPage %>'
                                ToolTip="Edit Question"
                                NavigateUrl='<%# string.Format("/ui/admin/workflow/forms/questions/change?form={0}&question={1}", SurveyID, Eval("Identifier")) %>'
                                CssClass="reorder-trigger reorder-hide"
                                Name="pencil" />
                        </div>
                        <div style="margin-bottom:5px;">
                            <insite:IconLink runat="server"
                                Visible='<%# CanEdit && (Shift.Constant.SurveyQuestionType)Eval("Type") == Shift.Constant.SurveyQuestionType.Likert %>'
                                ToolTip="Define Likert Scales"
                                NavigateUrl='<%# string.Format("/ui/admin/workflow/forms/questions/define-likert-scales?form={0}&question={1}", SurveyID, Eval("Identifier")) %>'
                                CssClass="reorder-trigger reorder-hide"
                                Name="weight" />
                        </div>
                        <div style="margin-bottom:5px;">
                            <insite:IconButton runat="server" ID="CopyQuestion"
                                Visible="<%# CanEdit %>"
                                Type="Regular" CssClass="reorder-trigger reorder-hide"
                                ToolTip="Duplicate Question" 
                                Name="copy"
                                OnClientClick="return confirm('Are you sure you want to create a copy of this question?');"
                                CommandName="Copy"
                                />
                        </div>
                        <div style="margin-bottom:5px;">
                            <insite:IconLink runat="server" ID="DeleteQuestion"
                                Visible="<%# CanEdit %>"
                                ToolTip="Delete Question"
                                NavigateUrl='<%# string.Format("/ui/admin/workflow/forms/questions/delete?form={0}&question={1}", SurveyID, Eval("Identifier")) %>'
                                CssClass="reorder-trigger reorder-hide"
                                Name="trash-alt"
                                />
                        </div>
                    </td>
                    <td>
                        <div runat="server" id="AddConditionPanel" class="float-end">
                            <div class="dropdown">
                                <button runat="server" visible="<%# CanEdit %>" type="button" class="btn btn-default btn-sm dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class='fas fa-plus-circle'></i>
                                    <span class="text">Add</span>
                                    <span class="caret"></span>
                                </button>
                                <ul class="dropdown-menu">
                                    <li>
                                        <asp:HyperLink runat="server"
                                            CssClass="dropdown-item"
                                            ID="AddNewCondition"
                                            ToolTip="Add a new condition"
                                            NavigateUrl='<%# string.Format("/ui/admin/workflow/forms/conditions/add?form={0}&question={1}&returnpanel=questions", Eval("Form.Identifier"), Eval("Identifier")) %>'
                                            Text="Condition"
                                        >
                                            <i class="fas fa-code-branch"></i>
                                            Condition
                                        </asp:HyperLink>
                                    </li>
                                </ul>
                            </div>
                        </div>

                        <div style="margin-bottom:20px;">
                            <div class="lesson" style="margin-top:0px; margin-bottom:5px;">
                                <%# GetTitle((Guid)Eval("Identifier")) %>
                            </div>
                            <div style="margin-top:10px;">
                                <asp:Literal runat="server" ID="SurveyQuestionType" />
                            </div>
                        </div>

                        <asp:Panel runat="server" id="OptionListPanel">
                            <table>
                            <asp:Repeater runat="server" ID="OptionListRepeater">
                                <ItemTemplate>
                                    <tr>
                                        <td style="width:40px; vertical-align:top;">
                                            <div runat="server" style="margin-bottom: 10px;" visible='<%# Eval("IsLikert") %>'>
                                                <span class="badge bg-custom-default"><%# Eval("Sequence") %></span>
                                            </div>
                                            <insite:IconLink runat="server" ID="AddNewBranch"
                                                ToolTip="Change branch"
                                                Visible='<%# (bool)Eval("ListEnableBranch") && CanEdit %>'
                                                NavigateUrl='<%# Eval("Identifier", "/ui/admin/workflow/forms/change-branch?list={0}&returnpanel=questions") %>'
                                                Name="forward" />
                                        </td>
                                        <td>
                                            <strong><%# Eval("TitleHtml") %></strong>
                                        
                                            <asp:Repeater runat="server" ID="OptionItemRepeater">
                                                <HeaderTemplate><table class="table table-striped"></HeaderTemplate>
                                                <FooterTemplate></table></FooterTemplate>
                                                <ItemTemplate>
                                                    <tr class='<%# Container.ItemIndex >= MaxOptionCount ? "extra-option" : "" %>'>

                                                        <td style="width:40px"><%# Eval("Letter") %>.</td>

                                                        <td><%# Eval("TitleHtml") %></td>

                                                        <td style="width:120px" class="form-text option-points">
                                                            <%# Eval("PointsText") %>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>

                                            <a runat="server" id="ShowExtraOptions" class="show-extra-options" href="#">
                                                Show more options
                                            </a>
                                            <a class="hide-extra-options d-none" href="#">
                                                Show less options
                                            </a>

                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            </table>
                        </asp:Panel> 

                    </td>

                </tr>
            </ItemTemplate>
        </asp:Repeater>
    
        <div class="btn-group add-button" runat="server" id="Div1">
            <insite:Button runat="server" ID="AddNewQuestionBottom" ButtonStyle="Default" ToolTip="Add question" Icon="fas fa-plus-circle" Text="Add Question" />
        </div>
    </div>
</div>



<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            const instance = window.questionsManager = window.questionsManager || {};

            document
                .querySelectorAll(".show-extra-options")
                .forEach(a => a.addEventListener("click", showExtraOptions));

            document
                .querySelectorAll(".hide-extra-options")
                .forEach(a => a.addEventListener("click", hideExtraOptions));

            instance.scrollToQuestion = function (index) {
                var $rows = $('table#<%= QuestionRepeater.ClientID %> > tbody > tr');
                if (index < 0 || index >= $rows.length)
                    return;

                var $row = $rows.eq(index);
                var headerHeight = $('header.navbar:first').outerHeight();
                var scrollTo = $row.offset().top - headerHeight;

                if (isNaN(scrollTo) || scrollTo < 0)
                    return;

                $('html, body').scrollTop(scrollTo);
            };

            function showExtraOptions(e) {
                e.preventDefault();

                e.target.parentNode
                    .querySelectorAll(".extra-option")
                    .forEach(tr => tr.style.display = "table-row");

                e.target.parentNode
                    .querySelector(".hide-extra-options")
                    .classList
                    .remove("d-none");

                e.target.classList.add("d-none");
            }

            function hideExtraOptions(e) {
                e.preventDefault();

                e.target.parentNode
                    .querySelectorAll(".extra-option")
                    .forEach(tr => tr.style.display = "");

                e.target.parentNode
                    .querySelector(".show-extra-options")
                    .classList
                    .remove("d-none");

                e.target.classList.add("d-none");
            }
        })();
    </script>
</insite:PageFooterContent>