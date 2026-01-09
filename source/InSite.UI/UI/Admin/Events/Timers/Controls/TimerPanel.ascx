<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimerPanel.ascx.cs" Inherits="InSite.Admin.Events.Timers.Controls.TimerPanel" %>

<div class="row mb-3">
    <div class="col-lg-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="mb-3">
                    <insite:ComboBox runat="server" ID="EventNotificationCombo" Width="400px" />
                    <insite:IconButton runat="server" ID="EventNotificationButton" Name="bolt" />
                </div>

                <h3>Event Timers</h3>
                <p class="help" runat="server" id="EventTimerHelp">There are no timers started for this event.</p>

                <asp:Repeater runat="server" ID="EventTimerRepeater">
                    <ItemTemplate>

                        <div style="border-top: dotted #F5F5F5 1px; margin-bottom: 20px;">

                            <div>
                                <strong>
                                    <%# LocalizeTime(Eval("TriggerTime"), "span") %>
                                </strong>
                                <div class="float-end">
                                    <label class="badge bg-<%# (string)Eval("TimerStatus") == "Started" ? "primary" : "custom-default" %>">
                                        <%# Eval("TimerStatus") %>
                                    </label>
                                </div>
                            </div>

                            <div>
                                <%# Eval("TimerDescription") %>
                            </div>

                            <insite:IconButton runat="server" ID="ElapseButton" CommandArgument='<%# Eval("TriggerCommand") %>' CommandName="Elapse" Name="alarm-clock" ToolTip="Force this timer to elapse now" />
                            <insite:IconButton runat="server" ID="CancelButton" CommandArgument='<%# Eval("TriggerCommand") %>' CommandName="Cancel" Name="ban" ToolTip="Cancel this timer" />

                        </div>

                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </div>
    </div>
    <div class="col-lg-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="mb-3">
                    <insite:ComboBox runat="server" ID="RegistrationNotificationCombo" Width="200px" DropDown-Width="400px" />
                    <insite:ComboBox runat="server" ID="RegistrationCombo" Width="200px" />
                    <insite:IconButton runat="server" ID="RegistrationNotificationButton" Name="bolt" />
                </div>

                <h3>Candidate Timers</h3>
                <p class="help" runat="server" id="RegistrationTimerHelp">There are no timers started for this set of exam candidates.</p>

                <asp:Repeater runat="server" ID="RegistrationTimerRepeater">
                    <ItemTemplate>

                        <div style="border-top: dotted #F5F5F5 1px; margin-bottom: 20px;">

                            <div>
                                <strong>
                                    <%# LocalizeTime(Eval("TriggerTime"), "span") %>
                                </strong>
                                <div class="float-end">
                                    <label class="badge bg-<%# (string)Eval("TimerStatus") == "Started" ? "primary" : "custom-default" %>">
                                        <%# Eval("TimerStatus") %>
                                    </label>
                                </div>
                            </div>

                            <div>
                                <%# Eval("TimerDescription") %>
                            </div>

                            <insite:IconButton runat="server" CommandArgument='<%# Eval("TriggerCommand") %>' CommandName="Elapse" Name="alarm-clock" ToolTip="Force this timer to elapse now" />
                            <insite:IconButton runat="server" CommandArgument='<%# Eval("TriggerCommand") %>' CommandName="Cancel" Name="ban" ToolTip="Cancel this timer" />

                        </div>

                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </div>
    </div>
</div>

<div class="row">
    <div class="col-12">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <h3>Mailouts</h3>
                <p class="help" runat="server" id="MailoutHelp">There are no mailouts scheduled for this event.</p>

                <insite:Grid runat="server" ID="MailoutGrid" EnablePaging="false" DataKeyNames="MailoutIdentifier,MessageIdentifier">
                    <Columns>

                        <asp:TemplateField HeaderText="Scheduled" HeaderStyle-Width="200px" HeaderStyle-Wrap="False" ItemStyle-CssClass="datetime-column">
                            <ItemTemplate>
                                <%# LocalizeTime(Eval("MailoutScheduled")) %>
                                <div>
                                    <label class="badge bg-<%# (bool)Eval("IsOverdue") ? "danger" : "success" %>">
                                        <%# Eval("Age") %>
                                    </label>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Started" HeaderStyle-Width="200px" HeaderStyle-Wrap="False" ItemStyle-CssClass="datetime-column">
                            <ItemTemplate>
                                <%# LocalizeTime(Eval("MailoutStarted")) %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Completed" HeaderStyle-Width="200px" HeaderStyle-Wrap="False" ItemStyle-CssClass="datetime-column">
                            <ItemTemplate>
                                <%# LocalizeTime(Eval("MailoutCompleted")) %>
                                <div>
                                    <%# GetCompletionStatus(Container.DataItem) %>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Subject">
                            <ItemTemplate>
                                <%# Eval("MessageTitle") %>
                                <div class="form-text">
                                    <%# Eval("MessageName") %>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField HeaderText="Recipients" DataField="DeliveryCount" DataFormatString="{0:n0}" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="number-column text-end" />

                        <asp:TemplateField ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
                            <ItemTemplate>

                                <insite:IconLink runat="server" Name="file-alt" Type="Regular"
                                    NavigateUrl='<%# Eval("MailoutIdentifier", "/ui/admin/messages/reports/mailout-summary?mailout={0}") %>' />

                                <insite:IconButton runat="server" Visible='<%# !(bool)Eval("IsStarted") %>' Name="ban" ToolTip="Cancel Mailout"
                                    CommandName="Cancel" CommandArgument='<%# Eval("MailoutIdentifier") %>'
                                    OnClientClick="return confirm('Are you sure you want to cancel this mailout?')" />

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </insite:Grid>

            </div>
        </div>
    </div>
</div>