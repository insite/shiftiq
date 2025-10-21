<%@ Page Language="C#" CodeBehind="RenameSurveyForm.aspx.cs" Inherits="InSite.Admin.Surveys.Forms.Forms.RenameSurveyForm" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/SurveyFormInfo.ascx" TagName="SurveyDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Survey" />

    <section runat="server" id="NameSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-check-square me-1"></i>
            Rename Survey
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">
                        <div>
                            <uc:SurveyDetails runat="server" ID="SurveyDetail" />
                        </div>
                    </div>

                    <div class="col-md-6">
                        <div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Survey Name
                                <insite:RequiredValidator runat="server" ControlToValidate="Name" FieldName="Name" Display="Dynamic" ValidationGroup="Survey" />
                                    <insite:CustomValidator runat="server" ID="NameDuplicateValidator"
                                        ControlToValidate="Name"
                                        ErrorMessage="The system contains multiple survey forms having the same name. Please give each survey form a unique name."
                                        Display="Dynamic"
                                        ValidationGroup="Survey" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="Name" MaxLength="256" />
                                </div>
                                <div class="form-text">
                                    The survey name is used as an internal reference for filing the survey. It is not visible to the survey respondent.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Hook / Integration Code
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="Hook" MaxLength="100" />
                                </div>
                                <div class="form-text">
                                    Unique code for integration with internal toolkits and external systems.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Departments
                                </label>
                                <div>
                                    (Department Checklist Here)
                                </div>
                                <div class="form-text">
                                    The departments to which this survey is assigned.
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Survey" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>
</asp:Content>
