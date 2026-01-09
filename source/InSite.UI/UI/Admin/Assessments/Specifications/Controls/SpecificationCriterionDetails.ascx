<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecificationCriterionDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Specifications.Controls.SpecificationCriterionDetails" %>

<%@ Register TagPrefix="uc" TagName="CriterionDetail" Src="../../Criteria/Controls/Detail.ascx" %>
<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<insite:Nav runat="server">
            
    <insite:NavItem runat="server" ID="CriterionTab" Title="Criterion">
        <uc:CriterionDetail runat="server" ID="CriterionDetail" />
    </insite:NavItem>

    <insite:NavItem runat="server" ID="SpecificationTab" Title="Specification">
        <assessments:ContentRepeater runat="server" ID="SpecificationContent" ControlPath="~/UI/Admin/Assessments/Specifications/Controls/SpecificationDetails.ascx" />
    </insite:NavItem>

</insite:Nav>
