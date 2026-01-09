<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Contacts.Groups.Controls.SearchResults" %>

<insite:PageHeadContent runat="server">

    <style type="text/css">
        .merge-checkbox hide {
            display:none;
        }
    </style>

</insite:PageHeadContent>

<asp:Label id="Instructions" runat="server" CssClass="help" Visible="False" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="GroupIdentifier" AllowSorting="true">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="merge-checkbox hide" ItemStyle-CssClass="merge-checkbox hide">
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="MergeCheckBox" Visible='<%# (string)Eval("GroupType") == "Employer" %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Name">
            <ItemTemplate>
                <asp:HyperLink runat="server" Text='<%# Eval("GroupName") %>'
                    NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' />
                <div class="form-text">
                    <%# TrimText(Eval("GroupDescription")) %>
                </div>
            </ItemTemplate>
        </asp:TemplateField> 
            
        <asp:TemplateField HeaderText="Group Expiration Date">
            <ItemTemplate>
                <%# Localize((DateTimeOffset?)Eval("GroupExpiry")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Tag">
            <ItemTemplate>
                <%# Eval("GroupLabel") %>
                <div class="form-text">
                    <%# Eval("GroupType") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Group Code">
            <ItemTemplate>
                <%# Eval("GroupCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Category">
            <ItemTemplate>
                <%# Eval("GroupCategory") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Status">
            <ItemTemplate>
                <%# Eval("GroupStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Office">
            <ItemTemplate>
                <%# Eval("GroupOffice") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Phone">
            <ItemTemplate>
                <%# Eval("GroupPhone") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Region">
            <ItemTemplate>
                <%# Eval("GroupRegion") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Capacity">
            <ItemTemplate>
                <%# Eval("GroupCapacity") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Shipping Address">
            <ItemTemplate>
                <asp:Literal ID="ShippingAddressLiteral" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Billing Address">
            <ItemTemplate>
                <asp:Literal ID="BillingAddressLiteral" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Physical Address">
            <ItemTemplate>
                <asp:Literal ID="PhysicalAddressLiteral" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Number of Employees">
            <ItemTemplate>
                <%# Eval("NumberOfEmployees") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Group Size" DataField="GroupSize" DataFormatString="{0:n0}" ItemStyle-CssClass="text-end"  HeaderStyle-CssClass="text-end" />

        <asp:BoundField HeaderText="Membership Status Size" DataField="MembershipStatusSize" DataFormatString="{0:n0}" ItemStyle-CssClass="text-end"  HeaderStyle-CssClass="text-end" />

        <asp:BoundField HeaderText="Subgroups" DataField="ChildCount" DataFormatString="{0:n0}" ItemStyle-CssClass="text-end"  HeaderStyle-CssClass="text-end" />

        <asp:TemplateField HeaderText="Parent">
            <ItemTemplate>
                <insite:Container runat="server" ID="HierarchyParentContainer" Visible='<%# Eval("HierarchyParent.GroupIdentifier") != null %>'>
                    <div style="font-weight:bold; color:#555;">
                        Hierarchical 
                    </div>
                    <div>
                        <asp:HyperLink runat="server" Text='<%# Eval("HierarchyParent.GroupName") %>'
                            NavigateUrl='<%# Eval("HierarchyParent.GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' />
                    </div>
                </insite:Container>
                <asp:Repeater runat="server" ID="FunctionalParentsRepeater">
                    <HeaderTemplate>
                        <div style="font-weight:bold; color:#555; margin-top:8px;">
                            Functional
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div>
                            <asp:HyperLink runat="server" Text='<%# Eval("GroupName") %>'
                                NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>'/>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Mandatory Form" DataField="SurveyFormName" />
        
        <asp:BoundField HeaderText="Invoice Product" DataField="MembershipProductName" />

        <asp:TemplateField ItemStyle-Width="60px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit"
                    NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete" Visible='<%# CanWrite %>'
                    NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/delete?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<div runat="server" id="ButtonPanel" class="row" style="margin-bottom:15px;">
    <div class="col-md-12 text-end">
        <insite:Button runat="server" ID="StartMergingButton" ButtonStyle="Default" Text="Merge" Icon="fas fa-code-merge" />
        <insite:Button runat="server" ID="MergeButton" ButtonStyle="Success" style="display:none;" disabled="disabled" Text="Start Merge" Icon="fas fa-cloud-upload" />
        <insite:Button runat="server" ID="StopMergingButton" ButtonStyle="Danger" style="display:none;" Text="Stop Merge" Icon="fas fa-code-merge" />
    </div>
</div>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">

        $(document).ready(function () {
            $('#<%= StartMergingButton.ClientID %>').click(function (e) {
                e.preventDefault();

                $(this).hide();
                $('#<%= StopMergingButton.ClientID %>').show();
                $('#<%= MergeButton.ClientID %>').show();

                $('.merge-checkbox').removeClass('hide');
            });

            $('#<%= StopMergingButton.ClientID %>').click(function (e) {
                e.preventDefault();

                $(this).hide();
                $('#<%= StartMergingButton.ClientID %>').show();
                $('#<%= MergeButton.ClientID %>').hide();

                $('.merge-checkbox').addClass('hide');
            });

            $('#<%= MergeButton.ClientID %>').click(function (e) {
                if (typeof $(this).attr('disabled') !== 'undefined' && $(this).attr('disabled') !== false)
                    e.preventDefault();
            });

            $('.merge-checkbox input').change(function () {
                var count = $('.merge-checkbox input:checked').length;

                if (this.checked) {
                    if (count > 2) {
                        alert('It is possible to select only two person for combining');
                        $(this).prop('checked', false);

                        count--;
                    }
                }

                if (count == 2)
                    $('#<%= MergeButton.ClientID %>').removeAttr('disabled');
                else
                    $('#<%= MergeButton.ClientID %>').attr('disabled', 'disabled');
            });
        });

    </script>

</insite:PageFooterContent>