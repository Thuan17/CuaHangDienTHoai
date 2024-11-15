



$(document).ready(function () {
    $('.auto').autoNumeric('init');
    $('#demoWage').bind('blur focusout keypress keyup', function () {
        var demoGet = $('#demoWage').autoNumeric('get');
        $('#Wage').val(demoGet);
        $('#Wage').autoNumeric('set', demoGet);
    });
    function checkFunction() {
        var functionId = $('#functionChose option:selected').text().trim();

        // Ghi log giá trị functionId để kiểm tra
        console.log('Selected functionId:', functionId);

        if (functionId === "Quản trị viên" || functionId === "Quản lý") {
            console.log('Hiding manager field and expanding Function field');
            $('#managerField').parent().hide();
            $('#manager').removeClass('col-md-6');
            $('#GrFunction').removeClass('col-lg-6').addClass('col-12');
        } else {
            console.log('Showing manager field and resizing Function field');
            $('#GrFunction').removeClass('col-12').addClass('col-lg-6');
            $('#managerField').parent().show();
            $('#manager').addClass('col-md-6');
        }
    }

    // Gọi checkFunction() ngay khi trang tải
    checkFunction();

    // Thêm sự kiện change để xử lý khi chọn mới
    $('#functionChose').change(function () {
        console.log('Dropdown value changed');
        checkFunction();
    });
   

    $('#myFormAddEmp').submit(function (e) {
        e.preventDefault();


        let isValid = validateForm();
        let manager = true;
        var NameEmployee = $('#NameEmployee').val().trim();
        var functionId = $('#functionChose option:selected').text().trim();
        if (functionId !== "Quản trị viên" && functionId !== "Quản lý") {
            var managerId = $('#managerField').val();
            if (!managerId || managerId === "-Chọn người quản lý-") {
                showError('managerField', "Vui lòng chọn người quản lý  " + NameEmployee.trim());
                $('#managerField').addClass('error-border');
                isValid = false;
            } else {
                $('#managerField').removeClass('error-border');
            }
        }

       

        if (isValid && manager) {
            //for (var instance in CKEDITOR.instances) {
            //    CKEDITOR.instances[instance].updateElement();
            //}
            //const formData = $('#myFormAddEmp').serialize();
            //const token = $('input[name="__RequestVerificationToken"]').val();


            var form = document.getElementById('myFormAddEmp');
            var formData = new FormData(form);
          
            if (formData) {
                $.ajax({
                    url: '/Admin/Employee/Add',
                    type: 'Post', 
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.Success) {
                            if (res.Code = 1) {
                                const Toast = Swal.mixin({
                                    toast: true,
                                    position: "top-end",
                                    showConfirmButton: false,
                                    timer: 3000,
                                    timerProgressBar: true,
                                    didOpen: (toast) => {
                                        toast.onmouseenter = Swal.stopTimer;
                                        toast.onmouseleave = Swal.resumeTimer;
                                    }
                                });

                                Toast.fire({
                                    icon: "success",
                                    title: res.msg,
                                });

                                function SetTime() {
                                    setTimeout(function () {
                                        location.reload();

                                    }, 3000);
                                }
                                SetTime();
                               
                            }
                        } else {
                            if (res.Code == -2) {
                                Swal.fire({
                                    icon: "warning",
                                    title: "Chú ý",
                                    text: res.msg,
                                });
                            } else if (res.Code == -3) {
                                Swal.fire({
                                    icon: "warning",
                                    title: "Chú ý",
                                    text: res.msg
                                });
                                function SetTime() {
                                    setTimeout(function () {
                                        indow.location.href = "/Admin/Account/LogOut";

                                    }, 3000);
                                }
                                SetTime();
                            }

                            else if (res.Code == -99) {
                                Swal.fire({
                                    icon: "warning",
                                    title: "Không thành công...",
                                    text: "Lỗi trong quá trình thêm",
                                });
                            }
                        }
                    },
                });
            }
        }
    });


   
});
function previewImage(event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('newImagePreview');
        var lbImagePreview = document.getElementById('lbImagePreview');
        var defaultImage = document.getElementById('defaultImage');

        output.src = reader.result;
        output.style.display = 'block';  
        defaultImage.style.display = 'none';  
        if (lbImagePreview) {
            lbImagePreview.style.display = 'block';  
            defaultImage.style.display = 'none';  
        }
    };
    reader.readAsDataURL(event.target.files[0]);
    output.val() = reader;
}

function updateHiddenBirthday() {
    const birthdayInput = document.getElementById("birthdayInput").value;
    document.getElementById("Birthday").value = birthdayInput;
}
function validateForm() {
    var NameEmployee = $('#NameEmployee').val().trim();
    var PhoneNumber = $('#PhoneNumber').val().trim();
    var CCCD = $('#CCCD').val().trim();
  
    var Email = $('#Email').val().trim();
    var diachi = $('#Location').val().trim();
   
    var Sex = $('#Sex').val().trim();
    var birthdayDate = new Date(birthdayInput);
    const Wage = parseInt($('#demoWage').val().replace(/\D/g, ''), 10);


 /*   var functionId = $('#FunctionId').val();*/
    var functionId = $('#functionChose').val();
    var startDateStr = $('#birthdayInput').val().trim();
  
    var currentDate = new Date().getFullYear();



    let isValid = true;

  

    const allFields = ['NameEmployee', 'PhoneNumber', 'CCCD', 'birthdayInput', 'Email', 'Location', 'Sex','demoWage'];
    const isAllEmpty = allFields.every(id => $(`#${id}`).val().trim() === "");
   
    if (isAllEmpty) {
      
        allFields.forEach(id => {
            $(`#${id}`).addClass('error-border');
        });
        showError(allFields, "Vui lòng điền đầy đủ thông tin ");
        return false;
    }
    if (!NameEmployee) {
        showError('NameEmployee', "Tên nhân viên không được để trống.");
        $('#NameEmployee').addClass('error-border');
        isValid = false;
    } else {
        $('#NameEmployee').removeClass('error-border');
    }

    if (isNaN(Wage) || Wage === 0) {
        showError('demoWage', "Vui lòng nhập lương nhân viên.");
        $('#demoWage').addClass('error-border');
        isValid = false;
    } else {
        $('#demoWage').removeClass('error-border');
    }

    const phonePattern = /^0[0-9]{9}$/; // Số điện thoại phải bắt đầu bằng 0 và có 11 ký tự
    if (!PhoneNumber || !phonePattern.test(PhoneNumber)) {
      
        showError('PhoneNumber', "Số điện thoại phải bắt đầu bằng 0 và có 11 ký tự.");
        $('#PhoneNumber').addClass('error-border');
        isValid = false;
    } else {
        $('#PhoneNumber').removeClass('error-border');
    }
  

    if (!Email) {
        showError('Email', "Email nhân viên không được để trống.");
        $('#Email').addClass('error-border');
        isValid = false;
    } else {
        $('#Email').removeClass('error-border');
    }

    if (!diachi) {
        showError('Location', "Địa chỉ  nhân viên không được để trống.");
        $('#Location').addClass('error-border');
        isValid = false;
    } else {
        $('#Location').removeClass('error-border');
    }
    if (!CCCD) {
        showError('CCCD', "Số căn cước công dân không để trống.");
        $('#CCCD').addClass('error-border');
        isValid = false;
    } else if (CCCD.length !==11) {
        showError('CCCD', "Sai định dạng số căn cước công dân .");
        $('#CCCD').addClass('error-border');
        isValid = false;
    } else {
        $('#CCCD').removeClass('error-border');
    }
    if (!Sex ) {
        showError('Sex', "Địa chỉ  nhân viên không được để trống.");
        $('#Sex').addClass('error-border');
        isValid = false;
    } else {
        $('#Sex').removeClass('error-border');
    }


    if (!startDateStr) {
        showError('birthdayInput', "Vui lòng chọn ngày sinh.");
        $('#birthdayInput').addClass('error-border');
        isValid = false;
    } else {
        var startDate = new Date(startDateStr);
        if (currentDate - startDate.getFullYear() < 18) {
            showError('birthdayInput', "Ngày sinh không hợp lệ, yêu cầu trên 18 tuổi.");
            $('#birthdayInput').addClass('error-border');
            isValid = false;
        } else {
            $('#birthdayInput').removeClass('error-border');
        }
    }

    if (!functionId || functionId === "-Chọn danh mục chức năng-") {
        showError('FunctionId', "Vui lòng chọn chức vụ.");
        $('#FunctionId').addClass('error-border');
        isValid = false;
    } else {
        $('#FunctionId').removeClass('error-border');
    }
   
 


    if (!Sex) {
        showError('Sex', "Vui lòng chọn giới tính.");
        $('#Sex').addClass('error-border');
        isValid = false;
    } else {
        $('#Sex').removeClass('error-border');
    }

    var luong = $('#demoWage').autoNumeric('get');
    if (parseFloat(luong) >= 5000000.00) {
        showError('demoWage', "Lương không quá 5 triệu.");
        $('#demoWage').addClass('error-border');
        isValid = false;
    } else {
        $('#demoWage').removeClass('error-border');
    }


    return isValid;
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
