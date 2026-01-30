// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    var emailModal = document.getElementById('emailModal');
    if (emailModal) {
        emailModal.addEventListener('show.bs.modal', function (event) {
            // Button that triggered the modal
            var button = event.relatedTarget;
            // Extract info from data-bs-* attributes
            var planId = button.getAttribute('data-plan-id');
            var planName = button.getAttribute('data-plan-name');

            // Update the modal's content.
            var modalTitle = emailModal.querySelector('.modal-title #planName');
            var modalPlanIdInput = emailModal.querySelector('.modal-body #planId');

            modalTitle.textContent = planName;
            modalPlanIdInput.value = planId;
        });
    }
});
