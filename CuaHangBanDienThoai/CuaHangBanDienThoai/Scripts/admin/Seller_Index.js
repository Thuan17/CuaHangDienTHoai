﻿$(document).ready(function () {
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

    // Event delegation to prevent multiple bindings
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





    $(document).on('input', '#searchinput', function () {
        var input = $(this).val().trim();
        if (input && input.length > 4) {
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
        }
    });
});

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