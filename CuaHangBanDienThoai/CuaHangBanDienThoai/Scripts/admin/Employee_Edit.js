
function NewImage(event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('newImagePreview');
        var defaultImage = document.getElementById('defaultImage');

        // Cập nhật ảnh mới và hiển thị
        output.src = reader.result;
        output.style.display = 'block';
        defaultImage.style.display = 'none';
    };
    reader.readAsDataURL(event.target.files[0]);
}



function toggleCheckbox(event) {
    event.preventDefault();
    const checkbox = document.getElementById('toggleIsLock');
    const hiddenInput = document.getElementById('IsLock');
    checkbox.checked = !checkbox.checked;
    hiddenInput.value = checkbox.checked ? "true" : "false";
}

function checkFunction() {
    var functionId = $('#functionChose option:selected').text().trim();
    if (functionId === "Quản trị viên" || functionId === "Quản lý") {
        $('#manager').hide();  
        $('#GrFunction').removeClass('col-sm-6').addClass('col-12'); 
        $('#ManagerId').val(null); 
        var selectedManagerId = $('#ManagerId').val();
        $('#ManagerId').val(selectedManagerId);
    } else {
        $('#manager').show();  
        $('#GrFunction').removeClass('col-12').addClass('col-sm-6'); 
    }
}

$(document).ready(function () {


    checkFunction();
    $('#managerField').change(function () {
        var selectedManagerId = $(this).val();
        $('#ManagerId').val(selectedManagerId);
        console.log("selectedManagerId", selectedManagerId);
    });
    $('#functionChose').change(function () {
        checkFunction();  
    });

    $('.auto').autoNumeric('init');
    $('#demoWage').bind('blur focusout keypress keyup', function () {
        var demoGet = $('#demoWage').autoNumeric('get');
        $('#Wage').val(demoGet);
        $('#Wage').autoNumeric('set', demoGet);
    });
  
    $('#saveEmployee').on('click', function (e) {
        e.preventDefault();
        var originalText = $(this).html();
        let isValid = validateForm();
        let manager = true;
        var employeeid = $(this).data('id');
        var NameEmployee = $('#NameEmployee').val().trim();
        var funId = $('#functionChose ');
        console.log("funId", funId);
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


        if (isValid) {
            var $button = $(this); // Gán nút vào biến để dùng trong các callback
            $button.prop('disabled', true);
            $button.find('.loading-image').show();
            $button.find('.button-text').hide();



            var form = document.getElementById('EditEmployee');
            var formData = new FormData(form);
            if (formData) {
                $.ajax({
                    url: '/Admin/Employee/Edit',
                    type: 'Post',
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.Success) {
                            if (res.Code = 1) {
                                createToast('success', 'fa-solid fa-circle-check', 'Thành công', res.msg)

                                function SetTime() {
                                    setTimeout(function () {
                                        location.reload();

                                    }, 5000);
                                }
                                SetTime();

                            }
                        } else {
                            $button.prop('disabled', false);
                            $button.find('.loading-image').hide();
                            $button.find('.button-text').show();
                            if (res.Code == -99) {

                                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', res.msg);
                            } else {

                                createToast('warning', 'fa-solid fa-circle-exclamation', 'Chú ý', res.msg);
                            }
                        }
                    }, error: function () {

                        $button.prop('disabled', false);
                        $button.find('.loading-image').hide();
                        $button.find('.button-text').show();

                        createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Đã xảy ra lỗi trong quá trình gửi dữ liệu.');
                    }
                });
            }
          
        }
    });


   
});






function validateForm() {
    var NameEmployee = $('#NameEmployee').val().trim();
    var PhoneNumber = $('#PhoneNumber').val().trim();
    var CCCD = $('#CCCD').val().trim();

    var Email = $('#Email').val().trim();
    var diachi = $('#Location').val().trim();
    var Sex = $('#Sex').val().trim();
    var birthdayDate = new Date(birthdayInput);
    const Wage = parseInt($('#demoWage').val().replace(/\D/g, ''), 10);
    var functionId = $('#functionChose').val();
    var startDateStr = $('#birthdayInput').val().trim();
    var currentDate = new Date().getFullYear();

    let isValid = true;
    const allFields = ['NameEmployee', 'PhoneNumber', 'CCCD', 'birthdayInput', 'Email', 'Location', 'Sex', 'demoWage'];
    const isAllEmpty = allFields.every(id => $(`#${id}`).val().trim() === "");

    if (isAllEmpty) {

        allFields.forEach(id => {
            $(`#${id}`).addClass('error-border');
        });
        showError('allFields', 'Vui lòng điền đầy đủ thông tin ');

        return false;
    } else {
        removeError('allFields');
    }




    if (!NameEmployee) {
        showError('NameEmployee', "Tên nhân viên không được để trống.");

        isValid = false;
    } else {
        removeError('#NameEmployee');
    }



    if (isNaN(Wage) || Wage === 0) {
        showError('demoWage', "Vui lòng nhập lương nhân viên.");

        isValid = false;
    } else {
        removeError('#demoWage');

    }

    const phonePattern = /^0[0-9]{9}$/; // Số điện thoại phải bắt đầu bằng 0 và có 11 ký tự
    if (!PhoneNumber || !phonePattern.test(PhoneNumber)) {

        showError('PhoneNumber', "Số điện thoại phải bắt đầu bằng 0 và có 11 ký tự.");

        isValid = false;
    } else {
        removeError('#PhoneNumber');
    }


    if (!Email) {
        showError('Email', "Email nhân viên không được để trống.");

        isValid = false;
    } else {
        removeError('#Email');
    }

    if (!diachi) {
        showError('Location', "Địa chỉ  nhân viên không được để trống.");
        isValid = false;
    } else {
        removeError('#Location');

    }
    if (!CCCD) {
        showError('CCCD', "Số căn cước công dân không để trống.");

        isValid = false;
    } else if (CCCD.length !== 11) {
        showError('CCCD', "Sai định dạng số căn cước công dân .");
        isValid = false;
    } else {
        removeError('#CCCD');

    }
    if (!Sex) {
        showError('Sex', "Địa chỉ  nhân viên không được để trống.");
        isValid = false;
    } else {
        removeError('#Sex');
    }


    if (!startDateStr) {
        showError('birthdayInput', "Vui lòng chọn ngày sinh.");
        isValid = false;
    } else {
        var startDate = new Date(startDateStr);
        if (currentDate - startDate.getFullYear() < 18) {
            showError('birthdayInput', "Ngày sinh không hợp lệ, yêu cầu trên 18 tuổi.");
            isValid = false;
        } else {
            removeError('#birthdayInput');
        }
    }

    if (!functionId || functionId === "-Chọn danh mục chức năng-") {
        showError('FunctionId', "Vui lòng chọn chức vụ.");
        isValid = false;
    } else {
        removeError('#FunctionId');
    }


    if (!Sex) {
        showError('Sex', "Vui lòng chọn giới tính.");
        isValid = false;
    } else {
        removeError('#Sex');
    }

    var luong = $('#demoWage').autoNumeric('get');
    if (parseFloat(luong) >= 5000000.00) {
        showError('demoWage', "Lương không quá 5 triệu.");
        isValid = false;
    } else {
        removeError('#demoWage');
    }


    return isValid;
}

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
function removeError(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.classList.remove('error-border');
    }
}

function showError(elementId, errorMessage) {
    const element = document.getElementById(elementId);
    if (element) {
        element.classList.add('error-border');
    }
    createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', errorMessage);
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