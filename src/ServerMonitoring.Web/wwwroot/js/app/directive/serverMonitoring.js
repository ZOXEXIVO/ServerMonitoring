(function () {
    var module = angular.module('serverMonitoring', ['ngSanitize']);

    module.directive('serverMonitoring', ['_monitoringService', '_chartService', '$sce', '$interval', '$timeout', function (_monitoringService, _chartService, $sce, $interval, $timeout) {
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
                    var options = {
                        host: getSafeHost(attrs.host),
                        uiClasses: {
                            totalCpu: attrs.totalCpuClass || 'col-lg-12',
                            cpu: attrs.cpuClass || 'col-lg-3',
                            memory: attrs.memoryClass || 'col-lg-12',
                            network: attrs.networkClass || 'col-lg-6',
                            disk: attrs.networkClass || 'col-lg-6',
                            unknown: attrs.unknownClass || 'col-lg-12'
                        }
                    };

                    scope.data = {
                        timerIntervals: {
                            refreshDataInterval: null,
                            refreshServerInterva: null
                        },
                        itemTypes: {
                            cpu: 0,
                            memory: 1,
                            disk: 2,
                            network:3
                        }
                    };

                    scope.uiHelpers = {
                        getStatusImageSource: function (server) {
                            return options.host + '/images/' + (server.isActive ? 'green.png' : 'red.png');
                        },

                        getItemClass: function (item) {
                            if (item.type == scope.data.itemTypes.memory) {
                                return options.uiClasses.memory;
                            }

                            if (item.type == scope.data.itemTypes.network) {
                                return options.uiClasses.network;
                            }

                            if (item.type == scope.data.itemTypes.disk) {
                                return options.uiClasses.disk;
                            }

                            if (item.type == scope.data.itemTypes.cpu && item.name.indexOf('_Total') != -1) {
                                return options.uiClasses.totalCpu;
                            }

                            if (item.type == scope.data.itemTypes.cpu) {
                                return options.uiClasses.cpu;
                            }

                            return options.uiClasses.unknown;
                        }
                    };

                    scope.servers = [];
                    scope.currentServer = null;
                    
                    scope.$on("$destroy", function () {
                        if (scope.data.timerIntervals.refreshServerInterva) {
                            $timeout.cancel(scope.data.timerIntervals.refreshServerInterva);
                        }

                        if (scope.data.timerIntervals.refreshDataInterval) {
                            $timeout.cancel(scope.data.timerIntervals.refreshDataInterval);
                        }
                    });

                    scope.selectServer = function (server) {
                        scope.currentServer = server;
                        
                        if (scope.serverData)
                            scope.serverData.items = [];

                        scope.refreshData();
                    };


                    scope.refreshServers = function () {
                        var op = _monitoringService.getServers(options.host);
                        op.success(function (data) {
                            if (!scope.servers || scope.servers.length == 0)
                                scope.servers = data;
                            else {
                                //update servers
                                data.forEach(function (item) {
                                    var serverItem = scope.servers.filter(function (sitem) {
                                        return item.id == sitem.id;
                                    })[0];

                                    if (serverItem) {
                                        serverItem.isActive = item.isActive;
                                    } else {
                                        scope.servers.push(item);
                                    }
                                });
                            }
                        });
                    };

                    scope.refreshServers();

                    scope.data.timerIntervals.refreshServerInterval = $interval(scope.refreshServers, 1000);
                    
                    scope.refreshData = function () {
                        if (!scope.currentServer)
                            return;

                        var op = _monitoringService.pull(options.host, { server: { id: scope.currentServer.id } });

                        op.success(function (data) {
                            scope.serverData = data;
         
                            scope.updateData();

                            if (!scope.data.timerIntervals.refreshDataInterval)
                                scope.data.timerIntervals.refreshDataInterval = $interval(scope.updateData, 1000);
                        });
                    };

                    scope.updateData = function () {
                        var op = _monitoringService.pull(options.host, { server: { id: scope.currentServer.id } });

                        op.success(function (data) {
                            data.items.forEach(function (dataItem) {
                                var serverItem = scope.serverData.items.filter(function (sitem) {
                                    return dataItem.name == sitem.name;
                                })[0];

                                if (serverItem) {
                                    serverItem.currentValue = dataItem.currentValue;
                                    serverItem.currentValueDisplay = dataItem.currentValueDisplay;
                                    serverItem.data = dataItem.data;
                                } else {
                                    scope.serverData.items.push(dataItem);
                                }
                            });

                            scope.serverData.items.forEach(function (item) {
                                var element = document.getElementById("mchart_" + item.name);

                                if (!element)
                                    return;

                                if (item.chartLine) {
                                    item.chartLine.destroy();
                                }

                                var ctx = element.getContext("2d");

                                item.chartLine = new Chart(ctx).Line(_chartService.getChartData(item.data), {
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

                    
                }
            };
        }
    ]);

    module.service('_monitoringService', [
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

    module.service('_chartService', function () {
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