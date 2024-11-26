$(document).ready(function () {

    $('#registerForm').submit(function (event) {
        event.preventDefault();
        let isValid = validateForm();

        const formData = $('#registerForm').serialize();
        const token = $('input[name="__RequestVerificationToken"]').val();

     
        if (isValid) {
            var $button = $('.btnregister'); 
            $button.prop('disabled', true);
            $button.find('.loading-image').show();
            $button.find('.button-text').hide();
            $.ajax({
                url: '/Account/Register',
                type: 'POST',
                data: formData,
                success: function (res) {

                    if (res.success) {

                        createToast('success', 'fa-solid fa-circle-check', 'Thành công', res.msg);

                        if (res.Url) {
                            setTimeout(() => {
                                window.location.href = res.Url;
                            }, 1500);
                        } 

                    } else {
                        $button.prop('disabled', false);
                        $button.find('.loading-image').hide();
                        $button.find('.button-text').show();
                        if (res.code == -2) {
                            createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', res.msg);
                        } else if (res.code == -100) {
                            createToast('error', 'fa-solid fa-triangle-exclamation', 'Lỗi', res.msg);
                        }
                    }


                },
                error: function (xhr) {
                    $button.prop('disabled', false);
                    $button.find('.loading-image').hide();
                    $button.find('.button-text').show();
                    createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', res.msg);
                    console.error(xhr.responseText);
                }
            });
        }

    });
});


function validateForm() {
    const phone = $('#phone').val().trim();
    const passwordfield = $('#password-field').val().trim();
    const email = $('#Email').val().trim();
    const fullName = $('#fullName').val().trim();
    const confirmpasswordfield = $('#confirm-password-field').val().trim();
    var birthdayStr = $('#Birthday').val().trim();
    const allFields = [
        'phone', 'password-field', 'Email', 'fullName', 'confirm-password-field'
    ];

    const isAllEmpty = allFields.every(id => $(`#${id}`).val().trim() === "");
    var birthday = new Date(birthdayStr);
    if (isAllEmpty) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Vui lòng nhập đầy đủ');
        return false;
    }

    let isValid = true;


    const phonePattern = /^0[0-9]{9}$/; // Số điện thoại phải bắt đầu bằng 0 và có 11 ký tự
    if (!phone || !phonePattern.test(phone)) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Số điện thoại phải bắt đầu bằng 0 và có 11 ký tự');
        isValid = false;
    }
    if (isNaN(birthday.getTime())) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Vui lòng chọn ngày sinh');
        isValid = false;
        isValid = false;
    }


    const passwordPattern = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    if (!passwordfield || !passwordPattern.test(passwordfield)) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ cái, số và ký tự đặc biệt');
        isValid = false;
    }


    const emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    if (!email || !emailPattern.test(email)) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Email không hợp lệ');
        isValid = false;
    }


    if (!fullName) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Tên không để trống');
        isValid = false;
    }


    if (!confirmpasswordfield) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Mật khẩu xác nhận không để trống');
        isValid = false;
    }


    if (passwordfield !== confirmpasswordfield) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Mật khẩu và mật khẩu xác nhận phải giống nhau');
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



