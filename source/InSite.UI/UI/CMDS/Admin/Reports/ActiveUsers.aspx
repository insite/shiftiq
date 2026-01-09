<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ActiveUsers.aspx.cs" Inherits="InSite.Cmds.Actions.Reports.ActiveUsers" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <style>
        .table.table-striped th {
            border-color: #e8e8e8;
        }
        select.btn {
            text-transform: none;
        }
        .employment-types label + input {
            margin-left: 10px;
        }
    </style>

    <div id="desktop">

        <section class="mb-3">

            <insite:DownloadButton runat="server" ID="DownloadButton" Text="Download" CssClass="mb-4" />

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="UpdatePanel">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-lg-2">
                                    <label class="form-label">Group by</label>
                                    <insite:ComboBox runat="server" ID="ddlGroupBy">
                                        <Items>
                                            <insite:ComboBoxOption Text="Organization" Value="Organization" />
                                            <insite:ComboBoxOption Text="Department" Value="OrganizationAndDepartment" />
                                            <insite:ComboBoxOption Text="Role" Value="Role" />
                                        </Items>
                                    </insite:ComboBox>
                                </div>
                                <div class="col-lg-2">
                                    <label class="form-label">Exclude</label>
                                    <insite:TextBox runat="server" ID="ExcludeGroup" Text="Skills Passport" />
                                </div>
                                <div class="col-lg-8">
                                    <label class="form-label d-block">Filter by</label>
                                    <insite:CheckBoxList runat="server" ID="MembershipFunction" RepeatLayout="Flow" RepeatDirection="Horizontal" />
                                    
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:PlaceHolder runat="server" ID="place"></asp:PlaceHolder>
                                </div>
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>
            </div>
        </section>

    </div>

</asp:Content>
