<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Cmds.Admin.Uploads.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:BoundField HeaderText="Object Type" DataField="ContainerType" />

        <asp:TemplateField HeaderText="Upload Type" >
            <ItemTemplate>
                <%# (string)Eval("UploadType") == UploadType.CmdsFile ? "File" : (string)Eval("UploadType") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:HyperLinkField HeaderText="File/Link Name" DataTextField="Title" DataNavigateUrlFields="UploadIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/uploads/edit?id={0}" />
        <asp:BoundField HeaderText="Uploaded" DataField="Uploaded" DataFormatString="{0:MMM d, yyyy}" ItemStyle-Wrap="false" />

    </Columns>
</insite:Grid>
