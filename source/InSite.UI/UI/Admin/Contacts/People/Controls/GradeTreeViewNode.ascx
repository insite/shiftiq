<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradeTreeViewNode.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.GradeTreeViewNode" %>

<asp:Repeater runat="server" ID="NodeRepeater">
    <ItemTemplate>
        <li class="outline-item" data-key='<%# Eval("ID") %>'>
            <div>
                <div>
                    <div class="node-title">
                        <table style="width:100%;">
                            <tr>
                                <td>
                                    <%# Eval("Name") %>
                                    <div runat="server" visible='<%# Eval("ClassName") != null %>' class="form-text">
                                        Class  <b><%# Eval("ClassName") %></b>
                                        scheduled on <%# GetLocalTime(Eval("ClassStartDate")) %> - <%# GetLocalTime(Eval("ClassEndDate")) %>
                                    </div>
                                    <div class="form-text"><%# Eval("Comment") %></div>
                                </td>
                                <td style="width:150px;">
                                    <%# Eval("ScoreValue") %>
                                </td>
                            </tr>
                        </table>                        
                    </div>
                </div>
            </div>

            <ul runat="server" id="ChildNodes" class='tree-view'>

            </ul>
        </li>
    </ItemTemplate>
</asp:Repeater>