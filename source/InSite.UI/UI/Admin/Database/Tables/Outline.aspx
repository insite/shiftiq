<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Utilities.Tables.Forms.Read" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Columns/Controls/ColumnGrid.ascx" TagName="ColumnGrid" TagPrefix="uc" %>
<%@ Register Src="../Constraints/Controls/ConstraintGrid.ascx" TagName="ConstraintGrid" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="Status" />

    <section class="pb-5 mb-md-2">

        <div class="row">
            <div class="col-lg-6">

                <h2 class="h4 mb-3">Table</h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <dl>

                            <dt>Schema and Table Name</dt>
                            <dd><asp:Literal runat="server" ID="SchemaTableTitle" /></dd>

                            <dt>Number of Columns</dt>
                            <dd><asp:Literal runat="server" ID="ColumnCount" /></dd>

                            <dt>Number of Rows</dt>
                            <dd><asp:Literal runat="server" ID="RowCount" /></dd>

                        </dl>
                    
                    </div>
                </div>

            </div>        
        </div>

    </section>

    <section class="pb-5 mb-md-2">

        <div class="row">
            <div class="col-lg-6">

                <h2 class="h4 mb-3">Columns <asp:Label runat="server" ID="ColumnCountTitle" CssClass="fs-xs badge bg-info" /></h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:ColumnGrid runat="server" ID="ColumnsGrid" />

                    </div>
                </div>

            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2">

        <div class="row">
            <div class="col-lg-6">

                <h2 class="h4 mb-3">Foreign Keys <asp:Label runat="server" ID="ForeignKeyCountTitle" CssClass="fs-xs badge bg-info" /></h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h5 class="card-title" title="Other tables upon which this table depends">Upstream Dependencies</h5>

                        <uc:ConstraintGrid runat="server" ID="ReferencesGrid" />

                        <asp:Panel runat="server" ID="NoReferencesPanel">
                            <p><strong>This table has no references to other tables.</strong></p>
                        </asp:Panel> 

                        <h5 class="card-title" title="Other tables that depend upon this table">Downstream Dependencies</h5>

                        <uc:ConstraintGrid runat="server" ID="DependenciesGrid" />

                        <asp:Panel runat="server" ID="NoDependenciesPanel">
                            <p><strong>No other tables are dependent upon this table.</strong></p>
                        </asp:Panel>

                    </div>
                </div>

            </div>        
        </div>

    </section>

</asp:Content>
