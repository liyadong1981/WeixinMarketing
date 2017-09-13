var page = {
    stack: [],
    $container: $('.container'),
    $error_message: $('#error_message'),
    $tip_message: $('#tip_message span'),
    $toast_complate: $('#toast_complate'),
    $loadingToast: $('#loadingToast'),
    $dialogTemplate: $('#weui_dialog_alert_template'),
    $confirmTemplate: $('#weui_dialog_confirm_template'),
    addPageStack: function (id, template, vm) {
        var $tpl = $($(template).html()).addClass('slideIn').addClass(id);
        this.$container.append($tpl);
        this.stack.push($tpl);
        history.pushState({ id: id }, '', '#' + id);
        $($tpl).on('webkitAnimationEnd', function () {
            $(this).removeClass('slideIn');
        }).on('animationend', function () {
            $(this).removeClass('slideIn');
        });
        //重新编译刚刚载入的DOM
        vm.$compile(this.$container[0]);
    },
    post: function (url, data, success) {
        page.$loadingToast.show();
        $.ajax({
            url: url,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json;',
            success: function (json) {
                if (json.Success) {
                    success(json.Result);
                } else {
                    page.show_confirm("提示", json.Result.Message);

                }
            },
            error: function (xhr, errorType, error) {
                page.show_error('发生错误，请联系管理员');
            },
            complete: function () {
                page.$loadingToast.hide();
            }
        });
    },
    get: function (url, success) {
        page.$loadingToast.show();
        $.ajax({
            url: url,
            type: 'GET',
            success: function (json) {
                if (json.Success) {
                    success(json.Result);
                    page.$loadingToast.hide();
                }
                else {
                    page.show_error(json.Result.Message);
                }
            },
            error: function (xhr, errorType, error) {
                page.show_error('发生错误，请联系管理员');
            },
            complete: function () {
                page.$loadingToast.hide();
            }
        });
    },
    show_tip: function (message) {
        page.$tip_message.html(message).show();
        setTimeout(function () {
            page.$tip_message.hide();
        }, 1500);
    },
    show_error: function (message) {
        page.$error_message.html(message).show();
        setTimeout(function () {
            page.$error_message.hide();
        }, 3500);
    },
    show_complate: function () {
        page.$toast_complate.show();
        setTimeout(function () {
            page.$toast_complate.hide();
        }, 1000);
    },
    show_alert: function (title, message) {
        page.$dialogTemplate.find('.weui_dialog_title').text(title);
        page.$dialogTemplate.find('#weui_dialog_alert_template_content').text(message);
        page.$dialogTemplate.show();
    },
    show_confirm: function (title, message, func) {
        page.$confirmTemplate.find('.weui_dialog_title').text(title);
        page.$confirmTemplate.find('#weui_dialog_confirm_template_content').text(message);
        page.$confirmTemplate.show();
        $('#weui_btn_confirm_sure').off('click');
        $('#weui_btn_confirm_sure').on('click', func);
    },
    show_toast_success: function (message) {
        var template = $('#weui_toast_success_template');
        template.find('.weui_toast_content').text(message);
        template.show();

        setTimeout(function () {
            template.hide();
        }, 2000);
    },

    init: function () {
        window.removeEventListener('message', page.messageReciever, false);
        window.addEventListener('message', page.messageReciever, false);

        $(window).on('popstate', function () {
            var $top = page.stack.pop();
            if (!$top) {
                return;
            }
            $top.addClass('slideOut').on('animationend', function () {
                $top.remove();
            }).on('webkitAnimationEnd', function () {
                $top.remove();
            });
        });
    },
    messageReceived: function (message) { }
};
page.init();