<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UnitRepeater.ascx.cs" Inherits="InSite.UI.Portal.Learning.Controls.UnitRepeater" %>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .item-disabled {
            opacity: 0.5;
            pointer-events: none;
        }
    </style>
</insite:PageHeadContent>

<asp:Repeater runat="server" ID="SidebarUnitRepeater">
    <HeaderTemplate>
        <div class="accordion" id="MenuAccordion">
    </HeaderTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
    <ItemTemplate>

        <div class='<%# "accordion-item widget widget-categories mb-4 alert alert-light unit-item " + ((bool)Eval("IsLocked") && (bool)Eval("IsAdaptive") ? " d-none": "") %>'>
            <h2 class='<%# "accordion-header" + (AllowMultipleUnits ? "" : " d-none") + ((bool)Eval("IsLocked") ? ((bool)Eval("IsAdaptive") ? " d-none" : " item-disabled opacity-100") : "") %>' id='<%# Eval("Asset", "AccordionHeader{0}") %>'>
                <button class='<%# (bool)Eval("IsActive") ? "accordion-button text-body" : "accordion-button collapsed text-body" %>'
                        type="button"
                        data-bs-toggle="collapse"
                        data-bs-target='#<%# Eval("Asset", "AccordionPanel{0}") %>'
                        aria-expanded='<%# Eval("IsActive") %>'
                        aria-controls='<%# Eval("Asset", "AccordionPanel{0}") %>'
                >
                    <i class='<%# ((bool)Eval("IsLocked") ? "me-1 fas fa-lock-alt text-danger" : "") %>'></i>
                    <%# AllowMultipleUnits ? Eval("Name") : "" %>
                </button>
            </h2>
            <div class='<%# (bool)Eval("IsActive") ? "accordion-collapse collapse show" : "accordion-collapse collapse" %>'
                    id='<%# Eval("Asset", "AccordionPanel{0}") %>'
                    aria-labelledby="'<%# Eval("Asset", "AccordionHeader{0}") %>'"
                    data-bs-parent="#MenuAccordion"
            >

                <asp:Repeater runat="server" ID="ModuleRepeater">
                    <HeaderTemplate>
                        <ul id='<%# "AllModulesPanel" + ++ModuleListIndex %>'>
                    </HeaderTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                    <ItemTemplate>
                        <li>
                            <a class='<%# "widget-link" + ((bool)Eval("IsActive") ? "" : " collapsed") + ((bool)Eval("IsLocked") ? ((bool)Eval("IsAdaptive") ? " d-none" : " item-disabled opacity-100") : "") %>'
                                href='<%# Eval("Asset", "#ModulePanel{0}") %>'
                                data-bs-toggle="collapse"
                                aria-expanded='<%# Eval("IsActive") %>'
                            > 
                                <i class='<%# ((bool)Eval("IsLocked") ? "me-1 fas fa-lock-alt text-danger" : "") %>'></i>
                                <strong class="mb-0 text-primary"><%# Eval("Name") %></strong>
                            </a>
                            <ul class='<%# (bool)Eval("IsActive") ? "collapse show" : "collapse" %>' id='<%# Eval("Asset", "ModulePanel{0}") %>' data-bs-parent='<%# "AllModulesPanel" + ModuleListIndex %>'>
                                <asp:Repeater runat="server" ID="ActivityRepeater">
                                    <ItemTemplate>
                                        <li>
                                            <a runat="server" id="ActivityLink" class='<%# Eval("ActivityClass", "widget-link {0}") %>'>
                                                <i class='<%# Eval("ActivityIcon", "me-1 {0}") %>'></i><%# Eval("ActivityName") %>
                                            </a>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
                    
            </div>
        </div>

    </ItemTemplate>
</asp:Repeater>
