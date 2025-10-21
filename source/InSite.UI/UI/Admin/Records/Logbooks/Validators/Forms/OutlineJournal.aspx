<%@ Page Language="C#" CodeBehind="OutlineJournal.aspx.cs" Inherits="InSite.UI.Admin.Records.Validators.Forms.OutlineJournal" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	
<%@ Register Src="~/UI/Admin/Records/Logbooks/Controls/LogbookDetailsV2.ascx" TagName="LogbookDetail" TagPrefix="uc" %>	
<%@ Register Src="~/UI/Admin/Records/Logbooks/Controls/CompetencyProgressGrid.ascx" TagName="CompetencyProgressGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/CommentList.ascx" TagName="CommentList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="LogbookPanel" Title="Logged Entries" Icon="far fa-pencil-ruler" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Logged Entries
                </h2>
                                   
                <div class="mb-3">
                    <insite:Button runat="server" ID="AddExperienceLink" Text="Add Entry" Icon="fas fa-plus-circle" ButtonStyle="Default" />
                </div>

                <div class="row">

                    <div class="col-lg-6">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <div runat="server" id="NoExperiencePanel" class="alert alert-warning mb-3">
                                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                                    This user does not have any entry.
                                </div>

                                <div runat="server" id="ExperiencePanel">
                                    <h3>Entries</h3>

                                    <asp:Repeater runat="server" ID="ExperienceRepeater">
                                        <HeaderTemplate>
                                            <table class="table table-striped">
                                                <thead>
                                                    <tr>
                                                        <th>#</th>
                                                        <th>Created</th>
                                                        <th>Status</th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <FooterTemplate>
                                                </tbody>
                                            </table>
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <%# Eval("Sequence") %>
                                                </td>
                                                <td>
                                                    <%# GetLocalTime(Eval("Created")) %>
                                                </td>
                                                <td>
                                                    <%# Eval("Status") %>
                                                </td>
                                                <td class="text-end" style="width:90px;">
                                                    <insite:IconLink runat="server" Name='<%# Eval("ValidateButtonIcon") %>' ToolTip='<%# Eval("ValidateButtonHint") %>'
                                                        NavigateUrl='<%# Eval("ExperienceIdentifier", "/ui/admin/records/logbooks/validators/validate-experience?experience={0}") %>'
                                                    />
                                                    <insite:IconLink runat="server" Name="search" ToolTip='View Entry'
                                                        NavigateUrl='<%# Eval("ViewExperienceUrl") %>'
                                                    />
                                                    <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete" NavigateUrl='<%# Eval("DeleteUrl") %>' />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-3">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <h3>Learner Details</h3>
                                <uc:PersonDetail runat="server" ID="PersonDetail" />
                            </div>
                        </div>
                    </div>

                        <div class="col-lg-3">                                    
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                    <h3>Logbook Details</h3>
                                        <uc:LogbookDetail runat="server" ID="LogbookDetail" />
                            </div>
                        </div>
                    </div>
                </div>

            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ProgressPanel" Title="Progress" Icon="far fa-check" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Progress
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:CompetencyProgressGrid runat="server" ID="ProgressGrid" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CommentsPanel" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Comments
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:CommentList runat="server" ID="Comments" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>


</div>
</asp:Content>
