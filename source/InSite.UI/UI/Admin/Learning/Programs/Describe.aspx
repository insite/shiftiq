<%@ Page Language="C#" CodeBehind="Describe.aspx.cs" Inherits="InSite.Admin.Records.Programs.Describe" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" />

    <section runat="server" id="ProgramSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-graduation-cap me-1"></i>
            Describe Program
        </h2>

        <div class="row">
            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Name
                                    <insite:RequiredValidator runat="server" ControlToValidate="ProgramName" FieldName="Name" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="ProgramName" MaxLength="200" />
                            </div>
                            <div class="form-text">The name for this program.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Code
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="ProgramCode" MaxLength="20" />
                            </div>
                            <div class="form-text">The catalog code for this program.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Department
                            </label>
                            <div>
                                <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Options
                            </label>
                            <div>
                                <insite:CheckBox runat="server" ID="IsHidden" Text="Hidden" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Tag
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="ProgramTag" MaxLength="40" />
                            </div>
                            <div class="form-text">An <i>optional</i> term to categorize the program.</div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Description
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="ProgramDescription" TextMode="MultiLine" Rows="5" MaxLength="500"/>
                            </div>
                            <div class="form-text">The description for this program.</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </section>

    <insite:SaveButton runat="server" ID="SaveButton" />
    <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />

</asp:Content>
