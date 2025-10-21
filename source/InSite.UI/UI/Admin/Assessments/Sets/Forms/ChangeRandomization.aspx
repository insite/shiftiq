<%@ Page Language="C#" CodeBehind="ChangeRandomization.aspx.cs" Inherits="InSite.Admin.Assessments.Sets.Forms.ChangeRandomization" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="RandomizationInput" Src="../../Questions/Controls/RandomizationInput.ascx" %>
<%@ Register Src="~/UI/Admin/Assessments/Banks/Controls/BankInfo.ascx" TagName="BankDetails" TagPrefix="uc" %>
<%@ Register Src="../Controls/SetInfo.ascx" TagName="SetDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-th-list"></i>
            Question Set Randomization
        </h2>

        <div class="row mb-3">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Question Set</h3>
                        <uc:SetDetails runat="server" ID="SetDetails" />

                        <div class="form-group mb-3">
                            <label class="form-label">List or Shuffle</label>
                            <div>
                                <uc:RandomizationInput runat="server" ID="SetRandomizationDetails" />
                            </div>
                            <div class="form-text">Questions may be displayed in the configuration sequence or in random order.</div>
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

        <div class="row">

            <div class="col-lg-12" runat="server" id="QuestionRandomizationSection">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Question Item Randomization</h3>
                        <asp:Repeater runat="server" ID="QuestionRepeater">
                            <HeaderTemplate>
                                <table class="table table-striped">
                                    <tbody>
                            </HeaderTemplate>
                            <FooterTemplate></tbody></table></FooterTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td style="width: 55px;"><strong>
                                        <asp:Literal runat="server" ID="BankSequence" /></strong></td>
                                    <td><span style="white-space: pre-wrap;">
                                        <asp:Literal runat="server" ID="Title" /></span></td>
                                    <td class="w-25">
                                        <uc:RandomizationInput runat="server" ID="RandomizationDetails" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </div>
        </div>

    </section>


    <div class="row">
        <div class="col-md-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>


</asp:Content>
