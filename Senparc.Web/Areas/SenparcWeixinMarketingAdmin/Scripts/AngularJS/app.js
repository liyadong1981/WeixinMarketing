angular.module('app',
    [
        'ui.bootstrap', 'toaster', 'akFileUploader', 'ivh.treeview', 'summernote'
    ])
    .config(['$controllerProvider', function ($controllerProvider) {
        $controllerProvider.allowGlobals();
    }]);
var App = {

};
var app = angular.module('app');
app.directive('fileread', function () {
    return {
        scope: {
            fileread: "="
        },
        link: function (scope, element, attributes) {
            element.bind("change", function (changeEvent) {
                var reader = new FileReader();
                reader.onload = function (loadEvent) {
                    scope.$apply(function () {
                        scope.fileread = loadEvent.target.result;
                    });
                }
                reader.readAsDataURL(changeEvent.target.files[0]);
            });
        }
    }
});
app.directive('autofocus', ['$timeout', function ($timeout) {
    return {
        restrict: 'A',
        link: function ($scope, $element) {
            $timeout(function () {
                $element[0].focus();
            });
        }
    }
}]);
app.directive('focusMe', ['$timeout', '$parse', function ($timeout, $parse) {
    return {
        link: function (scope, element, attrs) {
            var model = $parse(attrs.focusMe);
            var focusModel = $parse(attrs.focusModel);
            scope.$watch(model, function (value) {
                if (value === true) {
                    $timeout(function () {
                        element[0].focus();
                    });
                }
            });
            var handler = function () {
                scope.$apply(function () {
                    model.assign(scope, false);
                    focusModel.assign(scope, true);
                });
                window.removeEventListener('touchmove', handler, false);
            };
            angular.element(document).ready(function () {
                window.addEventListener('touchmove', handler, false);
            });
        }
    };
}]);
app.filter('to_trusted', [
        '$sce', function ($sce) {
            return function (text) {
                return $sce.trustAsHtml(text);
            };
        }
]);
app.directive('insertText', ['$rootScope', function ($rootScope) {
    return {
        link: function (scope, element, attrs) {
            $rootScope.$on('Text_Insert', function (e, val) {
                var domElement = element[0];

                if (document.selection) {
                    domElement.focus();
                    var sel = document.selection.createRange();
                    sel.text = val;
                    domElement.focus();
                } else if (domElement.selectionStart || domElement.selectionStart === 0) {
                    var startPos = domElement.selectionStart;
                    var endPos = domElement.selectionEnd;
                    var scrollTop = domElement.scrollTop;
                    domElement.value = domElement.value.substring(0, startPos) + val + domElement.value.substring(endPos, domElement.value.length);
                    domElement.focus();
                    domElement.selectionStart = startPos + val.length;
                    domElement.selectionEnd = startPos + val.length;
                    domElement.scrollTop = scrollTop;
                } else {
                    domElement.value += val;
                    domElement.focus();
                }

            });
        }
    }
}])
app.config(['$provide', '$httpProvider', 'ivhTreeviewOptionsProvider', function ($provide, $httpProvider, ivhTreeviewOptionsProvider) {
    $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
    //加载自定义拦截器
    //$httpProvider.interceptors.push('interceptorFactory');
    ivhTreeviewOptionsProvider.set({
        idAttribute: 'id',
        labelAttribute: 'label',
        childrenAttribute: 'children',
        selectedAttribute: 'selected',
        useCheckboxes: true,
        expandToDepth: 3,
        indeterminateAttribute: '__ivhTreeviewIndeterminate',
        defaultSelectedState: false,
        validate: true,
        twistieExpandedTpl: '',
        twistieCollapsedTpl: '',
        twistieLeafTpl: '　'
    });
}]);
