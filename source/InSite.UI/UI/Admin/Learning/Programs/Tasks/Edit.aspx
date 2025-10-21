<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Records.Programs.Tasks.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="TaskPresentationSetup" Src="./Controls/TaskPresentationSetup.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <section runat="server" id="TaskSection" class="mb-3">

        <div class="row">

                <div class="col-md-6">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <insite:Nav runat="server">

                                <insite:NavItem runat="server" Title="Task Content">

                                    <div class="form-group mb-3">
                                        <label class="form-label">Language</label>
                                        <div>
                                            <insite:ComboBox runat="server" ID="Language" AllowBlank="false" />
                                        </div>
                                    </div>

                                    <div class="row row-translate">
                                        <div class="col-md-12">
                                            <asp:Repeater runat="server" ID="ContentRepeater">
                                                <ItemTemplate>
                                                    <div class="form-group mb-3">
                                                        <insite:DynamicControl runat="server" ID="Container" />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>

                                </insite:NavItem>

                                <insite:NavItem runat="server" ID="PresentationTab" Title="Presentation">

                                    <uc:TaskPresentationSetup runat="server" ID="TaskPresentationSetup" />

                                </insite:NavItem>

                            </insite:Nav>

                        </div>
                    </div>

                </div>

            <div class="col-md-6">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div runat="server" id="PrerequisiteRepeaterField" class="form-group mb-3">
                            <label class="form-label">
                                Prerequisite(s)
                            </label>
                            <asp:Repeater runat="server" ID="PrerequisiteRepeater">
                                <HeaderTemplate>
                                    <table class="table table-striped">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td style="width: 100%">
                                            <%# Eval("TriggerChange") %>:
                                            <%# Eval("TriggerDescription") %>
                                        </td>
                                        <td>
                                            <insite:IconButton runat="server" Name="trash-alt" ToolTip="Delete Prerequisite" CommandName="PrerequisiteDelete" CommandArgument='<%# Eval("PrerequisiteIdentifier") %>' />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                New Prerequisite
                            </label>
                            <div class="row">
                                <div class="col-md-12">
                                    <insite:ComboBox runat="server" ID="TriggerChange" AllowBlank="true">
                                        <Items>
                                            <insite:ComboBoxOption />
                                            <insite:ComboBoxOption Value="TaskCompleted" Text="Task Completed" />
                                            <insite:ComboBoxOption Value="TaskViewed" Text="Task Viewed" />
                                        </Items>
                                    </insite:ComboBox>
                                </div>
                            </div>
                            <div class="row mt-1" runat="server" id="TriggerTaskInProgram" visible="false">
                                <div class="col-md-12">
                                    <insite:ComboBox runat="server" ID="TaskInProgram" AllowBlank="true" />
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </section>

    <insite:SaveButton runat="server" ID="SaveButton" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
