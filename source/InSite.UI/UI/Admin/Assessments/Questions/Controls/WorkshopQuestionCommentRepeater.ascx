<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkshopQuestionCommentRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.WorkshopQuestionCommentRepeater" %>

<%@ Register TagPrefix="uc" TagName="CommentRepeater" Src="CommentRepeater.ascx" %>

<div class="col-md-12">
    <h3 class="p-1 bg-secondary">
        Administrator Comments
        <span class="float-end d-block me-1 fs-6" style="line-height:26px;">
            <a runat="server" id="CandidateCommentaryLink" target="_blank"></a>
            <a data-action="show-all" class="ms-2"><i class="icon fas"></i></a>
        </span>
    </h3>

    <div class="posted-comments">
        <uc:CommentRepeater runat="server" ID="CommentRepeater" />
    </div>
</div>

<insite:PageHeadContent runat="server" ID="CommonStyle" RenderRequired="true">
    <style type="text/css">
        .posted-comments .question-comment.comment-hidden {
            display: none;
        }

        .posted-comments.show-hidden .question-comment.comment-hidden {
            display: inherit;
            opacity: 0.6;
        }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="CommonScript" RenderRequired="true">
    <script type="text/javascript">
        (function () {
            var isLocked = false;

            Sys.Application.add_load(onLoad);
            $(document).ajaxSuccess(onLoad);

            function onLoad() {
                $('.posted-comments').each(function () {
                    var $this = $(this);
                    if ($this.data('inited') === true)
                        return;

                    $this.find('a[data-action="hide"]').on('click', onHideClick).each(function () { updateHideLink(this); });
                    $this.parent().find('> h3 [data-action="show-all"]').on('click', onShowAllClick).data('container', $this).each(function () { updateShowAllLink(this) });
                    $this.data('inited', true);
                });
            }

            function onShowAllClick(e) {
                e.preventDefault();

                var $container = $(this).data('container');
                if ($container.hasClass('show-hidden'))
                    $container.removeClass('show-hidden');
                else
                    $container.addClass('show-hidden');

                updateShowAllLink(this);
            }

            function onHideClick(e) {
                e.preventDefault();

                if (isLocked)
                    return;

                var $link = $(this);
                var $container = $link.closest('div.question-comment');
                var isHidden = $container.hasClass('comment-hidden');

                var confirmText;
                if (isHidden)
                    confirmText = 'Are you sure you want to make this comment visible?';
                else
                    confirmText = 'Are you sure you want to hide this comment?';

                if (!confirm(confirmText))
                    return;

                isLocked = true;

                var data = {
                    bank: '<%= BankID %>',
                    comment: $link.data('id'),
                };

                if (isHidden)
                    data.hide = 0;
                else
                    data.hide = 1;

                $.ajax({
                    type: 'GET',
                    dataType: 'json',
                    url: '/api/assessments/hide-comment',
                    headers: { 'user': '<%= InSite.Api.Settings.ApiHelper.GetApiKey() %>' },
                    data: data,
                    success: function (result) {
                        if (typeof result == 'string') {
                            if (result == 'OK') {
                                if (isHidden)
                                    $container.removeClass('comment-hidden');
                                else
                                    $container.addClass('comment-hidden');
                            } else {
                                alert(result);
                            }
                        }

                        updateHideLink($link);
                    },
                    error: function (xhr) {
                        alert('Error: ' + xhr.status);
                    },
                    complete: function () {
                        isLocked = false;
                    },
                });
            }

            function updateHideLink(link) {
                var $link = $(link);

                if ($link.closest('div.question-comment').hasClass('comment-hidden'))
                    $link.attr('href', '#unhide').attr('title', 'Unhide Comment').find('> i:first').removeClass('fa-eye-slash').addClass('fa-eye');
                else
                    $link.attr('href', '#hide').attr('title', 'Hide Comment').find('> i:first').removeClass('fa-eye').addClass('fa-eye-slash');
            }

            function updateShowAllLink(link) {
                var $link = $(link);
                var $container = $link.data('container');

                if ($container.hasClass('show-hidden'))
                    $link.attr('href', '#hide-hidden').attr('title', 'Hide Hidden Comments').find('> i:first').removeClass('fa-eye').addClass('fa-eye-slash');
                else
                    $link.attr('href', '#show-hidden').attr('title', 'Show Hidden Comments').find('> i:first').removeClass('fa-eye-slash').addClass('fa-eye');
            }
        })();
    </script>
</insite:PageFooterContent>
