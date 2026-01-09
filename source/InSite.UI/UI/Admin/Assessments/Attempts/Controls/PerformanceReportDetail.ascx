<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PerformanceReportDetail.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.PerformanceReportDetail" %>

<insite:ValidationSummary runat="server" ValidationGroup="Report" />

<asp:CustomValidator runat="server"
    ID="AssesmentTypeValidator"
    ErrorMessage="Sum of CBA and SLA should equals 100%"
    Display="None"
    ValidationGroup="Report"
/>

<div class="row mb-3">

    <div class="col-lg-6 mb-3 mb-lg-0">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Report</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Name
                        <insite:RequiredValidator runat="server"
                            ControlToValidate="ReportName"
                            FieldName="Name"
                            ValidationGroup="Report"
                        />
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="ReportName" MaxLength="100" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Language
                    </label>
                    <div>
                        <insite:ComboBox runat="server" ID="Language" CssClass="w-50">
                            <Items>
                                <insite:ComboBoxOption Value="en" Text="English" Selected="true" />
                                <insite:ComboBoxOption Value="fr" Text="French" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Nursing Role
                        <insite:RequiredValidator runat="server"
                            ControlToValidate="RequiredRole"
                            FieldName="Nursing Role"
                            ValidationGroup="Report"
                        />
                    </label>
                    <div>
                        <insite:ComboBox runat="server" ID="RequiredRole" CssClass="w-50" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        % Required for Emergent
                        <insite:RequiredValidator runat="server"
                            ControlToValidate="EmergentScore"
                            FieldName="% Required for Emergent"
                            ValidationGroup="Report"
                        />
                    </label>
                    <div>
                        <insite:NumericBox runat="server" ID="EmergentScore" NumericMode="Integer" MinValue="0" MaxValue="100" CssClass="w-25" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        % Required for Consistent
                        <insite:RequiredValidator runat="server"
                            ControlToValidate="ConsistentScore"
                            FieldName="% Required for Consistent"
                            ValidationGroup="Report"
                        />
                    </label>
                    <div>
                        <insite:NumericBox runat="server" ID="ConsistentScore" NumericMode="Integer" MinValue="0" MaxValue="100" CssClass="w-25" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <table class="table table-striped">
                        <tr>
                            <th class="w-50">Assessment Type</th>
                            <th class="w-50">Weight</th>
                        </tr>
                        <tr>
                            <td>
                                CBA
                                <insite:RequiredValidator runat="server" ControlToValidate="CBAWeight" FieldName="CBA Weight" ValidationGroup="Report" />
                            </td>
                            <td>
                                <insite:NumericBox runat="server" ID="CBAWeight" NumericMode="Integer" MinValue="0" MaxValue="100" CssClass="w-50" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                SLA
                                <insite:RequiredValidator runat="server" ControlToValidate="SLAWeight" FieldName="SLA Weight" ValidationGroup="Report" />
                            </td>
                            <td>
                                <insite:NumericBox runat="server" ID="SLAWeight" NumericMode="Integer" MinValue="0" MaxValue="100" CssClass="w-50" />
                            </td>
                        </tr>
                    </table>
                </div>

                <div class="form-group">
                    <asp:Repeater runat="server" ID="RoleRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <tr>
                                    <th class="w-50">Reporting Tag</th>
                                    <th class="w-50">Weight</th>
                                </tr>
                        </HeaderTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Literal runat="server" ID="Name" Text='<%# Eval("Name") %>' />
                                </td>
                                <td>
                                    <insite:NumericBox runat="server"
                                        ID="Weight"
                                        NumericMode="Integer"
                                        MinValue="0"
                                        MaxValue="100"
                                        ValueAsInt='<%# Eval("Weight") %>'
                                        CssClass="w-50"
                                    />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>

    <div class="col-lg-6">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Content</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Nursing Role
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="NursingRoleText" MaxLength="100" />
                    </div>
                    <div class="form-text">
                        If not specified then will be used default text
                    </div>
                </div>

                <div class="form-group">
                    <label class="form-label">
                        Description
                    </label>
                    <div>
                        <insite:MarkdownEditor runat="server" ID="Description" />
                    </div>
                    <div class="form-text">
                        If not specified then will be used default text
                    </div>
                </div>
            </div>
        </div>
    </div>

</div>