angular.module('tourApp', ['ui.toggle', 'ngTagsInput'])
    .controller('driverRegistrationCtrl', ['$scope', function() {
        
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
        $scope.getAllStudentsPDF = function() {
            $http.get("api/student").then(function(response) {
                var students = response.data;

                var doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(10);

                var subsetAttr = function(attrList, obj) {
                    return attrList.reduce(function(o, k) {
                        o[k] = obj[k];
                        return o;
                    }, {});
                };

                var i, j, temparray, chunk = 25;
                for (i = 0, j = students.length; i < j; i += chunk) {
                    temparray = students.slice(i, i + chunk);

                    var str = stringTable.create(temparray.map(function(student) {
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
        $scope.getAllDriversPDF = function() {
            $http.get("api/driver").then(function(response) {
                var drivers = response.data;

                var doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(10);

                var subsetAttr = function(attrList, obj) {
                    return attrList.reduce(function(o, k) {
                        o[k] = obj[k];
                        return o;
                    }, {});
                };

                var i, j, temparray, chunk = 25;
                for (i = 0, j = drivers.length; i < j; i += chunk) {
                    temparray = drivers.slice(i, i + chunk);

                    var str = stringTable.create(temparray.map(function(driver) {
                        return subsetAttr(["displayId", "fullname", "email", "phone", "totalSeats"], driver);
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
    }]);