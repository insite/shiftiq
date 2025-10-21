<%@ Page Language="C#" CodeBehind="Post.aspx.cs" Inherits="InSite.Admin.Events.Comments.Forms.Post" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="../Controls/CommentPanel.ascx" TagName="CommentPanel" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="Status" />

    <section runat="server" ID="CommentSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-comment me-1"></i>
            Comment
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
            
                <div class="row">

                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Comment Text
                                <insite:RequiredValidator runat="server" FieldName="Comment Text" ControlToValidate="CommentText" Display="Dynamic" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="CommentText" TextMode="MultiLine" Width="100%" Height="150px" />
                            </div>
                        </div>

                    </div>

                </div>

            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
        <div class="col-lg-12 mt-3">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <uc:CommentPanel ID="CommentPanel" runat="server" IsReadOnly="true" />

                </div>
            </div>
        </div>
    </div>

</asp:Content>
