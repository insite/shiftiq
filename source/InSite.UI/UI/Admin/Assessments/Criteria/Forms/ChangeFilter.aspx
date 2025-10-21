<%@ Page Language="C#" CodeBehind="ChangeFilter.aspx.cs" Inherits="InSite.Admin.Assessments.Criteria.Forms.ChangeFilter" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="CriterionInput" Src="../Controls/CriterionInput.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-clipboard-list"></i>
            Change Filter
        </h2>

        <div class="row mb-3">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Specification</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Specification Name</label>
                            <div>
                                <asp:Literal runat="server" ID="SpecificationName" />
                            </div>
                            <div class="form-text">
                                The specification that contains the new criteria.
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Question Sets</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Set Names
                            </label>
                            <div>
                                <asp:Repeater runat="server" ID="SetRepeater">
                                    <HeaderTemplate>
                                        <ul>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <%# Eval("Name") %>
                                            <span class="form-text">&mdash;
                                            <%# Eval("Questions.Count", "{0:n0}") %> questions
                                            </span>
                                        </li>
                                    </ItemTemplate>
                                    <FooterTemplate></ul></FooterTemplate>
                                </asp:Repeater>
                            </div>
                            <div class="form-text">
                                The sets from which questions are selected and filtered for this criterion.
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>

        <uc:CriterionInput runat="server" ID="CriterionInput" />

    </section>


    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" CausesValidation="true" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
