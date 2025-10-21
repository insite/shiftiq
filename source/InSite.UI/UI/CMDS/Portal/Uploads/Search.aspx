<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.Cmds.Portal.Uploads.Search" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section>
        <h2 class="h4 mt-4 mb-3">
            Training Achievements: <asp:Literal runat="server" ID="ActiveCompanyName" />
        </h2>

        <p class="text-body-secondary">
            The following information achievements are specific to your organization. Please contact your administrator if you have any questions regarding this material.
        </p>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:Grid runat="server" ID="Downloads">
                    <Columns>
                        <asp:TemplateField HeaderText="Name" >
                            <ItemTemplate>
                                <a target="_blank" runat="server" href='<%# GetUploadUrl(Eval("UploadType"), Eval("ContainerIdentifier"), Eval("Name")) %>'><%# Eval("Title") %></a>
                                <span class="form-text">
                                    <%# (string)Eval("UploadType") == UploadType.CmdsFile ? ((int)Eval("ContentSize") / 1024).ToString("n0") + " KB" : string.Empty %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="Uploaded" DataFormatString="{0:MMM d, yyyy}" HeaderText="Updated" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="NumberOfResources"  HeaderText="Achievements" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="NumberOfCompetencies"  HeaderText="Competencies" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false" />
                    </Columns>
                </insite:Grid>

            </div>
        </div>
    </section>

</asp:Content>


