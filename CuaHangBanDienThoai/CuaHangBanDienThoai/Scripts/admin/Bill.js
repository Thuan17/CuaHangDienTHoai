$(document).ready(function () {
    var bg = $(".bg-sg");
    var popup = $("#popupBill");
    var btnCapNhatBill = $('.btnCapNhatBill');
    var btnXemBill = $('.btnXemBill');
    bg.on('click', function () {
        bg.hide();
        popup.hide();
    });

    $('.btnCloseEditBill').on('click', function () {
        bg.hide();
        popup.hide();
    });
    btnCapNhatBill.each(function () {
        $(this).on('click', function (e) {

            e.preventDefault();

            bg.show();
            popup.show();

            var id = $(this).data('id');

            if (id != null) {
                $.ajax({
                    url: '/Admin/Bill/Partial_EditBill',
                    type: 'GET',
                    data: { id: id },
                    success: function (response) {
                        $("#loadDataBillEdit").html(response);
                    },
                    error: function (xhr, status, error) {
                        console.error(xhr.responseText);
                    }
                });
            }
        });
    });
    btnXemBill.each(function () {
        $(this).on('click', function (e) {

            e.preventDefault();

            bg.show();
            popup.show();

            var id = $(this).data('id');

            if (id != null) {
                $.ajax({
                    url: '/Admin/Bill/Partial_DetailBill',
                    type: 'GET',
                    data: { id: id },
                    success: function (response) {
                        $("#loadDataBillEdit").html(response);
                    },
                    error: function (xhr, status, error) {
                        console.error(xhr.responseText);
                    }
                });
            }
        });
    });


    function updateSelectColor(selectElement) {
        var selectedOption = $(selectElement).find("option:selected");
        var selectedClass = selectedOption.attr("class");
        $(selectElement).removeClass("bg-warning bg-success bg-danger bg-dark")
            .addClass(selectedClass);
    }

    $("select[name='Trangthai']").change(function () {
        var selectElement = $(this);
        var billId = selectElement.attr("id").split("_")[1];
        var selectedStatus = selectElement.val(); // Lấy giá trị đã chọn


        $.ajax({
            url: '/Admin/Bill/UpdateOrderStatus', // URL tới Action của bạn
            type: 'POST',
            data: {
                billId: billId,
                status: selectedStatus
            },
            success: function (response) {
                if (response.success) {

                } else {
                    Swal.fire("Oppo !" + "\n" + response.message + "\n" + response.code);
                    console.log("Lỗi khi cập nhật trạng thái: " + response.message + "" + response.code);
                    selectElement.val("Chưa giao");
                    updateSelectColor(selectElement);
                }
            },
            error: function () {
                console.log("Lỗi khi cập nhật trạng thái: " + response.message);
                selectElement.val("Chưa giao");
                updateSelectColor(selectElement);
            },
            complete: function () {

                updateSelectColor(selectElement);
            }
        });

        // Cập nhật màu sắc ngay lập tức
        updateSelectColor(this);
    });

    // Cập nhật màu mặc định cho mỗi select khi tải trang
    $("select[name='Trangthai']").each(function () {
        updateSelectColor(this);
    });


    $('body').on('click', '.btnIsConfirm', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data("id");
        var selectElement = $('#typeBill_' + id);



        $.ajax({
            url: '/admin/Bill/IsConfirm',
            type: 'POST',
            data: { page: 1, id: id },
            success: function (rs) {
                if (rs.success) {
                    if (rs.isConfirm) {
                        btn.html("<i class='fa fa-check text-success'></i>");

                    } else {
                        btn.html("<i class='fas fa-times text-danger'></i>");
                        selectElement.val("Chưa giao");
                        updateSelectColor(selectElement);
                    }
                }
                else {
                    btn.html("<i class='fas fa-times text-danger'></i>");
                    selectElement.val("Chưa giao");
                    updateSelectColor(selectElement);
                    Swal.fire("Oppo !" + "\n" + response.message);
                }

            },
            error: function () {
                console.log("Lỗi khi xác nhận đơn hàng: " + response.message);
                selectElement.val("Chưa giao");
                updateSelectColor(selectElement);
            },


        });
    });
});














$(document).ready(function () {

    $('#clearInputBill').click(function () {
        $("#searchBill").val("");

        window.location.href = "/quan-ly-don-hang";
    });
    $('#searchBill').on('input', function () {
        var keyword = $(this).val().trim();
        if (keyword.length >= 3) {
            $('.loadding').show();
            $.ajax({
                url: '/Admin/Bill/SuggestBill',
                type: 'GET',
                data: { search: keyword },
                beforeSend: function () {
                    $('#loaddata').html('<div class="text-center"> <img src="~/Content/ckfinder/ckfinder/plugins/gallery/colorbox/images/loading.gif" /></div>');
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






