<%@ Page Language="C#" CodeBehind="ScanImages.aspx.cs" Inherits="InSite.Admin.Assessments.Attachments.Forms.ScanImages" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        table.table-images {

        }

            table.table-images td.img-status {
                width: 30px;
            }

            table.table-images td.img-index {
                text-align: center;
                width: 90px;
            }

            table.table-images td.img-cmd {
                width: 73px;
                white-space: nowrap;
            }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="ScreenStatus" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <section>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-images me-1"></i>
                    Images
                </h4>

                <p runat="server" id="NoImages">No Images found</p>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
                <insite:UpdatePanel runat="server" ID="UpdatePanel">
                    <ContentTemplate>
                        <asp:Repeater runat="server" ID="ImageRepeater">
                            <HeaderTemplate>
                                <table class="table table-striped table-images">
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <th class="text-center">Question</th>
                                            <th>Title</th>
                                            <th>File Name</th>
                                            <th>Extension</th>
                                            <th class="text-end">Size</th>
                                            <th class="text-center">Thumbnail</th>
                                            <th class="text-nowrap text-end">
                                                <insite:Button runat="server" ID="AddAllButton" Icon="fas fa-plus-circle" ButtonStyle="Default" ToolTip="Add All Attachments"
                                                    CommandName="AddAll" OnClientClick="return confirm('Are you sure you want to add all found images to attachments?');"/>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <FooterTemplate></tbody></table></FooterTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="img-status">
                                        <i runat="server" visible='<%# Eval("IsAttachment") %>' class="far fa-check-circle text-success" title="In attachments"></i>
                                        <i runat="server" visible='<%# !(bool)Eval("IsAttachment") %>' class="far fa-exclamation-circle text-danger" title="Not in attachments"></i>
                                    </td>
                                    <td class="img-index"><%# Eval("BankIndexHtml") %></td>
                                    <td><%# Eval("Title") %></td>
                                    <td><%# Eval("FileName") %></td>
                                    <td><%# Eval("FileExtension") %></td>
                                    <td class="text-end"><%# Eval("FileSize") %></td>
                                    <td class="text-center">
                                        <img runat="server" visible='<%# Eval("FileSize") != null %>' src='<%# Eval("Url") %>' alt='<%# Eval("Title") %>' style="max-width:100px;max-height:100px;" />
                                    </td>
                                    <td class="img-cmd">
                                        <insite:Button runat="server" visible='<%# Eval("FileSize") != null %>' NavigateUrl='<%# Eval("Url") %>' ButtonStyle="Primary" NavigateTarget="_blank" 
                                            ToolTip="Download" Icon="fas fa-download" />
                                        <insite:Button runat="server" ID="AddOneButton" ToolTip="Add Attachment" Icon="fas fa-plus-circle" ButtonStyle="Default"
                                            Visible='<%# CanCreate && !(bool)Eval("IsAttachment") && Eval("FileSize") != null %>'
                                            CommandName="AddOne" CommandArgument='<%# Eval("Index") %>' 
                                            OnClientClick="return confirm('Are you sure you want to add this image to attachments?');"
                                        />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>

    </section>

    <div class="mt-3">
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
