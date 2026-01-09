<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Standards.Collections.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Standards/Occupations/Controls/CompetenciesPanel.ascx" TagName="CompetenciesPanel" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<insite:Alert runat="server" ID="EditorStatus" />
<insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Collection" />
<section class="mb-3">        
    <div class="row button-group mb-3">
        <div class="col-lg-12">
            <insite:Button runat="server" ID="NewClassLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" />

            <insite:ButtonSpacer runat="server" ID="Separator1" />

            <insite:Button runat="server" ID="DuplicateButton" Text="Duplicate" ButtonStyle="Default" Icon="fas fa-copy" CausesValidation="false" Visible="true"/>
            <insite:ButtonSpacer runat="server" ID="Separator2" />

            <insite:Button runat="server" ID="DownloadLink" Text="Download JSON" icon="fas fa-download" ButtonStyle="Primary" />
            <insite:Button runat="server" id="PrintDocumentButton" ButtonStyle="Default" ToolTip="Print the document" Text="Print" Icon="far fa-print" />
            <insite:DeleteButton runat="server" ID="DeleteLink" />
        </div>
    </div>

    <h2 class="h4 mb-1">Collection</h2>

    <div class="row">
        <div class="col-lg-12">
            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <uc:Detail runat="server" ID="Detail" />
                </div>
            </div>
        </div>
    </div>
</section>

<section class="pb-5 mb-md-2">        
    <h2 class="h4 mb-1">Competencies</h2>
    <div class="row">
        <div class="col-lg-12">
            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <uc:CompetenciesPanel runat="server" ID="CompetenciesPanel" />
                </div>
            </div>
        </div>
    </div>
</section>
</asp:Content>
