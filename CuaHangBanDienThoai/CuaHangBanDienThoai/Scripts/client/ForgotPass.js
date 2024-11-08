$(document).ready(function () {

    $('#UpdateForgotPassForm').submit(function (event) {
        event.preventDefault();
        let isValid = validateForm();

        const formData = $('#UpdateForgotPassForm').serialize();
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log("UpdateForgotPassForm", formData)
        if (isValid) {
            let counter = 5;
            let counterurl = 5;
            console.log("UpPassForget")
            $.ajax({
                url: '/Account/UpPassForget',
                type: 'POST',
                data: formData,
                success: function (res) {

                    if (res.Success) {
                        if (res.Code = 1) {
                            createToast('success', 'fa-solid fa-circle-check', 'Thành công', res.msg);
                            if (res.Url) {
                                setTimeout(() => {
                                    window.location.href = '/dang-nhap';
                                }, 1500);
                            }

                        }

                    } else {
                        if (res.Code == -2) {
                            createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', res.msg);
                        } else if (res.Code == -3) {
                            createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', res.msg);

                           
                            let interval = setInterval(() => {
                                createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', res.msg + `Chuyển trang trong ${counter} giây`);
                                counter--; 
                              
                                if (counter < 0) {
                                    clearInterval(interval); 
                                    window.location.href = res.Url; 
                                }
                            }, 1500);
                        }
                        else if (res.code == -99) {
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


function validateForm() {
    
    const passwordfield = $('#password-field').val().trim();
 
    const code = $('#code-field').val().trim();
    const confirmpasswordfield = $('#confirm-password-field').val().trim();

    const allFields = [
        'code-field', 'password-field', 'confirm-password-field'
    ];

    const isAllEmpty = allFields.every(id => $(`#${id}`).val().trim() === "");
   
    if (isAllEmpty) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Vui lòng nhập đầy đủ');
        return false;
    }

    let isValid = true;
    const passwordPattern = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    if (!passwordfield || !passwordPattern.test(passwordfield)) {
        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', 'Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ cái, số và ký tự đặc biệt');
        isValid = false;
    }


   

    if (!code) {
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