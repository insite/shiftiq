<%@ Page Language="C#" CodeBehind="AddPrivacyGroup.aspx.cs" Inherits="InSite.UI.Admin.Courses.Outlines.AddPrivacyGroup" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <style type="text/css">
        h3 {
            margin: 0;
        }

        .form-header {
            display: none;
        }

        header.navbar {
            display: none;
        }
    </style>

    <div class="section-panel">

        <h3 runat="server" id="ContainerName"></h3>
        <div class="row">
            <div class="col-lg-12">
                <div class="form-text">Grant access to groups.</div>

                <div runat="server" id="SearchPanel" class="form-group my-3">
                    <insite:GroupTypeComboBox runat="server" ID="GroupType" EmptyMessage="Group Type" Width="600px" Style="margin-top: 4px;" />
                    <insite:TextBox ID="SearchText" runat="server" EmptyMessage="Group Name" Width="600px" style="margin-top: 4px;" />
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <div class="form-group mb-3">
                    <insite:SearchButton ID="FilterButton" runat="server" />
                    <insite:ClearButton ID="ClearButton" runat="server" />
                    &nbsp;
                    <asp:Literal ID="FoundGroupText" runat="server" />
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <div class="form-group mb-3">

                    <insite:Grid runat="server" ID="NewGroups" DataKeyNames="GroupIdentifier" PageSize="20">
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="40">
                                <HeaderTemplate>
                                    <input type="checkbox" onclick="selectGroupsPanel.selectAll_clicked(this.checked);" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" ID="IsSelected" onclick="selectGroupsPanel.isSelected_clicked();" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="User">
                                <ItemTemplate>
                                    <%# Eval("GroupName") %>
                                    <span class="form-text"><%# Eval("GroupType") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </insite:Grid>

                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <insite:Button ID="AddButton" runat="server" Text="Add" ButtonStyle="Success" Icon="fas fa-cloud-upload" />
            </div>
        </div>

        <script type="text/javascript">

            var selectGroupsPanel = {
                onLoad: function () {
                    selectGroupsPanel.isSelected_clicked();
                },
                selectAll_clicked: function (isChecked) {
                    setCheckboxes('<%= NewGroups.ClientID %>', isChecked);
                },
                isSelected_clicked: function () {
                    var isAllChecked = true;
                    $('#<%= NewGroups.ClientID %> td > input[type="checkbox"]').each(function () {
                        if (!this.checked) {
                            isAllChecked = false;
                            return false;
                        }
                    });

                    var list = $('#<%= NewGroups.ClientID %> th > input[type="checkbox"]');
                    if (list.length > 0)
                        list[0].checked = isAllChecked;
                },
            };

            $(document).ready(function () {
                selectGroupsPanel.onLoad();

                $('#<%= SearchPanel.ClientID %>').on('keyup', function (e) {
                    if (e.which === 13) {
                        $('#<%= FilterButton.ClientID %>')[0].click();
                    }
                });
            });

        </script>

    </div>

    <div class="clearfix"></div>
</asp:Content>
