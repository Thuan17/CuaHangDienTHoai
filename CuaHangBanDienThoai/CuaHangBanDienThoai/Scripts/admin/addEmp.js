$(document).ready(function () {
    $('.auto').autoNumeric('init');
    $('#demoWage').bind('blur focusout keypress keyup', function () {
        var demoGet = $('#demoWage').autoNumeric('get');
        $('#Wage').val(demoGet);
        $('#Wage').autoNumeric('set', demoGet);
    });
   
});


function validateForm() {
    var NameEmployee = $('#NameEmployee').val().trim();
    var PhoneNumber = $('#PhoneNumber').val().trim();
    var CCCD = $('#CCCD').val().trim();
    var birthdayInput = $('#birthdayInput').val().trim();
    var Email = $('#Email').val().trim();
    var diachi = $('#Location').val().trim();
    var Sex = $('#Sex').val().trim();
    var birthdayDate = new Date(birthdayInput);
    const Wage = parseInt($('#demoWage').val().replace(/\D/g, ''), 10);
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
    if (!birthdayInput) {
        showError('birthdayInput', 'fa-solid fa-triangle-exclamation', 'Chú ý', "Vui lòng chọn ngày sinh");
        $('#birthdayInput').addClass('error-border');
        isValid = false;
      
    } else if (isNaN(birthdayDate.getTime())) {
        showError('birthdayInput', 'fa-solid fa-triangle-exclamation', 'Chú ý', "Ngày sinh không hợp lệ");

        $('#birthdayInput').addClass('error-border');
        isValid = false;

    } else if (new Date().getFullYear() - birthdayDate.getFullYear() < 18) {
        showError('warning', 'fa-solid fa-triangle-exclamation', 'Chú ý', "Chưa đủ 18 tuổi");
        isValid = false;
    } else {
        $('#birthdayInput').removeClass('error-border');
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
    } else if (CCCD.lenght > 11) {
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
