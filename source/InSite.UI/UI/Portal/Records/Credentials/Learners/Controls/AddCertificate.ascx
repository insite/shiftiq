<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddCertificate.ascx.cs" Inherits="InSite.UI.Portal.Records.Credentials.Learners.Controls.AddCertificate" %>

<div class="row">

    <div class="col-lg-6">

        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">

                <asp:MultiView runat="server" ID="Wizard" ActiveViewIndex="0">
                    
                    <asp:View runat="server">

                        <insite:Alert runat="server" ID="ValidationStatus" Indicator="Error" Visible="false" />

                        <h3>Step 1 of 2</h3>

                        <div class="form-group mb-3">
                            <span class="float-end text-danger fs-sm">* Required</span>
                            <label class="form-label">Achievement</label>
                            <div>
                                <cmds:FindAchievement ID="AchievementSelector" runat="server" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <span class="float-end text-danger fs-sm">* Required</span>
                            <label class="form-label">Issued</label>
                            <div>
                                <insite:DateSelector ID="AchievementIssued" runat="server" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <span class="float-end text-danger fs-sm">* Required</span>
                            <label class="form-label">Valid for (months)</label>
                            <div>
                                <insite:NumericBox ID="AchievementLifetime" runat="server" NumericMode="Integer" MinValue="1" MaxValue="120" CssClass="w-25" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <span class="float-end text-info fs-sm">*.jpg, *.pdf, or *.png</span>
                            <insite:FileUploadV2 runat="server" ID="AchievementFile" LabelText="Select a file to attach" AllowedExtensions=".jpg,.pdf,.png" FileUploadType="Image" />
                        </div>

                        <insite:NextButton runat="server" ID="NextButton" />

                    </asp:View>

                    <asp:View runat="server">

                        <h3>Step 2 of 2</h3>

                        <div class="alert alert-info">Please confirm the following details, then click Save to finish adding your new certificate.</div>

                        <dl>
                            <dt>Achievement</dt>
                            <dd>
                                <asp:Label runat="server" ID="ConfirmAchievement" /></dd>
                            <dt>Issued</dt>
                            <dd>
                                <asp:Label runat="server" ID="ConfirmIssued" /></dd>
                            <dt>Expiry</dt>
                            <dd>
                                <asp:Label runat="server" ID="ConfirmExpiry" /></dd>
                            <dt>Attachment</dt>
                            <dd>
                                <asp:Label runat="server" ID="ConfirmFile" /></dd>
                        </dl>

                        <insite:SaveButton runat="server" ID="SaveButton" />
                        <insite:CancelButton runat="server" ID="CancelButton" />

                    </asp:View>

                </asp:MultiView>
                
            </div>

        </div>

    </div>

    <div class="col-lg-6">

        <insite:Alert runat="server" ID="EmbeddedHelp" />

    </div>

</div>
