$(document).ready(function () {

    var btnAddBill = $(".btnAddBill");
    LoadListProduct();
    $('#productChose').change(function () {
        var productId = $(this).val();
        console.log("productChose", productId)
        if (productId) {
            $('.loadding').show();
            $.ajax({
                url: '/Admin/Seller/Partail_ProductById',
                data: { productId: productId },
                type: 'GET',
                success: function (res) {
                    $('.loading').hide();

                    $('#loaddataProduct').html(res);
                }, error: function (res) {

                }
            });
        } else {
            Partial_Product();
        }
    });
    btnAddBill.each(function (e) {
        $(this).on('click', function (event) {
            event.preventDefault();
            var productid = $(this).data('productid');
            if (productid) {
                $.ajax({
                    url: '/Admin/Seller/AddBill',
                    data: {
                        productid: productid, quantity: 1
                    },
                    type: 'POST',
                    success: function (res) {
                        if (res.Success && res.Code == 1) {
                            LoadListProduct();
                            createToast('success', 'fa-solid fa-circle-check', 'Thành công', res.msg)
                        } else {
                            if (res.Code == -99) {
                                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', res.msg);
                            } else {

                                createToast('warning', 'fa-solid fa-circle-exclamation', 'Chú ý', res.msg);
                            }
                        }
                       
                    }, error: function (res) {

                    }
                });
            }
        });
    });


});


function LoadListProduct() {
    $.ajax({
        url: '/Admin/Seller/Partial_ListProductBill',
        type: 'GET',
        success: function (res) {
            LoadTotalPrice();
            $('#listProductItem').html(res);
        }, error: function (res) {
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