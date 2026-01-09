<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.ValidationChanges.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="ChangeIdentifier">
    <Columns>
        <asp:TemplateField ItemStyle-Width="80px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" Type="Regular" ToolTip='Edit Validation Change'
                    NavigateUrl='<%# string.Format("/ui/cmds/admin/validations/changes/edit?id={0}", Eval("ChangeIdentifier")) %>' />

                <insite:IconButton runat="server" Name="trash-alt" ToolTip="Remove" 
                    CommandName="DeleteRecord"
                    ConfirmText="Are you sure you want to remove this validation change?" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Competency">
            <ItemTemplate>
                <%# Eval("StandardTitle") %>
                <%# Eval("StandardCode") != null ? Eval("StandardCode", "<span class=form-text>#{0}</span>") : "" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="User" DataField="UserFullname" />

        <asp:TemplateField HeaderText="Change">
            <ItemTemplate>
                <b>
                    <%# Eval("ChangeStatus") %>
                    <%# Eval("AuthorFullName") != null ? Eval("AuthorFullName", "by {0}") : null %>
                </b>
                on <%# GetLocalTime(Eval("ChangePosted")) %>:
                <div>
                    <%# Eval("ChangeComment") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>