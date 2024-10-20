
(function ($) {

    "use strict";

    var fullHeight = function () {

        $('.js-fullheight').css('height', $(window).height());
        $(window).resize(function () {
            $('.js-fullheight').css('height', $(window).height());
        });

    };
    fullHeight();

    $(".toggle-password").click(function () {

        $(this).toggleClass("fa-eye fa-eye-slash");
        var input = $($(this).attr("toggle"));
        if (input.attr("type") == "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
    });

})(jQuery);
function createToast(type, icon, title, text) {

    const notifications = document.querySelector('.notifications');
    if (!notifications) {
        console.warn('Không tìm thấy phần tử notifications trong DOM');
        return;
    }

    let newToast = document.createElement('div');
    newToast.innerHTML = `
        <div class="toastNotifi ${type}">
            <i class="${icon}"></i>
            <div class="content">
                <div class="title">${title}</div>
                <span>${text}</span>
            </div>
            <i class="close fa-solid fa-xmark" onclick="this.parentElement.remove()"></i>
        </div>`;
    notifications.appendChild(newToast);


    newToast.timeOut = setTimeout(() => newToast.remove(), 5000);
}


$(document).ready(function () {
    $('#loginForm').submit(function (e) {
        e.preventDefault();

        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    createToast(
                        'success',
                        'fa-solid fa-check',
                        'Đăng nhập thành công',
                        response.msg
                    );
                    if (response.redirectUrl) {
                        window.location.href = response.redirectUrl;
                    }
                } else {
                    createToast(
                        'error',
                        'fa-solid fa-xmark',
                        'Đăng nhập thất bại',
          
                        response.msg,
                    );
                }
            },
            error: function () {
                createToast(
                    'error',
                    'fa-solid fa-xmark',
                    'Lỗi',
                    'Đã xảy ra lỗi khi đăng nhập.'
                );
            }
        });
    });

    function getErrorMessage(code) {
        switch (code) {
            case -1:
                return 'Chức năng không hợp lệ';
            case -2:
                return 'Chức năng không hợp lệ';
            case -3:
                return 'Tài khoản đã bị khóa';
            case -4:
                return 'Tài khoản hoặc mật khẩu không đúng';
            case -5:
                return 'Dữ liệu không hợp lệ';
            case -6:
                return 'Dữ liệu không hợp lệ';
            default:
                return 'Đã xảy ra lỗi khi đăng nhập.';
        }
    }
});
