<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Utilities.Constraints.Forms.Read" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-5 mb-md-2">

        <insite:Alert runat="server" ID="Status" />

        <h2 class="h4 mb-3">Setup</h2>

        <div class="row">
            <div class="col-lg-6">

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
            
                        <div class="col-12 mb-2">
                            <asp:Label ID="UniqueNameLabel" runat="server" Text="Unique Name" CssClass="form-label" AssociatedControlID="UniqueName" />
                            <div><asp:Literal runat="server" ID="UniqueName" /></div>
                        </div>
                        <div class="col-12 mb-2">
                            <asp:Label ID="ChildSchemaNameLabel" runat="server" Text="Schema Name" CssClass="form-label" AssociatedControlID="ChildSchemaName" />
                            <div><asp:Literal runat="server" ID="ChildSchemaName" /></div>
                        </div>
                        <div class="col-12 mb-2">
                            <asp:Label ID="ChildTableNameLabel" runat="server" Text="Table Name" CssClass="form-label" AssociatedControlID="ChildTableName" />
                            <div><asp:Literal runat="server" ID="ChildTableName" /></div>
                        </div>
                        <div class="col-12 mb-2">
                            <asp:Label ID="ChildColumnNameLabel" runat="server" Text="Column Name" CssClass="form-label" AssociatedControlID="ChildColumnName" />
                            <div><asp:Literal runat="server" ID="ChildColumnName" /></div>
                        </div>
                        <div class="col-12 mb-2">
                            <asp:Label ID="MasterSchemaNameLabel" runat="server" Text="Primary Schema Name" CssClass="form-label" AssociatedControlID="MasterSchemaName" />
                            <div><asp:Literal runat="server" ID="MasterSchemaName" /></div>
                        </div>
                        <div class="col-12 mb-2">
                            <asp:Label ID="MasterTableNameLabel" runat="server" Text="Primary Table Name" CssClass="form-label" AssociatedControlID="MasterTableName" />
                            <div><asp:Literal runat="server" ID="MasterTableName" /></div>
                        </div>
                        <div class="col-12 mb-2">
                            <asp:Label ID="IsEnforcedLabel" runat="server" Text="Primary Table Name" CssClass="form-label" AssociatedControlID="IsEnforced" />
                            <div>
                                <asp:RadioButtonList runat="server" ID="IsEnforced" CssClass="radiobuttonspacing" Enabled="false" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                    <asp:ListItem Value="true" Text="Yes" />
                                    <asp:ListItem Value="false" Text="No" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>        
                </div>

            </div>
        </div>

    </section>

</asp:Content>