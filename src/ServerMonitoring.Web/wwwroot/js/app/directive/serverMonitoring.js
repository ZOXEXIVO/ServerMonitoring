(function () {
    var module = angular.module('serverMonitoring', ['ngSanitize']);

    module.directive('serverMonitoring', ['_monitoringService', '_chartService', '$sce', '$interval', '$timeout', function (_monitoringService, _chartService, $sce, $interval, $timeout) {
            Array.prototype.min = function() {
                return Math.min.apply(Math, this);
            };

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
                    var defaultMinuteFilter = { text: '5 minutes', value: 5 };

                    scope.query = {
                        minuteFilter: defaultMinuteFilter
                    };

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
                        },
                        filterVarians: []
                    };

                    scope.initVarians = function () {
                        var minutes = [1, 5, 10, 15, 20, 25, 30, 60];
                        for (var i = 0; i < minutes.length; i++) {
                            scope.data.filterVarians.push({
                                text: minutes[i] + ' minute(s)',
                                value: minutes[i]
                            });
                        }
                    }();

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

                        scope.query.server = { id: scope.currentServer.id };

                        if (scope.serverData)
                            scope.serverData.items = [];

                        if (!scope.data.timerIntervals.refreshDataInterval)
                            $interval.cancel(scope.data.timerIntervals.refreshDataInterval);

                        scope.query.minuteFilter = defaultMinuteFilter;
                        scope.query.sinceMinute = defaultMinuteFilter.value;

                        scope.initServerData();
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

                    scope.data.timerIntervals.refreshServerInterval = $interval(scope.refreshServers, 5000);

                    scope.$watch(function() { return scope.query.minuteFilter; }, function(newVal, oldVal) {
                        if (newVal) {
                            $interval.cancel(scope.data.timerIntervals.refreshDataInterval);
                            scope.data.timerIntervals.refreshDataInterval = null;

                            scope.query.sinceDate = null;
                            scope.query.sinceMinute = newVal.value;

                            if (scope.data.timerIntervals.refreshDataInterval)
                                $interval.cancel(scope.data.timerIntervals.refreshDataInterval);

                            scope.initServerData();
                        }
                    });

                    scope.initServerData = function () {
                        if (!scope.currentServer)
                            return;

                        scope.query.sinceDate = null;

                        var op = _monitoringService.pull(options.host, scope.query);

                        op.success(function (data) {
                            scope.serverData = data;
         
                            scope.updateData();

                            if (!scope.data.timerIntervals.refreshDataInterval)
                                scope.data.timerIntervals.refreshDataInterval = $interval(scope.updateData, 1000);
                        });
                    };

                    scope.updateData = function () {
                        var op = _monitoringService.pull(options.host, scope.query);

                        op.success(function (data) {
                            //save last data
                            scope.query.sinceDate = data.lastPush;

                            data.items.forEach(function (dataItem) {
                                var serverItem = scope.serverData.items.filter(function (sitem) {
                                    return dataItem.name == sitem.name;
                                })[0];

                                if (serverItem) {
                                    serverItem.currentValue = dataItem.currentValue;
                                    serverItem.currentValueDisplay = dataItem.currentValueDisplay;
                                    
                                    var dataLength = dataItem.data.length;
                                    
                                    var limitLength = scope.query.minuteFilter.value * 60;

                                    var itemsToRemove = limitLength - (serverItem.data.length + dataLength);

                                    if (itemsToRemove < 0)
                                        serverItem.data.splice(0, -itemsToRemove);

                                    dataItem.data.forEach(function(item) {
                                        serverItem.data.push(item);
                                    });
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