<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeeCertificateGrid.ascx.cs" Inherits="InSite.Custom.CMDS.User.Progressions.Controls.EmployeeCertificateGrid" %>

<insite:Grid runat="server" ID="Grid">
        <Columns>
            <asp:BoundField HeaderText="Certificate" DataField="ProfileTitle" />
            <asp:BoundField HeaderText="Institution" DataField="InstitutionName" ItemStyle-Width="150" />
            <asp:BoundField HeaderText="Requested" DataField="DateRequested" DataFormatString="{0:MMM d, yyyy}" ItemStyle-Width="150" />
            <asp:BoundField HeaderText="Granted" DataField="DateGranted" DataFormatString="{0:MMM d, yyyy}" ItemStyle-Width="150" />
            <asp:BoundField HeaderText="Submitted" DataField="DateSubmitted" DataFormatString="{0:MMM d, yyyy}" ItemStyle-Width="150" />
        </Columns>
</insite:Grid>
