<%@ Page Language="C#" CodeBehind="PrioritizeCompetencies.aspx.cs" Inherits="InSite.Cmds.Actions.Contact.Department.Competency.PrioritizeCompetencies" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" />

    <section runat="server" ID="DepartmentSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Competencies
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:Grid runat="server" ID="Levels" AutoBinding="false">
                    <Columns>
                
                        <asp:TemplateField HeaderText="Time-Sensitive" ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Label ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                                <asp:Label ID="ProfileStandardIdentifier" runat="server" Text='<%# Eval("ProfileStandardIdentifier") %>' Visible="false" />
                                <span style="line-height:43px;">
                                    <asp:CheckBox runat="server" ID="IsTimeSensitive" Checked='<%# Eval("IsTimeSensitive") != DBNull.Value && (Boolean)Eval("IsTimeSensitive") %>' />
                                </span>
                                <span runat="server" id="ValidForContainer">
                                    <insite:Icon runat="server" ID="ClockIcon" Type="Regular" Name="history" />
                                    <insite:TextBox ID="ValidForCount" runat="server" CssClass="d-inline-block align-middle" Width="60px" MaxLength="3" Text='<%# Eval("ValidForCount") %>' />
                                    <cmds:ValidForUnitSelector2 ID="ValidForUnit" runat="server" Width="120px" />
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    
                        <asp:TemplateField HeaderText="Priority" ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <cmds:CompetencyPrioritySelector2 ID="Priority" runat="server" Width="155px" />
                                <insite:Icon runat="server" ID="CriticalIcon" Type="Regular" Name="flag-checkered" Color="Danger" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Number" HeaderText="Number" />
                    
                        <asp:TemplateField HeaderText="Summary">
                            <ItemTemplate>
                                <%# Eval("Summary") %>
                                <span class="form-text"><%# Eval("NumberOld") != DBNull.Value ? "<br />Old # " + (String)Eval("NumberOld") : "" %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Profile">
                            <ItemTemplate>
                                <%# (Eval("ProfileNumber") == DBNull.Value ? "": Eval("ProfileNumber") + ": ") + Eval("ProfileTitle") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </insite:Grid>

            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            function isTimeSensitiveChanged(chkId, priorityId, criticalImageId, validForContainerId) {
                var checked = document.getElementById(chkId).checked;
                
                if (!checked) {
                    $(document.getElementById(validForContainerId)).addClass('d-none');
                    return;
                }

                $(document.getElementById(validForContainerId)).removeClass('d-none');

                var priority = document.getElementById(priorityId);
                var options = priority.getElementsByTagName('option');

                for (var i = 0; i < options.length; i++) {
                    if (options[i].innerHTML == 'Critical')
                        options[i].selected = true;
                }

                document.getElementById(criticalImageId).style.display = '';
            }
    
            function isPriorityChanged(priorityId, criticalImageId) {
                var priority = document.getElementById(priorityId);
                var options = priority.getElementsByTagName('option');

                for (var i = 0; i < options.length; i++) {
                    if (options[i].selected)
                        document.getElementById(criticalImageId).style.display = options[i].innerHTML == 'Critical' ? '' : 'none';
                }
            }
        </script>
    </insite:PageFooterContent>
</asp:Content>
