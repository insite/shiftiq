<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewEducationSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.ViewEducationSection" %>

<div class="card mb-3">
    <div class="card-body">

        <h4 id="education" class="card-title mb-3">Education and Training</h4>

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
        <insite:UpdatePanel runat="server" ID="UpdatePanel">
            <ContentTemplate>

                <div runat="server" id="NoEducationItems" class="alert alert-warning" role="alert">
                    No Education Item Added
                </div>

                <asp:Repeater runat="server" ID="EducationList">
                    <SeparatorTemplate><div class="mb-4"></div></SeparatorTemplate>
                    <ItemTemplate>
                        <div class="card">
                            <div class="card-body">

                                <h5 class="card-title mb-3">
                                    <%# GetYearString((DateTime?)Eval("EducationDateFrom")) %> - <%# GetYearString((DateTime?)Eval("EducationDateTo")) %>
                                    <%# EvalEducationStatus() %>
                                </h5>

                                <div class="card-text mb-3">
                                    <div class="row" id='item<%# Eval("EducationIdentifier") %>'>

                                        <div class="col-12">
                                            <div class="row">
                                                <div class="col-md-3">
                                                    <%# EvalEncode("EducationName") %>
                                                </div>

                                                <div class="col-md-3">
                                                    <%# EvalEncode("EducationInstitution") %>
                                                </div>

                                                <div class="col-md-3">
                                                    <%# EvalEncode("EducationQualification") %>
                                                </div>

                                                <div class="col-md-3">
                                                    <%# EvalAddress("EducationCity", "EducationCountry") %>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

            </ContentTemplate>
        </insite:UpdatePanel>

    </div>
</div>
