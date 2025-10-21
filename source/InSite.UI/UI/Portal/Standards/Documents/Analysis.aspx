<%@ Page Language="C#" CodeBehind="Analysis.aspx.cs" Inherits="InSite.UI.Portal.Standards.Documents.Analysis" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="AnalysisInputsJobFit1" Src="../Controls/AnalysisInputsJobFit1.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisInputsJobFit2" Src="../Controls/AnalysisInputsJobFit2.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisInputsCareerMap" Src="../Controls/AnalysisInputsCareerMap.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisInputsStandards" Src="../Controls/AnalysisInputsStandards.ascx" %>

<%@ Register TagPrefix="uc" TagName="AnalysisReportJobFit1" Src="../Controls/AnalysisReportJobFit1.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisReportJobFit2" Src="../Controls/AnalysisReportJobFit2.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisReportCareerMap" Src="../Controls/AnalysisReportCareerMap.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisReportStandards" Src="../Controls/AnalysisReportStandards.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<style>
    input[type=radio], input[type=checkbox] {
        margin-right:7px;
    }

    .container small {
        font-size:0.75em;
    }
</style>

<section class="container mb-2 mb-sm-0 pb-sm-5">
    
    <h2 class="mb-0"><insite:Literal runat="server" ID="HeaderLabel" Text="Document Analysis" /></h2>
    <div class="mb-4"></div>

    <insite:Alert runat="server" ID="StatusAlert" />

    <div class="row">
        <div class="col-md-6">
            <div class="card card-hover shadow mb-3">
                <div class="card-header">
                    <h3 class="m-0 text-primary"><%= Translate("Criteria") %></h3>
                </div>
                <div class="card-body">
                    <div runat="server" id="ReportTypeField" class="form-group mb-3">
                        <label class="form-label" for="<%# ReportTypeSelector.ClientID %>">
                            <insite:Literal runat="server" Text="Report Type" />
                            <insite:RequiredValidator runat="server" ControlToValidate="ReportTypeSelector" FieldName="Report Type" ValidationGroup="Analysis" />
                        </label>
                        <div>
                            <insite:ComboBox runat="server" ID="ReportTypeSelector" EnableTranslation="true" Width="100%" />
                        </div>
                    </div>

                    <uc:AnalysisInputsJobFit1 runat="server" ID="InputsJobFit1" />
                    <uc:AnalysisInputsJobFit2 runat="server" ID="InputsJobFit2" />
                    <uc:AnalysisInputsCareerMap runat="server" ID="InputsCareerMap" />
                    <uc:AnalysisInputsStandards runat="server" ID="InputsStandards" />
                </div>
            </div>
        </div>
        <div runat="server" id="ReportColumn" class="col-md-6" visible="false">
            <div class="card card-hover shadow mb-3">
                <div class="card-header">
                    <h3 class="m-0 text-primary">
                        <asp:Literal runat="server" ID="ReportTitle" />
                    </h3>
                </div>
                <div class="card-body">
                    <uc:AnalysisReportJobFit1 runat="server" ID="ReportJobFit1" />
                    <uc:AnalysisReportJobFit2 runat="server" ID="ReportJobFit2" />
                    <uc:AnalysisReportCareerMap runat="server" ID="ReportCareerMap" />
                    <uc:AnalysisReportStandards runat="server" ID="ReportStandards" />
                </div>
            </div>
        </div>
    </div>

</section>
</asp:Content>
