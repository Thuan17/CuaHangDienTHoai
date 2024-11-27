$(document).ready(function () {
    var btnDetail = $('.btnDetail');
    var btnEdit = $('.btnEdit');
    var bg = $(".bg-sg");
    var popup = $("#popupBill");
    bg.on('click', function () {
        bg.hide();
        popup.hide();
    });

    $('.btnCloseEditBill').on('click', function () {
        bg.hide();
        popup.hide();
    });
    btnDetail.each(function () {
        $(this).on('click', function (e) {
            e.preventDefault();
            bg.show();
            popup.show();
            var id = $(this).data('id');
            if (id) {
                $.ajax({
                    url: '/Admin/ImportWarehouse/Partail_Detail',
                    data: { id: id },
                    type: 'GET',
                    success: function (res) {
                        $('#popupBill').html(res);
                    }, error: function () {
                        alert("Có lỗi khi load chi tiết");
                    }
                });
            }
        });
    });
    btnEdit.each(function () {
        $(this).on('click', function (e) {
            e.preventDefault();
            bg.show();
            popup.show();
            var id = $(this).data('id');
            if (id) {
                $.ajax({
                    url: '/Admin/ImportWarehouse/Partail_Edit',
                    data: { id: id },
                    type: 'GET',
                    success: function (res) {
                        $('#popupBill').html(res);
                    }, error: function () {
                        alert("Có lỗi khi load chi tiết");
                    }
                });
            }
        });
    });


 





});