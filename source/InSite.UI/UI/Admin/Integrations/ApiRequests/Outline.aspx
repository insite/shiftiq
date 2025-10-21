<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Reports.ApiRequests.Forms.Read" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="InputPanel" Title="Input" Icon="far fa-inbox-in" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Input
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">Request Started</label>
                            <div>
                                <asp:Literal runat="server" ID="RequestStarted" />
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Request Status</label>
                            <div>
                                <asp:Literal runat="server" ID="RequestStatus" />
                                (<asp:Literal runat="server" ID="ValidationStatus" />)
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Request URL</label>
                            <div>
                                <asp:Literal runat="server" ID="RequestUrl" />
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Request Headers</label>
                            <div>
                                <asp:Literal runat="server" ID="RequestHeaders" />
                            </div>
                        </div>
                        <div class="form-group mb-3" runat="server" id="ValidationErrorsField">
                            <label class="form-label">Validation Errors</label>
                            <div class="text text-danger">
                                <asp:Literal runat="server" ID="ValidationErrors" />
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Request Data</label>
                            <div>
                                <pre style="white-space: pre-wrap"><code runat="server" id="InputData"></code></pre>
                            </div>
                        </div>
            
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="OutputPanel" Title="Output" Icon="far fa-inbox-out" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Output
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">Response Completed</label>
                            <div>
                                <asp:Literal runat="server" ID="ResponseCompleted" />
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Response Status</label>
                            <div>
                                <asp:Literal runat="server" ID="ResponseStatus" />
                            </div>
                        </div>
                        <div class="form-group mb-3" runat="server" id="ExecutionErrorsField">
                            <label class="form-label">Execution Errors</label>
                            <div class="text text-danger">
                                <asp:Literal runat="server" ID="ExecutionErrors" />
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Response Time</label>
                            <div>
                                <asp:Literal runat="server" ID="ResponseTime" />
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Response Data</label>
                            <div>
                                <pre style="white-space: pre-wrap"><code runat="server" id="OutputData"></code></pre>
                            </div>
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
