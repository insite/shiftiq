<%@ Page Language="C#" CodeBehind="ValidateExperience.aspx.cs" Inherits="InSite.UI.Admin.Records.Validators.Forms.ValidateExperience" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ValidationDetail" Src="../../Controls/ValidationDetail.ascx" %>
<%@ Register TagPrefix="uc" TagName="CompetencyValidateGrid" Src="../../Controls/CompetencyValidateGrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="CommentList" Src="../Controls/CommentList.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section>
        <div class="row">
            <div class="col-md-3">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>
                            <i class="far fa-walking me-1"></i>
                            Steps
                        </h3>

                        <insite:Nav runat="server"
                            ID="NavPanel"
                            ItemType="Pills"
                            ItemAlignment="Vertical"
                            ContentRendererID="NavContent"
                        >
                            <insite:NavItem runat="server" ID="LogbookPanel" Title="Entry" Icon="far fa-pencil-ruler" IconPosition="BeforeText">
                                <uc:ValidationDetail runat="server" ID="Detail" />

                                <div class="mt-3">
                                    <insite:NextButton runat="server" ID="NextButton1" />
                                    <insite:CloseButton runat="server" ID="CancelButton1" />
                                </div>
                            </insite:NavItem>
                            <insite:NavItem runat="server" ID="CompetenciesPanel" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText" Visible="false">
                                <uc:CompetencyValidateGrid runat="server" ID="Competencies" />

                                <div class="mt-3">
                                    <insite:NextButton runat="server" ID="NextButton2" />
                                    <insite:CloseButton runat="server" ID="CancelButton2" />
                                </div>
                            </insite:NavItem>
                            <insite:NavItem runat="server" ID="CommentsPanel" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText" Visible="false">
                                <uc:CommentList runat="server" ID="Comments" />

                                <div class="mt-3">
                                    <insite:NextButton runat="server" ID="NextButton3" />
                                    <insite:CloseButton runat="server" ID="CancelButton3" />
                                </div>
                            </insite:NavItem>
                            <insite:NavItem runat="server" ID="FinalPanel" Title="Confirm Validation" Icon="far fa-graduation-cap" IconPosition="BeforeText" Visible="false">
                                <div class="row mb-3">
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

                                <div class="mt-3">
                                    <insite:Button runat="server" ID="ValidateButton" Text="Validate" Icon="fas fa-cloud-upload" ButtonStyle="Success" />
                                    <insite:CloseButton runat="server" ID="FinalCancelButton" />
                                </div>
                            </insite:NavItem>

                        </insite:Nav>
                    </div>
                </div>
            </div>
            <div class="col-md-9">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <insite:NavContent runat="server" ID="NavContent" />
                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
