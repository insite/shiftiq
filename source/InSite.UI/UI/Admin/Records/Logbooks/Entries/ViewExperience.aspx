<%@ Page Language="C#" CodeBehind="ViewExperience.aspx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Entries.ViewExperience" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	
<%@ Register Src="~/UI/Admin/Records/Logbooks/Controls/LogbookDetailsV2.ascx" TagName="LogbookDetail" TagPrefix="uc" %>	
<%@ Register TagPrefix="uc" TagName="LogEntryDetail" Src="Controls/LogEntryDetail.ascx" %>
<%@ Register TagPrefix="uc" TagName="CompetencyGrid" Src="Controls/LogEntryCompetencyGrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="CommentList" Src="Controls/LogEntryComments.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="LearnerLogbookPanel" Title="Logged Entry" Icon="far fa-book-open" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Logged Entry
                </h2>

                <div class="row">
                    <div class="col-md-6">
                        <div class="card border-0 shadow-lg mb-3">
                            <div class="card-body">
                                <h3>Learner</h3>
                                <uc:PersonDetail runat="server" ID="PersonDetail" />
                            </div>
                        </div>

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <h3>Status</h3>
                                <div class="float-end">
                                    <insite:Button runat="server" ID="ValidateButton" class="btn btn-default" Text="Validate" Icon="fas fa-question-circle" ButtonStyle="Success" />
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Validation Status
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="ValidationStatus" />
                                    </div>
                                </div>

                                <div class="form-group mb-3" runat="server" id="SkillRatingDiv">
                                    <label class="form-label">
                                        Skill Rating
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="SkillRating" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-6 ">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <h3>Logbook Details</h3>
                                <uc:LogbookDetail runat="server" ID="LogbookDetail" />
                            </div>
                        </div>
                    </div>
                </div>

            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="LogbookPanel" Title="Entry" Icon="far fa-pencil-ruler" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Entry
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <uc:LogEntryDetail runat="server" ID="Detail" />
                        </div>
                    </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CompetenciesPanel" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competencies
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                    <uc:CompetencyGrid runat="server" ID="Competencies" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CommentsPanel" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Comments
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:CommentList runat="server" ID="Comments" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
