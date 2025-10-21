<%@ Page Language="C#" CodeBehind="UserTrainingSummary.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.UserTrainingSummary" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

    <section runat="server" ID="CriteriaSection" class="mb-3">
        <insite:DownloadButton runat="server"
            ID="DownloadButton"
            Text="Download Report"
            CssClass="mb-3"
            DisableAfterClick="true"
            EnableAfter="10000"
        />

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="form-group mb-3">
                    <label class="form-label">
                        Department
                    </label>
                    <div>
                        <cmds:FindDepartment ID="Department" runat="server" Width="350" EmptyMessage="All Departments" />
                    </div>
                    <div class="form-text"></div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Categories
                    </label>
                    <div>
                        <asp:CheckBoxList ID="AchievementCategories" runat="server" CssClass="check-list"/>
                    </div>
                    <div class="form-text"></div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Achievements
                    </label>
                    <div>
                        <asp:RadioButtonList ID="IsRequired" runat="server" RepeatLayout="Flow" >
                            <asp:ListItem Text="All" Selected="True" />
                            <asp:ListItem Value="True" Text="Only Required" />
                        </asp:RadioButtonList>
                    </div>
                    <div class="form-text"></div>
                </div>

                <p>
                    Please note that this report is generated as a comma-separated-values (CSV) file.
                    You can open this file using Microsoft Excel.
                    If you make formatting changes to the report in Excel then please remember to save the file as a Microsoft Excel Workbook (XLS) - otherwise your formatting changes will not be saved.
                </p>
            </div>
        </div>
    </section>

</asp:Content>
