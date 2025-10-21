<%@ Page Language="C#" CodeBehind="Download.aspx.cs" Inherits="InSite.Admin.Messages.Outlines.Forms.Download" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="MessageDetail" Src="~/UI/Admin/Messages/Messages/Controls/MessageDetailsInfo.ascx" %>	
<%@ Register TagPrefix="uc" TagName="Details" Src="~/UI/Admin/Messages/Messages/Controls/OutputDetails.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Download" />

    <section>
        <h2 class="h4 mb-3">
            <i class="far fa-download me-1"></i>
            Download
        </h2>

        <div class="row">
            <div class="col-lg-4 mb-3">

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
                            <label class="form-label">
                                Output Format
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="FileFormatSelector" RepeatDirection="Vertical" RepeatLayout="Flow">
                                    <asp:ListItem Text="Message Setup (*.json)" Value="JSON" Selected="True" />
                                </asp:RadioButtonList>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Compression
                            </label>
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
            <div  class="col-lg-8 mb-3">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Message</h3>
                        <uc:Details runat="server" ID="Details" />
                    </div>
                </div>
            </div>
        </div>
    </section>

    <div>
        <insite:DownloadButton runat="server" ID="DownloadButton" ValidationGroup="Download" />
        <insite:CloseButton runat="server" ID="CancelLink" />
    </div>


</asp:Content>