<%@ Page Language="C#" CodeBehind="AssignDepartment.ascx.cs" Inherits="InSite.Cmds.Actions.BulkTool.Assign.PeopleToDepartments" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section runat="server" ID="DepartmentSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-users me-1"></i>
            People
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                <insite:UpdatePanel runat="server" ID="UpdatePanel">
                    <ContentTemplate>

                        <div class="row">
                            <div class="col-lg-6">
                                <h3>Department</h3>
                
                                <div class="form-group mb-3">
                                    <cmds:FindDepartment ID="Department" runat="server" />
                                </div>
                            </div>
                        </div>

                        <asp:PlaceHolder runat="server" ID="EmployeesPanel">
                            <h3>People</h3>
                            <asp:CheckBoxList ID="EmployeeList" runat="server" RepeatColumns="5" RepeatDirection="Vertical" />
                        </asp:PlaceHolder>

                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" />
        <insite:CancelButton runat="server" NavigateUrl="/ui/admin/tools" />
    </div>

</asp:Content>
