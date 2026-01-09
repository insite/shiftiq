<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.ValidationChanges.Controls.Detail" %>

<insite:ValidationSummary runat="server" ValidationGroup="Competency" />

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
                            Posted
                            <insite:RequiredValidator runat="server" ControlToValidate="ChangePosted" FieldName="Posted" ValidationGroup="Competency" />
                        </label>
                        <div>
                            <insite:DateTimeOffsetSelector ID="ChangePosted" runat="server" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Competency
                            <insite:RequiredValidator runat="server" ControlToValidate="StandardIdentifier" FieldName="Competency" ValidationGroup="Competency" />
                        </label>
                        <div>
                            <cmds:FindCompetency ID="StandardIdentifier" runat="server" />
                        </div>
                        <div class="form-text"></div>
                    </div>
                    
                    <div class="form-group mb-3">
                        <label class="form-label">
                            User
                            <insite:RequiredValidator runat="server" ControlToValidate="UserIdentifier" FieldName="User" ValidationGroup="Competency" />
                        </label>
                        <div>
                            <cmds:FindPerson runat="server" ID="UserIdentifier" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Validator
                        </label>
                        <div>
                            <cmds:FindPerson runat="server" ID="ValidatorUserIdentifier" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Status
                        </label>
                        <div>
                            <cmds:CompetencyStatusSelector runat="server" ID="ChangeStatus" />
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
                            Comment
                        </label>
                        <div>
                            <insite:TextBox runat="server" ID="ChangeComment" TextMode="MultiLine" Rows="10" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                </div>
            </div>

        </div>
    </div>

</section>