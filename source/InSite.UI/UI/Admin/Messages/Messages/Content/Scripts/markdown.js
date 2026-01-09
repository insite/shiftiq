(function ($, showdown) {
    var instance = window.messageMarkdown = window.messageMarkdown || {};

    var converter = new showdown.Converter({ simpleLineBreaks: true });
    var docType = 'web';
    var renderer = null;

    function tableRenderer() {
        var columns = new Array();
        var currentColumn = null;
        var currentRow = null;
        var maxRowsCount = 0;

        this.addColumn = function () {
            if (columns.length >= 2)
                return;

            currentColumn = { rows: new Array() };
            currentRow = null;

            columns.push(currentColumn);
        };
        this.addRow = function (css) {
            if (!this.hasColumns())
                this.addColumn();

            currentRow = { css: css, lines: new Array() };
            currentColumn.rows.push(currentRow);

            if (maxRowsCount < currentColumn.rows.length)
                maxRowsCount = currentColumn.rows.length;
        };
        this.addLine = function (value) {
            if (currentRow == null)
                return;

            currentRow.lines.push(value);
        };

        this.renderWeb = function (lines) {
            if (maxRowsCount <= 0)
                return;

            lines.push('<div class="section-container">');

            for (var y = 0; y < maxRowsCount; y++) {
                var rowCss = null;
                var firstColumn = columns[0];
                if (y < firstColumn.rows.length) {
                    var firstColumnRow = firstColumn.rows[y];
                    if (firstColumnRow.css != null)
                        rowCss = firstColumnRow.css;
                }

                if (rowCss != null)
                    lines.push('<div class="section-row ' + rowCss + '">');
                else
                    lines.push('<div class="section-row">');

                for (var x = 0; x < columns.length; x++) {
                    lines.push('<div style="width:' + (columns.length == 0 ? 100 : 100 / columns.length) + '%;float:left;box-sizing:border-box;" class="section-column">');

                    var column = columns[x];
                    if (y < column.rows.length) {
                        var rowLines = column.rows[y].lines;

                        for (var i = 0; i < rowLines.length; i++) {
                            lines.push(rowLines[i]);
                        }
                    } else {
                        lines.push('&nbsp;');
                    }

                    lines.push('</div>');
                }

                lines.push('<div style="clear:both;"></div></div>');
            }

            lines.push('</div>');
        }

        this.renderEmail = function (lines) {
            if (maxRowsCount <= 0)
                return;

            lines.push('<table class="section-break">');

            for (var y = 0; y < maxRowsCount; y++) {
                var rowCss = null;
                var firstColumn = columns[0];
                if (y < firstColumn.rows.length) {
                    var firstColumnRow = firstColumn.rows[y];
                    if (firstColumnRow.css != null)
                        rowCss = firstColumnRow.css;
                }

                if (rowCss != null)
                    lines.push('<tr class="' + rowCss + '">');
                else
                    lines.push('<tr>');

                for (var x = 0; x < columns.length; x++) {
                    lines.push('<td>');

                    var column = columns[x];
                    if (y < column.rows.length) {
                        var rowLines = column.rows[y].lines;

                        for (var i = 0; i < rowLines.length; i++) {
                            lines.push(rowLines[i]);
                        }
                    }

                    lines.push('</td>');
                }

                lines.push('</tr>');
            }

            lines.push('</table>');
        };

        this.hasColumns = function () {
            return currentColumn != null;
        };
    }

    instance.makeHtml = function (md) {
        renderer = new tableRenderer();

        var result = preprocessor(md);
        result = converter.makeHtml(result);
        result = postprocessor(result);

        result = replaceAll(result, '{Current-Host}', window.location.origin);
        result = replaceAll(result, '$AppUrl', window.location.origin);
        result = replaceAll(result, '$CurrentYear', new Date().getFullYear());

        renderer = null;

        return result;

        function replaceAll(str, find, replace) {
            find = find.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
            return str.replace(new RegExp(find, 'g'), replace);
        }
    };

    instance.setDocType = function (value) {
        if (value == 'web' || value == 'email') {
            docType = value;
        }
    };

    function preprocessor(value) {
        var inputLines = disassesmble(value);
        if (inputLines == null)
            return value;

        return value;
    }

    function postprocessor(value) {
        var inputLines = disassesmble(value);
        if (inputLines == null)
            return value;

        var outputLines = new Array();

        for (var i = 0; i < inputLines.length; i++) {
            var line = inputLines[i];
            var isControl = false;

            if (line.indexOf('<p>$') == 0 && line.lastIndexOf('</p>') == line.length - 4) {
                var controlString = line.substring(4, line.length - 4);
                if (controlString.length > 0) {
                    if (controlString[0] == '$') {
                        line = '<p>' + controlString + '</p>';
                    } else if (instance.templates) {
                        var parts = controlString.split('#');
                        var name = parts[0];

                        if (instance.templates.hasOwnProperty(name)) {
                            var template = instance.templates[name];

                            var language = 'en'
                            if (parts.length > 1)
                                language = parts[1];

                            if (!template.hasOwnProperty(language))
                                language = 'en';

                            if (!template.hasOwnProperty(language)) {
                                language = null;
                                for (var prop in template) {
                                    if (template.hasOwnProperty(prop)) {
                                        language = prop;
                                        break;
                                    }
                                }
                            }

                            if (language)
                                line = template[language];
                        }
                    }
                }
            }

            if (!isControl) {
                if (renderer.hasColumns()) {
                    renderer.addLine(line);
                } else {
                    outputLines.push(line);
                }
            }
        }

        if (docType === 'email') {
            renderer.renderEmail(outputLines);
        } else if (docType === 'web') {
            renderer.renderWeb(outputLines);
        }

        return assemble(outputLines);
    }

    function assemble(lines) {
        var result = null;

        if (lines instanceof Array) {
            result = '';

            for (var i = 0; i < lines.length; i++) {
                var line = lines[i];
                if (line != null)
                    result += line + '\r\n';
            }
        }

        return result;
    }

    function disassesmble(value) {
        if (typeof value != 'string' || value.length == 0)
            return null;

        return value.replace('\r', '').split('\n');
    }

})($, showdown);