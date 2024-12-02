$(document).ready(function () {
    $('#myFormProduct').submit(function (event) {
        event.preventDefault();

        console.log('Form Data myFormProduct submit:');
        let isValid = validateForm();
        if (isValid) {
            for (var instance in CKEDITOR.instances) {
                CKEDITOR.instances[instance].updateElement();
            }
            const formData = $('#myFormProduct').serialize();
            const token = $('input[name="__RequestVerificationToken"]').val();
            console.log('Form Data:', formData);

            $.ajax({
                url: '/Admin/Product/Add',
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
    const ProductCategoryId = $('#ProductCategoryId').val().trim();
    const ProductCompanyId = $('#ProductCompanyId').val().trim();
    /*  const image = $('#tCurrentId').val().trim();*/
    const CPU = $('#CPU').val().trim();
    const CPUspeed = $('#CPUspeed').val().trim();
    const OperatingSystem = $('#OperatingSystem').val().trim();
    const GPUspeed = $('#GPUspeed').val().trim();
    const MobileNetwork = $('#MobileNetwork').val().trim();
    const Sim = $('#Sim').val().trim();
    const Wifi = $('#Wifi').val().trim();
    const Bluetooth = $('#Bluetooth').val().trim();
    const BatteryType = $('#BatteryType').val().trim();
    const ChargingSupport = $('#ChargingSupport').val().trim();
    const BatteryTechnology = $('#BatteryTechnology').val().trim();
    const GPU = $('#GPU').val().trim();
    const BatteryCapacity = $('#BatteryCapacity').val().trim();
    const Connector = $('#Connector').val().trim();
    const Screensize = $('#Screensize').val().trim();


    const allFields = [
        'Title', 'tCurrentId',
        'CPU', 'CPUspeed', 'OperatingSystem', 'GPUspeed',
        'MobileNetwork', 'Sim', 'Wifi', 'Bluetooth',
        'BatteryType', 'ChargingSupport', 'BatteryTechnology',
        'GPU', 'BatteryCapacity', 'Connector', 'Screensize'
    ];

    const isAllEmpty = allFields.every(id => $(`#${id}`).val().trim() === "");

    if (isAllEmpty) {
        showError(allFields, "Vui lòng điền đầy đủ thông tin.");
        return false;
    }
    let isValid = true;
    if (!Title) {
        showError('Title', "Vui lòng nhập tên sản phẩm .");
        isValid = false;
    } else {
        removeError('Title');
    }

    if (!ProductCategoryId || ProductCategoryId === "-Chọn danh mục sản phẩm-") {
        showError('ProductCategoryId', "Vui lòng chọn mục sản phẩm.");
        isValid = false;
    } else {
        removeError('ProductCategoryId');
    }
    if (!ProductCompanyId || ProductCompanyId === "-Chọn danh mục sản phẩm-") {
        showError('ProductCompanyId', "Vui lòng chọn mục sản phẩm.");
        isValid = false;
    } else {
        removeError('ProductCompanyId');
    }
    if (!CPU) {
        showError('CPU', "Vui lòng nhập Chip .");
        isValid = false;
    } else {
        removeError('CPU');
    }
    if (!Screensize) {
        showError('Screensize', "Vui lòng nhập kích cỡ màn hình .");
        isValid = false;
    } else {
        removeError('Screensize');
    }
    if (!CPUspeed) {
        showError('CPUspeed', "Tốc độ Chip.");
        isValid = false;
    } else {
        removeError('CPUspeed');
    }

    if (!OperatingSystem) {
        showError('OperatingSystem', "Vui lòng nhập hệ điều hành .");
        isValid = false;
    } else {
        removeError('OperatingSystem');
    }

    if (!GPUspeed) {
        showError('GPUspeed', " Vui lòng nhập tốc độ GPU .");
        isValid = false;
    } else {
        removeError('GPUspeed');
    }
    if (!Screensize) {
        showError('Screensize', " Vui lòng nhập kích cỡ màn hình .");
        isValid = false;
    } else {
        removeError('Screensize');
    }
    if (!Connector) {
        showError('Connector', " Vui lòng nhập cổng kết nối .");
        isValid = false;
    } else {
        removeError('Connector');
    }

    if (!MobileNetwork) {
        showError('MobileNetwork', " Vui lòng nhập mạng di động.");
        isValid = false;
    } else {
        removeError('MobileNetwork');
    }
    if (!Sim) {
        showError('Sim', " Vui lòng nhập sim.");
        isValid = false;
    } else {
        removeError('Sim');
    }
    if (!Wifi) {
        showError('Wifi', " Vui lòng nhập Wifi.");
        isValid = false;
    } else {
        removeError('Wifi');
    }




    if (!Bluetooth) {
        showError('Bluetooth', " Vui lòng nhập Bluetooth.");
        isValid = false;
    } else {
        removeError('Bluetooth');
    }



    if (!BatteryType) {
        showError('BatteryType', " Vui lòng nhập loại Pin.");
        isValid = false;
    } else {
        removeError('BatteryType');
    }

    if (!ChargingSupport) {
        showError('ChargingSupport', " Vui lòng nhập loại hỗ trợ sạc.");
        isValid = false;
    } else {
        removeError('ChargingSupport');
    }
    if (!BatteryTechnology) {
        showError('BatteryTechnology', " Vui lòng nhập cồng nghệ Pin.");
        isValid = false;
    } else {
        removeError('BatteryTechnology');
    }
    if (!GPU) {
        showError('GPU', " Vui lòng nhập công nghệ GPu.");
        isValid = false;
    } else {
        removeError('GPU');
    }
    if (!BatteryCapacity) {
        showError('BatteryCapacity', " Vui lòng nhập dung lượn Pin.");
        isValid = false;
    } else {
        removeError('BatteryCapacity');
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
        icon: "warning",
        title: "Chú ý",
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
                title: "Thêm sản phẩm thành công",
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });

            setTimeout(() => window.location.href = "/san-pham", 1500);
        } else {
            // Xử lý các mã phản hồi khác nếu cần
        }
    } else {
        let errorMessage = "Có lỗi trong quá trình";
        switch (res.Code) {
            case -2:
                errorMessage = res.msg;
                break;
            case -3:
                errorMessage = "Danh mục đã tồn tại...";
                break;
            case -5:
                errorMessage = "Chọn ảnh đại diện";
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

$(document).on('click', '.btnXoaAnh', function (e) {
    e.preventDefault();
    const id = $(this).data('id');
    $(`#trow_${id}`).remove();
    $('#tCurrentId_Product').val(0);
});

$(document).on('click', '.btnXoaAnh', function (e) {
    e.preventDefault();
    var id = $(this).data('id');
    $('#trow_' + id).remove();
    $('#tCurrentId_Product').val(0);
});