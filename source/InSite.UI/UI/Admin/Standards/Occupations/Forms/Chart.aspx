<%@ Page Language="C#" CodeBehind="Chart.aspx.cs" Inherits="InSite.Admin.Standards.Occupations.Forms.Chart" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:PageHeadContent runat="server">

        <style type="text/css">
            .occupation-line {
                margin-bottom: 20px;
            }

            .competency {
                border: 1px solid #818285;
                border-radius: 5px;
                box-shadow: 3px 3px 3px #444;
                padding: 10px;
                margin-bottom: 20px;
                min-height: 91px;
            }

            .competency-name {
                min-height: 46px;
            }
        </style>

    </insite:PageHeadContent>

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="icon far fa-boxes"></i>
            Chart
        </h2>

        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <asp:Repeater runat="server" ID="LineRepeater">
                            <ItemTemplate>
                                <div class="row section-panel">
                                    <div class="col-lg-1">
                                        <h3><%# Eval("Letter") %></h3>
                                    </div>
                                    <div class="col-lg-11">
                                        <h3><%# Eval("Name") %></h3>

                                        <div class="row">
                                            <asp:Repeater runat="server" ID="CompetencyRepeater">
                                                <ItemTemplate>

                                                    <div class="col-md-4">
                                                        <div class="competency">
                                                            <div class="competency-name">
                                                                <strong><%# Eval("Letter") %><%# Eval("Number") %></strong>
                                                                <%# Eval("Name") %>
                                                            </div>
                                                            <div class="competency-levels">
                                                                <%# GetLevelLabel(1,(bool)Eval("HasLevel1"),(string)Eval("Level1Url")) %>
                                                                <%# GetLevelLabel(2,(bool)Eval("HasLevel2"),(string)Eval("Level2Url")) %>
                                                                <%# GetLevelLabel(3,(bool)Eval("HasLevel3"),(string)Eval("Level3Url")) %>
                                                                <%# GetLevelLabel(4,(bool)Eval("HasLevel4"),(string)Eval("Level4Url")) %>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>

                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>

                </div>

            </div>
        </div>

    </section>



</asp:Content>
