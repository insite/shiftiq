<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementListGroup.ascx.cs" Inherits="InSite.UI.CMDS.Common.Controls.User.AchievementListGroup" %>

<div runat="server" id="Wrapper">

<asp:Repeater runat="server" ID="VisibilityRepeater">
    <HeaderTemplate>
        <div id='<%# ClientID %>'>
    </HeaderTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
    <SeparatorTemplate>
        <div class="mb-5"></div>
    </SeparatorTemplate>
    <ItemTemplate>

        <h2 runat="server" visible='<%# Eval("AccountScope") != null %>' class="text-primary">
            <asp:Literal runat="server" ID="AccountScope" Text='<%# Eval("AccountScope") %>' />
        </h2>

        <asp:Repeater runat="server" ID="AchievementGroups">
            <SeparatorTemplate>
                <div class="mb-4"></div>
            </SeparatorTemplate>
            <ItemTemplate>
                <div data-type="achievement-group-container">

                    <h3 class="p-2 bg-secondary my-0">
                        <insite:CheckBox runat="server" ID="GroupCheckBox" RenderMode="Input" CssClass="mt-1 me-2" />
                        <small class="float-end">
                            <%# Eval("VisibilityLabel") %>
                            <insite:Button runat="server" ID="GroupAddButton" CommandName="GroupAddAchievement" DisableAfterClick="true" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="Success" Size="ExtraSmall" />
                            <insite:DeleteButton runat="server" ID="GroupDeleteButton" CommandName="GroupDeleteAchievement" OnClientClick="return confirm('Are you sure to delete selected achievements in this group?');" Size="ExtraSmall" />
                        </small>
                        <span data-groupcheck-label="1">
                            <asp:Literal runat="server" ID="GroupName" Text='<%# GetDisplay((string)Eval("GroupName")) %>' />
                        </span>
                    </h4>

                    <asp:Repeater ID="AchievementCategories" runat="server">
                        <HeaderTemplate>
                            <div class="ms-3">
                        </HeaderTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                        <ItemTemplate>
                            <div class="pt-3">
                                <div runat="server" visible='<%# Eval("HasCategory") %>' class="pb-3 fw-bold" ><%# Eval("CategoryName") %></div>
                            </div>

                            <asp:Repeater ID="Achievements" runat="server">
                                <HeaderTemplate>
                                    <div class="ms-3">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# (Container.ItemIndex % 2) == 0 ? "<div class='row'>": "" %>
                                        <div class="col-6 p-1 pe-4">
                                            <asp:Literal ID="AchievementIdentifier" runat="server" Text='<%# Eval("AchievementIdentifier") %>' Visible="false" />
                                            <insite:CheckBox runat="server" ID="AchievementSelected" />
                                            <asp:Literal runat="server" ID="AchievementLabel" />
                                        </div>
                                    <%# (Container.ItemIndex % 2) != 0 ? "</div>": "" %>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <%# (((Repeater)Container.Parent).Items.Count % 2) != 0 ? "</div>": "" %>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>

                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </ItemTemplate>
        </asp:Repeater>

    </ItemTemplate>
</asp:Repeater>

</div>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            Sys.Application.add_load(function () {
                const groupCheckPostfix = '$GroupCheckBox';
                const containers = document.querySelectorAll('div[data-type="achievement-group-container"]');
                for (let container of containers) {
                    const allChecks = container.querySelectorAll('input[type="checkbox"]');
                    if (allChecks.length <= 1)
                        continue;

                    const groupCheck = allChecks[0];
                    if (!groupCheck.name.endsWith(groupCheckPostfix) || groupCheck._childChecks)
                        continue;

                    const childChecks = [...allChecks].splice(1);

                    groupCheck._childChecks = childChecks;
                    groupCheck.addEventListener('change', onGroupCheckChanged);

                    const label = groupCheck.parentNode.querySelector('[data-groupcheck-label]');
                    if (label) {
                        label._groupCheck = groupCheck;
                        label.addEventListener('click', onLabelClick);
                        label.style.cursor = 'default';
                    }

                    for (let check of childChecks) {
                        check._groupCheck = groupCheck;
                        check.addEventListener('change', onChildCheckChanged);
                    }

                    updateGroupCheckState(groupCheck);
                }
            });

            function onLabelClick() {
                this._groupCheck.click();
            }

            function onGroupCheckChanged() {
                const checked = this.checked;
                const childChecks = this._childChecks;

                for (let check of childChecks) {
                    check.checked = checked;
                }
            }

            function onChildCheckChanged() {
                updateGroupCheckState(this._groupCheck);
            }

            function updateGroupCheckState(groupCheck) {
                const childChecks = groupCheck._childChecks;
                if (childChecks.length === 0)
                    return;

                let checked = true;

                for (let check of childChecks) {
                    if (!check.checked) {
                        checked = false;
                    }
                }

                groupCheck.checked = checked;
            }
        })();
    </script>
</insite:PageFooterContent>
