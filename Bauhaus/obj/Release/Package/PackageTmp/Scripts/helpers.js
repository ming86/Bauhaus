// Set of common functions for Bauhaus

// Configuration for all toaster messages.
toastr.options = {
    "closeButton": true,
    "debug": false,
    "positionClass": "toast-bottom-right",
    "onclick": null,
    "showDuration": "500",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

// Function to display all application alerts.
function displayAlert(Type, Message) {
    if (Message != "") {
        switch (Type) {
            case "info":
                toastr.info(Message);
                break;
            case "success":
                toastr.success(Message);
                break;
            case "warning":
                toastr.warning(Message);
                break;
            case "danger":
                toastr.error(Message);
                break;
            default:
                toastr.info(Message);
                break;
        }
    }
}