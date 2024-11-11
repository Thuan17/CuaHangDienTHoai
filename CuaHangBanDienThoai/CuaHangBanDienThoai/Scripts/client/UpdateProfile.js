
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

    $('#UpdateForm').submit(function (event) {
        event.preventDefault();
        let isValid = validateFormUpdate();

        const formData = $('#UpdateForm').serialize();
        const token = $('input[name="__RequestVerificationToken"]').val();


        if (isValid) {

            $.ajax({
                url: '/Account/UpdateProFile',
                type: 'POST',
                data: formData,
                success: function (res) {

                    if (res.Success) {

                        createToast('success', 'fa-solid fa-circle-check', 'Thành công', res.msg);

                        setTimeout(() => {
                            window.location.reload();
                        }, 5000);

                    } else {
                        if (res.Code == -2) {
                            createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', res.msg);
                        } else if (res.Code == -99) {
                            createToast('error', 'fa-solid fa-triangle-exclamation', 'Lỗi', res.msg);
                        }
                    }


                },
                error: function (xhr) {
                    createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', res.msg);
                    console.error(xhr.responseText);
                }
            });
        }

    });
});
document.querySelector(".toggle-password").addEventListener("click", function () {
    const passwordField = document.querySelector("#password-field");
    const isPasswordHidden = passwordField.getAttribute("type") === "password";

  
    passwordField.setAttribute("type", isPasswordHidden ? "text" : "password");

  
    this.classList.toggle("fa-eye");
    this.classList.toggle("fa-eye-slash");
});

function validateFormUpdate() {
    const phone = $('#phone').val().trim();
    const passwordfield = $('#password-field').val().trim();
    const email = $('#Email').val().trim();
    const fullName = $('#fullName').val().trim();

    var birthdayStr = $('#Birthday').val().trim();
    const allFields = [
        'phone', 'password-field', 'Email', 'fullName'
    ];

    const isAllEmpty = allFields.every(id => $(`#${id}`).val().trim() === "");
    var birthday = new Date(birthdayStr);
    if (isAllEmpty) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Vui lòng nhập đầy đủ');
        return false;
    }

    let isValid = true;


    const phonePattern = /^0[0-9]{9}$/;
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




    return isValid;
}
