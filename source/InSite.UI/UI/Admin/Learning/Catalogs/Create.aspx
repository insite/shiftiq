<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Admin.Learning.Catalogs.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="Step1Section" Title="Catalog" Icon="far fa-pencil-ruler" IconPosition="BeforeText">
            <section>
                
                <div class="row mb-3">
                    <div class="col-md-6">
                        <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Catalog" />
                    </div>
                </div>

                <div runat="server" ID="NewSection" class="row">
                
                    <div class="col-md-6">                                    
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Catalog Name
                                        <insite:RequiredValidator runat="server" ControlToValidate="CatalogName" FieldName="Name" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="CatalogName" MaxLength="200" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <div>
                                        <insite:SaveButton runat="server" ID="SaveButton" />
                                        <insite:CancelButton runat="server" ID="CancelButton" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
    
                </div>

            </section>
        </insite:NavItem>
        
    </insite:Nav>

</asp:Content>
