<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Admin.Databases.Columns.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    
    <insite:Alert runat="server" ID="Status" />

    <section class="pb-5 mb-md-2">

        <div class="row">
            <div class="col-lg-6">

                <h2 class="h4 mb-3">Column</h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <dl>

                            <dt>Schema and Table and Column Name</dt>
                            <dd><asp:Literal runat="server" ID="SchemaTableColumn" /></dd>

                            <dt>Data Type</dt>
                            <dd><asp:Literal runat="server" ID="DataType" /></dd>

                            <dt>Maximum Length</dt>
                            <dd><asp:Literal runat="server" ID="MaximumLength" /></dd>
                            
                            <dt>Required</dt>
                            <dd><asp:Literal runat="server" ID="IsRequired" /></dd>

                            <dt>Non-Null Values</dt>
                            <dd><asp:Literal runat="server" ID="NonNullValues" /></dd>

                            <dt>Distinct Values</dt>
                            <dd><asp:Literal runat="server" ID="DistinctValues" /></dd>

                            <dt runat="server" visible="false">Usage per Organization <span class="form-text">where count is greater than 1</span></dt>
                            <dd runat="server" visible="false">
                                <asp:GridView runat="server" ID="UsageGrid" AutoGenerateColumns="false" CssClass="table table-striped table-bordered">
                                    <Columns>
                                        <asp:BoundField DataField="Organization" HeaderText="Organization" />
                                        <asp:BoundField DataField="Value" HeaderText="Value" />
                                        <asp:BoundField DataField="Count" HeaderText="Count" />
                                    </Columns>
                                </asp:GridView>
                            </dd>

                        </dl>

                    </div>
                </div>

            </div>        
        </div>

    </section>
        
</asp:Content>
