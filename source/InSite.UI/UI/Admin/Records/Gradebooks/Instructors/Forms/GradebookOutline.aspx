<%@ Page Language="C#" CodeBehind="GradebookOutline.aspx.cs" Inherits="InSite.Records.Gradebooks.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/Admin/Records/Gradebooks/Controls/OutlineProgressList.ascx" TagName="OutlineProgressList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Records/Gradebooks/Controls/GradeItemsGrid.ascx" TagName="GradeItemsGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="ScoresSectionPanel" Title="Scores" Icon="far fa-ballot-check" IconPosition="BeforeText">
            <section runat="server" ID="ScoresSection">

                <h2 class="h4 mt-4 mb-3">
                    Scores
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <uc:OutlineProgressList runat="server" ID="ScoreList" UserMode="Design" />

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ConfigurationSection" Title="Grade Items" Icon="far fa-list-ul" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Grade Items
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:GradeItemsGrid runat="server" ID="GradeItemsGrid" />

                    </div>
                </div>
            </section>
        </insite:NavItem>
    </insite:Nav>

</asp:Content>
