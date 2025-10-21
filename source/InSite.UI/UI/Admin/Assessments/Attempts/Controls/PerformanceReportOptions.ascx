<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PerformanceReportOptions.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.PerformanceReportOptions" %>

<h3>Report Options</h3>

<asp:CustomValidator runat="server" ID="ReportValidator" ErrorMessage="Roles to include are required" Display="None" ValidationGroup="Report" />

<div class="form-group mb-3">
    <label class="form-label">
        Select which Roles to include
    </label>
    <div>
        <asp:Repeater runat="server" ID="ReportRepeater">
            <ItemTemplate>
                <insite:RadioButton runat="server"
                    ID="Selected"
                    Text='<%# Container.DataItem %>'
                    Value="<%# Container.ItemIndex %>"
                    Checked="<%# Container.ItemIndex == 0 %>"
                    GroupName="Report"
                />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Apply the following weights to the Assessment Types
    </label>
    <div>
        <asp:Repeater runat="server" ID="AssessmentTypeRepeater">
            <ItemTemplate>
                <div class="hstack mt-2">
                    <insite:NumericBox runat="server"
                        ID="Weight"
                        MinValue="0"
                        MaxValue="100"
                        NumericMode="Integer"
                        CssClass="w-25 text-end"
                        ValueAsInt='<%# Eval("Weight") %>'
                    />
                    %
                    <%# Eval("Name") %>
                
                    <insite:RequiredValidator runat="server" FieldName="Weight" ControlToValidate="Weight" ValidationGroup="Report" />
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>