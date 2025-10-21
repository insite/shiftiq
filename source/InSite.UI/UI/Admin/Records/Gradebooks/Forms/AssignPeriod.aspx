<%@ Page Language="C#" CodeBehind="AssignPeriod.aspx.cs" Inherits="InSite.Admin.Records.Gradebooks.Forms.AssignPeriod" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="../Controls/AssignPeriodGrid.ascx" TagName="AssignPeriodGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-user me-1"></i>
            Students
        </h2>

        <div class="row mb-3">
            <div class="col-md-4">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div>
                            <insite:TextBox runat="server" ID="FilterTextBox" Width="300" EmptyMessage="Student" />
                            <insite:PageFooterContent runat="server">
                                <script type="text/javascript"> 
                                    (function () {
                                        $('#<%= FilterTextBox.ClientID %>')
                                            .off('keydown', onKeyDown)
                                            .on('keydown', onKeyDown);

                                        function onKeyDown(e) {
                                            if (e.which === 13) {
                                                e.preventDefault();
                                                $('#<%= SearchButton.ClientID %>')[0].click();
                                            }
                                        }
                                    })();
                                </script>
                            </insite:PageFooterContent>
                        </div>
                        <div class="pt-1">
                            <insite:DateSelector ID="GradedSince" runat="server" EmptyMessage="Graded Since" Width="300" />
                        </div>
                        <div class="pt-1">
                            <insite:DateSelector ID="GradedBefore" runat="server" EmptyMessage="Graded Before" Width="300" />
                        </div>
                        <div class="pt-1">
                            <insite:FindPeriod runat="server" ID="FilterPeriodIdentifier" EmptyMessage="All Periods" Width="300" />
                        </div>
                        <div class="pt-1">
                            <asp:CheckBox runat="server" ID="HideAssignedPeriod" Text="Hide Assigned Period" />
                        </div>
                        <div class="pt-1">
                            <insite:FilterButton runat="server" ID="SearchButton" />
                            <insite:ClearButton runat="server" ID="ClearButton" />
                        </div>

                    </div>
                </div>
            </div>
            <div class="col-md-8">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <insite:FindPeriod runat="server" ID="UpdatePeriodIdentifier" Width="380" />

                        <insite:Button runat="server" ID="UpdateButton"
                            Text="Update all shown with period"
                            Icon="fas fa-cloud-upload"
                            ButtonStyle="Success"
                            ConfirmText="Are you sure to assign selected priod to these students?"
                        />

                    </div>
                </div>
            </div>

        </div>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row">
                    <div class="col-lg-12">
                        <uc:AssignPeriodGrid runat="server" ID="AssignPeriodGrid" />
                    </div>
                </div>

            </div>
        </div>

    </section>

</asp:Content>
