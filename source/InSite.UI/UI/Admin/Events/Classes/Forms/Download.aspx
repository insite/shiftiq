<%@ Page Language="C#" CodeBehind="Download.aspx.cs" Inherits="InSite.UI.Admin.Events.Classes.Forms.Download" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ClassSummaryInfo.ascx" TagName="SummaryInfo" TagPrefix="uc" %>	
<%@ Register Src="../Controls/ClassLocationInfo.ascx" TagName="LocationInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Download" />

    <section class="mb-3">
        <div class="row">
            <div class="col-md-4 mb-3 mb-md-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Output Settings</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                File Name
                                <insite:RequiredValidator runat="server" ControlToValidate="FileName" ValidationGroup="Download" />
                            </label>
                            <insite:TextBox runat="server" ID="FileName" />
                            <div class="form-text">
                            </div>
                        </div>

                        <div runat="server" id="FileFormatField" class="form-group mb-3">
                            <label class="form-label">
                                Output Format
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="FileFormatSelector" RepeatDirection="Vertical" RepeatLayout="Flow">
                                    <asp:ListItem Text="Class Setup (*.json)" Value="JSON" Selected="True" />
                                </asp:RadioButtonList>
                            </div>
                            <div class="form-text">
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
                            <div class="form-text">
                            </div>
                        </div>

                    </div>
                </div>
            </div>
            <div class="col-md-4 mb-3 mb-md-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Class Summary</h3>
                        <uc:SummaryInfo runat="server" ID="SummaryInfo" />

                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Class Location and Schedule</h3>
                        <uc:LocationInfo runat="server" ID="LocationInfo" />

                    </div>
                </div>
            </div>
        </div>
    </section>

    <section>
        <insite:DownloadButton runat="server" ID="DownloadButton" ValidationGroup="Download" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

</asp:Content>