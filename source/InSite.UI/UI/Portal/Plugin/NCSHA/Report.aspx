<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="InSite.UI.Variant.NCSHA.Reports.Preview" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <asp:MultiView runat="server" ID="MultiView">

        <asp:View runat="server" ID="ErrorView">

            <div class="row">
                <div class="col-lg-12">
                    <div runat="server" id="ErrorPanel" class="alert alert-warning"></div>
                </div>
            </div>

        </asp:View>

        <asp:View runat="server" ID="NormalView">

            <div class="row">
                <div class="col-lg-2">
                    <div class="form-group mb-3">
                        <label class="form-label">Report Table</label>
                        <asp:DropDownList runat="server" ID="ReportTable" CssClass="form-select"></asp:DropDownList>
                    </div>
                </div>
                <div class="col-lg-2">
                    <div class="form-group mb-3">
                        <label class="form-label">Report Year</label>
                        <asp:DropDownList ID="ReportYear" runat="server" CssClass="form-select">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-lg-1">
                    <div class="form-group mb-3">
                        <label class="form-label">&nbsp;</label>
                        <div>
                            <asp:Repeater runat="server" ID="ExportButtonRepeater">
                                <HeaderTemplate>
                                    <div class="btn-group" role="group">
                                        <button type="button" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                            Download
                                        </button>
                                        <ul class="dropdown-menu" aria-labelledby="btnGroupDrop1">
                                </HeaderTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <li>
                                        <asp:LinkButton runat="server" CssClass="dropdown-item" CommandName="Export" CommandArgument='<%# Eval("ID") %>'>
                                            <%# Eval("Name") %> (*.<%# Eval("Extension") %>)
                                        </asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>

            <div runat="server" id="ReportViewerSection" class="row">
                <div class="col-lg-12">
                    <div style="min-height: 240px;">
                        <microsoft:ReportViewer runat="server" ID="Viewer"
                            ProcessingMode="Remote"
                            AsyncRendering="false"
                            KeepSessionAlive="false"
                            SizeToReportContent="true"
                            ShowBackButton="false"
                            ShowFindControls="false"
                            ShowParameterPrompts="false"
                            ShowPrintButton="false"
                            ShowZoomControl="false"
                            ShowExportControls="false" />
                    </div>
                </div>
            </div>

        </asp:View>

    </asp:MultiView>

</asp:Content>