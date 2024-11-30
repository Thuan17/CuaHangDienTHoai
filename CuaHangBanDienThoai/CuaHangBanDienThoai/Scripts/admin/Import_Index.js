$(document).ready(function () {
   /* var btnDetail = $('.btnDetail');*/
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
    //btnDetail.each(function () {
    //    $(this).on('click', function (e) {
    //        e.preventDefault();
    //        bg.show();
    //        popup.show();
    //        var id = $(this).data('id');
    //        if (id) {
    //            $.ajax({
    //                url: '/Admin/ImportWarehouse/Partail_Detail',
    //                data: { id: id },
    //                type: 'GET',
    //                success: function (res) {
    //                    $('#popupBill').html(res);
    //                }, error: function () {
    //                    alert("Có lỗi khi load chi tiết");
    //                }
    //            });
    //        }
    //    });
    //});
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


$(document).ready(function () {

    $('#clearInputBill').click(function () {
        $("#searchBill").val("");

        window.location.href = "/danh-sach-phieunhap";
    });
    $('#DateImport').on('change', function () {
        var ngaynhap = $('#DateImport').val();
        console.log("nagy nhap ", ngaynhap);
        if (ngaynhap) {
            $.ajax({
                url: '/Admin/ImportWareHouse/GetImportByDate',
                type: 'GET',
                data: { ngaynhap: ngaynhap },
                beforeSend: function () {
                    $('#loaddata').html('<div class="text-center"> <img src="/images/gif/loading.gif" /></div>');
                },
                success: function (data) {
                    $('#loaddata').html(data);
                },
                error: function (xhr, status, error) {
                    console.log('Đã xảy ra lỗi khi gửi yêu cầu:', error);
                }
            });
        }
    });

    $('#searchImport').on('input', function () {
        var keyword = $(this).val().trim();
        if (keyword.length >= 3) {
            $('.loadding').show();
            $.ajax({
                url: '/Admin/ImportWareHouse/SuggestImport',
                type: 'GET',
                data: { search: keyword },
                beforeSend: function () {
                    $('#loaddata').html('<div class="text-center"> <img src="~/images/gif/loading.gif" /></div>');
                },
                success: function (response) {
                    $('.loadding').hide();
                    $('#loaddata').html(response);
                },
                error: function () {
                    // Xử lý lỗi nếu có
                    console.log('Đã xảy ra lỗi khi tải dữ liệu.');
                }
            });
        } else {
            $('.loadding').hide();
            window.Location.href = "/quan-ly-don-hang";
        }
    });

});
