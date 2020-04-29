window.TASK = {
    rootPath: '',
    baseUrl: '',
    loggedInUser: '',
    timer: ''
};

var taskModel = function () {
    var self = this;
    self.Id = -1;
    self.Title = '';
    self.Description = '';
    self.CreatedDate = '';
    self.ModifiedDate = '';
    self.TaskStateValue = '';
    self.TaskState = '';
    self.LoggedInUser = '';
};

var movieModel = function () {
    var self = this;
    self.Title = '';
    self.Year = '';
    self.imdbID = '';
};

var initializeApp = function (rootPath, baseUrl) {
    TASK.rootPath = rootPath;
    TASK.baseUrl = baseUrl;
};

var task_commonModule = angular.module('task_Common', ['ngRoute', 'ui.bootstrap', 'trNgGrid', 'ngMessages']);

var task_appMainModule = angular.module('task_AppMain', ['task_Common']);

task_commonModule.factory('commonService', function ($http, $q, $window, $sce, $interval) {
    var thisService = this;

    this.ajaxCallHelper = new function () {
        var self = this;
        self.message = '';
        self.modelIsValid = true;
        self.modelErrors = [];
        self.isLoading = false;
        self.modelData = '';

        self.apiGet = function (uri, data, success, failure, always) {
            self.isLoading = true;
            self.modelIsValid = true;
            var deferred = $q.defer();
            $http.get(TASK.rootPath + uri, data)
                .success(function (result) {
                    _success(self, success, always, deferred, result);
                })
                .error(function (result, status) {
                    _failure(self, status, failure, always, deferred, result);
                });
            return deferred.promise;
        }

        self.apiPost = function (uri, data, success, failure, always) {
            self.isLoading = true;
            self.modelIsValid = true;
            var deferred = $q.defer();

            $http.post(TASK.rootPath + uri, data)
                .success(function (result) {
                    _success(self, success, always, deferred, result);
                })
                .error(function (result, status) {
                    _failure(self, status, failure, always, deferred, result);
                });
            return deferred.promise;
        }

        self.apiPut = function (uri, data, success, failure, always) {
            self.isLoading = true;
            self.modelIsValid = true;
            var deferred = $q.defer();

            $http.put(TASK.rootPath + uri, data)
                .success(function (result) {
                    _success(self, success, always, deferred, result);
                })
                .error(function (result, status) {
                    _failure(self, status, failure, always, deferred, result);
                });
            return deferred.promise;
        }

        self.apiGetCORS = function (uri, data, success, failure, always) {
            self.isLoading = true;
            self.modelIsValid = true;
            var deferred = $q.defer();
            $http.get(uri, data)
                .success(function (result) {
                    _success(self, success, always, deferred, result);
                })
                .error(function (result, status) {
                    _failure(self, status, failure, always, deferred, result);
                });
            return deferred.promise;
        }

        function _success(self, success, always, deferred, result) {
            if (success != null) {
                success(result);
            }
            if (always != null)
                always();
            self.isLoading = false;
            deferred.resolve(result);
        }

        function _failure(self, status, failure, always, deferred, result) {
            if (failure == null) {
                if (status === 400) { // dealing with bad request.
                    // get the errors in the result
                    if (result != null && result.length > 0)
                        if (result instanceof Array) {
                            for (var i = 0; i < result.length; i++) {
                                self.modelErrors[result[i].Key] = result[i].Message;
                            }
                        } else {
                            self.modelErrors[0] = result;
                        }
                }
                else if (status === 401) {
                    result = result.Message;
                    alert('UnAuthorised');
                }
                else if (status === 409) {
                    self.modelData = result;
                }
                self.modelIsValid = false;
            } else {
                failure(result);
            }
            if (always != null)
                always();
            self.isLoading = false;
            deferred.resolve(self);
        }

        return this;
    };

    this.gridHelper = new function () {
        var self = this;

        self.gridOptions = {
            currentPage: 0,
            pageItems: 10,
            filterBy: null,
            id: -1,
            orderBy: null,
            orderByReverse: false,
            totalItems: null,
            enableFiltering: true,
            enableSorting: true,
            itemsSet: [],
            uri: '',
            searchFieldName: '',
            additionalParams: '',
            idSet: [],
            loggedInUser: ''
        };

        function setFormatForDates() {
            if (self.gridOptions.itemsSet != null) {
                $.each(self.gridOptions.itemsSet, function (index, item) {
                    if (typeof item.CreatedDate !== "undefined") {
                        item.CreatedDate = moment(item.CreatedDate).format(constants.format.dateFormat).toString();
                    }
                    if (typeof item.ModifiedDate !== "undefined") {
                        item.ModifiedDate = moment(item.ModifiedDate).format(constants.format.dateFormat).toString();
                    }
                });
            }
        }

        self.setListData = function (data) {
            if (typeof data !== "undefined") {
                self.gridOptions.itemsSet = data;
                setFormatForDates();
            }
        }

        self.getSetItems = function () {
            var options = self.gridOptions;

            // currentPage, pageItems, filterBy, filterByFields, orderBy, orderByReverse
            var iUrl = options.uri + "?Page=" + options.currentPage + "&PageSize=" + options.pageItems + "&OrderBy=" + options.orderBy + "&LoggedInUser=" + options.loggedInUser;

            if (typeof options.filterBy != "undefined" && options.filterBy != null) {
                iUrl = iUrl + "&FilterBy=" + options.filterBy;
            }

            //if (!$.isEmptyObject(options.filterByFields)) {
            //    iUrl = iUrl + "&FilterByFields=" + options.filterByFields;
            //}

            if (typeof options.orderByReverse != "undefined" && options.orderByReverse != null) {
                iUrl = iUrl + "&OrderByReverse=" + options.orderByReverse;
            }

            if (typeof options.searchFieldName != "undefined" && options.searchFieldName != "") {
                iUrl = iUrl + "&SearchFieldName=" + options.searchFieldName;
            }

            // check for additionalParams
            if (typeof options.additionalParams !== "undefined") {
                iUrl = iUrl + "&" + options.additionalParams;
            }

            return thisService.ajaxCallHelper.apiGet(iUrl);
        };
    };

    this.savetaskData = function (taskModel) {
        var iUrl = constants.url.apiHome;
        taskModel.loggedInUser = TASK.loggedInUser;

        // create
        if ((typeof taskModel.Id == "undefined") || (taskModel.Id === -1) || (taskModel.Id === null)) {
            taskModel.Id = -1;
            return thisService.ajaxCallHelper.apiPost(iUrl, taskModel);
        }

        // edit
        return thisService.ajaxCallHelper.apiPut(iUrl, taskModel);
    };

    this.gettaskData = function (id) {
        var iUrl = constants.url.apiGetTaskById.format(id) + '?loggedInUser=' + TASK.loggedInUser;
        return thisService.ajaxCallHelper.apiGet(iUrl);
    };

    this.getMovieData = function () {
        var iUrl = 'http://www.omdbapi.com/?s=Batman&page=2';
        return thisService.ajaxCallHelper.apiGetCORS(iUrl);
    }

    return this;
});

var task_appMainController = task_appMainModule.controller('taskController', ['$scope', 'commonService', '$window', '$modal', '$sce', '$interval', function ($scope, commonService, $window, $modal, $sce, $interval) {
    $scope.saveMode = 'L';
    $scope.gridHelper = commonService.gridHelper;
    $scope.taskModel = new taskModel();
    $scope.dateFormat = constants.format.dateFormat;
    $scope.filterBy = '';
    $scope.movieData = [];
    $scope.movieModel = new movieModel();

    function getTaskList() {
        $scope.gridHelper.getSetItems().then(function (data) {
            $scope.gridHelper.setListData(data);
            $scope.isTaskSetLoaded = true;
        },
            function (errorMessage) {
                $scope.error = errorMessage;
            });
    };

    $scope.init = function (loggedInUserId) {
        TASK.loggedInUser = loggedInUserId;
    }

    $scope.onTaskListRequested = function (currentPage, pageItems, filterBy, filterByFields, orderBy, orderByReverse) {
        $scope.isTaskSetLoaded = false;
        $scope.gridHelper.gridOptions.uri = constants.url.apiHome;
        $scope.gridHelper.gridOptions.currentPage = currentPage;
        $scope.gridHelper.gridOptions.pageItems = pageItems;
        $scope.gridHelper.gridOptions.filterBy = filterBy;
        // $scope.gridHelper.gridOptions.filterByFields = filterByFields;
        $scope.gridHelper.gridOptions.orderBy = orderBy;
        $scope.gridHelper.gridOptions.orderByReverse = orderByReverse;
        $scope.gridHelper.gridOptions.loggedInUser = TASK.loggedInUser;
        getTaskList();
    }

    $scope.onTaskNameSearch = function (filterBy) {
        $scope.gridHelper.gridOptions.filterBy = filterBy;
        $scope.gridHelper.gridOptions.searchFieldName = 'Title';
        $scope.gridHelper.gridOptions.itemsSet = [];
        $scope.gridHelper.gridOptions.loggedInUser = TASK.loggedInUser;
        $scope.gridHelper.gridOptions.currentPage = 0;
    }

    $scope.addNewtask = function () {
        $scope.saveMode = 'A';
        $scope.taskModel = new taskModel();
        $('#dvMessage').removeClass('alert alert-success');
        $('#dvMessage').removeClass('alert alert-danger');
        $('#dvMessage').html('');
    }

    $scope.editTask = function (id) {
        $scope.saveMode = 'E';
        commonService.gettaskData(id).then(function (data) {
            $scope.taskModel = data;
        });

        $('#dvMessage').removeClass('alert alert-success');
        $('#dvMessage').removeClass('alert alert-danger');
        $('#dvMessage').html('');
    }

    $scope.openedCalendarCreatedDate = { opened: false };
    $scope.openCalendarCreatedDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.openedCalendarCreatedDate.opened = true;
    };

    $scope.openedCalendarModifiedDate = { opened: false };
    $scope.openCalendarModifiedDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.openedCalendarModifiedDate.opened = true;
    };

    $scope.saveTask = function () {
        $scope.istaskLoaded = false;
        if ($scope.taskModel.CreatedDate != null && $scope.taskModel.CreatedDate !== '') {
            $scope.taskModel.CreatedDate = moment($scope.taskModel.CreatedDate).format($scope.dateFormat).toString();
        } else {
            $scope.taskModel.CreatedDate = '';
        }
        if ($scope.taskModel.ModifiedDate != null && $scope.taskModel.ModifiedDate !== '') {
            $scope.taskModel.ModifiedDate = moment($scope.taskModel.ModifiedDate).format($scope.dateFormat).toString();
        } else {
            $scope.taskModel.ModifiedDate = '';
        }

        commonService.savetaskData($scope.taskModel).then(function (result) {
            if (result != null) {
                if (typeof result.modelIsValid === 'undefined') {
                    // redirect to Edit mode.
                    if (result.Task != null) {
                        $('#dvMessage').removeClass('alert alert-success');
                        $('#dvMessage').removeClass('alert alert-danger');
                        $('#dvMessage').html('Saved Successfully');
                        $('#dvMessage').addClass('alert alert-success');
                        $scope.saveMode = 'L';
                        $scope.onTaskListRequested('');
                    }
                } else if (!result.modelIsValid) {
                    alert('Error Occured');
                }
            } else {
                alert('Error Occured');
            }
        },
            function (errorMessage) {
                alert('Error Occured');
            });
    };

    commonService.getMovieData().then(function (data) {
        $scope.movieData = data.Search;
        $scope.movieModel = $scope.movieData[0];
    });

    var c = 0;
    var timer = $interval(function () {
        $scope.movieModel = $scope.movieData[c];
        if (c === 10) {
            c = 0;
            $scope.movieModel = $scope.movieData[0];
        }
        c++;
    }, 10000);
}]);

String.prototype.format = function () {
    var str = this;
    for (var i = 0; i < arguments.length; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        str = str.replace(reg, arguments[i]);
    }
    return str;
}