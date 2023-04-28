angular.element(document).ready(() => {
    window.setTimeout(() => {
        angular.element('.alert a').click();
    }, 5000);
});

// Ignore deprecation warnings
moment.suppressDeprecationWarnings = true;

angular.module('angular-async-await', [])
    .factory('$async', ['$rootScope', '$log', ($rootScope, $log) => ($scope, func) => async (...args) => {
        if (!$scope.contour) {
            $scope.contour = 0;
        }
        
        $scope.contour++;
        
        try {
            return await func(...args);
        } catch (e) {
            $log.error(e);
        } finally {
            $scope.contour--;
            if (!$scope.contour) {
                $log.info("running $rootScope.$apply()");
                $rootScope.$apply();
            } else {
                $log.info("skipped running $rootScope.$apply()");
            }
        }
    }]);

angular.module('arber.js', [])
    .factory('arber.js-renderer', () => (elm) => {
        const canvas = angular.element(elm).get(0);
        const ctx = canvas.getContext("2d");
        let particleSystem;

        const that = {
            init: system => {
                //
                // the particle system will call the init function once, right before the
                // first frame is to be drawn. it's a good place to set up the canvas and
                // to pass the canvas size to the particle system
                //
                // save a reference to the particle system for use in the .redraw() loop
                particleSystem = system

                // inform the system of the screen dimensions so it can map coords for us.
                // if the canvas is ever resized, screenSize should be called again with
                // the new dimensions
                particleSystem.screenSize(canvas.width, canvas.height)
                particleSystem.screenPadding(80) // leave an extra 80px of whitespace per side

                // set up some event handlers to allow for node-dragging
                that.initMouseHandling()
            },

            redraw: () => {
                // 
                // redraw will be called repeatedly during the run whenever the node positions
                // change. the new positions for the nodes can be accessed by looking at the
                // .p attribute of a given node. however the p.x & p.y values are in the coordinates
                // of the particle system rather than the screen. you can either map them to
                // the screen yourself, or use the convenience iterators .eachNode (and .eachEdge)
                // which allow you to step through the actual node objects but also pass an
                // x,y point in the screen's coordinate system
                // 
                ctx.fillStyle = "white"
                ctx.fillRect(0, 0, canvas.width, canvas.height)

                particleSystem.eachEdge((edge, pt1, pt2) => {
                    // edge: {source:Node, target:Node, length:#, data:{}}
                    // pt1:  {x:#, y:#}  source position in screen coords
                    // pt2:  {x:#, y:#}  target position in screen coords

                    // draw a line from pt1 to pt2
                    ctx.strokeStyle = "rgba(0,0,0, .333)"
                    ctx.lineWidth = 1
                    ctx.beginPath()
                    ctx.moveTo(pt1.x, pt1.y)
                    ctx.lineTo(pt2.x, pt2.y)
                    ctx.stroke()
                })

                particleSystem.eachNode((node, pt) => {
                    // node: {mass:#, p:{x,y}, name:"", data:{}}
                    // pt:   {x:#, y:#}  node position in screen coords

                    // draw a rectangle centered at pt
                    const w = 10;
                    ctx.fillStyle = (node.data.alone) ? "orange" : "black"
                    ctx.fillRect(pt.x - w / 2, pt.y - w / 2, w, w)
                })
            },

            initMouseHandling: () => {
                // no-nonsense drag and drop (thanks springy.js)
                let dragged = null;

                // set up a handler object that will initially listen for mousedowns then
                // for moves and mouseups while dragging
                const handler = {
                    clicked: e => {
                        const pos = angular.element(canvas).offset();
                        _mouseP = arbor.Point(e.pageX - pos.left, e.pageY - pos.top)
                        dragged = particleSystem.nearest(_mouseP);

                        if (dragged && dragged.node !== null) {
                            // while we're dragging, don't let physics move the node
                            dragged.node.fixed = true
                        }

                        angular.element(canvas).bind('mousemove', handler.dragged)
                        angular.element(window).bind('mouseup', handler.dropped)

                        return false
                    },
                    dragged: e => {
                        const pos = angular.element(canvas).offset();
                        const s = arbor.Point(e.pageX - pos.left, e.pageY - pos.top);

                        if (dragged && dragged.node !== null) {
                            const p = particleSystem.fromScreen(s);
                            dragged.node.p = p
                        }

                        return false
                    },

                    dropped: e => {
                        if (dragged === null || dragged.node === undefined) return
                        if (dragged.node !== null) dragged.node.fixed = false
                        dragged.node.tempMass = 1000
                        dragged = null
                        angular.element(canvas).unbind('mousemove', handler.dragged)
                        angular.element(window).unbind('mouseup', handler.dropped)
                        _mouseP = null
                        return false
                    }
                };

                // start listening
                angular.element(canvas).mousedown(handler.clicked);
            },

        };
        return that;
    });

angular.module('tourApp', ['ui.toggle', 'ngTagsInput', 'chart.js', 'ngSanitize', 'angular-async-await', 'angular-loading-bar', 'arber.js'])
    .config(['cfpLoadingBarProvider', cfpLoadingBarProvider => {
        cfpLoadingBarProvider.includeSpinner = false;
        cfpLoadingBarProvider.latencyThreshold = 300;
    }])
    .constant('jsPDF', (jspdf || window.jspdf).jsPDF)
    .directive('validateBeforeGoing', ['$window', $window => ({
        restrict: 'A',
        link: (scope, element, attrs) => {
            angular.element(element).on('click', e => {
                if (!$window.confirm(attrs.message)) {
                    e.preventDefault();
                }
            });
        }
    })])
    .controller('apiEventsCtrl', ['$scope', '$http', '$sce', '$async', async ($scope, $http, $sce, $async) => {

        $scope.count = 0;
        $scope.events = [];
        $scope.eventsRawDump = '';

        $scope.appendEvent = evt => {
            $scope.events.unshift(evt);
            $scope.eventsRawDump = $sce.trustAsHtml($scope.events.map(x => `${moment(x.recordedDate).local().format('YYYY-MM-DD HH:mm:ss')}\t${x.description}`).join('\n'));
            $scope.$apply();
        };

        $scope.init = $async($scope, async () => {
            const {data: events} = await $http.get('/apiEvents/latest');
            const {data: {token}} = await $http.get('/identity/token');

            events.sort((left, right) => moment.utc(left.recordedDate).diff(moment.utc(right.recordedDate))).forEach(evt => {
                $scope.appendEvent(evt);
            });

            const options = {
                accessTokenFactory: () => token
            };

            const connection = new signalR.HubConnectionBuilder()
                .withUrl('/hub', options)
                .withAutomaticReconnect()
                .build();

            connection.on('count', count => {
                $scope.count = count;
                $scope.$apply();
            });

            connection.on('log', (state, connectionId, username) => {
                $scope.appendEvent({
                    description: `SignalR event [${username}]: ${state} ${connectionId}`,
                    recordedDate: moment().toString()
                });
            });

            connection.on('events', evt => {
                $scope.appendEvent(evt);
            });

            // Start the connection.
            await connection.start();
        });

        await $scope.init();
    }])
    .controller('statsCtrl', ['$scope', '$http', '$async', async ($scope, $http, $async) => {
        $scope.countryDistribution = {};
        $scope.year = 'All';
        $scope.countryDistributionChartData = [];
        $scope.countryDistributionChartLabels = [];

        $scope.getCountryDistribution = $async($scope, async () => {
            const {data} = await $http.get('/stats/countryDistribution');
            $scope.countryDistribution = data;
            $scope.refreshCountryDistributionChart();
        });

        $scope.handleYearChange = $event => {
            $event.preventDefault();
            $scope.year = $event.target.getAttribute('data-year');
            $scope.refreshCountryDistributionChart();
        };

        $scope.refreshCountryDistributionChart = () => {
            $scope.countryDistributionChartLabels = Object.keys($scope.countryDistribution[$scope.year]);
            $scope.countryDistributionChartData = Object.values($scope.countryDistribution[$scope.year]);
        };

        $scope.init = $async($scope, async () => {
            await $scope.getCountryDistribution();
        });

        await $scope.init();
    }])
    .controller('eventInfoCtrl', ['$scope', '$http', '$window', '$async', ($scope, $http, $window, $async) => {

        $scope.mapStudent = $async($scope, async $event => {
            $event.preventDefault();

            const eventId = $scope.eventId;
            const studentId = $scope.studentId;

            if (eventId && studentId) {
                await $http.post(`/api/event/map/${eventId}/${studentId}`);
                await $scope.fetchInfo();
            }
        });

        $scope.unMapStudent = $async($scope, async ($event, studentId) => {
            $event.preventDefault();

            const eventId = $scope.eventId;

            if (eventId && studentId) {
                await $http.post(`/api/event/unmap/${eventId}/${studentId}`);
                await $scope.fetchInfo();
            }
        });

        $scope.sendAdHocEmail = $async($scope, async $event => {
            $event.preventDefault();

            const eventId = $scope.eventId;
            const emailSubject = $scope.emailSubject;
            const emailBody = angular.element('.summernote').eq(0).summernote('code');

            if (eventId && emailSubject && emailBody && $window.confirm('Are you sure to send an email to RSVPed students?')) {
                await $http.post('/api/event/email', {
                    emails: $scope.event.students.map(x => x.student.email),
                    body: emailBody,
                    subject: emailSubject
                });
                $window.alert('Successfully sent email!');
            }
        });

        $scope.fetchInfo = $async($scope, async () => {
            const {data} = await $http.get(`/api/event/info/${$scope.eventId}`);
            $scope.event = data.event;
            $scope.availableStudents = data.availableStudents;
        });

        // Start the text editor
        angular.element('.summernote').summernote({height: 150});

    }])
    .controller('userListCtrl', ['$scope', '$http', ($scope, $http) => {

    }])
    .controller('emailUtilityCtrl', ['$timeout', $timeout => {

        // Hide the .autoclose
        $timeout(() => {
            angular.element('.autoclose').fadeOut();
        }, 2000);

        // Start the text editor
        angular.element('.summernote').summernote({height: 150});
    }])
    .controller('emailCheckInCtrl', ['$scope', '$http', '$async', async ($scope, $http, $async) => {
        $scope.changeAttendance = $async($scope, async (type, id, value) => {
            await $http.post(`/utility/emailCheckInAction/${type}/${id}?present=${value}`);
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
    .controller('studentRegistrationCtrl', ['$scope', $scope => {

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
    .controller('studentListCtrl', ['$scope', '$http', 'jsPDF', '$async', ($scope, $http, jsPDF, $async) => {

        $scope.downloadTable = {
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

        const subsetAttr = (attrList, obj) => attrList.reduce((o, k) => {
            o[k] = String(obj[k]);
            return o;
        }, {});

        $scope.getAllStudentsCSV = $async($scope, async () => {
            const {data: students} = await $http.get('/api/student');
            const attributes = Object.keys($scope.downloadTable).filter(value => $scope.downloadTable[value]);

            const rows = students
                .map(student => attributes.map(x => student[x]));

            let csvContent = "data:text/csv;charset=utf-8,";

            [attributes].concat(rows).forEach(rowArray => {
                let row = rowArray.join(",");
                csvContent += row + "\r\n";
            });

            download(csvContent, "student-list.csv", "text/csv");
        });

        $scope.getAllStudentsPDF = $async($scope, async () => {
            const {data: students} = await $http.get('/api/student');

            const doc = new jsPDF({
                orientation: 'l',
                lineHeight: 1.5
            });

            doc.setFont('courier');

            let i, j, temporary;
            const chunk = 25;
            const attributes = Object.keys($scope.downloadTable).filter(value => $scope.downloadTable[value]);

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

            doc.save('student-list.pdf');
        });
    }])
    .controller('driverListCtrl', ['$scope', '$http', 'jsPDF', '$async', ($scope, $http, jsPDF, $async) => {
        $scope.getAllDriversPDF = $async($scope, async () => {
            const {data: drivers} = await $http.get('/api/driver');

            const doc = new jsPDF({
                orientation: 'l',
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
                    driver.navigator = driver.navigator || driver.navigator === 'null' ?
                        (driver.navigator.length > 20 ? `${driver.navigator.substring(0, 20)} ...` : driver.navigator) : '-';

                    return subsetAttr(['displayId', 'fullname', 'capacity', 'navigator', 'role', 'haveChildSeat'], driver);
                }));

                // Needed
                str = str.replace(/’/g, "'");

                if (i === 0) {
                    str = `Driver List ( count of drivers: ${drivers.length} )\n\n${str}`;
                }

                doc.text(20, 20, str);

                if (i + chunk < j) {
                    doc.addPage();
                }
            }

            doc.save('driver-list.pdf');
        });
    }])
    .controller('hostListCtrl', ['$scope', '$http', 'jsPDF', '$async', ($scope, $http, jsPDF, $async) => {
        $scope.getAllHostPDF = $async($scope, async () => {
            const {data: hosts} = await $http.get('/api/host');

            const doc = new jsPDF({
                orientation: 'l',
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
            for (i = 0, j = hosts.length; i < j; i += chunk) {
                temporary = hosts.slice(i, i + chunk);

                let str = stringTable.create(temporary.map(host => subsetAttr(['fullname', 'email', 'phone', 'address'], host)));

                if (i === 0) {
                    str = `Host List ( count of hosts: ${hosts.length} )\n\n${str}`;
                }

                doc.text(20, 20, str);

                if (i + chunk < j) {
                    doc.addPage();
                }
            }

            doc.save('host-list.pdf');
        });
    }])
    .controller('studentDriverMappingCtrl', ['$scope', '$http', '$window', 'jsPDF', '$async', async ($scope, $http, $window, jsPDF, $async) => {

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

        $scope.getAllDriverMappingPDF = $async($scope, async () => {
            const {data: {mappedDrivers: driverBucket}} = await $http.get('/api/studentDriverMapping/status');

            const doc = new jsPDF({
                orientation: 'l',
                lineHeight: 1.5
            });

            doc.setFont('courier');

            doc.setFontSize(11);

            const subsetAttr = (attrList, obj) => attrList.reduce((o, k) => {
                o[k] = obj[k];
                return o;
            }, {});

            driverBucket.map((driver, index) => {
                let str = '';

                if (driver) {
                    str += `Driver Name: ${driver.fullname}\n`;
                    str += `Driver Contact: ${driver.phone}\n`;
                    str += `Driver Capacity: ${driver.capacity}\n`;
                    str += '\n';
                }

                if (!driver.students) {
                    driver.students = [];
                }

                str += stringTable.create(driver.students.map(driver => subsetAttr(['fullname', 'email', 'phone', 'country', 'isPresent'], driver)));

                doc.text(20, 20, str);

                if (index + 1 < driverBucket.length) {
                    doc.addPage();
                }
            });

            doc.save('student-driver-mapping.pdf');
        });

        $scope.sendMailToDrivers = $async($scope, async $event => {
            if ($window.confirm('Are you sure to email mappings to drivers?')) {
                await $http.post('/api/studentDriverMapping/EmailMappings');
                $window.alert('Successfully sent the mappings');
            }
        });

        $scope.getStatus = $async($scope, async () => {
            const {data} = await $http.get('/api/studentDriverMapping/status');
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
                    const key = 'host' in x && !!x['host'] ? x['host'].fullname : 'Unassigned';
                    (rv[key] = rv[key] || []).push(x);
                    return rv;
                }, {});

            // Ensure state is preserved
            $scope.togglePresentStudents($scope.showPresentOnly);
        });

        $scope.map = async (studentId, driverId) => {
            await $scope.changeMap(studentId, driverId, 'map');
        };

        $scope.unmap = async (studentId, driverId) => {
            await $scope.changeMap(studentId, driverId, 'unmap');
        };

        $scope.changeMap = $async($scope, async (studentId, driverId, action) => {
            if (driverId && studentId) {
                await $http.post(`/api/studentDriverMapping/${action}`, {
                    driverId,
                    studentId
                });
                await $scope.getStatus();
            }
        });

        $scope.init = $async($scope, async () => {
            await $scope.getStatus();
        });

        await $scope.init();
    }])
    .controller('studentAttendanceCtrl', ['$scope', '$http', '$window', '$async', async ($scope, $http, $window, $async) => {
        $scope.students = [];
        $scope.allStudents = [];

        $scope.country = 'All Countries';
        $scope.attendanceFilter = 'all';
        $scope.fullname = '';
        $scope.drivers = [];
        $scope.availableDriversBuckets = {};

        $scope.generalFilterStudents = ({ignoreAttendance = false, ignoreCountry = false} = {}) => {
            let students = $scope.allStudents;
            if (!ignoreCountry && $scope.country && $scope.country !== 'All Countries') {
                students = students.filter(x => x.country === $scope.country);
            }

            if ($scope.fullname) {
                students = students.filter(x => x.fullname.toLowerCase().includes($scope.fullname.toLowerCase()));
            }

            if (!ignoreAttendance && $scope.attendanceFilter !== 'all') {
                students = students.filter(student => student.isPresent && $scope.attendanceFilter === 'yes' || !student.isPresent && $scope.attendanceFilter === 'no');
            }

            return students;
        };

        $scope.getCountAllStudents = () => {
            return $scope.generalFilterStudents({ignoreAttendance: true}).length;
        };

        $scope.getCountPresentStudents = () => {
            return $scope.generalFilterStudents({ignoreAttendance: true}).filter(x => x.isPresent).length;
        };

        $scope.getCountAbsentStudents = () => {
            return $scope.generalFilterStudents({ignoreAttendance: true}).filter(x => !x.isPresent).length;
        };

        $scope.countries = () => {
            const students = $scope.generalFilterStudents();
            return students.reduce((acc, student) => ({
                ...acc,
                [student.country]: student.country in acc ? acc[student.country] + 1 : 1
            }), {
                ['All Countries']: $scope.generalFilterStudents({ignoreCountry: true}).length
            });
        };

        $scope.resolvePassengers = driver => {
            if (driver.students && driver.students.length) {
                return driver.students.map(student => 1 + student.familySize).reduce((previousValue, currentValue) => previousValue + currentValue, 0);
            } else {
                return 0;
            }
        };

        // Get All Drivers
        $scope.getAllDrivers = $async($scope, async () => {
            const {data} = await $http.get('/api/driver');
            $scope.drivers = data.filter(driver => driver.role === 'Driver');

            $scope.availableDriversTable = $scope.drivers
                .reduce((acc, driver) => {
                    acc[driver.id] = driver.capacity > $scope.resolvePassengers(driver);
                    return acc;
                }, {});

            $scope.availableDriversBuckets = $scope.drivers.reduce((rv, x) => {
                const key = 'host' in x && !!x['host'] ? x['host'].fullname : 'Unassigned';
                (rv[key] = rv[key] || []).push(x);
                return rv;
            }, {});
        });

        $scope.addDriverMap = $async($scope, async (studentId, driverId) => {
            if (driverId && studentId) {
                await $http.post('/api/studentDriverMapping/map', {
                    driverId,
                    studentId
                });
                await Promise.all([$scope.getAllStudents(), $scope.getAllDrivers()]);
            }
        });

        $scope.checkInViaEmail = $async($scope, async () => {
            if ($window.confirm(`Are you sure to send check-in via email to [${$scope.getCountAllStudents()}] students?`)) {
                await $http.post('/api/attendance/student/sendCheckIn');
                $window.alert(`Check-in via email is sent to [${$scope.getCountAllStudents()}] students`);
            }
        });

        $scope.getAllStudents = $async($scope, async () => {
            const {data: allStudents} = await $http.get('/api/student');
            $scope.allStudents = allStudents;

            $scope.updateTable();
        });

        $scope.changeAttendance = $async($scope, async student => {
            await $http.post('/api/attendance/student/setAttendance', {
                id: student.id,
                attendance: student.isPresent
            });

            await $scope.getAllStudents();
        });

        $scope.updateTable = () => {
            $scope.students = $scope.generalFilterStudents();
            if (!$scope.country) {
                $scope.country = 'All Countries';
            }

            if (!$scope.attendanceFilter) {
                $scope.attendanceFilter = 'all';
            }
        };

        $scope.init = $async($scope, async () => {
            await Promise.all([$scope.getAllStudents(), $scope.getAllDrivers()]);
        });

        await $scope.init();
    }])
    .controller('driverHostMappingCtrl', ['$scope', '$http', '$window', 'jsPDF', '$async', async ($scope, $http, $window, jsPDF, $async) => {

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
                hostCapacity,
                hostAssigned
            };
        };

        $scope.getAllDriverMappingPDF = $async($scope, async () => {
            const response = await $http.get('/api/driverHostMapping/status');
            const hostBucket = response.data.mappedHosts;

            const doc = new jsPDF({
                orientation: 'l',
                lineHeight: 1.5
            });

            doc.setFont('courier');

            doc.setFontSize(11);

            const subsetAttr = (attrList, obj) => attrList.reduce((o, k) => {
                o[k] = obj[k];
                return o;
            }, {});

            hostBucket.map((host, index) => {
                let str = '';

                if (host) {
                    str += `Host Name: ${host.fullname}\n`;
                    str += `Host Address: ${host.address}\n`;
                    str += `Host Contact: ${host.phone}\n`;
                    str += '\n';
                }

                str += stringTable.create(host.drivers.map(driver => subsetAttr(['fullname', 'email', 'phone', 'capacity', 'isPresent'], driver)));

                doc.text(20, 20, str);

                if (index + 1 < hostBucket.length) {
                    doc.addPage();
                }
            });

            doc.save('driver-host-mapping.pdf');
        });

        $scope.sendMailToHosts = $async($scope, async $event => {
            if ($window.confirm('Are you sure to email mappings to hosts?')) {
                await $http.post('/api/driverHostMapping/EmailMappings');
                $window.alert('Successfully sent the mappings');
            }
        });

        $scope.getStatus = $async($scope, async () => {
            const {data} = await $http.get('/api/driverHostMapping/status');
            $scope.availableDrivers = data.availableDrivers;
            $scope.availableHosts = data.availableHosts;
            $scope.mappedHosts = data.mappedHosts;
        });

        $scope.map = $async($scope, async (driverId, hostId) => {
            await $scope.changeMap(driverId, hostId, 'map');
        });

        $scope.unmap = $async($scope, async (driverId, hostId) => {
            await $scope.changeMap(driverId, hostId, 'unmap');
        });

        $scope.changeMap = $async($scope, async (driverId, hostId, action) => {
            if (driverId && hostId) {
                await $http.post(`/api/driverHostMapping/${action}`, {
                    driverId,
                    hostId
                });
                await $scope.getStatus();
            }
        });

        $scope.init = $async($scope, async () => {
            await $scope.getStatus();
        });

        await $scope.init();
    }])
    .controller('driverAttendanceCtrl', ['$scope', '$http', '$window', '$async', async ($scope, $http, $window, $async) => {
        $scope.drivers = [];
        $scope.allDrivers = [];

        $scope.attendanceFilter = 'all';
        $scope.fullname = '';

        $scope.generalFilterDrivers = ({ignoreAttendance = false} = {}) => {
            let drivers = $scope.allDrivers;
            if ($scope.fullname) {
                drivers = drivers.filter(x => x.fullname.toLowerCase().includes($scope.fullname.toLowerCase()));
            }

            if (!ignoreAttendance && $scope.attendanceFilter !== 'all') {
                drivers = drivers.filter(driver => driver.isPresent && $scope.attendanceFilter === 'yes' || !driver.isPresent && $scope.attendanceFilter === 'no');
            }

            return drivers;
        };

        $scope.getCountAllDrivers = () => $scope.generalFilterDrivers({ignoreAttendance: true}).length;

        $scope.getCountPresentDrivers = () => $scope.generalFilterDrivers({ignoreAttendance: true}).filter(x => x.isPresent).length;

        $scope.getCountAbsentDrivers = () => $scope.generalFilterDrivers({ignoreAttendance: true}).filter(x => !x.isPresent).length;

        $scope.checkInViaEmail = $async($scope, async () => {
            if ($window.confirm(`Are you sure to send check-in via email to [${$scope.getCountAllDrivers()}] drivers?`)) {
                await $http.post('/api/attendance/driver/sendCheckIn');
                $window.alert(`Check-in via email is sent to [${$scope.getCountAllDrivers()}] drivers`);
            }
        });

        $scope.getAllDrivers =  $async($scope, async () => {
            const {data: drivers} = await $http.get('/api/driver');
            $scope.drivers = drivers.filter(value => value.role === 'Driver');
            $scope.allDrivers = drivers.filter(value => value.role === 'Driver');

            $scope.updateTable();
        })

        $scope.changeAttendance = $async($scope, async driver => {
            await $http.post('/api/attendance/driver/setAttendance', {
                id: driver.id,
                attendance: driver.isPresent
            });
            await $scope.getAllDrivers();
        });

        $scope.updateTable = () => {
            $scope.drivers = $scope.generalFilterDrivers();

            if (!$scope.attendanceFilter) {
                $scope.attendanceFilter = 'all';
            }
        };

        $scope.init = $async($scope, async () => {
            await $scope.getAllDrivers();
        });

        await $scope.init();
    }])
    .controller('locationListCtrl', ['$scope', '$http', '$window', '$async', 'jsPDF', async ($scope, $http, $window, $async, jsPDF) => {
        $scope.getAllLocationsPDF = $async($scope, async () => {
            const {data: locations} = await $http.get('/api/location');

            const doc = new jsPDF({
                orientation: 'l',
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
            for (i = 0, j = locations.length; i < j; i += chunk) {
                temporary = locations.slice(i, i + chunk);

                let str = stringTable.create(temporary.map(location => {
                    return subsetAttr(['name', 'address', 'description'], location);
                }));

                // Needed
                str = str.replace(/’/g, "'");

                if (i === 0) {
                    str = `Location List ( count of locations: ${locations.length} )\n\n${str}`;
                }

                doc.text(20, 20, str);

                if (i + chunk < j) {
                    doc.addPage();
                }
            }

            doc.save('location-list.pdf');
        });
    }])
    .controller('locationEditCtrl', ['$scope', '$http', '$window', '$async', async ($scope, $http, $window, $async) => {
    }])
    .controller('locationMappingCtrl', ['$scope', '$http', '$window', '$async', 'arber.js-renderer', ($scope, $http, $window, $async, Renderer) => {
        angular.element(document).ready(() => {
            const sys = arbor.ParticleSystem(1000, 600, 0.5); // create the system with sensible repulsion/stiffness/friction
            sys.parameters({gravity:true}); // use center-gravity to make the graph settle nicely (ymmv)
            sys.renderer = Renderer("#viewport"); // our newly created renderer will have its .init() method called shortly by sys...

            // add some nodes to the graph and watch it go...
            sys.addEdge('a','b');
            sys.addEdge('a','c');
            sys.addEdge('a','d');
            sys.addEdge('a','e');
            sys.addNode('f', {alone:true, mass:.25});
            
            sys.ren
        });
    }])
    .controller('locationMappingEditCtrl', ['$scope', '$http', '$window', '$async', async ($scope, $http, $window, $async) => {
    }])
    .controller('locationMappingSaveCtrl', ['$scope', '$http', '$window', '$async', async ($scope, $http, $window, $async) => {
    }]);