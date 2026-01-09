(function () {
    var instance = inSite.common.fileUploadV2 = inSite.common.fileUploadV2 || {};

    instance.init = function (settings) {
        if (!settings) {
            return;
        }

        $('#' + settings.UploadedFilesId).data("settings", settings);

        $('#' + settings.SelectButtonId)
            .off('click')
            .on('click', function () {
                $('#' + settings.SelectFilesId).trigger('click');
            });

        $('#' + settings.SelectFilesId)
            .off("change")
            .on("change", function (e) {
                let names = '';
                for (let i = 0; i < e.target.files.length; i++) {
                    names += e.target.files[i].name;
                    if (i < e.target.files.length - 1) {
                        names += '; ';
                    }
                }

                $('#' + settings.SelectedFileNamesId).val(names);

                if (!saveFile(settings)) {
                    $('#' + settings.SelectedFileNamesId).val("");
                }
            });
    }

    instance.trigger = function (id) {
        var settings = $('#' + id).data("settings");
        $('#' + settings.SelectFilesId).trigger('click');
    }

    instance.getFileName = function (id) {
        var settings = $('#' + id).data("settings");
        return $('#' + settings.SelectedFileNamesId).val();
    }

    instance.getMetadata = function (id) {
        var settings = $('#' + id).data("settings");
        return $('#' + settings.UploadedFilesId).val();
    }

    instance.clearFiles = function (id) {
        var settings = $('#' + id).data("settings");

        $('#' + settings.SelectedFileNamesId).val("");
        $('#' + settings.UploadedFilesId).val("");
    }

    function saveFile(settings) {
        const fileUpload = $('#' + settings.SelectFilesId).get(0);
        const files = fileUpload.files;
        if (files.length == 0) {
            return false;
        }

        const formData = new FormData();
        for (let i = 0; i < files.length; i++) {
            const file = files[i];

            if (!checkFileSize(settings, file)
                || !checkFileExtension(settings, file)
                || !checkFileNameLength(settings, file)
            ) {
                return false;
            }

            formData.append(file.name, file);
        }

        let url = "/api/assets/files";
        if (settings.SessionIdentifier) {
            url += `?surveySession=${settings.SessionIdentifier}`;
        }

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            success: function (data) {
                const json = JSON.stringify(data);
                $('#' + settings.UploadedFilesId).val(json);

                let executePostback;

                if (settings.OnClientFileUploaded) {
                    executePostback = runFunction(settings.OnClientFileUploaded, $('#' + settings.UploadedFilesId).get(0));
                }

                if (executePostback !== false && settings.UniqueId) {
                    __doPostBack(settings.UniqueId, "");
                }
            },
            error: function (data) {
                console.error('error: ' + JSON.stringify(data));

                if (settings.OnClientFileUploadFailed) {
                    runFunction(settings.OnClientFileUploadFailed, $('#' + settings.UploadedFilesId).get(0));
                }
            },
            cache: false,
            contentType: false,
            processData: false,
            xhr: function () {
                const xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener('progress', function (evt) {
                    if (evt.lengthComputable) {
                        const percentComplete = Math.round((evt.loaded / evt.total) * 100);

                        $('#' + settings.UploadProgressId)
                            .css('width', percentComplete + '%')
                            .attr('aria-valuenow', percentComplete)
                            .text(percentComplete + '%');
                    }
                }, false);
                return xhr;
            },
        });

        return true;

        function checkFileSize(settings, file) {
            if (file.size === 0) {
                alert(`The file ${file.name} is empty`);
                return false;
            }

            if (settings.MaxFileSize == null || file.size <= settings.MaxFileSize) {
                return true;
            }

            alert("The file " + file.name + " exceeds max allowed size");

            return false;
        }

        function checkFileExtension(settings, file) {
            if (settings.AllowedExtensionsRegex == null) {
                return true;
            }

            var regexp = new RegExp(settings.AllowedExtensionsRegex, 'gi');

            if (settings.AllowedExtensionsRegex == null || file.name.match(regexp)) {
                return true;
            }

            alert(file.name + " has a file name extension that is not permitted here. Allowed extensions: \n" + settings.AllowedExtensionsText);

            return false;
        }

        function checkFileNameLength(settings, file) {
            if (file.name.length <= settings.MaxFileNameLength) {
                return true;
            }

            alert(settings.Strings.FileNameLengthExceeded);

            return false;
        }

        function runFunction(name, sender) {
            var fn = inSite.common.getObjByName(name);
            if (typeof fn == 'function') {
                return fn.call(sender);
            }
        }
    };
})();