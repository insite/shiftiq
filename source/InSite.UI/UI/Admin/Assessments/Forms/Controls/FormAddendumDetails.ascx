<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormAddendumDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Forms.Controls.FormAddendumDetails" %>

<div class="row">
    <div class="col-lg-6">

        <h3>
            Attachments
            <insite:IconLink Name="pencil" runat="server" ID="EditLink" ToolTip="Select and/or Reorder" CssClass="form-addendum-link ms-1 fs-6" />
        </h3>

        <div class="form-group">
            <div runat="server" id="RepeaterPanel">
                <asp:Repeater runat="server" ID="GroupRepeater">
                    <HeaderTemplate>
                        <table class="table table-addendum">
                            <thead>
                                <tr>
                                    <th>Type</th>
                                    <th>Asset</th>
                                    <th>Title</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <th class="bg-secondary" colspan="3"><%# Eval("GroupTitle") %></th>
                        </tr>
                        <asp:Repeater runat="server" ID="ItemRepeater">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("TypeName") %></td>
                                    <td><%# $"{Eval("AssetNumber")}.{Eval("AssetVersion")}" %></td>
                                    <td><a target="_blank" href="<%# Eval("Url") %>?download=1"><%# Eval("Title") %></a></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="form-text">
                This is the list of attachments comprising the Addendum Booklet for the exam form.
            </div>
        </div>

    </div>
</div>

<insite:PageFooterContent runat="server" ID="FooterLiteral">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.form-addendum-link').each(function () {
                var $this = $(this);

                var section = $this.closest('[data-section]').data('section');
                if (section)
                    $this.prop('href', $this.prop('href') + '&section=' + String(section));
            });
        });
    </script>
</insite:PageFooterContent>