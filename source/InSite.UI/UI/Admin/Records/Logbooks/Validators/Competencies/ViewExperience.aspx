<%@ Page Language="C#" CodeBehind="ViewExperience.aspx.cs" Inherits="InSite.UI.Admin.Records.Validators.Competencies.ViewExperience" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	
<%@ Register Src="~/UI/Admin/Records/Logbooks/Controls/LogbookDetailsV2.ascx" TagName="LogbookDetail" TagPrefix="uc" %>	
<%@ Register TagPrefix="uc" TagName="LogEntryDetail" Src="~/UI/Admin/Records/Logbooks/Entries/Controls/LogEntryDetail.ascx" %>
<%@ Register TagPrefix="uc" TagName="CompetencyDetail" Src="~/UI/Admin/Records/Logbooks/Competencies/Controls/CompetencyDetail.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section runat="server" ID="LearnerLogbookPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Logged Competency
        </h2>

            <div class="row">
                <div class="col-md-3">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Competency</h3>
                            <uc:CompetencyDetail runat="server" ID="Competency" />
                        </div>
                    </div>
                </div>
                <div class="col-lg-3">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Entry</h3>
                            <uc:LogEntryDetail runat="server" ID="Detail" />

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
               <div class="col-lg-3">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Learner</h3>
                            <uc:PersonDetail runat="server" ID="PersonDetail" />
                        </div>
                    </div>
                </div>

                <div class="col-lg-3 ">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                                <h3>Logbook Details</h3>
                                <uc:LogbookDetail runat="server" ID="LogbookDetail" />
                        </div>
                    </div>
                </div>
            </div>

    </section>

</asp:Content>
