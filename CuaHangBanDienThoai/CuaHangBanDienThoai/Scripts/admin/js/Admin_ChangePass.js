$(document).ready(function () {
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

    function submitForm() {
        var isValid = true;
        const passold = $('#PassOld').val().trim();
        const newPassword = $('#NewPassword').val().trim();
        const confirmPassword = $('#ConfirmPassword').val().trim();

        if (!passold || !newPassword || !confirmPassword) {
            createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Vui lòng điền đầy đủ thông tin.');
            isValid = false;
        }

        if (newPassword !== confirmPassword) {
            createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Mật khẩu mới và mật khẩu xác nhận không khớp.');
            isValid = false;
        }

        const passwordMinLength = 8;
        const specialCharacterRegex = /[!@#$%^&*(),.?":{}|<>]/;

        if (newPassword.length < passwordMinLength) {
            createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', `Mật khẩu mới phải có ít nhất ${passwordMinLength} ký tự.`);
            isValid = false;
        }

        if (!specialCharacterRegex.test(newPassword)) {
            createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Mật khẩu mới phải chứa ít nhất một ký tự đặc biệt.');
            isValid = false;
        }

        if (isValid) {
            var form = $('#changepass')[0];
            var formData = new FormData(form);
            console.log("formData", formData);
            $.ajax({
                url: '/Admin/Account/Changepass',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        createToast('success', 'fa-solid fa-check', 'Thành công', 'Mật khẩu đã được thay đổi thành công.');
                    } else {
                        if (res.code === -99) {
                            createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Đã xảy ra lỗi. Vui lòng thử lại.');
                        } else if (res.code === -1 || res.code === -4) {
                            createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', res.message);
                            setTimeout(() => window.location.href = "/he-thong-nhan-vien", 1500);
                        } else {
                            createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', res.message);
                        }

                    }
                  
                },
                error: function (xhr, status, error) {
                    console.error("Lỗi AJAX:", xhr.responseText);
                    createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Đã xảy ra lỗi. Vui lòng thử lại.');
                }
            });
        }
    }


    function togglePasswordVisibility(inputId, iconId) {
        const passwordInput = document.getElementById(inputId);
        const toggleIcon = document.getElementById(iconId);

        if (passwordInput.type === "password") {
            passwordInput.type = "text";
            toggleIcon.classList.remove("fa-eye");
            toggleIcon.classList.add("fa-eye-slash");
        } else {
            passwordInput.type = "password";
            toggleIcon.classList.remove("fa-eye-slash");
            toggleIcon.classList.add("fa-eye");
        }
    }

    $('#savePass').click(function () {
        submitForm();
    });


    $('#togglePassOld').click(function () {
        togglePasswordVisibility('PassOld', 'togglePassOld');
    });

    $('#toggleNewPassword').click(function () {
        togglePasswordVisibility('NewPassword', 'toggleNewPassword');
    });

    $('#toggleConfirmPassword').click(function () {
        togglePasswordVisibility('ConfirmPassword', 'toggleConfirmPassword');
    });
});