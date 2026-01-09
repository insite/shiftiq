<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourseFileList.ascx.cs" Inherits="InSite.UI.Admin.Courses.Outlines.Controls.CourseFileList" %>

<insite:PageHeadContent runat="server">
    <style>
        td > span.file-link {
            cursor: pointer;
        }
    </style>
</insite:PageHeadContent>

<div class="row mb-3">
    <div class="col-md-12">

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="FileUpdatePanel" />
        <insite:UpdatePanel runat="server" ID="FileUpdatePanel" ClientEvents-OnResponseEnd="fileList.init">
            <ContentTemplate>

                <div class="row mb-3">
                    <div class="col-md-4">
                        <insite:FileUploadV1 runat="server" ID="FileUpload"
                            LabelText="Select and Upload File"
                            ContainerType="Course"
                            FileUploadType="Document"
                            OnClientFileUploaded="fileList.onFileUploaded"
                        />
                    </div>
                </div>

                <asp:Repeater runat="server" ID="FileRepeater">
                    <HeaderTemplate>
                        <label>Click URL to copy it into Clipboard</label>
                        <table class="table table-striped">
                            <tr>
                                <th></th>
                                <th>URL</th>
                                <th>Language</th>
                            </tr>
                    </HeaderTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <td style="width:80px;">
                                <insite:IconLink runat="server"
                                    NavigateUrl='<%# Eval("Url") %>'
                                    Target="_blank"
                                    ToolTip="View File"
                                    Name="external-link"
                                />
                                <insite:IconButton runat="server"
                                    ToolTip="Delete File"
                                    CommandName="DeleteFile"
                                    CommandArgument='<%# Eval("FileName") %>'
                                    Name="trash-alt"
                                    ConfirmText="Are you sure to delete this file?"
                                />
                            </td>
                            <td>
                                <span class="file-link">
                                    <%# Eval("Url") %>
                                </span>
                            </td>
                            <td>
                                <%# Eval("Language") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Button runat="server" ID="UploadButton" style="display:none;" />

            </ContentTemplate>
        </insite:UpdatePanel>

    </div>
</div>

<insite:PageFooterContent runat="server">
    <script>
        const fileList = window.fileList = window.fileList || {};

        fileList.onFileUploaded = () => {
            __doPostBack("<%= UploadButton.UniqueID %>", "");
        }

        fileList.init = () => {
            $("td > span.file-link")
                .off("click")
                .on("click", async (e) => {
                    const $span = $(e.target);
                    const text = $span.text().trim();

                    await navigator.clipboard.writeText(text);

                    $span.tooltip({
                        title: "URL has been copied",
                    });

                    $span.tooltip("show");

                    setTimeout(() => $span.tooltip("dispose"), 1000);
                });
        }

        fileList.init();
    </script>
</insite:PageFooterContent>