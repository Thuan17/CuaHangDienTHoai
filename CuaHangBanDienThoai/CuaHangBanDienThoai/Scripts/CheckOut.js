
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
    $('.inputQuantity').on('keydown input paste', function (e) {
        e.preventDefault();
    });

    $("#ddlProvinces").change(function () {
        var idProvinces = $(this).val();
        var selectedText = $("#ddlProvinces option:selected").text();

        $(".NameProvince").val(selectedText);
        console.log("Selected Province:", selectedText);
        var ddlDistricts = $("#ddlDistricts");
        var ddlWards = $("#ddlWards");
        $(".NameProvince").val(selectedText);
        console.log("Selected Province:", selectedText);
        ddlDistricts.html('<option value="">-Chọn danh mục Quận/Huyện-</option>');
        ddlWards.html('<option value="">Chọn danh mục Phường/Xã</option>');
        if (!idProvinces || idProvinces == 0 || selectedText === "Chọn danh mục Tỉnh/Thành phố") {
            ddlDistricts.html('<option value="">-Chọn danh mục Quận/Huyện-</option>');
            ddlWards.html('<option value="">Chọn danh mục Phường/Xã</option>');
            ddlWards[0].selectedIndex = 0;
            return;
        }


        $.getJSON('/ProvincesVN/GetDistrictsByProvince', { idProvinces: idProvinces }, function (districts) {
            var items = '<option value="">-Chọn danh mục Quận/Huyện-</option>';
            $.each(districts, function (i, district) {
                items += "<option value='" + district.idDistricts + "'>" + district.name + "</option>";
            });
            $("#ddlDistricts").html(items);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error("Error loading districts:", textStatus, errorThrown);
            console.error("Response from server:", jqXHR.responseText);
        });
    });

    $("#ddlDistricts").change(function () {
        var idDistricts = $(this).val();
        var selectedText = $("#ddlDistricts option:selected").text();
        $(".NameDistrict").val(selectedText);
        console.log("Selected District:", selectedText);

        $.getJSON('/ProvincesVN/GetWardsByDistrict', { idDistricts: idDistricts }, function (wards) {
            var items = '<option value="">-Chọn danh mục Phường/Xã-</option>';
            $.each(wards, function (i, ward) {
                items += "<option value='" + ward.idWards + "'>" + ward.name + "</option>";
            });
            $("#ddlWards").html(items);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error("Error loading wards:", textStatus, errorThrown);
            console.error("Response from server:", jqXHR.responseText);
        });
    });

    $("#ddlWards").change(function () {
        var idWards = $(this).val();
        var selectedText = $("#ddlWards option:selected").text();
        $(".NameWard").val(selectedText);
    });
});


$(document).ready(function () {

    var PriceShip = $('.PriceShip');


    $('.expressitem').on('click', function () {
        var checkbox = $(this).find('input[type="checkbox"]');
        var isChecked = checkbox.prop('checked');
        checkbox.prop('checked', !isChecked);
        const groupName = checkbox.attr('name');
        $(`input[name='${groupName}']`).not(checkbox).prop('checked', false);
        $(`input[name='${groupName}']`).closest('.expressitem').removeClass('highlight');
        if (!isChecked) {
            $(this).addClass('highlight');
            PriceShip.removeClass('hide');
        } else {
            $(this).removeClass('highlight');
            PriceShip.addClass('hide');
        }
    });

    function showErrorAddress(elements, message) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                const element = document.getElementById(id);
                if (element) {
                    const parent = element.closest('.custom__select');
                    if (parent) {
                        parent.classList.add('error');
                    }

                    const label = document.querySelector(`label[for='${id}']`);
                    if (label) {
                        label.classList.add('error');
                    }
                }
            });
        } else {
            const element = document.getElementById(elements);
            if (element) {
                const parent = element.closest('.custom__select');
                if (parent) {
                    parent.classList.add('error');
                }

                const label = document.querySelector(`label[for='${elements}']`);
                if (label) {
                    label.classList.add('error');
                }
            }
        }

        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: message,
        });
    }

    function removeErrorAddress(elements) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                const element = document.getElementById(id);
                if (element) {
                    const parent = element.closest('.custom__select');
                    if (parent) {
                        parent.classList.remove('error');
                    }

                    const label = document.querySelector(`label[for='${id}']`);
                    if (label) {
                        label.classList.remove('error');
                    }
                }
            });
        } else {
            const element = document.getElementById(elements);
            if (element) {
                const parent = element.closest('.custom__select');
                if (parent) {
                    parent.classList.remove('error');
                }

                const label = document.querySelector(`label[for='${elements}']`);
                if (label) {
                    label.classList.remove('error');
                }
            }
        }
    }

    function showError(elements, message) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                const element = document.getElementById(id);
                if (element) {
                    element.classList.add('error-border');
                    const parent = element.closest('.input-data');
                    if (parent) {
                        parent.classList.add('error');
                    }
                }
            });
        } else {
            const element = document.getElementById(elements);
            if (element) {
                element.classList.add('error-border');
                const parent = element.closest('.input-data');
                if (parent) {
                    parent.classList.add('error');
                }
            }
        }

        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: message,
        });
    }

    function removeError(elements) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                const element = document.getElementById(id);
                if (element) {
                    element.classList.remove('error-border');
                    const parent = element.closest('.input-data');
                    if (parent) {
                        parent.classList.remove('error');
                    }
                }
            });
        } else {
            const element = document.getElementById(elements);
            if (element) {
                element.classList.remove('error-border');
                const parent = element.closest('.input-data');
                if (parent) {
                    parent.classList.remove('error');
                }
            }
        }
    }

    function validateForm() {
        const NameCustomer = $('#NameCustomer').val().trim();
        const PhoneCustomer = $('#PhoneCustomer').val().trim();
        const Address = $('#Location').val().trim();
        const Email = $('#Email').val().trim();
        const Note = $('#Note').val().trim();
        const ddlProvinces = $('#ddlProvinces').val().trim();
        const ddlDistricts = $('#ddlDistricts').val().trim();
        const ddlWards = $('#ddlWards').val().trim();

        let isValid = true;
        if (!ddlProvinces || ddlProvinces === "Chọn danh mục Tỉnh/Thành phố") {
            showErrorAddress('ddlProvinces', "Vui lòng chọn Tỉnh/Thành phố.");
            isValid = false;
        } else {
            removeErrorAddress('ddlProvinces');
        }

        if (!ddlDistricts || ddlDistricts === "Chọn danh mục Quận/Huyện") {
            showErrorAddress('ddlDistricts', "Vui lòng chọn Quận/Huyện.");
            isValid = false;
        } else {
            removeErrorAddress('ddlDistricts');
        }

        if (!ddlWards || ddlWards === "Chọn danh mục Phường/Xã") {
            showErrorAddress('ddlWards', "Vui lòng chọn Phường/Xã.");
            isValid = false;
        } else {
            removeErrorAddress('ddlWards');
        }

        if (!NameCustomer) {
            showError('NameCustomer', "Vui lòng điền tên.");
            isValid = false;
        } else {
            removeError('NameCustomer');
        }

        const phoneNumberRegex = /^0\d{9}$/;

        if (!PhoneCustomer) {
            showError('PhoneCustomer', "Vui lòng điền số điện thoại.");
            isValid = false;
        } else if (PhoneCustomer.length > 11) {
            showError('PhoneCustomer', "Định dạng số điện thoại không đúng.");
            isValid = false;
        } else if (!phoneNumberRegex.test(PhoneCustomer)) {
            showError('PhoneCustomer', "Định dạng số điện thoại không đúng.");
            isValid = false;
        } else {
            removeError('PhoneCustomer');
        }

        if (!Address) {
            showError('Location', "Vui lòng điền địa chỉ.");
            isValid = false;
        } else {
            removeError('Location');
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!Email) {
            showError('Email', "Vui lòng điền Email.");
            isValid = false;
        } else if (!emailRegex.test(Email)) {
            showError('Email', "Email không hợp lệ.");
            isValid = false;
        } else if (Email.length > 300) {
            showError('Email', "Email quá dài.");
            isValid = false;
        } else {
            removeError('Email');
        }

        if (Note.length > 250) {
            showError('Note', "Ghi chú quá dài.");
            isValid = false;
        } else {
            removeError('Note');
        }

        return isValid;
    }
    function OnSuccessCO(res) {
        if (res.Success) {
            if (res.Code == 1) {
                location.href = "/mua-hang-thanh-cong";

            }
            else {
                location.href = res.Url;
            }
        }
        else {
            if (res.Code == -1) {
                Swal.fire({
                    icon: "error",
                    title: "Không thể bỏ trống...",
                    text: "Vui lòng điền đầy đủ thông tin",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");


            }
            if (res.Code == -2) {
        
                var message = "Số lượng không đủ...\n";

              
                var insufficientItems = res.InsufficientItems;
                if (insufficientItems && insufficientItems.length > 0) {
                    message += "Các sản phẩm sau không đủ số lượng:\n";
                    insufficientItems.forEach(function (item) {
                        message += item.ProductName + "\n"; 
                    });
                }

             
                Swal.fire({
                    icon: "warning",
                    title: "Số lượng không đủ",
                    text: message
                });

              
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");
            }

            if (res.Code == -3) {
                Swal.fire({
                    icon: "warning",
                    title: "Lỗi hệ thống..",
                    text: "Tài khoản tạm ngừng",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");


            }
            if (res.Code == -4) {
                Swal.fire({
                    icon: "warning",
                    title: "Lỗi hệ thống..",
                    text: "Dữ liệu không hợp lệ",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");


            }
            if (res.Code == -5) {
                Swal.fire({
                    icon: "warning",
                    title: "Lỗi hệ thống..",
                    text: "Hệ thống đang bảo trì",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");


            }
            if (res.Code == -7) {
                Swal.fire({
                    icon: "warning",
                    title: "Lỗi giảm giá ..",
                    text: "Áp dụng voucher thất bại",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");

            }
            if (res.Code == -100) {
                Swal.fire({
                    icon: "warning",
                    title: "Lỗi hệ thống..",
                    text: "Mua hàng thất bại",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");


            }

        }
    }

    $('.btnCheckOutS').off('click').on('click', function (event) {
        event.preventDefault();

        let isValid = validateForm();

        if (isValid) {
            const formData = $('#checkOutForm').serialize();
            const token = $('input[name="__RequestVerificationToken"]').val();

            $('.loading-overlay').show();
            $('.sent').show();
            $('.btnCheckOutS').prop('disabled', true);
            $.ajax({
                url: '/ShoppingCart/CheckOut',
                type: 'POST',
                data: formData,
                success: function (res) {
                    console.log(res);
                    if (res.Success) {
                        if (res.Code === 1) {
                           
                            createToast('success', 'fa-solid fa-circle-exclamation', 'Thành công ', 'Mã đơn:  '+res.OrderCode);
                            setTimeout(() => {
                                window.location.href = res.Url;
                            }, 1500);
                        }
                    } else {
                        console.log('Error Code:', res.Code); 
                        if (res.Code === -1) {
                            $('.loading-overlay').hide();
                            $('.sent').hide();
                            $('.btnCheckOutS').prop('disabled', false);
                            createToast('warning', 'fa-solid fa-triangle-exclamation', 'Thất bại', res.msg);

                        } else if (res.Code === -2) {
                            $('.loading-overlay').hide();
                            $('.sent').hide();
                            $('.btnCheckOutS').prop('disabled', false);
                            if (res.InsufficientItems && res.InsufficientItems.length > 0) {
                                message = "\n";
                                res.InsufficientItems.forEach(function (item) {
                                    message += "\n"+item.ProductName + "\n";
                                });

                                Swal.fire({
                                    icon: "warning",
                                    title: "Số lượng không đủ",
                                    text: message
                                });
                            } else {
                                createToast('warring', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Số lượng kho không đủ');
                            }

                     
                       
                        } else if (res.Code === -99) {
                            createToast('warring', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Lỗi máy chủ');
                        }

                      
                    }
                },
                error: function (xhr) {
                    $('.loading-overlay').hide();
                    $('.sent').hide();
                    $('.btnCheckOutS').prop('disabled', false);

                    Swal.fire({
                        icon: "error",
                        title: "Lỗi",
                        text: "Hệ thống gặp lỗi thêm sản phẩm.",
                    });
                    console.error('Error:', xhr.responseText);
                }
            });
        } else {
            console.log("Form validation failed");
        }
    });

});
