<%@ Page Language="C#" CodeBehind="Download.aspx.cs" Inherits="InSite.UI.Admin.Reports.Download" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ReportDetail" Src="~/UI/Admin/Reports/Controls/ReportInfo.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section class="mb-3">

        <insite:Alert runat="server" ID="Status" />

	    <div class="row">
		    <div class="col-xl-6">

                <div class="card border-0 shadow-lg">
			        <div class="card-body">
                        <h3>
                            <insite:Literal runat="server" Text="Settings" />
                        </h3>

                        <div class="row">
                            <div class="col-md-12">
                                <div class="settings">
                                    <div class="form-group mb-3">
                                        <asp:Label runat="server" CssClass="form-label" AssociatedControlID="FileName">File Name</asp:Label>
                                        <insite:RequiredValidator runat="server" ControlToValidate="FileName" ValidationGroup="Download" />
                                        <div>
                                            <insite:TextBox runat="server" ID="FileName" MaxLength="256" />
                                        </div>
                                    </div>

                                    <div runat="server" id="FileFormatField" class="form-group mb-3">
                                        <asp:Label runat="server" CssClass="form-label" AssociatedControlID="FileFormatSelector">Output Format</asp:Label>
                                        <div>
                                            <asp:RadioButtonList runat="server" ID="FileFormatSelector" RepeatDirection="Vertical" RepeatLayout="Flow" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <asp:Label runat="server" CssClass="form-label" AssociatedControlID="CompressionMode">Compression</asp:Label>
                                        <div>
                                            <insite:ComboBox runat="server" ID="CompressionMode" Width="200">
                                                <Items>
                                                    <insite:ComboBoxOption Text="Disabled" Selected="true" />
                                                    <insite:ComboBoxOption Text="ZIP File" Value="ZIP" />
                                                </Items>
                                            </insite:ComboBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

            </div>

		    <div class="col-xl-6">

                <div class="card border-0 shadow-lg">
			        <div class="card-body">
                        <h3>
                            <insite:Literal runat="server" Text="Report" />
                        </h3>

                        <uc:ReportDetail runat="server" ID="ReportDetail" />
                    </div>
                </div>

            </div>
        </div>

    </section>

    <section class="mb-3">
        <insite:Button runat="server" ID="DownloadButton" Text="Download" Icon="far fa-download" ValidationGroup="Download" ButtonStyle="Success" />
        <insite:CloseButton runat="server" ID="CancelLink" />
    </section>

</asp:Content>
