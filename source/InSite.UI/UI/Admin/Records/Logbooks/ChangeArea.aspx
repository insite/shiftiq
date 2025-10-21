<%@ Page Language="C#" CodeBehind="ChangeArea.aspx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.ChangeArea" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="ChangeArea" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Area Name
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="AreaName" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        <insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/>
                    </label>
                    <div>
                        <insite:NumericBox runat="server" ID="AreaHours" DecimalPlaces="2" Width="100" />
                    </div>
                </div>

            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="ChangeArea" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
