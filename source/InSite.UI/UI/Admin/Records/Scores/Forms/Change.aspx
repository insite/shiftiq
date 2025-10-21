<%@ Page Language="C#" CodeBehind="Change.aspx.cs" Inherits="InSite.Admin.Records.Scores.Forms.Change" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Records/Gradebooks/Controls/GradebookInfo.ascx" TagName="GradebookDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section runat="server" ID="GradebookPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ballot-check me-1"></i>
            Score
        </h2>


        <div class="row">
                        
            <div class="col-md-4">                                    
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Gradebook</h3>
                        <uc:GradebookDetail runat="server" ID="GradebookDetails" />

                    </div>
                </div>
            </div>
            
            <div class="col-md-4">
                            
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Score Item</h3>

                        <div class="form-group mb-3" runat="server">
                            <label class="form-label">
                                Name
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="ItemName" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server">
                            <label class="form-label">
                                Type
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="ScoreType" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="OptionField">
                            <label class="form-label">
                                <asp:Literal runat="server" ID="OptionName" />
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="OptionValue" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>

            <div class="col-md-4">
                            
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Student</h3>

                        <div class="form-group mb-3" runat="server">
                            <div>
                                <asp:Literal runat="server" ID="StudentFullName" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server">
                            <label class="form-label">
                                Ignore Score in Calculations
                            </label>
                            <div>
                                <insite:RadioButton runat="server" ID="IsIgnoredYes" Text="Yes" GroupName="IsIgnored" />
                                <insite:RadioButton runat="server" ID="IsIgnoredNo" Text="No" GroupName="IsIgnored" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="GradeField">
                            <label class="form-label">
                                Grade
                            </label>
                            <div runat="server" id="ScorePercentField">
                                <insite:NumericBox runat="server" ID="ScorePercent" DecimalPlaces="1" MinValue="0" MaxValue="100" Width="80" CssClass="d-inline text-end" /> %
                            </div>
                            <div runat="server" id="ScorePointField">
                                <insite:NumericBox runat="server" ID="ScorePoint" DecimalPlaces="2" MinValue="0" MaxValue="100000" Width="80" CssClass="d-inline text-end" />
                                <asp:Literal runat="server" ID="OutOfLiteral" />
                            </div>
                            <div runat="server" id="ScoreTextField">
                                <insite:TextBox runat="server" ID="ScoreText" MaxLength="100" />
                            </div>
                            <div runat="server" id="ScoreNumberField">
                                <insite:NumericBox runat="server" ID="ScoreNumber" DecimalPlaces="1" Width="80" CssClass="text-end" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="DateGradedField">
                            <label class="form-label">
                                Date Graded
                            </label>
                            <div>
                                <insite:DateTimeOffsetSelector runat="server" ID="Graded" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="ProgressStatusField">
                            <label class="form-label">
                                Status
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="ProgressStatus" RepeatDirection="Vertical">
                                    <asp:ListItem Value="Started" Text="Started" />
                                    <asp:ListItem Value="Completed" Text="Completed" />
                                    <asp:ListItem Value="Incomplete" Text="Incomplete" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                        
                        <insite:Container runat="server" ID="ProgressCompletedPanel">
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Completed
                                </label>
                                <div>
                                    <insite:DateTimeOffsetSelector runat="server" ID="ProgressCompleted" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Elapsed Seconds
                                </label>
                                <div>
                                    <insite:NumericBox runat="server" ID="ElapsedSeconds" NumericMode="Integer" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Fail or Pass?
                                </label>
                                <div>
                                    <asp:RadioButtonList runat="server" ID="ProgressFailOrPass">
                                        <asp:ListItem Text="Unknown" />
                                        <asp:ListItem Value="Pass" Text="Pass" />
                                        <asp:ListItem Value="Fail" Text="Fail" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Percent
                                </label>
                                <div>
                                    <insite:NumericBox runat="server" ID="CompletedProgressPercent" DecimalPlaces="1" MinValue="0" MaxValue="100" />
                                </div>
                            </div>
                        </insite:Container>

                        <div class="form-group mb-3" runat="server" id="ProgressStartedField">
                            <label class="form-label">
                                Started
                            </label>
                            <div>
                                <insite:DateTimeOffsetSelector runat="server" ID="ProgressStarted" />
                            </div>
                        </div>

                        <div class="form-group mb-3" runat="server">
                            <label class="form-label">
                                Comment
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="Comment" TextMode="MultiLine" Rows="4" MaxLength="1200" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>

        </div>

    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Score" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>
</asp:Content>
