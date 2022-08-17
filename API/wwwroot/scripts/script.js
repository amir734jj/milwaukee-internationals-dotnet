angular.element(document).ready(() => {
    window.setTimeout(() => {
        angular.element('.alert a').click();
    }, 5000);
});

angular.module('tourApp', ['ui.toggle', 'ngTagsInput'])
    .constant("jsPDF", (jspdf || window.jspdf).jsPDF)
    .directive('validateBeforeGoing', ["$window", $window => ({
        restrict: 'A',
        link: (scope, element, attrs) => {
            angular.element(element).on("click", (e) => {
                if (!$window.confirm(attrs.message)) {
                    e.preventDefault();
                }
            });
        }
    })])
    .controller('eventInfoCtrl', ['$scope', '$http', '$window', ($scope, $http, $window) => {

        $scope.mapStudent = $event => {
            $event.preventDefault();

            const eventId = $scope.eventId;
            const studentId = $scope.studentId;

            if (eventId && studentId) {
                $http.post("/api/event/map/" + eventId + "/" + studentId).then(() => {
                    $scope.fetchInfo();
                });
            }
        };

        $scope.unMapStudent = ($event, studentId) => {
            $event.preventDefault();

            const eventId = $scope.eventId;

            if (eventId && studentId) {
                $http.post("/api/event/unmap/" + eventId + "/" + studentId).then(() => {
                    $scope.fetchInfo();
                });
            }
        };

        $scope.sendAdHocEmail = $event => {
            $event.preventDefault();

            const eventId = $scope.eventId;
            const emailSubject = $scope.emailSubject;
            const emailBody = angular.element('.summernote').eq(0).summernote("code");

            if (eventId && emailSubject && emailBody && $window.confirm("Are you sure to send an email to RSVPed students?")) {
                $http.post("/api/event/email", {
                    emails: $scope.event.students.map(x => x.student.email),
                    body: emailBody,
                    subject: emailSubject
                }).then(() => {
                    $window.alert("Successfully sent email!");
                });
            }
        };

        $scope.fetchInfo = () => {
            $http.get('/api/event/info/' + $scope.eventId).then(response => {
                const data = response.data;
                $scope.event = data.event;
                $scope.availableStudents = data.availableStudents;
            });
        };

        // Start the text editor
        angular.element('.summernote').summernote({height: 150});

    }])
    .controller('userListCtrl', ['$scope', '$http', ($scope, $http) => {

    }])
    .controller('emailUtilityCtrl', ["$timeout", $timeout => {

        // Hide the .autoclose
        $timeout(() => {
            angular.element(".autoclose").fadeOut();
        }, 2000);

        // Start the text editor
        angular.element('.summernote').summernote({height: 150});
    }])
    .controller("emailCheckInCtrl", ['$scope', '$http', ($scope, $http) => {
        $scope.changeAttendance = (type, id, value) => $http.post("/utility/emailCheckInAction/" + type + "/" + id + "?present=" + value).then(() => {
            console.log("Updated the attendance");
        });
    }])
    .controller('hostEditCtrl', ['$scope', $scope => {

    }])
    .controller('hostRegistrationCtrl', ['$scope', $scope => {

    }])
    .controller('driverEditCtrl', ['$scope', $scope => {

    }])
    .controller('driverRegistrationCtrl', ['$scope', $scope => {

    }])
    .controller("studentRegistrationCtrl", ['$scope', $scope => {

    }])
    .controller('userRegistrationCtrl', ['$scope', $scope => {
        $scope.validateInvitationCode = $event => {
            if (!$scope.invitation_code || $scope.invitation_code.toLowerCase() !== $scope.invitation_code_value.toLowerCase()) {
                $scope.error = true;
                $event.preventDefault();
            } else {
                $scope.error = false;
            }
        }
    }])
    .controller('studentListCtrl', ['$scope', '$http', 'jsPDF', ($scope, $http, jsPDF) => {

        $scope.pdfDownloadTable = {
            id: false,
            displayId: false,
            fullname: true,
            major: false,
            university: true,
            email: false,
            phone: true,
            country: true,
            isFamily: true,
            familySize: false,
            needCarSeat: true,
            kosherFood: true,
            isPresent: true,
            maskPreferred: false
        };

        $scope.showDetail = false;

        $scope.toggleShowDetail = () => {
        };

        $scope.getAllStudentsPDF = () => {
            $http.get("/api/student").then(response => {
                let students = response.data;

                const doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                const subsetAttr = (attrList, obj) => attrList.reduce((o, k) => {
                    o[k] = String(obj[k]);
                    return o;
                }, {});

                let i, j, temporary;
                const chunk = 25;
                const attributes = Object.keys($scope.pdfDownloadTable).filter(value => $scope.pdfDownloadTable[value]);

                let fontSize = 10;
                if (attributes.length <= 7) {
                    fontSize = 10;
                } else if (attributes.length < 10) {
                    fontSize = 8;
                } else {
                    fontSize = 6;
                }

                doc.setFontSize(fontSize);

                for (i = 0, j = students.length; i < j; i += chunk) {
                    temporary = students.slice(i, i + chunk);

                    let str = stringTable.create(temporary.map(student => subsetAttr(attributes, student)));

                    // Needed
                    str = str.replace(/’/g, "'");

                    doc.text(15, 15, str);

                    if (i + chunk < j) {
                        doc.addPage();
                    }
                }

                doc.save("student-list.pdf");
            });
        };
    }])
    .controller("driverListCtrl", ['$scope', '$http', "jsPDF", ($scope, $http, jsPDF) => {
        $scope.getAllDriversPDF = () => {
            $http.get("/api/driver").then(response => {
                const drivers = response.data;

                const doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(10);

                const subsetAttr = (attrList, obj) => attrList.reduce((o, k) => {
                    o[k] = obj[k];
                    return o;
                }, {});

                let i, j, temporary;
                const chunk = 25;
                for (i = 0, j = drivers.length; i < j; i += chunk) {
                    temporary = drivers.slice(i, i + chunk);

                    let str = stringTable.create(temporary.map(driver => {

                        // Set the navigator for the PDF
                        driver.navigator = (driver.navigator || driver.navigator === "null") ?
                            (driver.navigator.length > 20 ? driver.navigator.substring(0, 20) + " ..." : driver.navigator) : "-";

                        return subsetAttr(["displayId", "fullname", "capacity", "navigator", "role", "haveChildSeat"], driver);
                    }));

                    // Needed
                    str = str.replace(/’/g, "'");

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
    .controller("hostListCtrl", ["$scope", "$http", "jsPDF", ($scope, $http, jsPDF) => {
        $scope.getAllHostPDF = () => {
            $http.get("/api/host").then(response => {
                const hosts = response.data;

                const doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(10);

                const subsetAttr = (attrList, obj) => attrList.reduce((o, k) => {
                    o[k] = obj[k];
                    return o;
                }, {});

                let i, j, temparray;
                const chunk = 25;
                for (i = 0, j = hosts.length; i < j; i += chunk) {
                    temparray = hosts.slice(i, i + chunk);

                    let str = stringTable.create(temparray.map(host => subsetAttr(["fullname", "email", "phone", "address"], host)));

                    if (i === 0) {
                        str = "Host List ( count of hosts: " + hosts.length + " )" + "\n\n" + str;
                    }

                    doc.text(20, 20, str);

                    if (i + chunk < j) {
                        doc.addPage();
                    }
                }

                doc.save("host-list.pdf");
            });
        };
    }])
    .controller("studentDriverMappingCtrl", ["$scope", "$http", "$window", "jsPDF", ($scope, $http, $window, jsPDF) => {

        $scope.showPresentOnly = false;

        $scope.resolvePassengers = driver => {
            if (driver.students && driver.students.length) {
                return driver.students.map(student => 1 + student.familySize).reduce((previousValue, currentValue) => previousValue + currentValue, 0);
            } else {
                return 0;
            }
        };

        $scope.togglePresentStudents = flag => {
            if (flag) {
                $scope.availableStudents = $scope.rawAvailableStudents.filter(student => student.isPresent);
            } else {
                $scope.availableStudents = $scope.rawAvailableStudents;
            }
        };

        $scope.getAllDriverMappingPDF = () => {
            $http.get("/api/studentDriverMapping/status").then(response => {
                const driverBucket = response.data.mappedDrivers;

                const doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(11);

                const subsetAttr = (attrList, obj) => attrList.reduce((o, k) => {
                    o[k] = obj[k];
                    return o;
                }, {});

                driverBucket.map((driver, index) => {
                    let str = "";

                    if (driver) {
                        str += "Driver Name: " + driver.fullname + "\n";
                        str += "Driver Contact: " + driver.phone + "\n";
                        str += "Driver Capacity: " + driver.capacity + "\n";
                        str += "\n";
                    }

                    if (!driver.students) {
                        driver.students = [];
                    }

                    str += stringTable.create(driver.students.map(driver => subsetAttr(["fullname", "email", "phone", "country", "isPresent"], driver)));

                    doc.text(20, 20, str);

                    if (index + 1 < driverBucket.length) {
                        doc.addPage();
                    }
                });

                doc.save("student-driver-mapping.pdf");
            });
        };

        $scope.sendMailToDrivers = $event => {
            if ($window.confirm("Are you sure to email mappings to drivers?")) {
                $http.post("/api/studentDriverMapping/EmailMappings").then(response => {
                    $window.alert("Successfully sent the mappings");
                });
            }
        };

        $scope.getStatus = () => $http.get("/api/studentDriverMapping/status").then(response => {
            const data = response.data;
            $scope.availableDrivers = data.availableDrivers;
            $scope.availableStudents = data.availableStudents;
            $scope.rawAvailableStudents = $scope.availableStudents;
            $scope.mappedDrivers = data.mappedDrivers;
            $scope.availableDriversTable = $scope.availableDrivers
                .reduce((acc, cur) => {
                    acc[cur.key.id] = cur.value;
                    return acc;
                }, {});

            $scope.availableDriversBuckets = data.availableDrivers
                .map(value => value.key)
                .reduce((rv, x) => {
                    const key = ('host' in x && !!x['host']) ? x['host'].fullname : 'Unassigned';
                    (rv[key] = rv[key] || []).push(x);
                    return rv;
                }, {});
        });

        $scope.map = (studentId, driverId) => {
            $scope.changeMap(studentId, driverId, "map");
        };

        $scope.unmap = (studentId, driverId) => {
            $scope.changeMap(studentId, driverId, "unmap");
        };

        $scope.changeMap = (studentId, driverId, action) => {
            if (driverId && studentId) {
                $http.post("/api/studentDriverMapping/" + action, {
                    "driverId": driverId,
                    "studentId": studentId
                }).then(() => {
                    $scope.getStatus();
                });
            }
        };

        $scope.init = () => {
            $scope.getStatus();
        };

        $scope.init();
    }])
    .controller("studentAttendanceCtrl", ["$scope", "$http", "$window", ($scope, $http, $window) => {
        $scope.countries = ["All Countries"];
        $scope.students = [];
        $scope.allStudents = [];
        $scope.countryCount = {};

        $scope.country = "All Countries";
        $scope.attendanceFilter = "all";
        $scope.fullname = "";
        $scope.drivers = [];
        $scope.availableDriversBuckets = {};

        $scope.getCountAllStudents = () => $scope.allStudents.length;

        $scope.getCountPresentStudents = () => $scope.allStudents.filter(x => x.isPresent).length;

        $scope.getCountAbsentStudents = () => $scope.allStudents.filter(x => !x.isPresent).length;

        $scope.resolvePassengers = driver => {
            if (driver.students && driver.students.length) {
                return driver.students.map(student => 1 + student.familySize).reduce((previousValue, currentValue) => previousValue + currentValue, 0);
            } else {
                return 0;
            }
        };

        // Get All Drivers
        $scope.getAllDrivers = () => {
            $http.get("/api/driver/").then(response => {
                $scope.drivers = response.data.filter(driver => driver.role === 'Driver');

                $scope.availableDriversTable = $scope.drivers
                    .reduce((acc, driver) => {
                        acc[driver.id] = driver.capacity > $scope.resolvePassengers(driver);
                        return acc;
                    }, {});

                $scope.availableDriversBuckets = $scope.drivers.reduce((rv, x) => {
                    const key = ('host' in x && !!x['host']) ? x['host'].fullname : 'Unassigned';
                    (rv[key] = rv[key] || []).push(x);
                    return rv;
                }, {});
            });
        };

        $scope.addDriverMap = (studentId, driverId) => {
            if (driverId && studentId) {
                $http.post("/api/studentDriverMapping/map", {
                    "driverId": driverId,
                    "studentId": studentId
                }).then(() => {
                    $scope.getAllDrivers();
                    $scope.getAllStudents();
                });
            }
        };

        $scope.checkInViaEmail = () => {
            if ($window.confirm("Are you sure to send check-in via email to students?")) {
                return $http.post("/api/attendance/student/sendCheckIn").then(() => {
                    alert("Check-in via email is sent to students");
                });
            }
        };

        $scope.getCountryCount = country => {
            if (country in $scope.countryCount) {
                return $scope.countryCount[country];
            } else {
                return 0;
            }
        };

        $scope.getAllStudents = () => $http.get("/api/student").then(response => {
            $scope.students = response.data;
            $scope.allStudents = response.data;

            $scope.students.forEach(student => {
                if (!$scope.countries.includes(student.country)) {
                    $scope.countries.push(student.country);
                }

                if (student.country in $scope.countryCount) {
                    $scope.countryCount[student.country] = 1 + $scope.countryCount[student.country];
                } else {
                    $scope.countryCount[student.country] = 1;
                }
            });

            $scope.countryCount["All Countries"] = $scope.allStudents.length;

            // Filter
            const countries = $scope.countries.filter(value => value !== "All Countries");

            // Sort
            countries.sort();

            $scope.countries = ["All Countries"].concat(countries);

            $scope.updateTable();
        });

        $scope.changeAttendance = student => $http.post("/api/attendance/student/setAttendance", {
            id: student.id,
            attendance: student.isPresent
        }).then(() => {
            $scope.getAllStudents();
        });

        $scope.updateTable = () => {
            let students = $scope.allStudents;

            const filteredStudents = {
                "country": [],
                "attendance": [],
                "fullname": []
            };

            if ($scope.country === "All Countries") {
                filteredStudents.country = students;
            } else {
                students.forEach(student => {
                    if (student.country === $scope.country) {
                        filteredStudents.country.push(student);
                    }
                });
            }

            students = filteredStudents.country;

            if ($scope.attendanceFilter === "all") {
                filteredStudents.attendance = students;
            } else {
                students.forEach(student => {
                    if ((student.isPresent && $scope.attendanceFilter === "yes") || (!student.isPresent && $scope.attendanceFilter === "no")) {
                        filteredStudents.attendance.push(student);
                    }
                });
            }

            students = filteredStudents.attendance;

            if (!$scope.fullname) {
                filteredStudents.fullname = students;
            } else {
                students.forEach(student => {
                    if (student.fullname.toLowerCase().indexOf($scope.fullname.toLowerCase()) > -1) {
                        filteredStudents.fullname.push(student);
                    }
                });
            }

            students = filteredStudents.fullname;

            $scope.students = students;
        };

        $scope.init = () => {
            $scope.getAllDrivers();
            $scope.getAllStudents();
        };

        $scope.init();
    }])
    .controller("driverHostMappingCtrl", ["$scope", "$http", "$window", "jsPDF", ($scope, $http, $window, jsPDF) => {

        $scope.resolvePassengers = driver => {
            if (driver.students && driver.students.length) {
                return driver.students.map(student => 1 + student.familySize).reduce((previousValue, currentValue) => previousValue + currentValue, 0);
            } else {
                return 0;
            }
        };

        $scope.getHostInfo = host => {
            let hostCapacity = 0;
            let hostAssigned = 0;

            if (host.drivers) {
                hostCapacity = host.drivers.map(driver => driver.capacity)
                    .reduce((accumulator, currentValue) => accumulator + currentValue, 0);

                hostAssigned = host.drivers.map(driver => $scope.resolvePassengers(driver)).reduce((accumulator, currentValue) => accumulator + currentValue, 0);
            }

            return {
                "hostCapacity": hostCapacity,
                "hostAssigned": hostAssigned
            };
        };

        $scope.getAllDriverMappingPDF = () => {
            $http.get("/api/driverHostMapping/status").then(response => {
                const hostBucket = response.data.mappedHosts;

                const doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(11);

                const subsetAttr = (attrList, obj) => attrList.reduce((o, k) => {
                    o[k] = obj[k];
                    return o;
                }, {});

                hostBucket.map((host, index) => {
                    let str = "";

                    if (host) {
                        str += "Host Name: " + host.fullname + "\n";
                        str += "Host Address: " + host.address + "\n";
                        str += "Host Contact: " + host.phone + "\n";
                        str += "\n";
                    }

                    str += stringTable.create(host.drivers.map(driver => subsetAttr(["fullname", "email", "phone", "capacity", "isPresent"], driver)));

                    doc.text(20, 20, str);

                    if (index + 1 < hostBucket.length) {
                        doc.addPage();
                    }
                });

                doc.save("driver-host-mapping.pdf");
            });
        };

        $scope.sendMailToHosts = $event => {
            if ($window.confirm("Are you sure to email mappings to hosts?")) {
                $http.post("/api/driverHostMapping/EmailMappings").then(response => {
                    $window.alert("Successfully sent the mappings");
                });
            }
        };

        $scope.getStatus = () => $http.get("/api/driverHostMapping/status").then(response => {
            const data = response.data;
            $scope.availableDrivers = data.availableDrivers;
            $scope.availableHosts = data.availableHosts;
            $scope.mappedHosts = data.mappedHosts;
        });

        $scope.map = (driverId, hostId) => {
            $scope.changeMap(driverId, hostId, "map");
        };

        $scope.unmap = (driverId, hostId) => {
            $scope.changeMap(driverId, hostId, "unmap");
        };

        $scope.changeMap = (driverId, hostId, action) => {
            if (driverId && hostId) {
                $http.post("/api/driverHostMapping/" + action, {
                    "driverId": driverId,
                    "hostId": hostId
                }).then(() => {
                    $scope.getStatus();
                });
            }
        };

        $scope.init = () => {
            $scope.getStatus();
        };

        $scope.init();
    }])
    .controller("driverAttendanceCtrl", ["$scope", "$http", "$window", ($scope, $http, $window) => {
        $scope.drivers = [];
        $scope.allDrivers = [];

        $scope.attendanceFilter = "all";
        $scope.fullname = "";

        $scope.getCountAllDrivers = () => $scope.allDrivers.length;

        $scope.getCountPresentDrivers = () => $scope.allDrivers.filter(x => x.isPresent).length;

        $scope.getCountAbsentDrivers = () => $scope.allDrivers.filter(x => !x.isPresent).length;

        $scope.checkInViaEmail = () => {
            if ($window.confirm("Are you sure to send check-in via email to drivers?")) {
                return $http.post("/api/attendance/driver/sendCheckIn").then(() => {
                    alert("Check-in via email is sent to drivers");
                });
            }
        };

        $scope.getAllDrivers = () => $http.get("/api/driver").then(response => {
            $scope.drivers = response.data.filter(value => value.role === 'Driver');
            $scope.allDrivers = response.data.filter(value => value.role === 'Driver');

            $scope.updateTable();
        });

        $scope.changeAttendance = driver => $http.post("/api/attendance/driver/setAttendance", {
            id: driver.id,
            attendance: driver.isPresent
        }).then(() => {
            $scope.getAllStudents();
        });

        $scope.updateTable = () => {
            let drivers = $scope.allDrivers;

            const filteredDrivers = {
                "attendance": [],
                "fullname": []
            };

            if ($scope.attendanceFilter === "all") {
                filteredDrivers.attendance = drivers;
            } else {
                drivers.forEach(driver => {
                    if ((driver.isPresent && $scope.attendanceFilter === "yes") || (!driver.isPresent && $scope.attendanceFilter === "no")) {
                        filteredDrivers.attendance.push(driver);
                    }
                });
            }

            drivers = filteredDrivers.attendance;

            if (!$scope.fullname) {
                filteredDrivers.fullname = drivers;
            } else {
                drivers.forEach(driver => {
                    if (driver.fullname.toLowerCase().indexOf($scope.fullname.toLowerCase()) > -1) {
                        filteredDrivers.fullname.push(driver);
                    }
                });
            }

            drivers = filteredDrivers.fullname;

            $scope.drivers = drivers;
        };

        $scope.init = () => {
            $scope.getAllDrivers();
        };

        $scope.init();
    }]);
    
