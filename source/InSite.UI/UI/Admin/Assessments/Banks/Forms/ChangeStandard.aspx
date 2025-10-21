<%@ Page Language="C#" CodeBehind="ChangeStandard.aspx.cs" Inherits="InSite.Admin.Assessments.Banks.Forms.ChangeStandard" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/StandardSelector.ascx" TagName="StandardSelector" TagPrefix="uc" %>
<%@ Register Src="../Controls/BankInfo.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-balance-scale"></i>
            Change Bank Standard
        </h2>

        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>

                        <uc:Details runat="server" ID="BankDetails" />

                        <div class="form-group mb-3">
                            <label class="form-label">Standard</label>
                            <div class="standard-list">
                                <uc:StandardSelector runat="server" ID="BankStandard" AssetType="Standard" AssetSubtype="Framework" />
                            </div>
                            <div class="form-text">
                                Select the asset that contains the standards you want to evaluate with questions in this bank.
                            </div>
                            <asp:Panel runat="server" ID="EmptyMessage" CssClass="alert alert-info" Style="margin-bottom: 0;" Visible="false">
                                There are no Standard assets.
                            </asp:Panel>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-lg-6" runat="server" id="QuestionSetColumn">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Create Question Sets</h3>

                        <div class="form-group mb-3">
                            <div class="form-text">
                                Select GACs to create empty question sets for each.
                            </div>
                        </div>

                        <asp:Repeater runat="server" ID="QuestionSetRepeater">
                            <ItemTemplate>
                                <insite:CheckBox runat="server" ID="QuestionSetCheckBox" Value='<%# Eval("Value") %>' Text='<%# Eval("Text") %>' Enabled='<%# (bool)Eval("Enabled") %>' Checked='<%# (bool)Eval("Checked") %>' />
                            </ItemTemplate>
                        </asp:Repeater>

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
