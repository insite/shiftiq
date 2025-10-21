<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.Validations.Controls.Detail" %>

<insite:ValidationSummary runat="server" ValidationGroup="Validation" />

<section runat="server" ID="DetailSection" class="mb-3">
    <h2 class="h4 mb-3">
        <i class="far fa-ruler-triangle me-1"></i>
        Details
    </h2>

    <div class="row">
        <div class="col-lg-6">

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Standard
                            <insite:RequiredValidator runat="server" ControlToValidate="StandardIdentifier" FieldName="Standard" ValidationGroup="Validation" />
                        </label>
                        <div>
                            <insite:FindStandard runat="server" ID="StandardIdentifier" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Assigned
                        </label>
                        <div>
                            <insite:DateTimeOffsetSelector runat="server" ID="Assigned" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Authority Name
                        </label>
                        <div>
                            <insite:TextBox runat="server" ID="AuthorityName" Width="100%" MaxLength="128" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Closed
                        </label>
                        <div>
                            <insite:DateTimeOffsetSelector runat="server" ID="Closed" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            User
                            <insite:RequiredValidator runat="server" ControlToValidate="UserIdentifier" FieldName="User" ValidationGroup="Validation" />
                        </label>
                        <div>
                            <cmds:FindPerson runat="server" ID="UserIdentifier" Width="100%" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Expired
                        </label>
                        <div>
                            <insite:DateTimeOffsetSelector runat="server" ID="Expired" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Notified
                        </label>
                        <div>
                            <insite:DateTimeOffsetSelector runat="server" ID="Notified" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Opened
                        </label>
                        <div>
                            <insite:DateTimeOffsetSelector runat="server" ID="Opened" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Process Step
                        </label>
                        <div>
                            <insite:TextBox runat="server" ID="ProcessStep" Width="100%" MaxLength="32" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Score Scaled
                            <insite:RequiredValidator runat="server" ControlToValidate="ScoreScaled" FieldName="Score Scaled" ValidationGroup="Validation" />
                        </label>
                        <div>
                            <insite:NumericBox ID="ScoreScaled" runat="server" DecimalPlaces="4" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                </div>
            </div>

        </div>

        <div class="col-lg-6">

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Self Assessment Date
                        </label>
                        <div>
                            <insite:DateTimeOffsetSelector runat="server" ID="SelfAssessmentDate" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Self Assessment Status
                        </label>
                        <div>
                            <cmds:SelfAssessmentStatusSelector ID="SelfAssessmentStatus" runat="server" Width="100%" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Validation Comment
                        </label>
                        <div>
                            <insite:TextBox runat="server" ID="ValidationComment" Width="100%" TextMode="MultiLine" Rows="5" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Validator
                        </label>
                        <div>
                            <cmds:FindPerson runat="server" ID="ValidatorUserIdentifier" Width="100%" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Validation Date
                        </label>
                        <div>
                            <insite:DateTimeOffsetSelector runat="server" ID="ValidationDate" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Validation Status
                        </label>
                        <div>
                            <cmds:CompetencyStatusSelector ID="ValidationStatus" runat="server" Width="100%" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Options
                        </label>
                        <div>
                            <asp:CheckBox runat="server" ID="IsEnabled" Text="Enabled" /><br />
                            <asp:CheckBox runat="server" ID="IsEvaluated" Text="Evaluated" /><br />
                            <asp:CheckBox runat="server" ID="IsSuccess" Text="Success" /><br />
                            <asp:CheckBox runat="server" ID="IsValidated" Text="Validated" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                </div>
            </div>

        </div>
    </div>
</section>