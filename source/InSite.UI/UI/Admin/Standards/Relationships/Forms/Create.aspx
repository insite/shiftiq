<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Standards.Relationships.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        h3 {
            margin: 0;
        }

        .form-text > strong {
            color: #909090;
        }

        .color-example {
            display: inline-block;
            height: 20px;
            width: 20px;
            border-radius: 10px;
            vertical-align: middle;
        }
    </style>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert ID="CreatorStatus" runat="server" />

    <asp:Panel runat="server">

        <div class="form-text">
            Create a relationship <strong>from</strong> this
            <asp:Literal runat="server" ID="AssetType" />
        </div>
        <h3 runat="server" id="FromAsset"></h3>
        <div class="form-text"><strong>to</strong> another standard.</div>

        <div class="form-group mt-3 mb-3" runat="server" id="DescriptorField">

            <asp:RadioButtonList runat="server" ID="DescriptorLabel" RepeatDirection="Horizontal">
                <asp:ListItem Value="References" Text="References" />
                <asp:ListItem Value="Resembles" Text="Resembles" />
                <asp:ListItem Value="Satisfies" Text="Satisfies" />
                <asp:ListItem Value="Uses" Text="Uses" />
            </asp:RadioButtonList>

            <insite:FindStandard runat="server" ID="AssetParentID" EmptyMessage="Parent Standard" Width="600px" Visible="true" />
            <div style="margin-top:4px;">
                <insite:StandardTypeComboBox runat="server" ID="StandardType" EmptyMessage="Standard Type" Width="600px" />
            </div>
            <div style="margin-top:4px;">
                <insite:TextBox ID="SearchText" runat="server" EmptyMessage="Standard Number or Keyword" Width="600px" />
            </div>

        </div>

        <div class="form-group mb-3">
            <insite:SearchButton ID="FilterButton" runat="server" />
            <insite:ClearButton ID="ClearButton" runat="server" />
            &nbsp;
            <asp:Literal ID="FoundAssetText" runat="server" />
        </div>

        <div class="form-group mb-3">

            <insite:Grid runat="server" ID="NewAssets" PageSize="20" DataKeyNames="StandardIdentifier">
                <Columns>

                    <asp:TemplateField >
                        <HeaderTemplate>
                            <input type="checkbox" onclick="selectAssetsPanel.selectAll_clicked(this.checked);" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="IsSelected" onclick="selectAssetsPanel.isSelected_clicked();" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField HeaderText="Type" DataField="SubType" />

                    <asp:TemplateField HeaderText="Number">
                        <ItemTemplate>
                            <a href="/ui/admin/standards/edit?id=<%# Eval("StandardIdentifier") %>"><%# Eval("AssetNumber") %></a>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Title">
                        <ItemTemplate>
                            <div class="form-text"><%# Eval("ParentTitle") != null ? "<i class='far fa-level-up'></i> " + (string)Eval("ParentTitle") : "" %></div>
                            <%# Eval("Title") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </insite:Grid>

        </div>

        <div class="form-group">
            <insite:Button ID="CreateRelationshipButton" runat="server" Text="Relate" Icon="fas fa-cloud-upload" ButtonStyle="Success" />
        </div>

        <script type="text/javascript">

            var selectAssetsPanel = {
                onLoad: function () {
                    selectAssetsPanel.isSelected_clicked();
                },
                selectAll_clicked: function (isChecked) {
                    setCheckboxes('<%= NewAssets.ClientID %>', isChecked);
                },
                isSelected_clicked: function () {
                    var isAllChecked = true;
                    $('#<%= NewAssets.ClientID %> td > input[type="checkbox"]').each(function () {
                        if (!this.checked) {
                            isAllChecked = false;
                            return false;
                        }
                    });

                    var list = $('#<%= NewAssets.ClientID %> th > input[type="checkbox"]');
                    if (list.length > 0)
                        list[0].checked = isAllChecked;
                },
                scrollTop: function () {
                    $(window.frameElement).closest('.modal').scrollTop(0);
                },
            };

            $(document).ready(function () {
                selectAssetsPanel.onLoad();

                $('#<%= DescriptorField.ClientID %>').on('keyup', function (e) {
                    if (e.which === 13) {
                        $('#<%= FilterButton.ClientID %>')[0].click();
                    }
                });
            });

        </script>

    </asp:Panel>

</asp:Content>
