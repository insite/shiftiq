<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactUploadsGrid.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.Controls.ContactUploadsGrid" %>

<div>


    <div>
        <insite:Grid runat="server" ID="Grid" DataKeyNames="UploadIdentifier">
            <Columns>

                <asp:BoundField HeaderText="Type" DataField="UploadType"></asp:BoundField>
                <asp:BoundField HeaderText="MIME" DataField="UploadMime"></asp:BoundField>
                <asp:TemplateField HeaderText="Size (KB)">
                    <ItemTemplate>
                        <%# Convert.ToInt32(Eval("UploadSize")) / 1024 %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="UploadUrl">
                    <ItemTemplate>
                        <a href='<%# Eval("UploadUrl") %>'>url</a>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>

    </div>

</div>
