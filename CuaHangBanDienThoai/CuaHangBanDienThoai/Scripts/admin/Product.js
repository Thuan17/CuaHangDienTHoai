//start add products

$(document).ready(function () {
    $('.auto').autoNumeric('init');
    $('#demoPrice').bind('blur focusout keypress keyup', function () {
        var demoGet = $('#demoPrice').autoNumeric('get');
        $('#Price').val(demoGet);
        $('#Price').autoNumeric('set', demoGet);
    });
    $('#demoPriceSale').bind('blur focusout keypress keyup', function () {
        var demoGet = $('#demoPriceSale').autoNumeric('get');
        $('#PriceSale').val(demoGet);
        $('#PriceSale').autoNumeric('set', demoGet);
    });
    $('#demoOriginalPrice').bind('blur focusout keypress keyup', function () {
        var demoGet = $('#demoOriginalPrice').autoNumeric('get');
        $('#OriginalPrice').val(demoGet);
        $('#OriginalPrice').autoNumeric('set', demoGet);
    });

    $('body').on('click', '.btnXoaAnh', function () {
        var conf = confirm('Bạn có muốn xóa ảnh này không?');
        if (conf === true) {
            var _id = $(this).data('id');
            $('#trow_' + _id).fadeTo('fast', 0.5, function () {
                $(this).slideUp('fast', function () { $(this).remove(); });
            });
            var temp = $('#tCurrentId').val();
            var currentId = parseInt(temp) - 1;
            $('#tCurrentId').val(currentId);
        }
    });
});
$(document).ready(function () {
   



    // Hàm xử lý submit form
    $('#myFormProduct').submit(function (event) {
        event.preventDefault();


        let isValid = validateForm();
       

        // Gửi dữ liệu form nếu hợp lệ
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
    });
});

// Hàm kiểm tra tính hợp lệ của form
function validateForm() {
    const Title = $('#Title').val().trim();
    const ProductCategoryId = $('#ProductCategoryId').val().trim();
    const ProductCompanyId = $('#ProductCompanyId').val().trim();
    const image = $('#tCurrentId').val().trim();
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

    const isAllEmpty = allFields.every(id=>$(`#${id}`).val().trim()==="");

    if (isAllEmpty) {
        showError(allFields, "Vui lòng điền thông tin vào ít nhất một trường.");
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

    if (!image) {
        Swal.fire({
            icon: "warning",
            title: "Chú ý",
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
                errorMessage = "Vui lòng điền tên tiêu đề";
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

//End add products
//Start details products

//$(document).ready(function () {
//    var bg = $(".bg-sg");
//    var popup = $("#popupBill");
//    var btnCapNhatBill = $('.btnCapNhatBill');
//    var btnDetailsProducts = $('.btnDetailsProducts');
//    bg.on('click', function () {
//        bg.hide();
//        popup.hide();
//    });

//    $('.btnCloseEditBill').on('click', function () {
//        bg.hide();
//        popup.hide();
//    });



//    btnDetailsProducts.each(function () {
//        $(this).on('click', function (e) {

//            e.preventDefault();

//            bg.show();
//            popup.show();

//            var id = $(this).data('id');

//            if (id != null) {
//                $.ajax({
//                    url: '/Admin/Bill/Partial_DetailBill',
//                    type: 'GET',
//                    data: { id: id },
//                    success: function (response) {
//                        $("#loadDataBillEdit").html(response);
//                    },
//                    error: function (xhr, status, error) {
//                        console.error(xhr.responseText);
//                    }
//                });
//            }
//        });
//    });

//})