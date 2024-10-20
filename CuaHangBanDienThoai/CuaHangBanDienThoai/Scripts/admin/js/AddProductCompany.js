$(document).ready(function () {
    $('#myFormProduct').submit(function (event) {
        event.preventDefault();


        let isValid = validateForm();
        
        // Gửi dữ liệu form nếu hợp lệ
        if (isValid) {
            for (var instance in CKEDITOR.instances) {
                CKEDITOR.instances[instance].updateElement();
            }
            const formData = $('#myFormProduct').serialize();
            const token = $('input[name="__RequestVerificationToken"]').val();
      

            $.ajax({
                url: '/Admin/ProductCompany/Add',
                type: 'POST',
                data: formData,
                success: function (res) {
                    OnSuccessCO(res);
                },
                error: function (xhr) {
                    Swal.fire({
                        icon: "error",
                        title: "Lỗi",
                        text: "Hệ thống gặp lỗi thêm sản phẩm.",
                    });
                    console.error(xhr.responseText);
                }
            });
        } else {
            console.log("Form validation failed");
        }
    });

});


// Hàm kiểm tra tính hợp lệ của form
function validateForm() {
    const Title = $('#Title').val().trim();
   
    const image = $('#tCurrentId').val().trim();
   

    let isValid = true;

    if (!Title) {
        showError('Title', "Vui lòng điền đầy đủ thông tin.");
        isValid = false;
    } else {
        removeError('Title');
    }

    

    if (!image) {
        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: "Vui lòng chọn ảnh sản phẩm.",
        });
        isValid = false;
    }

    return isValid;
}

// Hàm hiển thị lỗi
function showError(elements, message) {
    if (Array.isArray(elements)) {
        elements.forEach(id => {
            document.getElementById(id).classList.add('error-border');
        });
    } else {
        document.getElementById(elements).classList.add('error-border');
    }

    Swal.fire({
        icon: "error",
        title: "Lỗi",
        text: message,
    });
}

// Hàm xóa lỗi
function removeError(elements) {
    if (Array.isArray(elements)) {
        elements.forEach(id => {
            document.getElementById(id).classList.remove('error-border');
        });
    } else {
        document.getElementById(elements).classList.remove('error-border');
    }
}

// Hàm xử lý thành công
function OnSuccessCO(res) {
    if (res.Success) {
        if (res.Code === 1) {
            Swal.fire({
                icon: "success",
                title: "Thêm hãng sản phẩm thành công",
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });

            setTimeout(() => window.location.href = "/hang-san-pham", 1500);
        } else {
            // Xử lý các mã phản hồi khác nếu cần
        }
    } else {
        let errorMessage = "Có lỗi trong quá trình";
        switch (res.Code) {
            case -2:
                errorMessage = res.msg;
                break;
            
            case -100:
                errorMessage = "Lỗi hệ thống...";
                break;
        }

        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: errorMessage,
        });
    }
}