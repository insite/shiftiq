<%@ Page Language="C#" CodeBehind="Prepublish.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Prepublish" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:Nav runat="server" CssClass="mb-4">

        <insite:NavItem runat="server" Title="Form" Icon="far fa-window" IconPosition="BeforeText">
            <section>
                <div class="row mt-4">
                    <div class="col-lg-6 h-100">

                        <div class="card border-0 shadow-lg">
                            <div class="card-body h-100">
                                
                                <h3>Assessment Form</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Form Name</label>
                                    <div>
                                        <asp:Literal runat="server" ID="Name" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Form Title
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="FormTitle" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Asset Number and Version</label>
                                    <div>
                                        <asp:Literal runat="server" ID="Version" />
                                    </div>
                                </div>
                        
                                <h3>Standard</h3>

                                <div class="form-group mb-3">
                                    <assessments:AssetTitleDisplay runat="server" ID="FormStandard" />
                                </div>

                            </div>
                        </div>

                    </div>
                    <div runat="server" id="AssessmentAttemptPanel" class="col-lg-6">

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <h3>Start Assessment Attempt</h3>
                                <div class="form-group mb-3">
                                    <div>
                                        <asp:HyperLink runat="server" ID="AssessmentAttemptLink" />
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>
                                <div class="alert alert-danger">
                                    <strong>Warning: Administrator Access Only.</strong>
                                    This link is strictly for administrative testing and review of the assessment form. 
                                    Distribution to, or use by, actual assessment takers is <strong>NOT</strong> advised. 
                                    Any assessment attempts completed using this link are considered to be 
                                    <strong>tests</strong> and are <strong>NOT</strong> recorded by the system.
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
