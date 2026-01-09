<%@ Page Language="C#" CodeBehind="AddPrerequisite.aspx.cs" Inherits="InSite.Admin.Achievements.Achievements.Forms.AddPrerequisite" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />

    <asp:Panel runat="server" ID="ActionPanel">

        <section runat="server" ID="AchievementSection" class="mb-3">

                <div class="row">

                    <div class="col-lg-7">

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">Select Achievement</label>
                                    <div class="row">
                                        <div class="col-md-8 pe-0">
                                            <insite:FindAchievement runat="server" ID="AchievementCombo" />
                                        </div>
                                        <div class="col-md-4">
                                            <insite:Button runat="server" ID="AddButton" Text="Add" Icon="fas fa-plus-circle" />
                                            <insite:Button runat="server" ID="UndoButton" Text="Undo" Icon="fas fa-undo" />
                                        </div>

                                    </div>
                                </div>

                            </div>
                        </div>

                    </div>

                    <div class="col-lg-5">

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">Achievement Prerequisite(s)</label>
                                    <div>
                            
                                        <asp:Literal runat="server" ID="AchievementRepeaterEmpty" Text="None" />

                                           <asp:Repeater runat="server" ID="AchievementRepeater">
                                                <HeaderTemplate>
                                                    <ul>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <li><%# Eval("Text") %></li>
                                                </ItemTemplate>
                                                <SeparatorTemplate>
                                                   <span class="text-primary">
                                                       &ndash; or &ndash;
                                                   </span>
                                                </SeparatorTemplate>
                                                <FooterTemplate>
                                                    </ul>
                                                </FooterTemplate>
                                           </asp:Repeater>

                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                </div>
        </section>

        <insite:SaveButton runat="server" ID="SaveButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />

    </asp:Panel>

</asp:Content>
