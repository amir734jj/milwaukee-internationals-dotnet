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
    .controller('studentRegistrationCtrl', ['$scope', function () {
        
    }]);