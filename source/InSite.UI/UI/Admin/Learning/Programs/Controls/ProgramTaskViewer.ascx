<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramTaskViewer.ascx.cs" Inherits="InSite.UI.Admin.Records.Programs.Controls.ProgramTaskViewer" %>

<div class="form-group mb-3">
    <div class="float-end">
        <insite:IconLink runat="server" ID="TaskSearchLink" ToolTip="Search" Name="search" Target="_blank" />
    </div>
    <label runat="server" class="form-label" id="TaskLabel"></label>
    <div runat="server" id="HelperMsgPlaceholder" visible="false" class="form-text m-2 mt-1 text-info">
        <asp:Literal ID="HelperMsgText" runat="server"></asp:Literal>
    </div>
    <div>

        <asp:Repeater runat="server" ID="TaskRepeater">
            <ItemTemplate>
                <ul>
                    <li>
                        <asp:Literal runat="server" ID="TaskName" />
                        <asp:HiddenField runat="server" ID="TaskIdentifier" />
                        <asp:HiddenField runat="server" ID="ObjectType" />
                        <insite:IconLink runat="server" ID="EditLink" Name="pencil" ToolTip="Edit Task"
                            NavigateUrl='<%# string.Format("/ui/admin/learning/programs/tasks/edit?program={0}&task={1}", Eval("ProgramIdentifier"), Eval("TaskIdentifier")) %>' />
                        <ul>
                            <asp:Repeater runat="server" ID="ChildTaskRepeater">
                                <ItemTemplate>
                                    <li>
                                        <asp:Literal runat="server" ID="ChildTaskName" />
                                        <asp:HiddenField runat="server" ID="ChildTaskIdentifier" />
                                        <insite:IconLink runat="server" ID="EditLink" Name="pencil" ToolTip="Edit Task"
                                            NavigateUrl='<%# string.Format("/ui/admin/learning/programs/tasks/edit?program={0}&task={1}", Eval("ProgramIdentifier"), Eval("TaskIdentifier")) %>' />
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </li>
                </ul>
            </ItemTemplate>
        </asp:Repeater>

        <span class="form-text">
            <asp:Literal ID="NoTasks" runat="server" Text="This task list is empty. Click the Edit button to add tasks."></asp:Literal>
        </span>

    </div>
</div>
