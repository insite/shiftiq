<%@ Page Language="C#" CodeBehind="ChangePrivacy.aspx.cs" Inherits="InSite.UI.Admin.Events.Classes.ChangePrivacy" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>


<%@ Register Src="./Controls/ClassSummaryInfo.ascx" TagName="ClassSummaryInfo" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="ClassSummaryInfo" />

    <section class="mb-3">
            
        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:ClassSummaryInfo runat="server" ID="ClassSummaryInfo" />
                    </div>
                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <h3>Filter</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Group Type</label>
                            <div>
                                <insite:GroupTypeComboBox runat="server" ID="FilterGroupType" AllowBlank="false" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Group Name</label>
                            <div class="hstack">
                                <insite:TextBox runat="server" ID="FilterGroupName" MaxLength="100" CssClass="me-1" />
                                <insite:FilterButton runat="server" ID="FilterGroupButton" Text="Filter" />
                            </div>
                        </div>

                    </div>
                </div>

                <div class="card border-0 shadow-lg mt-3">
                    <div class="card-body">
                        <h3>Update Groups</h3>
                        <div class="form-group mb-3">
                            <asp:CheckBoxList runat="server" ID="FilterGroupList" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </section>


    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="ClassSummaryInfo" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
