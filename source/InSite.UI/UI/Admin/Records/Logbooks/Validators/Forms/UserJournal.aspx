<%@ Page Language="C#" CodeBehind="UserJournal.aspx.cs" Inherits="InSite.UI.Admin.Records.Validators.Forms.UserJournal" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="UserJournalTree" Src="~/UI/Admin/Records/Logbooks/Controls/UserJournalTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="UserCompetencySummary" Src="~/UI/Admin/Records/Logbooks/Controls/UserCompetencySummary.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-pencil-ruler me-1"></i>
            Logbooks
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

            <div class="row">
                <div class="col-lg-12">
                    <div runat="server" id="NoJournalPanel" class="alert alert-warning">
                        <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This user does not have any entries.
                    </div>

                    <uc:UserJournalTree runat="server" ID="UserJournalTree" />
                </div>
            </div>

            </div>
        </div>
    </section>

    <section runat="server" ID="CompetenciesPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Competencies
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:UserCompetencySummary runat="server" ID="Competencies" />
            </div>
        </div>
    </section>

</asp:Content>
