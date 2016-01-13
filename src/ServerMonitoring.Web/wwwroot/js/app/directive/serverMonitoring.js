(function () {
    var module = angular.module('serverMonitoring', ['ngSanitize']);

    module.directive('serverMonitoring', [
        'monitoringService', 'chartService', '$sce', '$interval', '$timeout', function (monitoringService, chartService, $sce, $interval, $timeout) {
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

                    scope.getWidthClass = function(name) {
                        if (name.indexOf('CPU') != -1 && name.indexOf('_Total') != -1) {
                            return "col-lg-12";
                        }

                        if (name.indexOf('CPU') != -1) {
                            return "col-lg-5";
                        }

                        if (name.indexOf('MEMORY') != -1) {
                            return "col-lg-12";
                        }

                        if (name.indexOf('NETWORK') != -1) {
                            return "col-lg-6";
                        }

                        return "col-lg-12";
                    };

                    scope.refreshServerList = function () {
                        var op = monitoringService.getServers(scope.host);

                        op.success(function (data) {
                            scope.servers = data;
                        });
                        op.error(function () {

                        });
                    };

                    scope.refreshData = function () {
                        if (!scope.currentServer)
                            return;

                        var op = monitoringService.pull(scope.host, { server: { id: scope.currentServer.id } });

                        op.success(function (data) {
                            scope.serverData = data;

                            var innerScope = scope;

                            innerScope.updateData();

                            if (!scope.refreshInterval)
                                scope.refreshInterval = $interval(function () { innerScope.updateData(); }, 1000);
                        });
                    };

                    scope.updateServerData = function (data) {
                        data.items.forEach(function (dataItem) {
                            var serverItem = scope.serverData.items.filter(function(sitem) {
                                return dataItem.name == sitem.name;
                            })[0];

                            if (serverItem) {
                                serverItem.currentValue = dataItem.currentValue;
                                serverItem.currentValueDisplay = dataItem.currentValueDisplay;
                                serverItem.data = dataItem.data;
                            }
                        });
                        
                    };

                    scope.updateData = function () {
                        var op = monitoringService.pull(scope.host, { server: { id: scope.currentServer.id } });

                        op.success(function (data) {
                            scope.updateServerData(data);

                            scope.serverData.items.forEach(function (item) {
                                var element = document.getElementById("mchart_" + item.name);

                                if (!element)
                                    return;

                                if (item.chartLine) {
                                    item.chartLine.destroy();
                                }

                                var ctx = element.getContext("2d");

                                item.chartLine = new Chart(ctx).Line(chartService.getChartData(item.data), {
                                    pointDotRadius: 1,
                                    bezierCurve: false,
                                    pointHitDetectionRadius: 1,
                                    datasetStrokeWidth: 1,
                                    pointDot: false,
                                    animation: false,
                                });
                            });
                        });
                    }

                    scope.setCurrentServer = function (server) {
                        scope.currentServer = server;

                        if (scope.refreshInterval) {
                            $interval.clear(scope.refreshInterval);
                            scope.refreshInterval = null;
                        }

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
                    return $http.get(url(host, 'getServers'), { params: {} });
                },

                pull: function (host, query) {
                    return $http.get(url(host, 'pull'), { params: { query: JSON.stringify(query) } });
                }
            };
        }
    ]);

    module.service('chartService', function () {
        return {
            getChartData: function (data) {
                var labels = [];

                for (var i = 0; i <= data.length; i++) {
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
                            data: data
                        }
                    ]
                };

                return chartData;
            }
        };
    });
})();