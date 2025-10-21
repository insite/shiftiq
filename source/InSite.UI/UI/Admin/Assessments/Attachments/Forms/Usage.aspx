<%@ Page Language="C#" CodeBehind="Usage.aspx.cs" Inherits="InSite.Admin.Assessments.Attachments.Forms.Usage" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="UsageHistory" Src="~/UI/Admin/Assessments/Attachments/Controls/UsageHistory.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionRepeater" Src="~/UI/Admin/Assessments/Questions/Controls/QuestionRepeater.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <asp:Repeater runat="server" ID="QuestionGroupRepeater">
        <ItemTemplate>
            <section class="mb-3">

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            <i class='<%# Eval("Icon", "far fa-{0} me-1") %>'></i>
                            <%# Eval("Title") %>
                            <span class='badge bg-info'><%# Eval("Count", "{0:n0}") %></span>
                        </h4>

                        <uc:QuestionRepeater runat="server" ID="QuestionRepeater" />

                    </div>
                </div>

            </section>
        </ItemTemplate>
    </asp:Repeater>

    <section class="mb-3">

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-history me-1"></i>
                    Usage History
                    <span runat="server" id="UsageHistoryCount" class='badge bg-info'></span>
                </h4>

                <uc:UsageHistory runat="server" ID="UsageHistory" />

            </div>
        </div>

    </section>

    <div class="mt-3">
        <insite:CloseButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
