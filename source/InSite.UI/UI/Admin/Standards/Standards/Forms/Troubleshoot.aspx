<%@ Page Language="C#" CodeBehind="Troubleshoot.aspx.cs" Inherits="InSite.UI.Admin.Standards.Standards.Forms.Troubleshoot" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<div class="row mb-3">
    <div class="col-lg-4 col-md-6">

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    Counters
                </h4>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Root Standards
                    </label>
                    <div runat="server" id="RootStandardsCount"></div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Structural Containment Relationships
                    </label>
                    <div runat="server" id="ChildStandardsCount"></div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Functional Containment Relationships
                    </label>
                    <div runat="server" id="ContainmentsCount"></div>
                </div>

            </div>
        </div>

    </div>
</div>

<div class="card border-0 shadow-lg mb-3">
    <div class="card-body">

        <h4 class="card-title mb-3">
            Orphan Relationships
            <small runat="server" id="OrphanCount" class="text-body-secondary ms-1"></small>
        </h4>

        <insite:UpdatePanel runat="server">
            <ContentTemplate>

                <insite:Grid runat="server" ID="OrphanGrid">
                    <Columns>

                        <asp:BoundField HeaderText="Connection" DataField="ConnectionType" />

                        <asp:TemplateField HeaderText="From Standard">
                            <ItemTemplate>
                                <span runat="server" visible='<%# !(bool)Eval("HasFromStandard") %>' class="text-danger"><%# Eval("FromStandardId") %></span>

                                <insite:Container runat="server" Visible='<%# (bool)Eval("HasFromStandard") %>'>
                                    <a runat="server" visible='<%# (bool?)Eval("FromStandard.AllowView") == true %>' target="_blank" href='<%# Eval("FromStandard.EditLink") %>'><%# Eval("FromStandard.Title") %></a>
                                    <span runat="server" visible='<%# (bool?)Eval("FromStandard.AllowView") == false %>'><%# Eval("FromStandard.Title") %></span>
                                    <small class='text-body-secondary text-nowrap'><%# Eval("FromStandard.Type") %> Asset #<%# Eval("FromStandard.Number") %></small>
                                    <small runat="server" class='text-body-secondary text-nowrap' visible='<%# (bool?)Eval("FromStandard.AllowView") == false %>'>(<%# Eval("FromStandard.Organization.Code") %>)</small>
                                </insite:Container>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="To Standard">
                            <ItemTemplate>
                                <span runat="server" visible='<%# !(bool)Eval("HasToStandard") %>' class="text-danger"><%# Eval("ToStandardId") %></span>

                                <insite:Container runat="server" Visible='<%# (bool)Eval("HasToStandard") %>'>
                                    <a runat="server" visible='<%# (bool?)Eval("ToStandard.AllowView") == true %>' target="_blank" href='<%# Eval("ToStandard.EditLink") %>'><%# Eval("ToStandard.Title") %></a>
                                    <span runat="server" visible='<%# (bool?)Eval("ToStandard.AllowView") == false %>'><%# Eval("ToStandard.Title") %></span>
                                    <small class='text-body-secondary text-nowrap'><%# Eval("ToStandard.Type") %> Asset #<%# Eval("ToStandard.Number") %></small>
                                    <small runat="server" class='text-body-secondary text-nowrap' visible='<%# (bool?)Eval("ToStandard.AllowView") == false %>'>(<%# Eval("ToStandard.Organization.Code") %>)</small>
                                </insite:Container>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </insite:Grid>

            </ContentTemplate>
        </insite:UpdatePanel>

        <p runat="server" id="OrphanNotFound">No orphan relationships found.</p>

    </div>
</div>

<div class="card border-0 shadow-lg mb-3">
    <div class="card-body">

        <h4 class="card-title mb-3">
            Invalid Hierarchies
            <small runat="server" id="InvalidHierarchyCount" class="text-body-secondary ms-1"></small>
        </h4>

        <insite:UpdatePanel runat="server">
            <ContentTemplate>

                <insite:Grid runat="server" ID="InvalidHierarchyGrid">
                    <Columns>

                        <asp:TemplateField HeaderText="Loop Path">
                            <ItemTemplate>
                                <asp:Repeater runat="server" ID="ItemRepeater">
                                    <HeaderTemplate><ul></HeaderTemplate>
                                    <FooterTemplate></ul></FooterTemplate>
                                    <ItemTemplate>
                                        <li class='<%# (bool)Eval("IsLoopNode") ? "fw-bold" : "" %>'>
                                            <span runat="server" visible='<%# !(bool)Eval("HasStandard") %>' class="text-danger"><%# Eval("StandardId") %></span>

                                            <insite:Container runat="server" Visible='<%# (bool)Eval("HasStandard") %>'>
                                                <a runat="server" visible='<%# (bool?)Eval("Standard.AllowView") == true %>' target="_blank" href='<%# Eval("Standard.EditLink") %>'><%# Eval("Standard.Title") %></a>
                                                <span runat="server" visible='<%# (bool?)Eval("Standard.AllowView") == false %>'><%# Eval("Standard.Title") %></span>
                                                <small class='text-body-secondary text-nowrap'><%# Eval("Standard.Type") %> Asset #<%# Eval("Standard.Number") %></small>
                                                <small runat="server" class='text-body-secondary text-nowrap' visible='<%# (bool?)Eval("Standard.AllowView") == false %>'>(<%# Eval("Standard.Organization.Code") %>)</small>
                                            </insite:Container>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </insite:Grid>

            </ContentTemplate>
        </insite:UpdatePanel>

        <p runat="server" id="InvalidHierarchyNotFound">No invalid hierarchies found.</p>

    </div>
</div>

<div class="card border-0 shadow-lg">
    <div class="card-body">

        <h4 class="card-title mb-3 float-start">
            Hierarchy Sizes
            <small runat="server" id="HierarchySizesCount" class="text-body-secondary ms-1"></small>
        </h4>

        <insite:UpdatePanel runat="server">
            <ContentTemplate>

                <div class="float-end mb-3">
                    <insite:CheckSwitch runat="server" ID="ShowHierarchySizesInvalidOnly" Text="Show only invalid hierarchies" />
                </div>

                <div class="clearfix"></div>

                <insite:Grid runat="server" ID="HierarchySizesGrid">
                    <Columns>

                        <asp:TemplateField HeaderText="Root">
                            <ItemTemplate>
                                <span runat="server" visible='<%# !(bool)Eval("HasStandard") %>' class="text-danger"><%# Eval("StandardId") %></span>

                                <insite:Container runat="server" Visible='<%# (bool)Eval("HasStandard") %>'>
                                    <a runat="server" visible='<%# (bool?)Eval("Standard.AllowView") == true %>' target="_blank" href='<%# Eval("Standard.EditLink") %>'><%# Eval("Standard.Title") %></a>
                                    <span runat="server" visible='<%# (bool?)Eval("Standard.AllowView") == false %>'><%# Eval("Standard.Title") %></span>
                                    <small class='text-body-secondary text-nowrap'><%# Eval("Standard.Type") %> Asset #<%# Eval("Standard.Number") %></small>
                                    <small runat="server" class='text-body-secondary text-nowrap' visible='<%# (bool?)Eval("Standard.AllowView") == false %>'>(<%# Eval("Standard.Organization.Code") %>)</small>
                                </insite:Container>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Validation">
                            <ItemTemplate>
                                <div runat="server" class="alert alert-danger" role="alert" visible='<%# !(bool)Eval("IsValidHierarchy") %>'>
                                    <i class="fas fa-stop-circle"></i>
                                    Invalid hierarchy.
                                </div>
                                <div runat="server" class="alert alert-warning" role="alert" visible='<%# !(bool)Eval("IsValidDepth") %>'>
                                    <i class="fas fa-exclamation-triangle"></i>
                                    The depth of the tree exceeded the depth of <strong><%# MaxHierarchyDepth %></strong>.
                                </div>
                                <div runat="server" class="alert alert-success" role="alert" visible='<%# (bool)Eval("IsValidHierarchy") && (bool)Eval("IsValidDepth") %>'>
                                    <i class="fas fa-check"></i>
                                    Valid
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Depth">
                            <ItemTemplate>
                                <%# (bool)Eval("IsValidHierarchy") ? Eval("Depth", "{0:n0}") : "N/A" %>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </insite:Grid>

            </ContentTemplate>
        </insite:UpdatePanel>

        <p runat="server" id="HierarchySizesNotFound">No records found.</p>

    </div>
</div>

</asp:Content>