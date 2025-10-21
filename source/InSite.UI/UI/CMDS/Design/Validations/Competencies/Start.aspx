<%@ Page Language="C#" CodeBehind="Start.aspx.cs" Inherits="InSite.Cmds.Actions.Talent.Employee.Competency.Validation.Summary" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <p runat="server" id="NoItems">
                    There are no competencies submitted for your validation.
                </p>

                <insite:Grid runat="server" ID="SummaryGrid">
                        <Columns>

                            <asp:TemplateField HeaderText="Employee">
                                <ItemTemplate>
                                    <a runat="server" href='<%# GetCompetencyLink(Eval("UserIdentifier")) %>'>
                                        <%# Eval("FullName") %>
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Competencies">
                                <ItemTemplate>
                                    <%# Eval("ItemCount") %>
                                    competencies to be validated
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                </insite:Grid>
            </div>
        </div>
    </section>

</asp:Content>
