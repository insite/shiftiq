<%@ Page Language="C#" CodeBehind="Validate.aspx.cs" Inherits="InSite.Cmds.Actions.BulkTool.Validate.Competency" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        table.competencies td { padding-right:5px; }
    </style>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Validate Competencies
        </h2>

        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">
                <h3>Profile and Person</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Person
                    </label>
                    <div>
                        <cmds:FindPerson ID="Employee" runat="server" CssClass="w-50"/>
                    </div>
                    <div class="form-text"></div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Profile
                    </label>
                    <div>
                        <cmds:FindProfile ID="CurrentProfile" runat="server" CssClass="w-50" />
                    </div>
                    <div class="form-text"></div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Self-Assessment Status
                    </label>
                    <div>
                        <asp:RadioButtonList runat="server" ID="SelfAssessmentStatus" />
                    </div>
                    <div class="form-text"></div>
                </div>
            </div>
        </div>

        <asp:Panel ID="ValidationPanel" runat="server">

            <div runat="server" id="CompetenciesPanel" class="card border-0 shadow-lg mb-3">
                <div class="card-body">

                    <h3>Competencies</h3>

                    <asp:Repeater ID="Competencies" runat="server">
                        <HeaderTemplate>
                            <table class="competencies">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Literal ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                                    <insite:CheckBox ID="IsSelected" runat="server" Text='<%# Eval("Number") %>' />
                                    <asp:Literal ID="CannotValidate" runat="server" Text="*" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                    <div class="mt-3">
                        <insite:Button ID="SelectAllButton" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                        <insite:Button ID="UnselectAllButton" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                    </div>

                    <div class="mt-3 text-muted fs-sm">
                        <i class="fas fa-info-circle"></i>
                        You can validate only those competencies for which you yourself have been validated.
                        Similarly, you cannot validate competencies for which you yourself have not been validated.
                    </div>
                </div>
            </div>
        
            <div class="card border-0 shadow-lg mb-3">
                <div class="card-body">

                    <h3>Validation</h3>

                    <p><strong>Do you agree with the candidate's self assessment of the competencies submitted for validation?</strong></p>
            
                    <div>
                        <asp:RadioButtonList ID="ValidatorSelection" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" style="display:none;">
                            <asp:ListItem Value="Yes" />
                            <asp:ListItem Value="No" />
                        </asp:RadioButtonList>
                
                        <insite:Button ID="YesButton" runat="server" ButtonStyle="Success" Icon="fas fa-check" Text="Yes" OnClientClick="return selectButton(0);" />
                        <insite:Button ID="NoButton" runat="server" ButtonStyle="Default" Icon="fas fa-ban" Text="No" OnClientClick="return selectButton(1);" />

                        <div class="d-none">
                            <insite:RequiredValidator runat="server"
                                ControlToValidate="ValidatorSelection"
                                ValidationGroup="Competency"
                                ErrorMessage="You <strong>must</strong> answer Yes or No to the validation question."
                            />
                        </div>
                    </div>

                </div>
            </div>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h3>
                        Validator Comments
                        <insite:RequiredValidator runat="server" ControlToValidate="ValidatorComment" ValidationGroup="Competency" FieldName="Validator Comment" ToolTip="Required Field" />
                    </h3>

                    <table class="table table-striped">
                    <asp:Repeater ID="PredefinedComments" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td class="text-nowrap align-top" style="padding-bottom:5px;">
                                    <insite:Button runat="server" ButtonStyle="Success" Icon="fas fa-plus-circle" Text="Add Comment" OnClientClick='<%# string.Format(@"addComment(""{0}""); return false;", Container.DataItem) %>' />
                                </td>
                                <td style="padding-bottom:5px;">
                                    <%# Container.DataItem %>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    </table>
            
                    <div>
                        <insite:TextBox ID="ValidatorComment" runat="server" Rows="10" TextMode="MultiLine" />
                    </div>

                </div>
            </div>

        </asp:Panel>

    </section>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Competency" />
    <insite:CancelButton runat="server" NavigateUrl="/ui/admin/tools" />

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            function addComment(text) {
                const input = $get("<%= ValidatorComment.ClientID %>");

                if (input.value == "")
                    input.value = text;
                else
                    input.value += "\n" + text;
            }

            function selectButton(index) {
                const selector = $get("<%= ValidatorSelection.ClientID %>");
                const list = selector.getElementsByTagName("input");

                list[index].checked = true;

                const yesButton = document.getElementById("<%= YesButton.ClientID %>");
                const noButton = document.getElementById("<%= NoButton.ClientID %>");

                switch (index) {
                    case 0:
                        yesButton.classList.replace("btn-default", "btn-success");
                        noButton.classList.replace("btn-success", "btn-default");
                        break;
                    case 1:
                        yesButton.classList.replace("btn-success", "btn-default");
                        noButton.classList.replace("btn-default", "btn-success");
                        break;
                }

                return false;
            }
        </script>
    </insite:PageFooterContent>
</asp:Content>
