document.addEventListener("DOMContentLoaded", function () {
    var toastElements = document.querySelectorAll('.toast');
    toastElements.forEach(function (toastEl) {
        var delay = toastEl.dataset.delay ? parseInt(toastEl.dataset.delay, 10) : 3000;
        new bootstrap.Toast(toastEl, { delay: delay }).show();
    });
});
