<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportDownloadSection.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.ReportDownloadSection" %>

<div class="row">
                            
    <div class="col-lg-6">
        <h3>Output Settings</h3>

        <div class="form-group mb-3">
            <label class="form-label">Text File Encoding</label>
            <div>
                <asp:RadioButtonList runat="server" ID="EncodingSelector" RepeatDirection="Vertical" RepeatLayout="Flow">
                    <asp:ListItem Value="UTF-8" Selected="True" />
                    <asp:ListItem Value="Unicode" />
                </asp:RadioButtonList>
            </div>
            <div class="form-text clear">Select the character encoding you want to use for the output files.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Download Options</label>
            <div>
                <asp:CheckBox runat="server" ID="IncludeAdditionalFiles" Text="Include additional metadata files" Checked="True" />
                <div class="form-text">Check this box to include additional files that describe each question, field, and option in the form.</div>
            </div>
            <div class="text-nowrap">
                <asp:RadioButtonList runat="server" ID="OptionFormat" RepeatDirection="Vertical" RepeatLayout="Flow">
                    <asp:ListItem Value="Text" Text="Do not encode answer options; download the text displayed to respondents" Selected="True" />
                    <asp:ListItem Value="Code" Text="Encode answer options as alphabetic codes (A, B, C, ...)" />
                    <asp:ListItem Value="Number" Text="Encode answer options as numbers (1, 2, 3, ...)" />
                </asp:RadioButtonList>
                <div class="form-text">Choose the setting to generate an output format that meets your needs.</div>
            </div>
        </div>

        <div class="form-group mb-3">
            <insite:DownloadButton runat="server" ID="DownloadButton" />
        </div>
    </div>

</div>
