<%@ Page Language="C#" CodeBehind="AddCompetencies.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.AddCompetencies" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:Alert runat="server" ID="NoCompetenciesAlert" />

    <section runat="server" id="MainPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Add Competencies
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h3><asp:Literal runat="server" ID="FrameworkTitle" /></h3>

                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th class="root-checkbox">
                                <asp:CheckBox runat="server" />
                            </th>
                            <th colspan="2">
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="AreaRepeater">
                            <ItemTemplate>
                                <tr>
                                    <td class="area-checkbox" style="width:20px;" data-identifier='<%# Eval("Identifier") %>'>
                                        <asp:CheckBox runat="server" />
                                    </td>
                                    <td colspan="2">
                                        <b><a href='<%# "/ui/admin/standards/edit?id=" + Eval("Identifier") %>'><%# Eval("Name") %> </a></b>
                                    </td>
                                </tr>

                                <asp:Repeater runat="server" ID="CompetencyRepeater">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>
                                            <td class="competency-checkbox" style="width:20px;" data-area='<%# AreaIdentifier %>'>
                                                <asp:CheckBox runat="server" ID="Selected" />
                                                <asp:Literal runat="server" ID="Identifier" Visible="false" Text='<%# Eval("Identifier") %>' />
                                            </td>
                                            <td>
                                                <a href='<%# "/ui/admin/standards/edit?id=" + Eval("Identifier") %>'><%# Eval("Name") %> </a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>

            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

<insite:PageFooterContent runat="server">
<script type="text/javascript">
    (function () {
        $(".root-checkbox input[type=checkbox]").on("click", function () {
            var $this = $(this);
            var checked = this.checked;

            $this.closest("table").find("input[type=checkbox]").prop("checked", checked);
        });

        $(".area-checkbox input[type=checkbox]").on("click", function () {
            var $this = $(this);
            var identifier = $this.parent().data("identifier");
            var checked = this.checked;

            $this.closest("table").find("td[data-area='" + identifier + "'] input[type=checkbox]").prop("checked", checked);
        });
    })();
</script>
</insite:PageFooterContent>
</asp:Content>
