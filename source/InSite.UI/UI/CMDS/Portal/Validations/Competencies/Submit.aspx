<%@ Page Language="C#" CodeBehind="Submit.aspx.cs" Inherits="InSite.Cmds.Actions.Talent.Employee.Competency.Assessment.Validate" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section class="mb-3">

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div runat="server" id="NoCompetenciesForValidation">
                    You do not have any self-assessed competencies ready for validation.
                </div>

                <asp:Panel ID="CompetenciesForValidation" runat="server">

                    <p>Here is the list of competencies for which you have assessed your skills. Please click the Submit button to request validation of these competencies. </p>

                    <table class="table table-striped">

                        <thead>
                            <tr>
                                <th>Competency</th>
                                <th>Summary</th>
                                <th class="text-center text-nowrap">Self-Assessment</th>
                                <th class="text-center">Validation</th>
                            </tr>
                        </thead>
            
                        <tbody>
            
                            <asp:Repeater ID="rpCredentials" runat="server">
                                <ItemTemplate>
                                    <tr class="<%# RowCssClass %>">
                                        <td>
                                            <a runat="server" href='<%# GetUrl(Eval("CompetencyStandardIdentifier")) %>'>
                                                <%# Eval("Number") %>
                                            </a>
                                        </td>
                                        <td>
                                            <a runat="server" href='<%# GetUrl(Eval("CompetencyStandardIdentifier")) %>'>
                                                <%# Eval("Summary") %>
                                            </a>
                                        </td>
                                        <td class="text-center">
                                            <%# Eval("SelfAssessmentDate") == DBNull.Value ? "-" : Shift.Common.TimeZones.Format((DateTimeOffset)Eval("SelfAssessmentDate"), User.TimeZone) %>
                                            <div class="form-text"><%# Eval("SelfAssessmentStatus") %></div>
                                        </td>
                                        <td class="text-center">
                                            <%# Eval("ValidationDate") == DBNull.Value ? "-" : Shift.Common.TimeZones.Format((DateTimeOffset)Eval("ValidationDate"), User.TimeZone) %>
                                            <div class="form-text"><%# Eval("ValidationStatus") %></div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                        </tbody>
                    </table>

                </asp:Panel>

            </div>
        </div>
    </section>

    <insite:SaveButton runat="server" ID="SaveButton" Text="Submit for Validation" DisableAfterClick="true" />

</asp:Content>
