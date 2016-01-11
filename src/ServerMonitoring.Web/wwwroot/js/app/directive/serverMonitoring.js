(function () {
    var module = angular.module('serverMonitoring', ['ngSanitize']);

    module.directive('serverMonitoring', [
        'monitoringService', '$sce', function (monitoringService, $sce) {
            var getSafeHost = function (host) {
                if (host.indexOf('/', host.length - 1) !== -1) {
                    return host.substr(0, host.length - 1);
                }

                return host;
            };

            return {
                restrict: 'EA',

                templateUrl: function (elem, attrs) {
                    var host = getSafeHost(attrs.host);

                    var fullTemplatePath = host + '/js/app/directive/monitoringTemplate.html';

                    return $sce.trustAsResourceUrl(fullTemplatePath);
                },

                scope: {
                    ngModel: '='
                },

                link: function (scope, element, attrs) {
                    scope.host = getSafeHost(attrs.host);

                }
            };
        }
    ]);

    module.service('monitoringService', [
        '$http', function ($http) {
            var resource = '/';

            var url = function (host, action) {
                return host + resource + action;
            };

            return {
                getServers: function (host, query) {
                    return $http.get(url(host, 'getServers'), { params: { query: JSON.stringify(query) } });
                },

                getServerStat: function (host, query) {
                    return $http.get(url(host, 'getServerStat'), { params: { query: JSON.stringify(query) } });
                }
            };
        }
    ]);
})();