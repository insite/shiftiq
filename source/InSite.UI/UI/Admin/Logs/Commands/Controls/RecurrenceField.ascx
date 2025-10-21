<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecurrenceField.ascx.cs" Inherits="InSite.Admin.Logs.Commands.Controls.RecurrenceField" %>

<div class="form-group mb-3">
    <label class="form-label">
        Recurrence
        <asp:CustomValidator runat="server" ID="RecurrenceValidator" ValidationGroup="Command" Display="None" />
    </label>
    <div>
        <div style="display:inline-block; width:calc(50% - 5px)">
            <insite:NumericBox runat="server" ID="RecurrenceInterval" NumericMode="Integer" MinValue="0" EmptyMessage="Interval" />
        </div>
        <div style="display:inline-block; width:50%">
            <insite:ComboBox runat="server" ID="RecurrenceUnit" EmptyMessage="Unit">
                <Items>
                    <insite:ComboBoxOption />
                    <insite:ComboBoxOption Text="Minutes" Value="minute" />
                    <insite:ComboBoxOption Text="Hours" Value="hour" />
                    <insite:ComboBoxOption Text="Days" Value="day" />
                    <insite:ComboBoxOption Text="Weeks" Value="week" />
                    <insite:ComboBoxOption Text="Months" Value="month" />
                </Items>
            </insite:ComboBox>
        </div>
        <div class="form-text" style="margin-top:5px;">
            <asp:Label runat="server" ID="LastInterval" />
            <asp:Label runat="server" ID="NextInterval" />
         </div>
    </div>
</div>
<div class="form-group mb-3">
    <label class="form-label">
        Activated
    </label>
    <div>
        <asp:CheckBoxList runat="server" ID="RecurrenceWeekdays">
            <asp:ListItem Value="Sun" Text="Sunday" />
            <asp:ListItem Value="Mon" Text="Monday" />
            <asp:ListItem Value="Tue" Text="Tuesday" />
            <asp:ListItem Value="Wed" Text="Wednesday" />
            <asp:ListItem Value="Thu" Text="Thursday" />
            <asp:ListItem Value="Fri" Text="Friday" />
            <asp:ListItem Value="Sat" Text="Saturday" />
        </asp:CheckBoxList>
    </div>
</div>