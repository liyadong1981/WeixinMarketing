(function () {
    var module = angular.module('custom-loading', []);
    module.provider('customLoadingProvider', function () {
        this.$get = [function () {
            return {
                isShow: false
            }
        }]
    })
    module.directive('customloading', ['customLoadingProvider', function (customLoadingProvider) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/Content/templates/custom-loading.html',
            scope: {
                isShow: '=isShow'
            },
            link: function ($scope, $ele, $attrs) {
            }
        }
    }]);
}());