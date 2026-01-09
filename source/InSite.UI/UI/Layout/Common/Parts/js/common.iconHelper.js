(function () {
    var self = inSite.common.iconHelper = inSite.common.iconHelper || {};

    function FontInfo(file, prefix) {
        var self = this;

        this.file = file;
        this.prefix = prefix;

        this.list = null;
        this.state = 'none';
        this.error = null;

        this.getPath = function () {
            return '/library/fonts/font-awesome/7.1.0/webfonts/' + self.file + '.svg'
        };
    }

    function IconInfo(font, name) {
        var self = this;

        this.name = name;

        this.getPrefix = function () {
            return font.prefix;
        }

        this.getClassName = function () {
            return 'fa-' + self.name;
        }

        this.getFullClassName = function () {
            return font.prefix + ' ' + self.getClassName();
        }
    }

    var fonts = {
        brands: new FontInfo('fa-brands-400', 'fab'),
        light: new FontInfo('fa-light-300', 'fal'),
        regular: new FontInfo('fa-regular-400', 'far'),
        solid: new FontInfo('fa-solid-900', 'fas')
    };
    var states = {
        error: 'error',
        loaded: 'loaded',
        loading: 'loading',
    };

    self.getFontName = function (className) {
        var result = null;

        for (var name in fonts) {
            if (!fonts.hasOwnProperty(name))
                continue;

            if (className.startsWith(fonts[name].prefix))
                return name;
        }

        return result;
    }

    self.getFontPrefix = function (name) {
        return fonts.hasOwnProperty(name) ? fonts[name].prefix : null;
    }

    self.listIcons = function (name) {
        var deferred = $.Deferred();

        if (fonts.hasOwnProperty(name)) {
            onFontFound(deferred, fonts[name]);
        } else {
            deferred.reject('Font not found: ' + String(name));
        }

        return deferred.promise();
    };

    function onFontFound(deferred, font) {
        if (font.state === states.error) {
            deferred.reject(font.error);
        } else if (font.state === states.loaded) {
            deferred.resolve(font.list);
        } else if (font.state === states.loading) {
            setTimeout(onFontFound, 25, deferred, font);
        } else {
            font.state = states.loading;
            font.error = null;
            font.list = null;

            $.ajax({
                type: 'GET',
                url: font.getPath(),
                dataType: 'xml',
                success: function (data, status, xhr) {
                    var result = [];

                    $(data).find('> svg > defs > font > glyph').each(function () {
                        var $this = $(this);

                        result.push(new IconInfo(font, $this.attr('glyph-name')));
                    });

                    if (result.length == 0) {
                        font.state = states.error;
                        font.error = 'Font file is empty: ' + String(font.getPath());

                        deferred.reject(font.error);
                    } else {
                        font.state = states.loaded;
                        font.list = result;

                        deferred.resolve(font.list);
                    }
                },
                error: function () {
                    font.state = states.error;
                    font.error = 'Unable to load font: ' + String(font.getPath());

                    deferred.reject(font.error);
                }
            });
        }
    }
})();

// Selector helper

(function () {
    var self = inSite.common.iconHelper.selector = inSite.common.iconHelper.selector || {};

    self.setup = function (type, name, $combo) {
        $combo.empty();
        $combo.prop('disabled', true);
        $combo.selectpicker('refresh');

        return inSite.common.iconHelper.listIcons(type).done(function (list) {
            $combo.append('<option>');

            var isSelected = false;

            for (var i = 0; i < list.length; i++) {
                var icon = list[i];
                var className = icon.getFullClassName();
                var $option = $('<option>')
                    .attr('value', className)
                    .addClass('icon-selector-' + className)
                    .html(icon.name);

                $combo.append($option);

                if (!isSelected && name.length > 0 && name.endsWith(icon.getClassName())) {
                    isSelected = true;
                    $option.prop('selected', true);
                }
            }

            $combo.prop('disabled', false);
            $combo.selectpicker('refresh');
        }).fail(function (err) {
            console.error(err);
        });
    }
})();