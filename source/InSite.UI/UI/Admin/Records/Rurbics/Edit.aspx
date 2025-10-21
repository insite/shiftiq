<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Admin.Records.Rurbics.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/RubricDetail.ascx" TagName="RubricDetail" TagPrefix="uc" %>
<%@ Register Src="./Controls/RubricCriteriaList.ascx" TagName="RubricCriteriaList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Rubric" />

    <insite:Nav runat="server">

        <insite:NavItem runat="server" ID="DetailsTab" Title="Details" Icon="far fa-table" IconPosition="BeforeText">

            <section class="mb-3">
                <h2 class="h4 mb-3">
                    Rubric
                </h2>

                <div class="row">
                    <div class="col-md-6">
                                    
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <uc:RubricDetail runat="server" ID="Detail" />

                            </div>
                        </div>

                    </div>
                </div>
            
            </section>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-question-circle" IconPosition="BeforeText">

            <section class="mb-3">
                <h2 class="h4 mb-3">
                    Criteria
                </h2>

                <uc:RubricCriteriaList runat="server" ID="CriteriaList" />
            
            </section>

        </insite:NavItem>

    </insite:Nav>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Rubric" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
