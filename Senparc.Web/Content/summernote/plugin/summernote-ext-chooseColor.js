(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery'], factory);
    } else {
        factory(window.jQuery);
    }
}(function ($) {
    var tmpl = $.summernote.renderer.getTemplate();
    var colors = ['#de4436', '#f4b100', '#ffd500', '#8abf1f', '#638516', '#00b5eb', '#0080a4', '#409995', '#b4b4b4', '#5a5a5a', '#414141', '#828282'];

    if (!Array.prototype.chunk) {
        Array.prototype.chunk = function (chunkSize) {
            var R = [];
            for (var i = 0; i < this.length; i += chunkSize)
                R.push(this.slice(i, i + chunkSize));
            return R;
        }
    }

    var dropdown = function () {
        return '<div class="dropdown-menu dropdown-keep-open" id="color-dropdown" style="width: 200px; padding: 10px;">' +
            '<div class="color-list">' +
            render(colors) +
            '</div>' +
            '</div>';
    };

    var render = function (colors) {
        var colorList = '';
        var chunks = colors.chunk(4);
        for (j = 0; j < chunks.length; j++) {
            colorList += '<div class="row">';
            for (var i = 0; i < chunks[j].length; i++) {
                var color = chunks[j][i];
                colorList += '<div class="col-xs-3">' +
                '<a href="javascript:void(0)" data-event="selectColor" data-value="' + color + '"><span style="background:' + color + '; display: inline-block; width: 24px; height: 24px;"></span></a>' +
                '</div>';
            }
            colorList += '</div>';
        }

        return colorList;
    };

    $.summernote.addPlugin({
        name: 'chooseColor',
        buttons: {
            chooseColor: function (lang, options) {
                return tmpl.iconButton('fa fa-pencil', {
                    title: lang.chooseColor.title,
                    hide: true,
                    dropdown: dropdown()
                });
            }
        },

        events: {
            selectColor: function (event, editor, layoutInfo, value) {
                var $editable = layoutInfo.editable();
                editor.foreColor($editable, value);
            }
        },

        langs: {
            'en-US': {
                chooseColor: {
                    title: 'Choose Color'
                }
            },
            'zh-CN': {
                chooseColor: {
                    title: '选择颜色'
                }
            }
        }
    });
}));
