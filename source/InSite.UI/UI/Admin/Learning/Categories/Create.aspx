<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Admin.Learning.Categories.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Learning/Categories/Controls/CategoryDetail.ascx" TagName="CategoryDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Category" />

    <section class="mb-3">
            
        <h2 class="h4 mb-3">
            <i class="far fa-tag"></i>
            Category
        </h2>

        <div class="row mb-3">
            <div class="col-md-6">
                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Category"/>
            </div>
        </div>

        <div runat="server" ID="NewSection" class="row">
                
            <div class="col-md-6">                                    
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <uc:CategoryDetail runat="server" ID="CategoryDetail" />

                    </div>
                </div>
            </div>
    
        </div>

    </section>

    <section>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Category" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

</asp:Content>
