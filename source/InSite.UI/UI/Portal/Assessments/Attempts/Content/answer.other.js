(function () {
    const settings = {
        initElement: window.attempts.initElement,
        imgUrlPattern: new RegExp(_imageUrlPattern),
        images: _images,
    };

    $(window).on('attempts:init', function () {
        $('.card-question > .card-header > .question-text a').each(function (el) {
            const $anchor = $(el);

            let href = $anchor.attr('href');
            if (!href)
                return;

            href = href.toLowerCase();

            if (href.endsWith('.pdf'))
                $anchor.attr('target', '_blank');
        });

        $('.card-question > .card-header > .question-text img').each(function () {
            settings.initElement(this, 'answer:other:img', function (el) {
                const $el = $(el);
                const imgSrc = $el.prop('src').toLowerCase();
                let imgKey = imgSrc;

                const imgUrlMatches = settings.imgUrlPattern.exec(imgKey);
                if (imgUrlMatches != null)
                    imgKey = imgUrlMatches[3] + imgUrlMatches[5];

                const $anchor = $('<a>')
                    .addClass('img-zoom')
                    .attr('href', imgSrc);

                if (settings.images.hasOwnProperty(imgKey))
                    $anchor.attr('data-width', settings.images[imgKey].width);

                let caption = $el.prop('title');

                if (!caption)
                    caption = $el.prop('alt');

                if (caption)
                    $anchor.attr('data-caption', caption);

                $el.wrap($anchor).tooltip({
                    title: caption,
                    placement: 'left',
                    delay: {
                        show: 500,
                        hide: 100
                    }
                });
            });
        });

        $('.card-question > .card-header > .question-text .img-zoom').each(function () {
            settings.initElement(this, 'answer:other:fancybox', function (el) {
                $(el).fancybox({});
            })
        });

        if (typeof _termsData !== 'undefined') {
            $('a').each(function () {
                const $this = $(this);

                const name = $this.attr('href');
                if (_termsData.hasOwnProperty(name)) {
                    settings.initElement(this, 'answer:other:glossary-term', function (el) {
                        $(el).addClass('glossary-term').attr('title', 'View Term Definition').on('click', onTermClick);
                    });
                }
            });
        }
    });

    $(window).on('show.bs.modal', function (e) {
        setTimeout(function (modal) {
            const data = bootstrap.Modal.getInstance(modal);
            if (!data)
                return;

            let startIndex = 1050;

            {
                const modals = document.querySelectorAll('.modal.show');
                for (let i = 0; i < modals.length; i++) {
                    if (modals[i] != modal)
                        startIndex += 10;
                }
            }

            data._element.style.zIndex = startIndex + 5;

            if (data._backdrop._element)
                data._backdrop._element.style.zIndex = startIndex;
        }, 0, e.target);
    });

    $('#attempt-commands [data-action="view-addendum"]').on('click', function (e) {
        e.preventDefault();

        $('#addendum-dialog').modal('show');
    });

    function onTermClick(e) {
        e.stopPropagation();
        e.preventDefault();

        const $this = $(this);

        const name = $this.attr('href');
        if (!_termsData.hasOwnProperty(name))
            return;

        const term = _termsData[name];

        helper.showInfo(term.title, term.descr);
    }
})();
