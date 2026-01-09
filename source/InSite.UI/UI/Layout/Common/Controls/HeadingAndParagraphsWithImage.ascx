<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeadingAndParagraphsWithImage.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.HeadingAndParagraphsWithImage" %>

<div class="container">
    <div class="row">
        <div class="col-lg-12">
            <h3 runat="server" id="BlockHeading"></h3>
            <div>
                <div class="ms-3 mb-3 float-end" style="max-width:50%">
                    <figure class="figure">
                      <img runat="server" id="BlockImage" src="#" class="figure-img img-fluid" />
                      <figcaption runat="server" id="BlockImageCaption" class="figure-caption text-center"></figcaption>
                    </figure>
                </div>
                <asp:Literal runat="server" ID="BlockParagraphs" />
            </div>
        </div>
    </div>
</div>