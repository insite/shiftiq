<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentRepeater.ascx.cs" Inherits="InSite.Custom.CMDS.Reports.Controls.DepartmentRepeater" %>

<div class="my-3">
    <insite:Button runat="server" ID="DownloadXlsx" ButtonStyle="Default" Text="Download XLSX" Icon="far fa-download" />
</div>

<asp:Repeater runat="server" ID="Repeater">
    <ItemTemplate>
        <h3><%# Eval("DepartmentName") %></h3>

        <insite:Grid runat="server" ID="DepartmentGrid" CssClass="table table-striped mb-4">
            <Columns>
                <asp:TemplateField HeaderText="As At">
                    <ItemTemplate>
                        <%# Shift.Common.TimeZones.Format((DateTimeOffset)Eval("AsAt"), User.TimeZone, true) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Statistic">
                    <ItemTemplate>
                        <%# Eval("StatisticName") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <HeaderTemplate><span title="Completed">CP</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# NumberOrEmpty(Eval("CountCP")) %> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <HeaderTemplate><span title="Expired">EX</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# NumberOrEmpty(Eval("CountEX")) %> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <HeaderTemplate><span title="Not Completed">NC</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# NumberOrEmpty(Eval("CountNC")) %> 
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <HeaderTemplate><span title="Not Applicable">NA</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# NumberOrEmpty(Eval("CountNA")) %> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <HeaderTemplate><span title="Needs Training">NT</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# NumberOrEmpty(Eval("CountNT")) %> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <HeaderTemplate><span title="Self-Assessed">SA</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# NumberOrEmpty(Eval("CountSA")) %> 
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <HeaderTemplate><span title="Submitted for Validation">SV</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# NumberOrEmpty(Eval("CountSV")) %> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <HeaderTemplate><span title="Validated">VA</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# NumberOrEmpty(Eval("CountVA")) %> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="" AccessibleHeaderText="" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <HeaderTemplate><span title="Validated as Not Applicable">VN</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# NumberOrEmpty(Eval("CountVN")) %> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <HeaderTemplate><span title="Required">RQ</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# NumberOrEmpty(Eval("CountRQ")) %> 
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" ItemStyle-Wrap="false">
                    <HeaderTemplate><span title="Unweighted Average (i.e. average of user scores)">Score</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# GetComplianceScore((double)Eval("Score")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" ItemStyle-Wrap="false">
                    <HeaderTemplate><span title="Weighted Average (i.e. percent of department requirements completed and/or validated)">Progress</span></HeaderTemplate>
                    <ItemTemplate>
                        <%# GetComplianceScore((double?)Eval("Progress")) %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>
    </ItemTemplate>
</asp:Repeater>
