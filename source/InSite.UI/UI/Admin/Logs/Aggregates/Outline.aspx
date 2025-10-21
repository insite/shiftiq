<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Logs.Aggregates.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ChangeGrid" Src="~/UI/Admin/Reports/Changes/Controls/ChangeGrid.ascx" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="ViewerStatus" />

    <insite:Nav runat="server">

        <insite:NavItem ID="FieldsTab" runat="server" Title="Changes" Icon="far fa-history">

            <section class="pb-5 mb-md-2">

                <div class="row">
                    <div class="col-lg-12">

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <uc:ChangeGrid runat="server" ID="ChangeGrid" />

                            </div>
                        </div>

                    </div>
                </div>

            </section>

        </insite:NavItem>

        <insite:NavItem ID="InstructionTab" runat="server" Title="Aggregate" Icon="far fa-ball-pile">

            <section class="pb-5 mb-md-2">
                <div class="row">
                    <div class="col-lg-6">

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <dl>
                                    <dt>Aggregate Identifier</dt>
                                    <dd>
                                        <asp:Literal runat="server" ID="AggregateIdentifier" /></dd>

                                    <dt>Aggregate Type</dt>
                                    <dd>
                                        <asp:Literal runat="server" ID="AggregateType" /></dd>

                                    <dt>Aggregate Class</dt>
                                    <dd>
                                        <asp:Literal runat="server" ID="AggregateClass" /></dd>

                                    <dt>Aggregate Expiry</dt>
                                    <dd>
                                        <asp:Literal runat="server" ID="AggregateExpires" /></dd>

                                    <dt>Organization Identifier</dt>
                                    <dd>
                                        <asp:Literal runat="server" ID="OrganizationIdentifier" /></dd>
                                </dl>
                            </div>
                        </div>

                    </div>
                </div>

            </section>


        </insite:NavItem>

    </insite:Nav>




    <insite:CancelButton runat="server" ID="CancelLink" />

</asp:Content>
