angular.module('tourApp', ['ui.toggle', 'ngTagsInput'])
    .controller('driverRegistrationCtrl', ['$scope', function () {

    }])
    .controller("studentRegistrationCtrl", ['$scope', function () {

    }])
    .controller('userRegistrationCtrl', ['$scope', function ($scope) {
        $scope.validateInvitationCode = function ($event) {
            if ($scope.invitation_code !== $scope.invitation_code_value) {
                $scope.error = true;
                $event.preventDefault();
            } else {
                $scope.error = false;
            }
        }
    }])
    .controller('studentListCtrl', ['$scope', '$http', function ($scope, $http) {
        $scope.getAllStudentsPDF = function () {
            $http.get("/api/student").then(function (response) {
                var students = response.data;

                var doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(10);

                var subsetAttr = function (attrList, obj) {
                    return attrList.reduce(function (o, k) {
                        o[k] = obj[k];
                        return o;
                    }, {});
                };

                var i, j, temparray, chunk = 25;
                for (i = 0, j = students.length; i < j; i += chunk) {
                    temparray = students.slice(i, i + chunk);

                    var str = stringTable.create(temparray.map(function (student) {
                        student.fullname = student.fullname.substring(0, 30);
                        return subsetAttr(["fullname", "country", "email", "university", "attendance"], student);
                    }));

                    doc.text(20, 20, str);

                    if (i + chunk < j) {
                        doc.addPage();
                    }
                }

                doc.save("student-list.pdf");
            });
        };
    }])
    .controller("driverListCtrl", ['$scope', '$http', function ($scope, $http) {
        $scope.getAllDriversPDF = function () {
            $http.get("/api/driver").then(function (response) {
                var drivers = response.data;

                var doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(10);

                var subsetAttr = function (attrList, obj) {
                    return attrList.reduce(function (o, k) {
                        o[k] = obj[k];
                        return o;
                    }, {});
                };

                var i, j, temparray, chunk = 25;
                for (i = 0, j = drivers.length; i < j; i += chunk) {
                    temparray = drivers.slice(i, i + chunk);

                    var str = stringTable.create(temparray.map(function (driver) {
                        return subsetAttr(["displayId", "fullname", "email", "phone", "capacity"], driver);
                    }));

                    if (i === 0) {
                        str = "Driver List ( count of drivers: " + drivers.length + " )" + "\n\n" + str;
                    }

                    doc.text(20, 20, str);

                    if (i + chunk < j) {
                        doc.addPage();
                    }
                }

                doc.save("driver-list.pdf");
            });
        };
    }])
    .controller("studentDriverMappingCtrl", ["$scope", "$http", function ($scope, $http) {
        $scope.getStatus = function() {
            return $http.get("/api/studentDriverMapping/status").then(function(response) {
                var data = response.data;
                $scope.availableDrivers = data.availableDrivers;
                $scope.availableStudents = data.availableStudents;
                $scope.mappedDrivers = data.mappedDrivers;
            });
        };

        $scope.map = function(studentId, driverId) {
            $scope.changeMap(studentId, driverId, "map");
        };

        $scope.unmap = function(studentId, driverId) {
            $scope.changeMap(studentId, driverId, "unmap");
        };

        $scope.changeMap = function(studentId, driverId, action) {
            if (driverId && studentId) {
                $http.post("/api/studentDriverMapping/" + action, {
                    "driverId": driverId,
                    "studentId": studentId
                }).then(function() {
                    $scope.getStatus();
                });
            }
        };

        $scope.init = function () {
            $scope.getStatus();
        };
        
        $scope.init();
    }])
    .controller("attendanceCtrl", ["$scope", "$http", function ($scope, $http) {
        $scope.countries = ["All Countries"];
        $scope.students = [];
        $scope.allStudents = [];
        
        $scope.country = "All Countries";
        $scope.attendanceFilter = "all";
        $scope.fullname = "";
        
        
        $scope.getAllStudents = function () {
            return $http.get("/api/student").then(function (response) {
                $scope.students = response.data;
                $scope.allStudents = response.data;

                $scope.students.forEach(function (student) {
                    if (!$scope.countries.includes(student.country)) {
                        $scope.countries.push(student.country);
                    }
                });
                
                $scope.updateTable();
            });
        };

        $scope.changeAttendance = function (student) {
            return $http.post("/api/attendance/setAttendance", {
                studentId: student.id,
                attendance: student.isPressent
            }).then(function () {
                $scope.getAllStudents();
            });
        };

        $scope.updateTable = function () {
            var students = $scope.allStudents;
            
            var filteredStudents = {
                "country": [],
                "attendance": [],
                "fullname": []
            };
            
            if ($scope.country === "All Countries") {
                filteredStudents.country = students;
            } else {
                students.forEach(function (student) {
                    
                    if (student.country === $scope.country) {
                        filteredStudents.country.push(student);
                    }
                });
            }
            
            students = filteredStudents.country;

            if ($scope.attendanceFilter === "all") {
                filteredStudents.attendance = students;
            } else {
                students.forEach(function (student) {
                    if ((student.isPressent && $scope.attendanceFilter === "yes") || (!student.isPressent && $scope.attendanceFilter === "no")) {
                        filteredStudents.attendance.push(student);
                    }
                });
            }
            
            students = filteredStudents.attendance;

            if (!$scope.fullname) {
                filteredStudents.fullname = students;
            } else {
                students.forEach(function (student) {
                    if (student.fullname.toLowerCase().indexOf($scope.fullname.toLowerCase()) > -1) {
                        filteredStudents.fullname.push(student);
                    }
                });
            }

            students = filteredStudents.fullname;
            
            $scope.students = students;
        };

        $scope.init = function () {
            $scope.getAllStudents();
        };

        $scope.init();
    }]);
    