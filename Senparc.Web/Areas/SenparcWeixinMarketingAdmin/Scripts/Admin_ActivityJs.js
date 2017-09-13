
//全局方法 Vue.filter() 注册一个自定义过滤器,必须放在Vue实例化前面
Vue.filter("CheckInputType", function (inputList, inputType, settingId) {   //inputList：所有的Output数据
    var requireInputList = [];   //符合要求的Input列表：例如“心情API”过滤掉除心情以外的元素
    $.each(inputList, function (i, item) {
        if (item.__type == inputType.__type && item.Id <= settingId) {  //判断inputList里的数据的__type与当前活动素材的Input的__type是否一致
            //item.Id <= settingId：过滤掉当前活动以及其后的活动数据
            requireInputList.push(item);
        }
    });

    //页面为”编辑“状态”时，显示保存时“选中的元素”
    $.each(requireInputList, function (i, item) {
        if (item.Name == inputType.Value) {
            var moverItem = requireInputList[0];
            requireInputList.splice(0, 1, item);   //把当前位置的元素放到首位
            requireInputList.splice(i, 1);       //删除当前位置的元素
            requireInputList.splice(requireInputList.length, 0, moverItem); //把第0个位置的元素放到最后面
        }
    });
    return requireInputList;
});
var activityVM = new Vue({
    el: '.page-container',
    data: {
        moduleTemplateList: {}, //活动模型json
        moduleSetting: [],       //深复制活动模型json
        inputList: [],   //Input列表：所有的Output数据
        _setInterval: null  //定时器
    },
    methods: {

        //深复制json数据
        copyModule: function () {
            //获取所有的OutPut
            $.each(activityVM.moduleSetting, function (i, item) {
                var inputValue = item.Value;
                $.each(item.Output, function (j, outputItme) {
                    var outPut = $.extend(true, {}, outputItme)
                    outPut.Name = item.Name + ' > ' + outputItme.Name;
                    outPut.Id = item.Id;    //用于过滤器 过滤当前module下面的所有output【不显示】

                    if (outPut.Name == inputValue) {
                        outPut.tag = true;
                    }

                    activityVM.inputList.push(outPut);
                });
            });

        },
        //Input数据为此活动素材上方所有的Output数据
        render: function (newModule) {//应该加入Action 根据不同动作来做成响应操作 
            var currentSettingId = 0//当前Input参数
            activityVM.inputList = [];
            var t = 1;   //若newModule的Name和前面的相同，则在Name后面加数字
            var module = $.extend(true, {}, newModule);    //深复制：为了newModule的Name值改变后，module的Name值不改变
            //activityVM.inputValue = "";
            //所有的Output数据
            $.each(activityVM.moduleSetting, function (i, item) {
                currentSettingId = i;
                item.Id = i;
                item.Step = i;
                $.each(item.Output, function (j, outputItme) {
                    var outPut = $.extend(true, {}, outputItme)
                    outPut.Name = item.Name + ' > ' + outputItme.Name;
                    //item.InputOption.push(outPut);
                    outPut.Id = currentSettingId; //用于过滤器

                    activityVM.inputList.push(outPut);
                });
                if (newModule != null && item.Name.indexOf(module.Name) >= 0) {
                    newModule.Name = module.Name + t;
                    t++;
                }
            });
            if (newModule != null) {//如果newModule为Null代表当前操作为 删除或上下移动  这个时候是不需要添加新的module的
                newModule.Id = currentSettingId;
                newModule.Step = currentSettingId;
                activityVM.moduleSetting.push(newModule);
            }
        },
        //内置流程
        builtInProcess:function(i){
            var newModule = $.extend(true, {}, activityVM.moduleTemplateList[i]);
            activityVM.render(newModule);

            //根据插入元素上方是否有元素判断是否显示“下一步icon”
            activityVM.displayNextStep();
        },
        //认知服务
        perceivedService: function (j) {
            var newModule = $.extend(true, {}, activityVM.moduleTemplateList[j]);
            activityVM.render(newModule);

            //根据插入元素上方是否有元素判断是否显示“下一步icon”
            activityVM.displayNextStep();
        },
       
        //根据插入元素上方是否有元素判断是否显示“下一步icon”
        displayNextStep: function () {
            if ($('.activity_cart').length >= 1) {
                $('.activity_cart .nextStep').show();
            } else {
                $('.activity_cart .nextStep').hide();
            }
        },
        //进入“编辑页面”显示、隐藏“下一步icon”
        displayNextStepEdit: function () {
            var activity_cart = $('.activity_cart');

            $.each(activity_cart, function (i, item) {
                if (i == (activity_cart.length - 1)) {
                    $(item).find('.nextStep').hide();
                } else {
                    $(item).find('.nextStep').show();
                }
            });
        },
        //提交Json数据
        Check: function () {

            //获取页面上选中的Input数据，并保存到对应的Json的Input中
            var activityCart = $('.activity_cart');
            $.each(activityCart, function (i, item) {
                activityVM.moduleSetting[i].Id = i;
                activityVM.moduleSetting[i].Step = i;
                if (activityVM.moduleSetting[i].Input.length > 0) {

                    $.each($(item).find('option:selected'), function (j, select) {  //根据"select框"的个数改变Input的Value值（Input的Value值可能有多个，目前就一个）
                        activityVM.moduleSetting[i].Input[j].Value = $(select).val();
                    });

                }
            });

            var compareMoney = true;
            $.each(activityVM.moduleSetting, function (i, item) {
                if (item.Output[0].ShowType == "TextBox") {
                    var maxMoney = parseInt(item.Input[1].Value);
                    var minMoney = parseInt(item.Input[2].Value);
                    if (maxMoney < minMoney) {
                        compareMoney = false;
                    } else {
                        //compareMoney = true;
                    }
                }

            });
            
            //保存Json数据
            $('#AppRedPackageActivity_Rule').attr("value", JSON.stringify(activityVM.moduleSetting));

            if (compareMoney) {
                $('#form_activity').submit();
            } else {
                alert("‘最大金额’不能小于‘最小金额’噢！");
            }
            
        },

        //删除活动素材
        activity_delete: function (index) {
            activityVM.moduleSetting.splice(index, 1); //删除json里的活动数据
            activityVM.render();
            setTimeout(function () {
                activityVM.displayNextStepEdit();  //隐藏“最后一步活动”的“下一步icon”
            }, 10);
        },
        //点击向上移动活动素材
        activity_top: function (index) {
            var newModule = $.extend(true, {}, activityVM.moduleTemplateList[0]);
            var newModule2 = $.extend(true, {}, activityVM.moduleTemplateList[1]);

            if (index > 0) {
                var _lock = false;

                //向上移动活动素材
                var dataPrev = activityVM.moduleSetting[index - 1];
                var data = activityVM.moduleSetting[index];
                activityVM.moduleSetting[index - 1] = [];
                activityVM.moduleSetting.splice(index - 1, 1, data);
                activityVM.moduleSetting.splice(index, 1, dataPrev);
                activityVM.render();

                setTimeout(function () {
                    activityVM.displayNextStepEdit();  //隐藏“最后一步活动”的“下一步icon”
                }, 10);

            }

        },
        //点击向下移动活动素材
        activity_bottom: function (index) {
            var newModule = $.extend(true, {}, activityVM.moduleTemplateList[0]);
            var newModule2 = $.extend(true, {}, activityVM.moduleTemplateList[1]);

            //向下移动活动素材
            var activitCart = $('.activity_cart');
            if (index < (activitCart.length - 1)) {
                var _lock = false;

                var dataNext = activityVM.moduleSetting[index + 1];
                var data = activityVM.moduleSetting[index];
                activityVM.moduleSetting.splice(index + 1, 1, data);
                activityVM.moduleSetting.splice(index, 1, dataNext);
                activityVM.render();

                setTimeout(function () {
                    activityVM.displayNextStepEdit();  //隐藏“最后一步活动”的“下一步icon”
                }, 10);
            }

        },
        //动态生成支付二维码
        payCode: function (activityId) {
            var money = $('#rechargeMoney').val();
            if (parseFloat(money) <= 0 || money == "") {
                alert("充值金额必须大于0元！");
                return;
            }
            page.post('/Azureadmin/Order/TenPay', {
                id: activityId,
                money: parseFloat(money)
            }, function (result) {
                $('#qrCode').qrcode(result.CodeUrl);
                $('.activity_payCode').show();

                activityVM._setInterval = self.setInterval("activityVM.ifPaySuccess(" + result.OrderId + ")", 1000); //间隔1s执行一次事件
            });
        },
        //显示二维码
        displayCode: function (activityId) {
            $("#qrCode").empty();
            //支付二维码
            activityVM.payCode(activityId);
        },
        //隐藏支付二维码
        hide_payCode: function () {
            $('.activity_payCode').hide();
        },
        //生成活动二维码
        activityCode: function (activityId) {
            $('#activityCode').qrcode('http://azuredemo.senparc.com/WX?id=' + activityId);
        },
        //显示活动二维码
        enlargeCode: function () {
            $('.packageActivityCode').show();
        },
        hide_enlargeCode: function () {
            $('.packageActivityCode').hide();
        },
        //判断订单是否支付
        ifPaySuccess: function (orderId) {
            page.post('/Azureadmin/Order/GetOrderState', {
                orderId: orderId
            }, function (result) {
                if (result.State == 1 || result.State == "已支付") {
                    window.clearInterval(activityVM._setInterval); //如果订单已支付，则终止定时器
                    $('.activity_payCode').hide();
                    $('.package_paySuccess').show().delay(2000).hide(0);
                    $('.redpackage_totalPrice').text(result.Money.ToString("C"));
                }
            });
        },
        
        //引用时间日期插件
        dateTime: function () {
            var start = {
                dateCell: '#AppRedPackageActivity_BeginTime',
                format: 'YYYY-MM-DD hh:mm',
                minDate: jeDate.now(0), //设定最小日期为当前日期
                festival: true,
                maxDate: '2099-06-16 23:59:59', //最大日期
                isTime: true,
                choosefun: function (datas) {
                    end.minDate = datas; //开始日选好后，重置结束日的最小日期
                }
            };
            var end = {
                dateCell: '#AppRedPackageActivity_EndTime',
                format: 'YYYY-MM-DD hh:mm',
                minDate: jeDate.now(0), //设定最小日期为当前日期
                festival: true,
                maxDate: '2099-06-16 23:59:59', //最大日期
                isTime: true,
                choosefun: function (datas) {
                    start.maxDate = datas; //将结束日的初始值设定为开始日的最大日期
                },
                okfun: function (val) {  }  
                //alert(val)
            };
            jeDate(start);
            jeDate(end);
        }
    }
});

