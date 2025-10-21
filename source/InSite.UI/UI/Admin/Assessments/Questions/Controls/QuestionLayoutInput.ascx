<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionLayoutInput.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionLayoutInput" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <insite:ComboBox runat="server" ID="LayoutType" Width="48%">
            <Items>
                <insite:ComboBoxOption Text="List" Value="List" Selected="true" />
                <insite:ComboBoxOption Text="Table" Value="Table" />
            </Items>
        </insite:ComboBox>

        <insite:Container runat="server" ID="TableColumnsContainer" Visible="false">
            <asp:Repeater runat="server" ID="TableColumnsRepeater">
                <HeaderTemplate>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Alignment</th>
                                <th>Style</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <insite:TextBox runat="server" ID="Name" MaxLength="100" />
                        </td>
                        <td>
                            <insite:ComboBox runat="server" ID="Alignment" Width="120px">
                                <Items>
                                    <insite:ComboBoxOption Text="Left" Value="Left" Selected="true" />
                                    <insite:ComboBoxOption Text="Center" Value="Center" />
                                    <insite:ComboBoxOption Text="Right" Value="Right" />
                                </Items>
                            </insite:ComboBox>
                        </td>
                        <td>
                            <insite:TextBox runat="server" ID="Style" MaxLength="100" />
                        </td>
                        <td style="width:30px; line-height:30px; white-space:nowrap;">
                            <insite:IconButton runat="server"
                                CommandName="Delete" CommandArgument='<%# Container.ItemIndex %>' Name="trash-alt" ToolTip="Delete" 
                                ConfirmText="Are you sure you want to delete this column?" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <div class="form-group mt-3">
                <insite:Button runat="server" ID="AddTableColumnButton" Icon="fas fa-plus-circle" Text="Add New Column"  ButtonStyle="Default" />
            </div>
        </insite:Container>
    </ContentTemplate>
</insite:UpdatePanel>