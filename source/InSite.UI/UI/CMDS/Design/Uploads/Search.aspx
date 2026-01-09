<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.Cmds.Design.Uploads.Search" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" Title="Files and Links" Icon="far fa-paperclip" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Files and Links
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:Grid runat="server" ID="Downloads" DataKeyNames="UploadIdentifier">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="40px" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <insite:IconLink runat="server" Type="Regular" Name="pencil" ToolTip="Edit File"
                                            NavigateUrl='/ui/cmds/design/uploads/edit?id=<%# Eval("UploadIdentifier") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                
                                <asp:TemplateField HeaderText="Name" >
                                    <ItemTemplate>
                                        <a target="_blank" runat="server" href='<%# GetUploadUrl(Eval("UploadType"), Eval("ContainerIdentifier"), Eval("Name")) %>'><%# Eval("Title") %></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                
                                <asp:TemplateField HeaderText="Type" >
                                    <ItemTemplate>
                                        <%# (string)Eval("UploadType") == UploadType.CmdsFile ? "File" : (string)Eval("UploadType") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                
                                <asp:BoundField DataField="Description" HeaderText="Description" />

                                <asp:TemplateField HeaderText="Size" >
                                    <ItemTemplate>
                                        <%# (string)Eval("UploadType") == UploadType.CmdsFile ? ((int)Eval("ContentSize") / 1024).ToString("n0") + " KB" : string.Empty %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="Uploaded" DataFormatString="{0:MMM d, yyyy}" HeaderText="Last Updated" />
                                <asp:BoundField DataField="NumberOfCompetencies"  HeaderText="# of Competencies" ItemStyle-HorizontalAlign="Center" />

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <cmds:IconButton runat="server" IsFontIcon="true" CssClass="trash-alt" ToolTip="Delete File" ConfirmText="Are you sure you want to delete this file?" CommandArgument='<%# Eval("UploadIdentifier") %>' CommandName="DeleteFile" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </insite:Grid>

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" Title="Upload Files and Links" Icon="far fa-upload" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Upload Files and Links
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Upload Type
                            </label>
                            <div>
                                <asp:RadioButtonList ID="SubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                    <asp:ListItem Value="File" Text="Files" Selected="True" />
                                    <asp:ListItem Value="Link" Text="Links" />
                                </asp:RadioButtonList>
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <asp:MultiView ID="UploadPanels" runat="server" ActiveViewIndex="0">
                    
                            <asp:View ID="FilesPanel" runat="server">

                                <insite:FileUploadV1 runat="server" ID="FileUpload" AllowMultiple="true" FileUploadType="Unlimited" Width="500px" />

                            </asp:View>

                            <asp:View ID="LinksPanel" runat="server">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        URL
                                        <insite:RequiredValidator runat="server" ControlToValidate="NavigationUrl" FieldName="URL" ValidationGroup="UploadFile" />
                                    </label>
                                    <insite:TextBox ID="NavigationUrl" runat="server" MaxLength="500" Width="400" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Title
                                        <insite:RequiredValidator runat="server" ControlToValidate="UrlTitle" FieldName="Title" ValidationGroup="UploadFile" />
                                    </label>
                                    <insite:TextBox ID="UrlTitle" runat="server" MaxLength="256" Width="400" />
                                </div>

                            </asp:View>
                        </asp:MultiView>

                        <div class="mt-3">
                            <cmds:CmdsButton ID="SubmitButton" runat="server" Text="<i class='fas fa-upload me-1'></i> Submit" CssClass="btn btn-success" ValidationGroup="UploadFile" />
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
