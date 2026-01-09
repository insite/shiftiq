<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TrainingRecordsReport.ascx.cs" Inherits="InSite.Admin.Contacts.People.Forms.TrainingRecordsReport" %>

<style type="text/css">
    body {
        font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
        font-size: 14px;
        line-height: 1.42857143;
        color: #333;
        margin: 0;
    }
    
    * {
        -webkit-box-sizing: border-box;
        -moz-box-sizing: border-box;
        box-sizing: border-box;
    }

    label {
        display: inline-block;
        max-width: 100%;
        margin-bottom: 5px;
        font-weight: bold;
    }

    table {
        background-color: transparent;
    }
    table {
        border-spacing: 0;
        border-collapse: collapse;
    }
    
    div.trade, table, tr, td, th, tbody, thead, tfoot {
        page-break-inside: avoid !important;
    }

    .form-group label { 
        font-weight:bold;
    }
</style>

<div>
    <asp:Image runat="server" ID="OrganizationLogo" style="max-height:80px;" />
</div>

<div style="width:100%; padding-top:5px;">
    <div style="text-align:center; border:solid 2px Black;">
        <b><asp:Literal ID="OrganizationName" runat="server" >RCABC Educational Foundation</asp:Literal></b> <br />
        <b>Record of Training</b>
    </div>

    <div style="text-align:center; border-bottom:solid 1px Black; border-left:solid 2px Black; border-right:solid 2px Black;">
        <div style="width:50%; float:left;position:relative; border-right: solid 1px Black; padding-bottom: 10px;padding-top:10px;">
            Apprentice/ Trainee Information
        </div>
        <div style="width:50%; float:left;position:relative; padding-bottom: 10px;padding-top:10px;">School Information</div>

        <div style="clear: both;"></div>
    </div>

    <div style="border-bottom:solid 1px Black; border-left:solid 2px Black; border-right:solid 2px Black;">
        <div style="width:50%; float:left;position:relative; border-right: solid 1px Black; padding-left: 5px;">
            <div>
                <label>Full Name:</label>
                <asp:Literal runat="server" ID="ApprenticeFullName" />
            </div>
            <div>
                <label>Address:</label>
                <asp:Literal runat="server" ID="ApprenticeAddress" />
            </div>
            <div>
                <label>Phone:</label>
                <asp:Literal runat="server" ID="ApprenticePhone" />
            </div>
            <div>
                <label>Email:</label>
                <asp:Literal runat="server" ID="ApprenticeEmail" />
            </div>
            <div>
                <label>SkilledTradesBC #:</label>
                <asp:Literal runat="server" ID="ApprenticeITA" />
            </div>
            <div>
                <label>Birthdate:</label>
                <asp:Literal runat="server" ID="ApprenticeBirthDate" />
            </div>
        </div>
        <div style="width:50%; float:left;position:relative; padding-left: 5px;">
            <div>
                <label>Name:</label>
                <asp:Literal runat="server" ID="OrganizationName2" />
            </div>
            <div>
                <label>Address:</label>
                <asp:Literal runat="server" ID="OrganizationAddress" />
            </div>
            <div>
                <label>Phone:</label>
                <asp:Literal runat="server" ID="OrganizationPhone" />
            </div>
            <div>
                <label>Email:</label>
                <asp:Literal runat="server" ID="OrganizationEmail" />
            </div>
        </div>

        <div style="clear: both;"></div>
    </div>

    <div style="text-align:center; border-top:solid 1px Black; border-left:solid 2px Black; border-right:solid 2px Black;">
        <b>Apprenticeship Training</b>
    </div>
    
    <asp:Repeater runat="server" ID="TradeRepeater">
        <ItemTemplate>
            <div class="trade">
                <div style="border:solid 2px Black;">
                    <b>Trade: <%# Eval("AchievementDescription") %></b>
                </div>

                <asp:Repeater runat="server" ID="LevelRepeater">
                    <HeaderTemplate>
                        <table style="width:100%;border-top: solid 1px Black;border-bottom: solid 2px Black;border-left: solid 2px Black;border-right: solid 2px Black;">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# (Container.ItemIndex % 2) == 0 ? "<tr>" : "" %>
                            <td style="width:50%; border: solid 1px Black; padding-bottom: 10px;padding-top:10px; padding-left:5px; padding-top:5px; vertical-align:top;">
                                <div runat="server" visible='<%# !(bool)Eval("IsEmpty") %>'>
                                    <div style="padding-bottom:10px;">
                                        <%# Eval("AchievementTitle") %>
                                    </div>
                                    <div>
                                        <label>Class:</label>
                                        <%# Eval("EventTitle") %> on <%# Eval("EventScheduledStart") %> - <%# Eval("EventScheduledEnd") %>
                                    </div>
                                    <div>
                                         <asp:Repeater runat="server" ID="ScoreRepeater">
                                            <ItemTemplate>
                                                <div>
                                                    <label><%# Eval("ScoreName") %>:</label>
                                                    <%# Eval("ScoreValue") %>
                                                    <div><%# Eval("ScoreComment") %></div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </td>
                        <%# (Container.ItemIndex % 2) == 1 ? "</tr>" : "" %>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <div style="text-align:center; border-top:solid 1px Black; border-bottom:solid 2px Black; border-left:solid 2px Black; border-right:solid 2px Black;">
        <b>Other Training</b>
    </div>

    <asp:Repeater runat="server" ID="CertificateRepeater">
        <HeaderTemplate>
            <table style="width:100%;border-top: solid 1px Black;border-bottom: solid 2px Black;border-left: solid 2px Black;border-right: solid 2px Black;">
        </HeaderTemplate>
        <ItemTemplate>
            <%# (Container.ItemIndex % 2) == 0 ? "<tr>" : "" %>
                <td style="width:50%; border: solid 1px Black; padding-bottom: 10px;padding-top:10px; padding-left:5px; padding-top:5px; vertical-align:top;">
                    <div runat="server" visible='<%# !(bool)Eval("IsEmpty") %>'>
                        <div>
                            <label>Certificate:</label>
                            <%# Eval("AchievementTitle") %>
                        </div>
                        <div>
                            <label>Class:</label>
                            <%# Eval("ClassInfo") %>
                        </div>
                        <div>
                            <label>Score</label>
                            <%# Eval("ScoreValue") %>
                        </div>
                        <div>
                            <label>Most Recently Granted:</label>
                            <%# Eval("Granted") %>
                        </div>
                        <div>
                            <label>Next Expiry</label>
                            <%# Eval("ExpirationExpected") %>
                        </div>
                    </div>
                </td>
            <%# (Container.ItemIndex % 2) == 1 ? "</tr>" : "" %>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>

    <div style="clear: both;"></div>

</div>