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
    .controller('studentListCtrl', ['$scope', '$http', function () {
        $scope.getAllStudentsPDF = function() {
            $http.get("list/json").then(function(response) {
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
    }]);