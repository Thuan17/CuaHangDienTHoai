document.addEventListener("DOMContentLoaded", () => {
    const togglePasswordIcons = document.querySelectorAll(".toggle-password");

    togglePasswordIcons.forEach(icon => {
        icon.addEventListener("click", () => {
            const input = document.querySelector(icon.getAttribute("data-toggle"));
            if (input.type === "password") {
                input.type = "text";
                icon.classList.remove("fa-eye");
                icon.classList.add("fa-eye-slash");
            } else {
                input.type = "password";
                icon.classList.remove("fa-eye-slash");
                icon.classList.add("fa-eye");
            }
        });
    });
});

$(document).ready(function () {
    $('.myFormUpEmp').click( function (event) {
        event.preventDefault();  // Ngăn trang tự động load lại

        var employeeid = $(this).data('employeeid');
        var id = $(this).data('id');

    

        let isValid = validateForm();
        if (!isValid) return;
        let counter = 5;

        if (employeeid) {
            var $button = $(this); // Gán nút vào biến để dùng trong các callback
            $button.prop('disabled', true);
            $button.find('.loading-image').show();
            $button.find('.button-text').hide();
            const formData = $('#myFormUpEmp').serialize();
            $.ajax({
                url: '/Admin/Account/UpdatePass',
                type: 'POST',
                data: formData,
                success: function (res) {
                    if (res.Success) {
                        if (res.Code = 1) {
                            createToast('success', 'fa-solid fa-circle-check', 'Thành công', res.msg);
                            let interval = setInterval(() => {
                                createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý',`Đang đăng xuất ${counter} giây`);
                                counter--;

                                if (counter < 0) {
                                    clearInterval(interval);
                                    window.location.href = '/Admin/Account/Logout';
                                }
                            }, 1500);
                        }
                    } else {
                        $button.prop('disabled', false);
                        $button.find('.loading-image').hide();
                        $button.find('.button-text').show();
                        if (res.Code = -2) {
                            createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', res.msg);

                        } else if (res.Code = -3) {
                            createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', res.msg);
                            let interval = setInterval(() => {
                                createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', res.msg + `Chuyển trang trong ${counter} giây`);
                                counter--;

                                if (counter < 0) {
                                    clearInterval(interval);
                                    window.location.href = res.Url;
                                }
                            }, 1500);
                        } else if (res.code == -99) {
                            createToast('error', 'fa-solid fa-triangle-exclamation', 'Lỗi', res.msg);
                        }

                    }
                },
                error: function (xhr) {
                    createToast('error', 'fa-solid fa-triangle-exclamation', 'Lỗi', 'Có lỗi xảy ra khi gửi yêu cầu');
                    console.error(xhr.responseText);
                }
            });
        }

    });

});

function validateForm() {

    const currentpassword = $('#current-password').val().trim();
    const newpassword = $('#new-password').val().trim();


    const confirmpassword = $('#confirm-password').val().trim();

    const allFields = [
        'current-password', 'new-password', 'confirm-password'
    ];

    const isAllEmpty = allFields.every(id => $(`#${id}`).val().trim() === "");

    if (isAllEmpty) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Vui lòng nhập đầy đủ');
        return false;
    }

    let isValid = true;
    const passwordPattern = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    if (!newpassword || !passwordPattern.test(newpassword)) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ cái, số và ký tự đặc biệt');
        isValid = false;
    }

    if (!confirmpassword) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Mật khẩu xác nhận không để trống');
        isValid = false;
    }
    if (!newpassword) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Mật khẩu mới không để trống');
        isValid = false;
    }

    if (newpassword !== confirmpassword) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Mật khẩu mới và mật khẩu xác nhận giống nhau');
        isValid = false;
    }

    return isValid;
}



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