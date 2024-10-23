

document.addEventListener('DOMContentLoaded', function () {
    var selectedPin = '@ViewBag.SelectedPin';
    var pinDropdown = document.getElementById('Pin');
    if (pinDropdown) {
        pinDropdown.value = selectedPin;
    }
});



$(document).ready(function () {
    // Khởi tạo CKEditor
    CKEDITOR.replace('txtContent', {
        customConfig: '/content/ckeditor/config.js',
        extraAllowedContent: 'span',
        allowedContent: true,
    });

    // Cập nhật sự hiển thị của các phần tử dựa trên danh mục sản phẩm
   
 
    // Xử lý sự kiện submit form
    $('#editProduct').submit(function (event)
    {
        event.preventDefault();

        let isValid = validateForm();
        Object.keys(CKEDITOR.instances).forEach(function (instance) {
            var editor = CKEDITOR.instances[instance];
            if (editor && typeof editor.updateElement === 'function') {
                try {
                    editor.updateElement();
                } catch (error) {
                    console.error('Error updating CKEditor instance:', instance, error);
                }
            } else {
                console.warn('Instance not valid or updateElement method missing:', instance);
            }
        });

        // Serialize form data
        const formData = new FormData($('#editProduct')[0]);
        const token = $('input[name="__RequestVerificationToken"]').val();

        if (isValid) {
            // Gọi Swal.fire để hỏi xác nhận trước khi thực hiện AJAX
            Swal.fire({
                title: "Lưu thay đổi",
                text: "Bạn có chắc chắn muốn lưu thay đổi không?", // Thay đổi thông báo ở đây
                icon: "question",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Thay đổi"
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Admin/Product/Edit',
                        type: 'POST',
                        data: formData,
                        contentType: false,
                        processData: false,
                        headers: {
                            'X-CSRF-TOKEN': token
                        },
                        success: function (res) {
                            handleFormSubmissionSuccess(res);
                        },
                        error: function (xhr) {
                            handleFormSubmissionError(xhr);
                        }
                    });
                } else {
                    Swal.fire({
                        icon: "warning",
                        title: "Chú ý",
                        text: "Đã huỷ yêu cầu cập nhập mới !",
                    });
                }
            });
        }
       
    });

  
    function validateForm() {
        const Title = $('#Title').val().trim();
        const ProductCategoryId = $('#ProductCategoryId').val().trim();
        const ProductCompanyId = $('#ProductCompanyId').val().trim();
       
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
            'Title',
            'CPU', 'CPUspeed', 'OperatingSystem', 'GPUspeed',
            'MobileNetwork', 'Sim', 'Wifi', 'Bluetooth',
            'BatteryType', 'ChargingSupport', 'BatteryTechnology',
            'GPU', 'BatteryCapacity', 'Connector', 'Screensize'
        ];

        const isAllEmpty = allFields.every(id => $(`#${id}`).val().trim() === "");

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

      

        return isValid;
    }

    function displayError(elements, message) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                $('#' + id).addClass('error-border');
            });
        } else {
            $('#' + elements).addClass('error-border');
        }

        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: message,
        });
    }

    function handleFormSubmissionSuccess(res) {

        let errorMessage = null; 
        if (res.Success) {
            Swal.fire({
                icon: "success",
                title: "Sửa sản phẩm thành công",
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });
            setTimeout(() => window.location.href = "/san-pham", 1500);
           
        } else {
            switch (res.Code) {
                case -1:
                   
                    Swal.fire({
                        title: res.msg,
                      
                        icon: "warning",
                        showCancelButton: true,
                        confirmButtonColor: "#3085d6",
                        cancelButtonColor: "#d33",
                        confirmButtonText: "Về trang"
                    }).then((result) => {
                        if (result.isConfirmed) {
                         

                            Swal.fire({
                                icon: "success",
                                title: "Đang chuyển trang",
                                toast: true,
                                position: "top-end",
                                showConfirmButton: false,
                                timer: 1500,
                                timerProgressBar: true,
                            });
                            setTimeout(() => window.location.href = "/san-pham", 1500);
                        }
                    });


                    break;
                case -2:
                    errorMessage = res.msg;
                    break;
                case -7:
                    errorMessage = res.msg;
                    setTimeout(() => window.location.href = "/hethongnhanvien", 1500);
                    break;
              
                case -100:
                    Swal.fire({
                        icon: "warning",
                        title: "Lỗi",
                        text: res.msg,
                    });
                    break;
            }
            if (errorMessage != null) {
                Swal.fire({
                    icon: "warning",
                    title: "Chú ý",
                    text: errorMessage,
                });
            }
            
        }
    }

    function handleFormSubmissionError(xhr) {
        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: "Hệ thống gặp lỗi sửa sản phẩm.",
        });
        console.error(xhr.responseText);
    }
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
  
});

