/**
 * Displays a SweetAlert2 alert based on the provided AlertMessage object.
 * @param {Object} alertMessage - The alert message object containing AlertType, Title, and Message.
 */
function showSwalAlert(alertMessage) {
    const iconMap = {
        'success': 'success',
        'info': 'info',
        'warning': 'warning',
        'danger': 'error'
    };
    const icon = iconMap[alertMessage.AlertType] || 'info';

    Swal.fire({
        icon: icon,
        title: alertMessage.Title,
        text: alertMessage.Message,
        confirmButtonText: 'Tamam'
    });
}
