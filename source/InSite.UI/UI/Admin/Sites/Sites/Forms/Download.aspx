<%@ Page Language="C#" CodeBehind="Download.aspx.cs" Inherits="InSite.Admin.Sites.Sites.Forms.Download" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Sites/Sites/Controls/SiteInfo.ascx" TagName="SiteDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:ValidationSummary runat="server" ValidationGroup="Download" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-cloud me-1"></i>
            Download
        </h2>

        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Output Settings</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                File Name
                                <insite:RequiredValidator runat="server" ControlToValidate="FileName" ValidationGroup="Download" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="FileName" />
                            </div>
                        </div>

                        <div runat="server" id="FileFormatField" class="form-group mb-3">
                            <label class="form-label">File Format</label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="FileFormatSelector" RepeatDirection="Vertical" RepeatLayout="Flow">
                                    <asp:ListItem Text="JSON (*.json)" Value="JSON" Selected="True" />
                                </asp:RadioButtonList>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Compression</label>
                            <div>
                                <insite:ComboBox runat="server" ID="CompressionMode">
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
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:SiteDetail id="SiteDetails" runat="server" />
                    </div>
                </div>
            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:DownloadButton runat="server" ID="DownloadButton" ValidationGroup="Download" />
            <insite:CloseButton runat="server" ID="CancelLink" />
        </div>
    </div>

</asp:Content>
