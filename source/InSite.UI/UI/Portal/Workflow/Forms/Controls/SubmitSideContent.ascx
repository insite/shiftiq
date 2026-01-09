<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubmitSideContent.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.SubmitSideContent" %>

<h3 runat="server" id="MenuHeading" class="rounded-top d-block bg-secondary fs-sm fw-semibold text-body-secondary mb-0 px-4 py-3">Help</h3>

<div class="p-4">
    <asp:HyperLink runat="server" ID="ReturnToCourseButton" Visible="false" CssClass="dropdown-item"><i class="me-1 fas fa-chevron-double-left"></i> Return to Course</asp:HyperLink>
</div>

<asp:Repeater runat="server" ID="SidebarUnitRepeater">
    <ItemTemplate>

        <div class="widget mb-4 alert alert-light shadow">
            <h3 runat="server" id="UnitName" visible="False"></h3>

            <asp:Repeater runat="server" ID="ModuleRepeater">
                <ItemTemplate>
                                        
                    <h4 class="widget-title mb-2 text-primary"><%# Eval("Name") %></h4>
                    <table class="table fs-sm">
                        <asp:Repeater runat="server" ID="ActivityRepeater">
                            <ItemTemplate>
                                <tr>
                                    <td style="padding-right: 0px !important;" >
                                        <%# Eval("ActivityIcon") %>
                                    </td>
                                    <td style="padding-left: 0px !important; width: 100%">
                                        <%# Eval("ActivityName") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                                        
                </ItemTemplate>
            </asp:Repeater>

        </div>

    </ItemTemplate>
</asp:Repeater>

<div runat="server" id="ProgramPanel" class="mt-4 alert alert-light shadow" visible="false">
    <h3 runat="server" id="ProgramName" class="widget-title mb-2">Program</h3>
    <div runat="server" id="ProgramNext" class="mb-2">
        <div class="fs-6 text-body-secondary">Next Course:</div>
        <a runat="server" id="ProgramNextLink" href="#"></a>
    </div>
    <div runat="server" id="ProgramPrev">
        <div class="fs-6 text-body-secondary">Previous Course:</div>
        <a runat="server" id="ProgramPrevLink" href="#"></a>
    </div>
</div>
