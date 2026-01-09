<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditTabOther.ascx.cs" Inherits="InSite.UI.Admin.Contacts.People.Controls.EditTabOther" %>

<div class="clearfix" ></div>
<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="MetadataUpdatePanel" />

<insite:UpdatePanel runat="server" ID="MetadataUpdatePanel">
    <ContentTemplate>
        <div class="row">
            <div class="col-lg-4 mb-3 mb-lg-0">
                <div class="card h-100">
                    <div class="card-body">

                        <h3>Miscellaneous</h3>
                        
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Preferred Language
                            </label>
                            <insite:LanguageComboBox runat="server" ID="Language" AllowBlank="false" />
                            <div class="form-text">
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Time Zone
                                <insite:RequiredValidator runat="server" ControlToValidate="TimeZone" FieldName="Time Zone" ValidationGroup="Person" Display="Dynamic" />
                            </label>
                            <insite:TimeZoneComboBox runat="server" ID="TimeZone" />
                            <div class="form-text">
                                All time zones in Canada and United States are supported. 
                                Contact us if you need another time zone added to this list.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Honorific
                            </label>
                            <insite:HonorificComboBox runat="server" ID="Honorific" />
                            <div class="form-text">
                                An <a target="_blank" href="https://en.wikipedia.org/wiki/Honorific">honorific</a> is a title that conveys esteem or respect for position or rank when used in addressing or referring to a person.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Social Insurance Number
                            </label>
                            <insite:TextBox ID="SocialInsuranceNumber" runat="server" MaxLength="16" />
                            <div class="form-text">
                                <asp:Literal runat="server" ID="SinModified" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
            <div runat="server" id="OrganizationColumn" class="col-lg-4 mb-3 mb-lg-0">
                <div class="card h-100">
                    <div class="card-body">

                        <h3 runat="server" id="OrganizationHeading"></h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Region
                            </label>
                            <insite:ItemNameComboBox runat="server" ID="RegionCombo" />
                            <insite:TextBox runat="server" ID="RegionInput" MaxLength="50" />
                            <div class="form-text"></div>
                        </div>

                        <asp:MultiView runat="server" ID="OrganizationMultiView">

                            <asp:View runat="server" ID="AssociationView">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Shipping Preference
                                    </label>
                                    <insite:TextBox ID="ShippingPreference" runat="server" MaxLength="20" />
                                    <div class="form-text">
                                    </div>
                                </div>
                            </asp:View>

                            <asp:View runat="server" ID="RcabcView">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Website URL
                                    </label>
                                    <insite:TextBox ID="WebSiteUrl" runat="server" MaxLength="500" />
                                    <div class="form-text">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Union Info
                                    </label>
                                    <insite:TextBox ID="UnionInfoRcabc" runat="server" MaxLength="32" />
                                    <div class="form-text">
                                        Local # if applicable
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    English Language Learner <asp:CheckBox ID="IsEnglishLearner" runat="server" Text="" />
                                    <div class="form-text">
                                    </div>
                                </div>
                            </asp:View>

                            <asp:View runat="server" ID="NcasView">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Regulator ID
                                    </label>
                                    <insite:TextBox ID="TradeworkerNumberNcas" runat="server" MaxLength="20" />
                                    <div class="form-text">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Origin of Nursing
                                    </label>
                                    <insite:TextBox ID="CredentialingCountry" runat="server" MaxLength="100" />
                                    <div class="form-text">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Reason for Referral
                                    </label>
                                    <div>
                                        <insite:CollectionItemComboBox runat="server" ID="ReferrerIdentifier" />
                                    </div>
                                </div>

                            </asp:View>

                        </asp:MultiView>

                    </div>
                </div>
            </div>
            <div class="col-lg-4 mb-3 mb-lg-0">
                <div runat="server" id="PersonFieldCard" class="card h-100">
                    <div class="card-body">

                        <asp:Repeater runat="server" ID="PersonFieldRepeater">
                            <HeaderTemplate>
                                <h3>Other Form Submissions</h3>
                            </HeaderTemplate>
                            <ItemTemplate>
                                        
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        <%# Eval("FieldName") %>
                                    </label>
                                    <div>
                                        <%# Eval("FieldValue") %>
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>
