<%@ Page Language="C#" CodeBehind="Analysis.aspx.cs" Inherits="InSite.Admin.Standards.Documents.Forms.Analysis" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="AnalysisInputsJobFit1" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisInputsJobFit1.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisInputsJobFit2" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisInputsJobFit2.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisInputsCareerMap" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisInputsCareerMap.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisInputsStandards" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisInputsStandards.ascx" %>

<%@ Register TagPrefix="uc" TagName="AnalysisReportJobFit1" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisReportJobFit1.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisReportJobFit2" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisReportJobFit2.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisReportCareerMap" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisReportCareerMap.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisReportStandards" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisReportStandards.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Analysis" />

    <section class="mb-3">

        <div class="row mb-3">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <h3>Report Settings</h3>

                        <div class="row">
                            <div class="col-md-12">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Report Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="ReportTypeSelector" FieldName="Report Type" ValidationGroup="Analysis" />
                                    </label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="ReportTypeSelector" Width="100%">
                                            <Items>
                                                <insite:ComboBoxOption />
                                                <insite:ComboBoxOption Text="Job Comparison Tool" Value="1" />
                                                <insite:ComboBoxOption Text="Career Map" Value="3" />
                                                <insite:ComboBoxOption Text="Standards Analysis" Value="4" />
                                                <insite:ComboBoxOption Text="Job Fit Analysis (Under Development)" Value="2" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                </div>

                                <uc:AnalysisInputsJobFit1 runat="server" ID="InputsJobFit1" />
                                <uc:AnalysisInputsJobFit2 runat="server" ID="InputsJobFit2" />
                                <uc:AnalysisInputsCareerMap runat="server" ID="InputsCareerMap" />
                                <uc:AnalysisInputsStandards runat="server" ID="InputsStandards" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div runat="server" id="ReportColumn" class="col-md-12" visible="false">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3 runat="server" id="ReportTitle"></h3>

                            <uc:AnalysisReportJobFit1 runat="server" ID="ReportJobFit1" />
                            <uc:AnalysisReportJobFit2 runat="server" ID="ReportJobFit2" />
                            <uc:AnalysisReportCareerMap runat="server" ID="ReportCareerMap" />
                            <uc:AnalysisReportStandards runat="server" ID="ReportStandards" />
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <div class="row">

            
        </div>

    </section>


</asp:Content>
