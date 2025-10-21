<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewDocumentSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.ViewDocumentSection" %>

<div class="row mb-3">

    <div class="col-12">

        <div class="card">

            <div class="card-body">

                <h4 class="card-title mb-3">Documents</h4>

                <div class="row">
                    <div class="col-12">

                        <div class="form-group mb-3 px-3">
                            <asp:HyperLink ID="ResumeUrl" runat="server"></asp:HyperLink>
                        </div>

                        <div class="form-group mb-3 px-3">
                            <asp:HyperLink ID="CoverLetterUrl" runat="server"></asp:HyperLink>
                        </div>

                    </div>
                </div>

                <asp:Panel ID="ReferenceLettersPanel" runat="server" Visible="false">

                    <div class="row">

                    <h5 class="card-title mb-3">Reference Letter(s)</h5>

                    <div class="col-12">

                        <div class="form-group mb-3 px-3">

                            <asp:Panel ID="ReferenceLetterPanel1" runat="server" Visible="false">
                                <div class="mb-3">
                                    <asp:HyperLink ID="ReferenceLetterUrl1" runat="server"></asp:HyperLink>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="ReferenceLetterPanel2" runat="server" Visible="false">
                                <div class="mb-3">
                                    <asp:HyperLink ID="ReferenceLetterUrl2" runat="server"></asp:HyperLink>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="ReferenceLetterPanel3" runat="server" Visible="false">
                                <div class="mb-3">
                                <asp:HyperLink ID="ReferenceLetterUrl3" runat="server"></asp:HyperLink>
                            </div>
                            </asp:Panel>

                            <asp:Panel ID="ReferenceLetterPanel4" runat="server" Visible="false">
                                <div class="mb-3">
                                <asp:HyperLink ID="ReferenceLetterUrl4" runat="server"></asp:HyperLink>
                            </div>
                            </asp:Panel>

                            <asp:Panel ID="ReferenceLetterPanel5" runat="server" Visible="false">
                                <div class="mb-3">
                                <asp:HyperLink ID="ReferenceLetterUrl5" runat="server"></asp:HyperLink>
                            </div>
                            </asp:Panel>

                        </div>

                    </div>

                </div>

                </asp:Panel>

                <asp:Panel ID="CertificatesPanel" runat="server" Visible="false">

                    <div class="row">

                    <h5 class="card-title mb-3">Certificate(s)</h5>

                    <div class="col-12">

                        <div class="form-group mb-3 px-3">

                            <asp:Panel ID="CertificatePanel1" runat="server" Visible="false">
                                <div class="mb-3">
                                    <asp:HyperLink ID="CertificateUrl1" runat="server"></asp:HyperLink>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="CertificatePanel2" runat="server" Visible="false">
                                <div class="mb-3">
                                    <asp:HyperLink ID="CertificateUrl2" runat="server"></asp:HyperLink>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="CertificatePanel3" runat="server" Visible="false">
                                <div class="mb-3">
                                <asp:HyperLink ID="CertificateUrl3" runat="server"></asp:HyperLink>
                            </div>
                            </asp:Panel>

                            <asp:Panel ID="CertificatePanel4" runat="server" Visible="false">
                                <div class="mb-3">
                                    <asp:HyperLink ID="CertificateUrl4" runat="server"></asp:HyperLink>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="CertificatePanel5" runat="server" Visible="false">
                                <div class="mb-3">
                                <asp:HyperLink ID="CertificateUrl5" runat="server"></asp:HyperLink>
                            </div>
                            </asp:Panel>
                        </div>

                    </div>

                </div>

                </asp:Panel>

            </div>
            
        </div>

    </div>

</div>