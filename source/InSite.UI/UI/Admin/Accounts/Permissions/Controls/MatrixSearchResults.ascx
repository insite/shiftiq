<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MatrixSearchResults.ascx.cs" Inherits="InSite.Admin.Accounts.Permissions.Controls.MatrixSearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" Width="100%" Height="700" 
    AutoGenerateColumns="false" style="overflow-x: auto; display: flow-root;" AlternatingRowStyle-Width="200">
    <%--<HeaderStyle Width="200" />
    <ItemStyle Width="200" HorizontalAlign="Center"/>
    <AlternatingItemStyle Width="200" HorizontalAlign="Center"/>--%>
    <%--<ClientSettings>
        <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
    </ClientSettings>--%>
</insite:Grid>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        <asp:Literal runat="server" ID="DictionaryLiteral" />

        (function () {
            var _timeout = 500;
            var _changes = [];
            var idIndex = Date.now();

            $("select.permission-selector")
                .each(function () {
                    this.id = 'permission_' + String(idIndex++);
                    this.className = 'insite-combobox';
                    inSite.common.comboBox.init({ id: this.id, width: '100%' });
                })
                .on("change", function () {
                    var $this = $(this);
                    var value = $this.val();
                    var row = $this.data("row");
                    var col = $this.data("col");
                    var toolkit = _toolkits[col];
                    var group = _groups[row];
                    var item = group + ":" + toolkit + ":" + value;

                    _changes.push(item);

                    $this.parent().css("background-color", _colors[this.selectedIndex]);
                });

            function saveChanges() {
                var changesToSend = [];
                while (_changes.length > 0)
                    changesToSend.push(_changes.pop());

                if (changesToSend.length == 0) {
                    setTimeout(saveChanges, _timeout);
                    return;
                }

                $.ajax({
                    type: 'POST',
                    data:
                    {
                        isPageAjax: true,
                        changes: JSON.stringify(changesToSend)
                    },
                    error: function () {
                        alert('An error occurred during data saving.');
                    },
                    success: function () {
                        setTimeout(saveChanges, _timeout);
                    }
                });
            }

            setTimeout(saveChanges, _timeout);
        })();
    </script>
</insite:PageFooterContent>
