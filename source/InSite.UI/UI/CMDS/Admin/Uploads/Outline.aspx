<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Cmds.Actions.Contact.Company.Achievement.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="UploadSection.ascx" TagName="UploadSection" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <style type="text/css">

        td.resource-group-list { vertical-align:top; padding-right:20px; }
        table.resource th.resource-type-heading { font-size: 20px; font-variant: small-caps; letter-spacing: 1px; padding-bottom: 10px; }

        table.resource td { vertical-align: top; padding: 5px 5px 5px 25px; }
        table.resource td ul { padding: 0 0 10px 30px; margin: 0; }
        table.resource tr.group strong { color: #666; font-size: 16px; }

    </style>

    <div runat="server" id="NoDownloadsPanel" class="card">
        <div class="card-body">
            <p>No uploaded achievements.</p>
        </div>
    </div>

    <section runat="server" id="MainSection">
        <div class="row">
            <div class="col-lg-4 tablist">
                <ul class="nav nav-pills flex-column" role="tablist">
                    <asp:Repeater runat="server" ID="SectionRepeater">
                        <ItemTemplate>

                            <li class="nav-item" role="presentation">
                                <a
                                    class='nav-link <%# Container.ItemIndex == 0 ? "active": "" %>'
                                    href='<%# "#vt" + Container.ItemIndex %>'
                                    data-bs-toggle="tab"
                                    role="tab"
                                    aria-controls='<%# "#vt" + Container.ItemIndex %>'
                                    aria-selected='<%# Container.ItemIndex == 0 ? "true": "" %>'
                                >
                                    <%# Eval("Title") ?? "Other Category" %>
                                </a>
                            </li>

                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
            <div class="col-lg-8 tab-content">
                <asp:Repeater runat="server" ID="SectionBodyRepeater">
                    <ItemTemplate>
                        <div id='<%# "vt" + Container.ItemIndex %>' class='tab-pane fade <%# Container.ItemIndex == 0 ? "show active": "" %>' role="tabpanel">

                            <div class="card">
                                <div class="card-body">
                                    <h3>
                                        <%# Eval("Title") ?? "Other Category" %>
                                    </h3>

                                    <uc:UploadSection runat="server" ID="Section" />
                                </div>
                            </div>

                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>

    <insite:PageFooterContent runat="server">
        <script>
            (() => {
                document.querySelectorAll(".tablist li").forEach(li => {
                    li.addEventListener("click", () => {
                        window.scrollTo(0, 0);
                    })
                })
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
