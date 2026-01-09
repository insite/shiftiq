<%@ Page Language="C#" CodeBehind="ChangeStandard.aspx.cs" Inherits="InSite.Admin.Assessments.Sets.Forms.ChangeStandard" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Banks/Controls/BankInfo.ascx" TagName="BankDetails" TagPrefix="uc" %>
<%@ Register Src="../Controls/SetInfo.ascx" TagName="SetDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            .standard-list {
                padding-left: 5px;
            }

                .standard-list table > tbody > tr > td > input {
                    position: absolute;
                    margin-top: 5px;
                }

                .standard-list table > tbody > tr > td > label {
                    padding-left: 25px;
                }
        </style>
    </insite:PageHeadContent>

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-th-list"></i>
            Change Standard
        </h2>

        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Question Set</h3>
                        <uc:SetDetails runat="server" ID="SetDetails" />

                        <div class="form-group mb-3">
                            <label class="form-label">Standard</label>
                            <div class="standard-list">
                                <asp:RadioButtonList runat="server" ID="StandardAssetID" />
                            </div>
                            <div class="form-text">
                                The standard evaluated by question items in this set.
                            </div>
                            <asp:Panel runat="server" ID="EmptyMessage" CssClass="alert alert-info mb-0" Visible="false">
                                There are no Standard assets.
                            </asp:Panel>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <div class="col-md-6">
                            <h3>Bank</h3>
                            <uc:BankDetails runat="server" ID="BankDetails" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>
