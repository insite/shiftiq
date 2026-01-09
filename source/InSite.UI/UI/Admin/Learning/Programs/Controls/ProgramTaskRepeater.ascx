<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramTaskRepeater.ascx.cs" Inherits="InSite.UI.Admin.Records.Programs.Controls.ProgramTaskRepeater" %>

<div class="form-group mb-3">
    <div class="float-end">
        <insite:IconLink runat="server" id="TaskSearchLink" ToolTip="Search" Name="search" Target="_blank"  />
    </div>
    <label runat="server" class="form-label" id="TaskLabel"></label>
    <div runat="server" id="HelperMsgPlaceholder" visible="false" class="form-text m-2 mt-1 text-info">
        <asp:Literal ID="HelperMsgText" runat="server"></asp:Literal>
    </div>
    <div>
        <asp:Repeater runat="server" ID="TaskRepeater">
            <ItemTemplate>
                <div>
                    <asp:CheckBox runat="server" ID="TaskName" Enabled='<%# !Disabled %>'/>
                    <asp:HiddenField runat="server" ID="TaskIdentifier" />
                    <asp:HiddenField runat="server" ID="ObjectIdentifier" />
                    <asp:HiddenField runat="server" ID="ObjectType" />
                    <asp:Repeater runat="server" ID="ChildTaskRepeater">
                        <ItemTemplate>
                            <div class="px-4">
                                <asp:CheckBox runat="server" ID="ChildTaskName"/>
                                <asp:HiddenField runat="server" ID="ChildTaskIdentifier" />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
