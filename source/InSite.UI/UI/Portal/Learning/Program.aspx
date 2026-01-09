<%@ Page Language="C#" CodeBehind="Program.aspx.cs" Inherits="InSite.UI.Portal.Records.Programs.Navigate" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">
    <style type="text/css">
        .col img{
                  max-width:100%;
                  max-height:100%;
                }

        .col .img-overlay{
                  position:absolute;
                  top:0;
                  left:0;
                  height:0;
                  bottom:0;
                  width:100%;
                  height:100%;
                  background:rgba(0,0,0,0.4);
                  border-radius: 1rem;
                }

        .disableLink{
               pointer-events: none !important;
               cursor: default;
        }
    </style>
</insite:PageHeadContent>

    <asp:PlaceHolder runat="server" ID="CourseStyles" />

    <div runat="server" id="EnabledPanel">
        
        <div class="lesson col-full-height">

            <asp:Literal runat="server" ID="CourseBreadcrumbs" />

            <h1 class="py-1 my-2 pb-2 mb-4">
                <asp:Literal runat="server" ID="ProgramTitle" />
            </h1>

            <h2><%= Translate("Program Overview") %></h2>
            <div class="mb-3">
                <asp:Literal runat="server" ID="OverviewText" />
            </div>

        </div>

        <div class="row row-cols-1 row-cols-md-4 g-4">

            <asp:Repeater runat="server" ID="TaskRepeater">
                <ItemTemplate>
                    <div class="col"> 
                        <a runat="server" id="cardLink" class="card card-hover card-tile border-0 shadow" href='<%# GetTaskUrl(Eval("TaskIdentifier")) %>' target='<%# Eval("ObjectType") %>' >
                            <asp:Literal runat="server" ID="CardStatus" />
                            <asp:Literal runat="server" ID="CardImage" />
                            <asp:Literal runat="server" ID="CardBadge" />
                            
                            <div class="card-body text-center">
                                <asp:Literal runat="server" ID="CardIcon" />
                                <h3 class="h5 nav-heading mb-2"><%# GetTaskName((Guid)Eval("TaskIdentifier"),(String)Eval("ObjectType")) %></h3>
                            </div>

                             <div runat="server" class="img-overlay" id="ImageOverlay" visible='<%# (bool)Eval("IsLocked") %>'></div>
                        </a>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        
        </div>


    </div>


</asp:Content>
