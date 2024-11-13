$(document).ready(function () {
    var bg = $(".bg-sg");
    var popup = $("#popupBill");
    var btnEdit = $('.btnEdit');
    var btnDetail = $('.btnDetail');

    bg.on('click', function () {
        bg.hide();
        popup.hide();
    });

    $('.btnCloseEditBill').on('click', function () {
        bg.hide();
        popup.hide();
    });
    btnEdit.each(function (e) {
        $(this).on('click', function () {
            bg.show();
            popup.show();
            var employeeid = $(this).data('id');
            if (employeeid) {
                $.ajax({
                    url: '/Admin/Employee/Partial_Edit',
                    type: 'GET',
                    data: { employeeid: employeeid },
                    success: function (res) {
                        $("#loadDataBillEdit").html(res);
                    }, error: function (res) {
                        alert("Lỗi load thông tin ");
                    }
                });
            }
        });

    });


    btnDetail.each(function (e) {
        $(this).on('click', function () {
            bg.show();
            popup.show();
            var employeeid = $(this).data('id');
            if (employeeid) {
                $.ajax({
                    url: '/Admin/Employee/Patial_Detial',
                    type: 'GET',
                    data: { employeeid: employeeid },
                    success: function (res) {
                        $("#loadDataBillEdit").html(res);
                    }, error: function (res) {
                        alert("Lỗi load thông tin ");
                    }
                });
            }
        });

    });
});