$(document).ready(function () {
    var bg = $(".bg-sg");
    var popup = $("#popupBill");
    var btnAddBill = $(".btnAddBill");
    LoadListProduct();

    $('#productChose').change(function () {
        var productId = $(this).val();
        console.log("productChose", productId);
        if (productId) {
            $('.loadding').show();
            $.ajax({
                url: '/Admin/Seller/Partail_ProductById',
                data: { productId: productId },
                type: 'GET',
                success: function (res) {
                    $('.loading').hide();
                    $('#loaddataProduct').html(res);
                },
                error: function (res) {
                    console.error("Error loading product:", res);
                }
            });
        } else {
            Partial_Product();
        }
    });


    $(document).on('click', '.btnAddBill', function (event) {
        event.preventDefault();
        var productid = $(this).data('productid');
        if (productid) {
            $.ajax({
                url: '/Admin/Seller/AddBill',
                data: {
                    productid: productid,
                    quantity: 1
                },
                type: 'POST',
                success: function (res) {
                    if (res.Success && res.Code === 1) {
                        LoadListProduct();
                        $('#CountProduct').html(res.Count);
                        createToast('success', 'fa-solid fa-circle-check', 'Thành công', res.msg);
                    } else {
                        var type = res.Code === -99 ? 'error' : 'warning';
                        createToast(type, 'fa-solid fa-circle-exclamation', 'Chú ý', res.msg);
                    }
                },
                error: function (res) {
                    console.error("Error adding bill:", res);
                }
            });
        }
    });

    $(document).on('click', '.btnDeleteItem', function (event) {
        event.preventDefault();
        var productid = $(this).data('productid');
        if (productid) {
            $.ajax({
                url: '/Admin/Seller/DeleteItemCart',
                data: { productid: productid },
                type: 'POST',
                success: function (res) {
                    if (res.Success && res.Code === 1) {
                        LoadListProduct();
                        $('#CountProduct').html(res.Count);
                        createToast('success', 'fa-solid fa-circle-check', 'Thành công', res.msg);

                    } else {
                        var type = res.Code === -99 ? 'error' : 'warning';
                        createToast(type, 'fa-solid fa-circle-exclamation', 'Thất bại', res.msg);
                    }
                },
                error: function (res) {
                    console.error("Error deleting item:", res);
                }
            });
        }
    });
    $(document).on('click', '.btnquantity', function (event) {

        let action = $(this).data('action');
        let productId = $(this).data('productid');
        let inputField = $(`input[data-productid='${productId}']`);
        let currentQuantity = parseInt(inputField.val()) || 1;
        let newQuantity = currentQuantity;

        if (action === 'increment') {
            newQuantity = currentQuantity + 1;
        } else if (action === 'decrement' && currentQuantity > 1) {
            newQuantity = currentQuantity - 1;
        }

        inputField.val(newQuantity);
        if (newQuantity && newQuantity >= 10) {
            createToast('warning', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Số lượng giới hạn 10');
            inputField.val(10);
        }
        $.ajax({
            url: '/Admin/Seller/UpdateQuantity',
            type: 'POST',
            data: { productid: productId, quantity: newQuantity },
            success: function (res) {
                if (res.Success) {
                    LoadTotalPrice();
                } else {
                    createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', res.msg);
                }
            },
            error: function () {
                createToast('error', 'fa-solid fa-circle-exclamation', 'Lỗi', 'Có lỗi xảy ra khi cập nhật số lượng.');
            }
        });
    });



    $(document).on('click', '#searchbtn', function () {
        var input = $('#searchinput').val().trim();


        if (input && input.length >= 3) {
            $.ajax({
                url: '/Admin/Seller/FindCustomer',
                type: 'POST',
                data: { input: input },
                success: function (res) {
                    $('#loadCustomer').html(res);
                },
                error: function () {
                    createToast('error', 'fa-solid fa-circle-exclamation', 'Lỗi', 'Có lỗi xảy ra khi cập nhật số lượng.');
                }
            });
        } else {
            alert("Vui lòng nhập ít nhất 3 ký tự để tìm kiếm!");
        }
    });

    $(document).on('input', '#searchinput', function () {
        var input = $(this).val().trim();
        if (input && input.length >= 3) {
            $.ajax({
                url: '/Admin/Seller/FindCustomer',
                type: 'POST',
                data: { input: input },
                success: function (res) {
                    $('#loadCustomer').html(res);
                },
                error: function () {
                    createToast('error', 'fa-solid fa-circle-exclamation', 'Lỗi', 'Có lỗi xảy ra khi cập nhật số lượng.');
                }
            });
        } else {
            $('#loadCustomer').html('');
        }
    });
    $('#clearInputBill').on('click', function () {
        document.getElementById('searchinput').value = ''; // DOM API
        $('#searchinput').val(''); // jQuery
        $('#loadCustomer').html('');
    });
    $(document).on('click', '.bg-sg', function (event) {
        bg.hide();
        popup.hide();
    });
    $(document).on('click', '.btnCloseEditBill', function (event) {
        bg.hide();
        popup.hide();
    });




    $(document).on('click', '.btnAddCusstomer', function (event) {
        event.preventDefault();

        bg.show();
        popup.show();
        $.ajax({
            url: '/Admin/Seller/Partail_AddCustomer',
            type: 'get',
            success: function (res) {
                $('#popupBill').html(res);
            }, error: function (res) {
                alert("Có lỗi khi mở thêm khách hàng");
            }
        });



    });
    $(document).on('click', '.checkOut', function (event) {
        event.preventDefault();
        var customerid = $(this).data('customerid');
        if (customerid) {
            var $button = $(this); // Gán nút vào biến để dùng trong các callback
            $button.prop('disabled', true);
            $button.find('.loading-image').show();
            $button.find('.button-text').hide();
            $.ajax({
                url: '/Admin/Seller/CheckOut',
                data: { customerid: customerid },
                type: 'POST',
                success: function (res) {
                    if (res.Success) {
                        if (res.Code == 1 && res.employeeid) {
                            window.location.href = '/Admin/Seller/DownloadWord?filePath=' + encodeURIComponent(res.filePath);

                            Swal.fire({
                                icon: 'success',
                                title: 'Xuất hóa đơn thành công',
                                text: 'Hóa đơn đã có trên hệ thống'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    LoadListProduct();
                                    document.getElementById('searchinput').value = '';
                                    $('#loadCustomer').html('');
                                    $button.prop('disabled', false);
                                    $button.find('.loading-image').hide();
                                    $button.find('.button-text').show();

                                }
                            });
                            loadBillEmployee(res.employeeid);
                        }
                    } else {
                        $button.prop('disabled', false);
                        $button.find('.loading-image').hide();
                        $button.find('.button-text').show();
                        var type = res.Code === -99 ? 'error' : 'warning';
                        createToast(type, 'fa-solid fa-circle-exclamation', 'Chú ý', res.msg);
                    }

                }, error: function (res) {
                    createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', res.msg);
                },
            });
        }
    });


   

    $(document).on('submit', '#myFormAddCustomer', function (event) {
        event.preventDefault(); // Ngăn form tự động submit
        let isValid = validateForm(); // Kiểm tra form

        if (isValid) {
            var $button = $(this).find('button[type="submit"]');
            $button.prop('disabled', true);
            $button.find('.loading-image').show();
            $button.find('.button-text').hide();

            var formData = new FormData(this);
            $.ajax({
                url: '/Admin/Seller/AddCustomer',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.Success) {
                        if (res.Code == 1 && res.PhoneNumber) {
                            $('#searchinput').val(res.PhoneNumber.trim());
                            $.ajax({
                                url: '/Admin/Seller/FindCustomer',
                                type: 'POST',
                                data: { input: res.PhoneNumber.trim() },
                                success: function (res) {
                                    $('#loadCustomer').html(res);
                                },
                                error: function () {
                                    createToast('error', 'fa-solid fa-circle-exclamation', 'Lỗi', 'Có lỗi xảy ra khi cập nhật số lượng.');
                                }
                            });
                            document.getElementById("myFormAddCustomer").reset();
                            bg.hide();
                            popup.hide();
                        }
                    } else {
                        $button.prop('disabled', false);
                        $button.find('.loading-image').hide();
                        $button.find('.button-text').show();
                        var type = res.Code === -99 ? 'error' : 'warning';
                        createToast(type, 'fa-solid fa-circle-exclamation', 'Chú ý', res.msg);
                    }
                },
                error: function () {
                    createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Có lỗi xảy ra.');
                }
            });
        }
    });


    //start tab hoá đơn
    $(document).on('change', '#DateBill', function () {
        var ngayxuat = $('#DateBill').val();
        if (ngayxuat) {

            $.ajax({
                url: '/Admin/Seller/GetBillByDate',
                type: 'GET',
                data: { ngayxuat: ngayxuat },
                beforeSend: function () {

                    $('#dataTableBody').hide();
                    $('.loadding img').show();
                },
                success: function (data) {
                    $('#loadbill').html(data);
                },
                error: function (xhr, status, error) {
                    console.log('Đã xảy ra lỗi khi gửi yêu cầu:', error);
                    $('#dataTableBody').show();
                    $('.loadding img').hide();
                }
            });
        }
    });
    $(document).on('click', '.btnclearSBill', function (event) {
        event.preventDefault();

        var employeeid = $(this).data('employeeid');
        console.log("btnclearSBill", employeeid);
        if (employeeid) {
            $.ajax({
                url: '/Admin/Seller/Partial_BillByEmployee',
                type: 'GET',
                data: { employeeid: employeeid },

                success: function (data) {
                    $('#nav-profile').html(data);
                },
                error: function (xhr, status, error) {
                    console.log('Đã xảy ra lỗi khi gửi yêu cầu:', error);
                    $('#dataTableBody').show();
                    $('.loadding img').hide();
                }
            });
        }


    });

    $(document).on('input', '#searchBill', function () {
        var search = $(this).val().trim();
        if (search && search.length > 2) {
            $.ajax({
                url: '/Admin/Seller/SearchBill',
                type: 'GET',
                data: { search: search },
                beforeSend: function () {

                    $('#dataTableBody').hide();
                    $('.loadding img').show();
                },
                success: function (data) {
                    $('#loadbill').html(data);
                },
                error: function (xhr, status, error) {
                    console.log('Đã xảy ra lỗi khi gửi yêu cầu:', error);
                    $('#dataTableBody').show();
                    $('.loadding img').hide();
                }
            });
        } else {

        }

    });

    $(document).on('click', '.btnupdatebill', function (event) {
        event.preventDefault();
        var billid = $(this).data('billid');
        var $button = $(this); // Gán nút vào biến để dùng trong các callback
        $button.prop('disabled', true);
        $button.find('.loading-image').show();
        $button.find('.button-text').hide();

        if (billid) {
            $.ajax({
                url: '/Admin/Seller/Partail_EditBill',
                data: { billid: billid },
                type: 'GET',
                success: function (res) {
                    bg.show();
                    popup.show();
                    $button.prop('disabled', false);
                    $button.find('.loading-image').hide();
                    $button.find('.button-text').show();
                    $('#popupBill').html(res);

                }, error: function () {
                    bg.hide();
                    popup.hide();
                    $button.prop('disabled', false);
                    $button.find('.loading-image').hide();
                    $button.find('.button-text').show();
                    alert("Lỗi load dữ liệu ");
                }
            });
        }
    });

    $(document).on('click', '.btnDeleteItemOrder', function () {
        var billdetailid = $(this).data('billdetailid');
        var billid = $(this).data('billid');

        if (confirm("Bạn có chắc chắn muốn xóa sản phẩm này không?")) {
            deleteItem(billid, billdetailid)
        }
    });


    $(document).on('click', '#saveBill', function () {
        var billid = $(this).data('billid');
       
        if (billid) {
            if (confirm("Bạn có chắc chắn muốn lưu thay đổi không ?")) {
                var form = $('#editBill')[0];
                var formData = new FormData(form);
                $.ajax({
                    url: '/Admin/Seller/Edit',
                    type: 'Post',
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        if (res.Success) {
                            if (res.Code == 1 && res.filePath && res.BillId) {
                                window.location.href = '/Admin/Seller/DownloadWord?filePath=' + encodeURIComponent(res.filePath);

                                bg.hide();
                                popup.hide();

                                Swal.fire({
                                    icon: 'success',
                                    title: 'Xuất hóa đơn thành công',
                                    text: 'Hóa đơn đã có trên hệ thống'
                                }).then((result) => {
                                    if (result.isConfirmed) {
                                        LoadRowBill(res.BillId);
                                    }
                                });
                            }
                        } else {
                            if (res.Code == -99) {
                                createToast('error', 'fa-solid fa-circle-exclamation', 'Chú ý', res.msg);
                            } else if (res.Code = -3) {
                                createToast('warning', 'fa-solid fa-circle-exclamation', 'Chú ý', res.msg);
                            } else {
                                createToast('warning', 'fa-solid fa-circle-exclamation', 'Chú ý', res.msg);
                            }
                        }
                    }, error: function () {
                        alert("Lỗi khi cập nhập hoá đơn");
                    }
                });
            }
        }

    });









    $(document).on('input', '.Update_Quantity_For_Edit_Order', function () {
        var input = $(this);
        var productdetailId = input.attr('id');
        var billdetailid = input.attr('billdetailid');
        var billid = input.attr('billid');
        var newQuantity = input.val().trim();


        if (!input.data('original-value')) {
            input.data('original-value', input.val());
        }
        if (newQuantity === "" || parseInt(newQuantity) === 0) {

            if (confirm("Số lượng không thể nhỏ hơn 0. Bạn có muốn khôi phục lại số lượng cũ không?")) {

                input.val(input.data('original-value'));
            } else {

                return;
            }
        } else {
            $.ajax({
                url: '/Admin/Seller/UpdateQuantityForEditNew',
                type: 'POST',
                data: {
                    productdetailId: productdetailId,
                    billid: billid,
                    billdetailid: billdetailid,
                    newQuantity: newQuantity
                },
                success: function (response) {
                    if (response.Success) {

                        updateTotalPriceitem(billid);
                    } else {

                        input.val(input.data('original-quantity'));
                        createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', response.msg);
                    }
                },
                error: function (xhr, status, error) {
                    createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', response.msg);

                    input.val(input.data('original-quantity'));
                }
            });
        }
    });

    //end tab hoá đơn 

});

function loadBillEmployee(employeeid) {
    console.log("loadBillEmployee", employeeid)
    if (employeeid) {
        $.ajax({
            url: '/Admin/Seller/Partial_BillByEmployee',
            data: { employeeid: employeeid },
            type: 'GET',
            success: function (res) {
                $('.loadAllBill').html(res);
            }, error: function () {
                alert("Có lỗi khi load hoá đơn!!!");
            }
        });
    }
}

function LoadRowBill(billid) {
    if (billid) {
        $.ajax({
            url: '/Admin/Seller/GetUpdatedBillRow',
            type: 'GET',
            data: { billid: billid },
            success: function (res) {
                $('.tr_IndexBill_' + billid).replaceWith(res);
                createToast('success', 'fa-solid fa-circle-check', 'Thành công', 'Cập nhập hoá đơn thành công');
            },
            error: function (xhr, status, error) {
                console.error("Lỗi AJAX:", xhr.responseText);
                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Load dữ liệu mới');
                window.location.reload();
            }
        });
    }
}
function deleteItem(billid, billdetailid) {

    $.ajax({
        url: '/Admin/Seller/DeleteItem',
        type: 'POST',
        data: { billid: billid, billdetailid: billdetailid },
        success: function (response) {
            if (response.success) {

                loadListItem(billid);
                updateTotalPriceitem(billid);
                createToast('success', 'fa-solid fa-circle-check', 'Thành công', response.message)
            } else {

                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            createToast()
        }
    });
}
function loadListItem(billid) {
    if (billid != null) {
        $.ajax({
            url: '/Admin/Seller/Partail_ItemEditBill',
            type: 'GET',
            data: { billid: billid },
            success: function (response) {
                $("#loadListItem").html(response);

            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Đã xảy ra lỗi khi tải lại danh sách sản phẩm.');
            }
        });
    }
}
function updateTotalPriceitem(billid) {

    $.ajax({
        url: '/admin/Seller/GetTotalPriceItem',
        type: 'GET',
        data: { billid: billid },
        success: function (response) {
            $('#loadPrice').html('   <b>Tổng tiền</b><span class="text-danger">' + response + '</span>');
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);

            createToast('error', 'fa-solid fa-circle-exclamation', 'Đã xảy ra lỗi khi cập nhật tổng tiền.');
        }
    });
}

function validateForm() {
    var phone = $('#PhoneNumber').val().trim();
    var customer = $('#CustomerName').val().trim();
    let isValid = true;





    const allFields = ['CustomerName', 'PhoneNumber'];
    const isAllEmpty = allFields.every(id => $(`#${id}`).val().trim() === "");

    if (isAllEmpty) {

        allFields.forEach(id => {
            $(`#${id}`).addClass('error-border');
        });
        showError(allFields, "Vui lòng điền đầy đủ thông tin ");
        return false;
    }



    const phoneNumberRegex = /^0\d{9}$/;
    if (!phone) {
        showError('PhoneNumber', 'Số điện thoại không để trống.');
        isValid = false;
    } else if (phone.length > 11) {
        showError('PhoneNumber', 'Định dạng số điện thoại không đúng.');
        isValid = false;
    } else if (!phoneNumberRegex.test(phone)) {
        showError('PhoneNumber', 'Định dạng số điện thoại không đúng.');
        isValid = false;
    } else {
        removeError('PhoneNumber');
    }
    if (!customer) {
        showError('CustomerName', 'Vui lòng điền tên.');
        isValid = false;
     
    } else {
        removeError('CustomerName');
    }



    return isValid;



}




function LoadListProduct() {
    $.ajax({
        url: '/Admin/Seller/Partial_ListProductBill',
        type: 'GET',
        success: function (res) {
            LoadTotalPrice();
            $('#listProductItem').html(res);
        },
        error: function (res) {
            createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', res.msg);
        }
    });
}

function LoadTotalPrice() {
    $.ajax({
        url: '/Admin/Seller/GetTotalPrice',
        type: 'GET',
        success: function (res) {
            if (res && res.TotalPrice) {
                $('#totalPrice').html(res.TotalPrice);
            }
        }, error: function (res) {
            createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', res.msg);
        }
    });
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
function Partial_Product() {
    $('.loadding').show();
    $.ajax({
        url: '/Admin/Seller/Partail_Product',

        type: 'GET',
        success: function (res) {
            $('.loading').hide();

            $('#loaddataProduct').html(res);
        }, error: function (res) {

        }
    });
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