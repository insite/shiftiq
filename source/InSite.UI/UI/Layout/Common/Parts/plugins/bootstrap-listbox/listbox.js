(function ($) {

    var isMouse = false;
    var isTouch = false;

    $(window)
        .on('mousedown', function () {
            isMouse = true;
        })
        .on('mouseup', function () {
            isMouse = false;
        })
        .on('touchstart', function () {
            isTouch = true;
        })
        .on('touchend', function () {
            isTouch = false;
        });

    $.fn.listbox = function (option, arg1) {
        var result = undefined;

        this.each(function () {
            if (this.tagName !== 'SELECT')
                return;

            var $this = $(this);
            var data = $this.data('listbox') || init($this);

            if (typeof option == 'string') {
                if (option === 'update') {
                    update($this);
                } else if (option === 'val') {
                    result = val($this, arg1);
                } else if (option === 'opt') {
                    result = opt($this);
                } else if (option === 'filter') {
                    result = filter($this, arg1);
                } else if (option === 'setFilter') {
                    result = setFilter($this, arg1);
                }
            }
        });

        return typeof result == 'undefined' ? this : result;
    };

    function init($select) {
        var data = { element: $select[0], filter: null, multiple: $select.prop('multiple') };

        $select
            .attr('tabindex', -1)
            .data('listbox', data)
            .on('change', onSelectChange);

        var $ul = $('<ul>')
            .attr({ tabindex: 0 })
            .on('keydown', onKeyDown)
            .on('focus', function () {
                if (isMouse || isTouch)
                    return;

                var $focused = $(this).find('> li.focused');
                if ($focused.length == 0)
                    $focused = $(this).find('li').first().addClass('focused');

                onItemFocus($focused);
            });

        var css = $select.attr('class');
        if (typeof css != 'undefined' && css.length > 0)
            $ul.attr('class', css);

        if (data.multiple)
            $ul.addClass('multiple');

        $('<div>')
            .addClass('list-box')
            .append($ul)
            .insertBefore($select)
            .append($select)
            ;

        update($select);

        return data;
    }

    function update($select) {
        var $ul = $select.siblings('ul').first().empty();
        var data = $select.data('listbox');
        var $focused = null;
        var filterRegex = data.filter && new RegExp('(' + escapeRegExp(data.filter) + ')', 'gi');

        $select.find('option').each(function () {
            var $option = $(this);
            var text = $option.text();
            
            if (filterRegex) {
                if (!filterRegex.test(text))
                    return;

                text = text.replace(filterRegex, '<span class="keyword">$1</span>')
            }

            var $li = $('<li>').append(
                $('<span>')
                    .html(text)
                    .on('click', onItemClick)
            ).data('option', this);

            if ($option.prop('selected') === true) {
                $li.addClass('checked');

                if ($focused === null)
                    $focused = $li.addClass('focused');
            }

            var tooltip = $option.attr('title');
            if (tooltip)
                $li.attr('title', tooltip);

            $ul.append($li);

            $option.data('listbox-item', $li[0]);
        });

        if ($focused === null)
            $focused = $ul.find('li').first();

        onItemFocus($focused);
    }
    
    function val($select, value) {
        if (typeof value != 'undefined') {
            $select.val(value);
            onSelectChange.call($select[0]);
        } else {
            var value = $select.val();
            return typeof value != 'undefined' ? value : null;
        }
    }

    function opt($select) {
        var $option = $select.find('option:selected');
        return $option.length === 0 ? null : $option;
    }

    function filter($select, text) {
        var data = $select.data('listbox');

        var newValue = null;
        if (typeof text === 'string' && text.length > 0)
            newValue = text.toUpperCase();

        if (data.filter === newValue)
            return;

        data.filter = newValue;

        update($select);
    }

    function setFilter($select, text) {
        var newValue = null
        if (typeof text === 'string' && text.length > 0)
            newValue = text.toUpperCase();

        $select.data('listbox').filter = newValue;
    }

    // event handlers

    function onSelectChange() {
        var $this = $(this);
        var $ul = $this.siblings('ul').eq(0);

        $ul.find('> li.checked').each(function () {
            var $li = $(this);
            
            var $option = $($li.data('option'));
            if (!$option.prop('selected'))
                $li.removeClass('checked');
        });

        var $liChecked = null;
        var $liFocused = null;

        $this.find(':selected').each(function () {
            var $option = $(this);

            var $li = $($option.data('listbox-item'));
            if (!$li.hasClass('checked')) {
                $li.addClass('checked');

                if ($liFocused === null)
                    $liFocused = $li;
            }

            if ($liChecked === null)
                $liChecked = $li;
        });
        
        var $currentLiFocused = $ul.find('li.focused');
        if ($currentLiFocused.length === 0 && $liFocused === null)
            $liFocused = $liChecked;

        if ($liFocused !== null) {
            $currentLiFocused.removeClass('focused');
            $liFocused.addClass('focused');
            onItemFocus($liFocused);
        }
    }

    function onKeyDown(e) {
        var $this = $(this);

        if (e.key == 'ArrowUp') {
            e.preventDefault();

            var $current = $(this).find('> li.focused');
            if ($current.length == 1) {
                $current = $current.removeClass('focused').prev();
                if ($current.length == 0)
                    $current = $(this).find('> li:last-child')
            } else {
                $current = $(this).find('> li:first-child');
            }

            $current.addClass('focused');

            onItemFocus($current);
        } else if (e.key == 'ArrowDown') {
            e.preventDefault();

            var $current = $(this).find('> li.focused');
            if ($current.length == 1) {
                $current = $current.removeClass('focused').next();
                if ($current.length == 0)
                    $current = $(this).find('> li:first-child')
            } else {
                $current = $(this).find('> li:first-child');
            }

            $current.addClass('focused');

            onItemFocus($current);
        } else if (e.which === 32 || e.which === 13) {
            e.preventDefault();

            var $current = $(this).find('> li.focused');
            if ($current.length == 1)
                $current.find('> span').click();
        } else if (e.key != null && e.key.length == 1) {
            var key = e.key.toLowerCase();
            if (key >= '0' && key <= '9' || key >= 'a' && key <= 'z') {
                e.preventDefault();

                // TODO
            }
        }
    }

    function onItemClick() {
        var $li = $(this).closest('li').siblings('li.focused').removeClass('focused').end().addClass('focused');
        var $option = $($li.data('option'));
        var $select = $option.closest('select');
        var data = $select.data('listbox');
        var isSelected = $option.prop('selected') === true;

        onItemFocus($li);

        if (data.multiple || !isSelected) {
            $option.prop('selected', !isSelected);
            $select.trigger('change');
        }
    }

    function onItemFocus($li) {
        var $scrollParent = $li
            .parents().filter(function () {
                return hasScroll(this, 'y');
            }).first();

        if ($scrollParent.length == 0)
            return;

        var parentTop = $scrollParent.scrollTop();
        var parentHeight = $scrollParent.outerHeight();
        var parentBottom = parentTop + parentHeight;

        var liTop = getTopPosition($li, $scrollParent) + parentTop;
        var liHeight = $li.outerHeight();
        var liBottom = liTop + liHeight;

        if (liTop < parentBottom && liBottom > parentBottom || liTop > parentBottom)
            $scrollParent.scrollTop(liTop - parentHeight + liHeight + 8);
        else if (liTop < parentTop)
            $scrollParent.scrollTop(liTop - 5);
    }

    // helpers

    function getRandomString() {
        return (((1 + Math.random()) * 0x1000000) | 0).toString(16);
    }

    function hasScroll(el, axis) {
        var $el = $(el),
            sX = $el.css('overflow-x'),
            sY = $el.css('overflow-y'),
            hidden = 'hidden',
            visible = 'visible',
            scroll = 'scroll';

        if (!axis) {
            if (sX === sY && (sY === hidden || sY === visible)) 
                return false;

            if (sX === scroll || sY === scroll)
                return true;
        } else if (axis === 'x') {
            if (sX === hidden || sX === visible)
                return false;

            if (sX === scroll)
                return true;
        } else if (axis === 'y') {
            if (sY === hidden || sY === visible)
                return false;

            if (sY === scroll)
                return true;
        }

        return $el.innerHeight() < $el[0].scrollHeight ||
            $el.innerWidth() < $el[0].scrollWidth;
    }

    function getTopPosition($el, $parent) {
        var result = 0;

        while (!$el.is($parent) && $el.parents().length > 0) {
            result += $el.position().top + parseInt($el.css('margin-top')) + parseInt($el.css('border-top-width'));
            $el = $el.offsetParent();
        }

        return result;
    }

    function escapeRegExp(value) {
        return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }
})(jQuery);