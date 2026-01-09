<%@ Page Language="C#" CodeBehind="ModifyCatalog.aspx.cs" Inherits="InSite.Admin.Records.Programs.ModifyCatalog" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ProgramCategoryList" Src="./Controls/ProgramCategoryList.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Catalog" />

    <section class="mb-3">
        <div class="row">
            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Catalog
                            </label>
                            <div>
                                <insite:CatalogComboBox runat="server" ID="CatalogIdentifier" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Sequence in Catalog
                            </label>
                            <div>
                                <insite:NumericBox runat="server" ID="CatalogSequence" NumericMode="Integer" />
                            </div>
                        </div>

                        <uc:ProgramCategoryList runat="server" ID="ProgramCategoryList" />

                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Catalog" />
        <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
    </div>

</asp:Content>