(function () {
    var module = angular.module('serverMonitoring', ['ngSanitize']);

    module.directive('serverMonitoring', [
        'monitoringService', '$sce', '$interval', '$timeout', function (monitoringService, $sce, $interval, $timeout) {
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

                    var fullTemplatePath = host + '/js/app/directive/monitoringTemplate.html?v=' + Math.random() * 10000;

                    return $sce.trustAsResourceUrl(fullTemplatePath);
                },

                link: function (scope, element, attrs) {
                    scope.host = getSafeHost(attrs.host);

                    scope.servers = [];
                    scope.currentServer = null;

                    scope.refreshInterval = null;

                    scope.refreshServerList = function() {
                        var op = monitoringService.getServers(scope.host);

                        op.success(function(data) {
                            scope.servers = data;
                        });
                        op.error(function() {

                        });
                    };

                    scope.refreshData = function () {
                        if (!scope.currentServer)
                            return;

                        var op = monitoringService.pull(scope.host, { server: { id: scope.currentServer.id } });

                        op.success(function(data) {
                            scope.serverData = data;

                            var innerScope = scope;

                            if(!scope.refreshInterval)
                                scope.refreshInterval = $interval(function () { innerScope.refreshData(); }, 1000);

                            $timeout(function() {
                                scope.serverData.items.forEach(function(item) {
                                    var element = document.getElementById("mchart_" + item.name);

                                    if (!element)
                                        return;

                                    if (item.chartLine) {
                                        item.chartLine.destroy();
                                    }

                                    var ctx = element.getContext("2d");

                                    var labels = [];
                                    for (var i = 0; i <= item.data.length; i++) {
                                        labels.push('');
                                    }

                                    var chartData = {
                                        labels: labels,
                                        datasets: [
                                            {
                                                label: '',
                                                fillColor: "rgba(220,220,220,0.2)",
                                                strokeColor: "rgba(220,220,220,1)",
                                                pointColor: "rgba(220,220,220,1)",
                                                pointStrokeColor: "#fff",
                                                pointHighlightFill: "#fff",
                                                pointHighlightStroke: "rgba(220,220,220,1)",
                                                data: [0]
                                            },
                                            {
                                                label: '',
                                                fillColor: "rgba(220,220,220,0.2)",
                                                strokeColor: "rgba(220,220,220,1)",
                                                pointColor: "rgba(220,220,220,1)",
                                                pointStrokeColor: "#fff",
                                                pointHighlightFill: "#fff",
                                                pointHighlightStroke: "rgba(220,220,220,1)",
                                                data: [100]
                                            },
                                            {
                                                label: '',
                                                fillColor: "rgba(151,187,205,0.2)",
                                                strokeColor: "rgba(151,187,205,1)",
                                                pointColor: "rgba(151,187,205,1)",
                                                pointStrokeColor: "#fff",
                                                pointHighlightFill: "#fff",
                                                pointHighlightStroke: "rgba(151,187,205,1)",
                                                data: item.data
                                            }
                                        ]
                                    };

                                    item.chartLine = new Chart(ctx).Line(chartData, {
                                        pointDotRadius: 1,
                                        bezierCurve: false,
                                        pointHitDetectionRadius: 1,
                                        datasetStrokeWidth: 1,
                                        pointDot: false,
                                        animation: false,
                                    });
                                });
                            }, 0);
                        });
                    };

                    scope.setCurrentServer = function(server) {
                        scope.currentServer = server;
                        scope.refreshData();
                    };

                    scope.refreshServerList();
                }
            };
        }
    ]);

    module.service('monitoringService', [
        '$http', function ($http) {
            var resource = '/monitoring/';

            var url = function (host, action) {
                return host + resource + action;
            };

            return {
                getServers: function (host) {
                    return $http.get(url(host, 'getServers'), { params: { } });
                },

                pull: function (host, query) {
                    return $http.get(url(host, 'pull'), { params: { query: JSON.stringify(query) } });
                }
            };
        }
    ]);
})();