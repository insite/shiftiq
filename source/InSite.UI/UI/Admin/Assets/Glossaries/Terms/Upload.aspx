<%@ Page Language="C#" CodeBehind="Upload.aspx.cs" Inherits="InSite.Admin.Assets.Glossaries.Terms.Forms.Upload" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>


<asp:Content runat="server" ContentPlaceHolderID="HelpContent">
    <style type="text/css">
        div.checkbox label {
            font-weight: normal !important;
        }

        .loading-panel div.RadUploadProgressArea {
            margin: 0 auto;
        }

        .table-language {
            margin: 0;
        }

        .table-language > tbody > tr > td {
            border: 0;
            padding-top: 0;
        }

        a.label {
            color: #fff !important;
        }
    </style>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <div runat="server" id="FileSection">
        <h2 class="h4 mb-3">
            <i class="far fa-upload me-1"></i>
            File
        </h2>

        <section class="mb-3">
            <div class="row">
                <div class="col-md-4">
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <h3>Select and Upload File</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">Encoding</label>
                                <div>
                                    <insite:ComboBox runat="server" ID="FileEncoding" Width="100%">
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
                                <label class="form-label">
                                    Select CSV File
                                </label>
                                <div>
                                    <insite:FileUploadV1 runat="server" ID="ImportFile" LabelText="" FileUploadType="Unlimited" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="col-md-8">
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <h3>Instructions</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">Follow these steps to import the terms into organization glossary.</label>
                                <div>
                                    <ul>
                                        <li>Your upload file must be a spreadsheet saved as a <strong>CSV UTF-8 (comma delimited) (*.csv)</strong> file.</li>
                                        <li>Click here to <a href="/Library/Documents/Upload-Glossary-Terms.csv">download a template</a>.</li>
                                        <li>Your spreadsheet must contain a title for every column.</li>
                                    </ul>
                                    <p>Here is a simple example:</p>
                                    <table class="table table-striped table-bordered">
                                        <tr><th>Name</th><th>Title</th><th>Definition</th><th>Title FR</th><th>Definition FR</th></tr>
                                        <tr><td>csv</td><td>A comma-separated values</td><td>A comma-separated values (CSV) file is a delimited text file that uses a comma to separate values. Each line of the file is a data record. Each record consists of one or more fields, separated by commas. The use of the comma as a field separator is the source of the name for this file format. A CSV file typically stores tabular data (numbers and text) in plain text, in which case each line will have the same number of fields.</td><td>Comma-separated values</td><td>Un fichier CSV est un fichier texte, par opposition aux formats dits « binaires ». Chaque ligne du texte correspond à une ligne du tableau et les virgules correspondent aux séparations entre les colonnes. Les portions de texte séparées par une virgule correspondent ainsi aux contenus des cellules du tableau. Une ligne est une suite ordonnée de caractères terminée par un caractère de fin de ligne (line break – LF ou CRLF), la dernière ligne pouvant en être exemptée.</td></tr>
                                    </table>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-12">
                    <insite:NextButton runat="server" ID="NextButton" Visible="false" />
                    <insite:CancelButton runat="server" ID="UploadCancelButton" NavigateUrl="/ui/admin/assets/glossaries/search" />
                </div>
            </div>
        </section>
    </div>

    <div runat="server" id="ImportSection" visible="false">
        <h2 class="h4 mb-3">
            <i class="far fa-th-list me-1"></i>
            Import Data
        </h2>

        <section class="mb-3">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <asp:Repeater runat="server" ID="ImportDataRepeater">
                        <HeaderTemplate>
                            <table class="table">
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>Definition</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Name") %><div><%# GetStatusHtml() %></div></td>
                                <td>
                                    <asp:Repeater runat="server" ID="LanguageRepeater">
                                        <HeaderTemplate>
                                            <table class="table table-language">
                                                <tbody>
                                        </HeaderTemplate>
                                        <FooterTemplate>
                                                </tbody>
                                            </table>
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td><strong><%# Eval("Lang") %></strong>:</td>
                                                <td><%# Eval("Html") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>

                    <div class="form-group mb-3">
                        <insite:SaveButton runat="server" ID="ImportButton" />
                        <insite:CancelButton runat="server" ID="CancelButton2" />
                    </div>

                </div>
            </div>

        </section>
    </div>

</asp:Content>
