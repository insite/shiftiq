<%@ Page Language="C#" CodeBehind="Reorder.aspx.cs" Inherits="InSite.Admin.Records.Items.Forms.Reorder" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">
    <style type="text/css">

        div.gradebook-item {
            padding: 8px;
            border-top: 1px solid #ddd;
            border-bottom: 1px solid #ddd;
            cursor: grab;
        }

        div.gradebook-item:nth-child(even) {
            background-color: #f9f9f9;
        }

        div.gradebook-item:hover {
            border-color: #cccccc;
        }

    </style>
</insite:PageHeadContent>

    <insite:Alert runat="server" ID="ReorderStatus" />

    <section runat="server" ID="ItemsSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-spell-check me-1"></i>
            <asp:Literal runat="server" ID="Instruction" />
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div id="gradebook-items">
                    <asp:Repeater runat="server" ID="ItemRepeater">
                        <ItemTemplate>
                            <div class="gradebook-item">
                                <%# Eval("Name") %>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

            </div>
        </div>
    </section>

    <p style="padding-bottom:10px;">	
        <insite:Button runat="server" ID="ReorderButton" Text="Reorder" Icon="fas fa-cloud-upload" ButtonStyle="Success" OnClientClick="reorder.save(); return false;" />	
        <insite:CancelButton runat="server" ID="CancelButton" />	
    </p>	

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        var reorder = {
            init: function () {
                var itemIndex = 0;

                $('div.gradebook-item').each(function () {
                    $(this).attr('itemid', String(itemIndex));
                    itemIndex++;
                });

                $('div#gradebook-items').sortable({
                    items: '> div.gradebook-item',
                    containment: 'document',
                    cursor: 'grabbing',
                    forceHelperSize: true,
                    axis: 'y',
                    opacity: 0.65,
                    tolerance: 'pointer',
                    start: inSite.common.gridReorderHelper.onSortStart,
                }).disableSelection();
            },

            save: function () {
                __doPostBack('<%= ReorderButton.UniqueID %>', reorder.getData());
            },
        
            getData: function () {
                var data = '';

                $('div.gradebook-item').each(function () {
                    var $this = $(this);
                    var itemid = $this.attr('itemid');

                    data += String(itemid) + ';';
                });

                return data;
            },
        };

        $(document).ready(reorder.init);
    </script>
</insite:PageFooterContent>
</asp:Content>
