<%@ Page Language="C#" CodeBehind="Upload.aspx.cs" Inherits="InSite.Admin.Standards.Standards.Forms.Upload" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="UploadFieldRepeater" Src="~/UI/Admin/Standards/Standards/Controls/UploadFieldRepeater.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:PageHeadContent runat="server">
        <style type="text/css">
            div.checkbox label {
                font-weight: normal !important;
            }

            .loading-panel div.RadUploadProgressArea {
                margin: 0 auto;
            }
        </style>
    </insite:PageHeadContent>

    <insite:Alert runat="server" ID="StatusTop" />
    <insite:ValidationSummary ID="FileValidationSummary" runat="server" ValidationGroup="File" />
    <insite:ValidationSummary ID="FieldsValidationSummary" runat="server" ValidationGroup="Fields" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="FileSection" Title="File" Icon="far fa-upload" IconPosition="BeforeText">
            <section>
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h2 class="h4 mt-4 mb-3">File</h2>
                        <div class="row">
                            <div class="col-md-7">

                                <h3>Select and Upload File</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Select CSV File
                                    </label>
                                    <div class="w-75">
                                        <insite:FileUploadV1 runat="server" ID="ImportFile" FileUploadType="Unlimited" AllowedExtensions=".csv" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Encoding</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="FileEncoding" CssClass="w-50">
                                            <Items>
                                                <insite:ComboBoxOption Text="ASCII" Value="us-ascii" />
                                                <insite:ComboBoxOption Text="UTF 7" Value="utf-7" />
                                                <insite:ComboBoxOption Text="UTF 8 (recommended)" Value="utf-8" Selected="true" />
                                                <insite:ComboBoxOption Text="UTF 16" Value="utf-16" />
                                                <insite:ComboBoxOption Text="UTF 32" Value="utf-32" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                    <div class="form-text">
                                        Select the character encoding that was used to create the CSV file you have selected.
                                        Most computer programs use 
                                        <a target="_blank" href="https://en.wikipedia.org/wiki/UTF-8">UTF-8</a>
                                        so you should select a different encoding only if you are certain of the encoding that was used.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <insite:NextButton runat="server" ID="UploadNextButton" ValidationGroup="File" Visible="false" />
                                </div>
                            </div>
                            <div class="col-md-5">

                                <h3>Instructions</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Follow these steps to import your standards.</label>
                                    <div>
                                        <ul>
                                            <li>Your upload file must be a .CSV file. You can save any Excel spreadsheet as a .CSV file.</li>
                                            <li>Download template:
                                                <a href="/UI/Admin/standards/standards/content/upload-standards.csv">Simple</a>
                                                <a href="/UI/Admin/standards/standards/content/upload-standards-extended.csv">Extended</a>
                                            </li>
                                            <li>Your file must contain titles on every column</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="FieldsSection" Title="Fields" Icon="far fa-clipboard-list" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Fields</h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <p>
                            Choose the fields into which you want to upload your standard data.
                            Required fields are indicated with an asterisk (*).
                        </p>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-horizontal">

                                    <div class="field-heading">
                                        <span>Standard</span>
                                    </div>
                                    <uc:UploadFieldRepeater runat="server" ID="FieldsStandard" ValidationGroup="Fields" />

                                    <div class="field-heading">
                                        <span>Competency Score</span>
                                    </div>
                                    <uc:UploadFieldRepeater runat="server" ID="FieldsCompetencyScore" ValidationGroup="Fields" />

                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-horizontal">
                                    <div class="field-heading">
                                        <span>Settings</span>
                                    </div>

                                    <uc:UploadFieldRepeater runat="server" ID="FieldsSettings" ValidationGroup="Fields" />
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-horizontal">
                                    <div class="field-heading">
                                        <span>Content EN</span>
                                    </div>
                                    <uc:UploadFieldRepeater runat="server" ID="FieldsContentEn" ValidationGroup="Fields" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-horizontal">
                                    <div class="field-heading">
                                        <span>Content ALT</span>
                                    </div>
                                    <uc:UploadFieldRepeater runat="server" ID="FieldsContentAlt" ValidationGroup="Fields" />
                                </div>
                            </div>
                        </div>

                        <div class="pt-3">
                            <insite:Button runat="server" ID="FieldsNextButton" ButtonStyle="Success" ValidationGroup="Fields" Text="Upload and Save Changes" Icon="fas fa-upload" />
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ValidationSection" Title="Validation" Icon="far fa-exclamation-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Validation</h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <insite:Alert runat="server" ID="ValidationStatus" />

                        <div class="pt-3">
                            <insite:Button runat="server" ID="ValidationNextButton" ButtonStyle="Success" Text="Continue" IconPosition="AfterText" Icon="fas fa-arrow-alt-right" />
                            <insite:Button runat="server" ID="ValidationBackButton" ButtonStyle="Default" Text="Back" IconPosition="AfterText" Icon="fas fa-arrow-alt-left" />
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CompleteSection" Title="Status" Icon="far fa-check-square" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Status</h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <insite:Alert runat="server" ID="StatusComplete" />

                        <div class="row">
                            <div class="col-lg-6">

                                <asp:Repeater runat="server" ID="FirstRecordsRepeater">
                                    <HeaderTemplate>
                                        <div class="form-text"><%= FirstRecordsTitle %></div>
                                        <table class="table table-striped table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>Title</th>
                                                    <th>Type</th>
                                                    <th>Code</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <a href="/ui/admin/standards/edit?id=<%# Eval("StandardIdentifier") %>"><%# Eval("ContentTitle") %></a>
                                            </td>
                                            <td>
                                                <%# Eval("StandardType") %>
                                            </td>
                                            <td>
                                                <%# Eval("Code") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate></tbody></table></FooterTemplate>
                                </asp:Repeater>

                            </div>
                            <div class="col-lg-6">

                                <asp:Repeater runat="server" ID="LastRecordsRepeater">
                                    <HeaderTemplate>
                                        <div class="form-text">Last 5 standards:</div>
                                        <table class="table table-striped table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>Title</th>
                                                    <th>Type</th>
                                                    <th>Code</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <a href="/ui/admin/standards/edit?id=<%# Eval("StandardIdentifier") %>"><%# Eval("ContentTitle") %></a>
                                            </td>
                                            <td>
                                                <%# Eval("StandardType") %>
                                            </td>
                                            <td>
                                                <%# Eval("Code") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate></tbody></table></FooterTemplate>
                                </asp:Repeater>

                            </div>
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <insite:ProgressPanel runat="server" ID="SaveProgress" HeaderText="Upload Standards" Cancel="Redirect">
        <Triggers>
            <insite:ProgressControlTrigger ControlID="FieldsNextButton" />
            <insite:ProgressControlTrigger ControlID="ValidationNextButton" />
        </Triggers>
        <Items>
            <insite:ProgressIndicator Name="Progress" Caption="Completed: {percent}%" />
            <insite:ProgressStatus Text="Status: Importing data{running_ellipsis}" />
            <insite:ProgressStatus Text="Elapsed time: {time_elapsed}s" />
        </Items>
    </insite:ProgressPanel>

</asp:Content>
