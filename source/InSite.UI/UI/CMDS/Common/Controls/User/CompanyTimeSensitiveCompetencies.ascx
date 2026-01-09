<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompanyTimeSensitiveCompetencies.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Companies.CompanyTimeSensitiveCompetencies" %>

<div class="row">
    <div class="col-lg-6 mb-3 mb-lg-0">

        <div class="card">
            <div class="card-header">
                Time-Sensitive Competencies
            </div>
            <div class="card-body">

                <div class="mb-3">
                    You can configure time-sensitive competencies to expire automatically on a specific date or after a
                    specific period of time has elapsed from the date of validation.
                </div>

                <div class="form-group mb-3">
                    <asp:RadioButtonList runat="server" ID="TimeSensitiveCompetencyExpiry" CssClass="radiolist-tsce">
                        <asp:ListItem Value="None" Text="Disable automatic expiration of time-sensitive competencies" Selected="True" />
                        <asp:ListItem Value="Interval" Text="Automatically expire the time sensitive competencies after a specific period of time has elapsed from the date of validation" />
                        <asp:ListItem Value="Date" Text="Automatically expire time-sensitive competencies on a specific date" />
                    </asp:RadioButtonList>
                </div>

                <div runat="server" id="SpecificDateContainer" class="row mb-3" visible="false">
                    <div class="col-6">
                        <insite:ComboBox runat="server" ID="SpecificMonth">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="1" Text="January" />
                                <insite:ComboBoxOption Value="2" Text="February" />
                                <insite:ComboBoxOption Value="3" Text="March" />
                                <insite:ComboBoxOption Value="4" Text="April" />
                                <insite:ComboBoxOption Value="5" Text="May" />
                                <insite:ComboBoxOption Value="6" Text="June" />
                                <insite:ComboBoxOption Value="7" Text="July" />
                                <insite:ComboBoxOption Value="8" Text="August" />
                                <insite:ComboBoxOption Value="9" Text="September" />
                                <insite:ComboBoxOption Value="10" Text="October" />
                                <insite:ComboBoxOption Value="11" Text="November" />
                                <insite:ComboBoxOption Value="12" Text="December" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                    <div class="col-6">
                        <insite:ComboBox runat="server" ID="SpecificDay">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="1" Text="1" />
                                <insite:ComboBoxOption Value="2" Text="2" />
                                <insite:ComboBoxOption Value="3" Text="3" />
                                <insite:ComboBoxOption Value="4" Text="4" />
                                <insite:ComboBoxOption Value="5" Text="5" />
                                <insite:ComboBoxOption Value="6" Text="6" />
                                <insite:ComboBoxOption Value="7" Text="7" />
                                <insite:ComboBoxOption Value="8" Text="8" />
                                <insite:ComboBoxOption Value="9" Text="9" />
                                <insite:ComboBoxOption Value="10" Text="10" />
                                <insite:ComboBoxOption Value="11" Text="11" />
                                <insite:ComboBoxOption Value="12" Text="12" />
                                <insite:ComboBoxOption Value="13" Text="13" />
                                <insite:ComboBoxOption Value="14" Text="14" />
                                <insite:ComboBoxOption Value="15" Text="15" />
                                <insite:ComboBoxOption Value="16" Text="16" />
                                <insite:ComboBoxOption Value="17" Text="17" />
                                <insite:ComboBoxOption Value="18" Text="18" />
                                <insite:ComboBoxOption Value="19" Text="19" />
                                <insite:ComboBoxOption Value="20" Text="20" />
                                <insite:ComboBoxOption Value="21" Text="21" />
                                <insite:ComboBoxOption Value="22" Text="22" />
                                <insite:ComboBoxOption Value="23" Text="23" />
                                <insite:ComboBoxOption Value="24" Text="24" />
                                <insite:ComboBoxOption Value="25" Text="25" />
                                <insite:ComboBoxOption Value="26" Text="26" />
                                <insite:ComboBoxOption Value="27" Text="27" />
                                <insite:ComboBoxOption Value="28" Text="28" />
                                <insite:ComboBoxOption Value="29" Text="29" />
                                <insite:ComboBoxOption Value="30" Text="30" />
                                <insite:ComboBoxOption Value="31" Text="31" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>

            </div>
        </div>

    </div>
    <div class="col-lg-6">

        <div class="card h-100">
            <div class="card-header">
                Compare Profile Competencies to Organization Competencies
            </div>
            <div class="card-body">

                <div class="mb-3">
                    Click the '<strong>Check Competencies</strong>' button to check each of the profiles assigned to this organization,
                    ensuring all of the competencies assigned to each profile are also assigned to this organization.
                </div>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                <insite:UpdatePanel runat="server" ID="UpdatePanel">
                    <ContentTemplate>
                        <div class="mb-3">
                            <cmds:CmdsButton runat="server" ID="CheckCompetenciesButton" Text="<i class='far fa-ruler-triangle me-1'></i> Check Competencies" />
                        </div>
                        <div>
                            <asp:Literal runat="server" ID="ResultLiteral" />
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>
            </div>
        </div>

    </div>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .radiolist-tsce input[type=radio] {
            position: absolute;
            margin-top: 5px;
        }

            .radiolist-tsce input[type=radio] + label {
                padding-left: 22px;
            }
    </style>
</insite:PageHeadContent>