<%@ Page Language="C#" CodeBehind="ValidateExperience.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.ValidateExperience" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ValidationDetail" Src="Controls/ValidationDetail.ascx" %>
<%@ Register TagPrefix="uc" TagName="CompetencyValidateGrid" Src="Controls/CompetencyValidateGrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="CommentList" Src="Controls/CommentList.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="LogbookPanel" Title="Entry" Icon="far fa-pencil-ruler" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Entry
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:ValidationDetail runat="server" ID="Detail" />

                        <div>
                            <insite:NextButton runat="server" ID="NextButton1" />
                            <insite:CloseButton runat="server" ID="CancelButton1" />
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CompetenciesPanel" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competencies
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:CompetencyValidateGrid runat="server" ID="Competencies" />

                        <div>
                            <insite:NextButton runat="server" ID="NextButton2" />
                            <insite:CloseButton runat="server" ID="CancelButton2" />
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CommentsPanel" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Comments
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                    <uc:CommentList runat="server" ID="Comments" />

                    <div>
                        <insite:NextButton runat="server" ID="NextButton3" />
                        <insite:CloseButton runat="server" ID="CancelButton3" />
                    </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="FinalPanel" Title="Confirm Validation" Icon="far fa-graduation-cap" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Confirm Validation
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Skill Rating
                                    </label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="SkillRating" Width="100px" />
                                    </div>
                                    <div class="form-text">
                                        This selection applies a skill rating to the entire log entry <i>(optional)</i>.
                                    </div>
                                </div>

                            </div>

                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <insite:Button runat="server" ID="ValidateButton" Text="Validate" Icon="fas fa-cloud-upload" ButtonStyle="Success" />
                                <insite:CloseButton runat="server" ID="FinalCancelButton" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
