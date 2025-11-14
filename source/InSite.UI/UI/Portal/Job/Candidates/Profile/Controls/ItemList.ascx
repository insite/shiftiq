<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemList.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.ItemList" %>

<asp:CustomValidator runat="server" ID="ItemsValidator" ErrorMessage="Minimum 2 and maximum 10 duties should be specified" ValidationGroup="Detail" />

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="mb-3">
            <div class="mb-3">
                <insite:Button runat="server" ID="AddButton" Icon="fas fa-plus-circle" ButtonStyle="Default" Text="Add Duties" ValidationGroup="ItemList" />
                <insite:RequiredValidator runat="server" ControlToValidate="Description" ValidationGroup="ItemList" Display="Dynamic" />
            </div>
            <div>

            </div>
            <div class="mb-3">
                <insite:TextBox runat="server" ID="Description" MaxLength="240" TextMode="MultiLine" Rows="2" />
            </div>
            <div class="mb-3">

                <asp:Repeater runat="server" ID="ItemRepeater">
                    <ItemTemplate>
                
                        <div class="card mb-3">
                            <div class="card-body">
                                <div class="card-text mb-3">
                                    <div class="row">
                                        <div class="col-11">
                                            <insite:TextBox runat="server" ID="Description" MaxLength="240" TextMode="MultiLine" Rows="2" Text='<%# Container.DataItem %>' />
                                        </div>
                                        <div class="col-1">
                                            <div class="float-end">
                                                <insite:IconButton runat="server" Name="trash-alt" ToolTip="Delete" CommandName="Delete" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>