$(document).ready(function () {
    var bg = $(".bg-sg");
    var popup = $("#popupBill");

    // Xử lý ẩn/hiện popup
    function hidePopup() {
        bg.hide();
        popup.hide();
    }

    function showPopup() {
        bg.show();
        popup.show();
    }

    // Đóng popup
    bg.on('click', hidePopup);
    $('.btnCloseEditBill').on('click', hidePopup);

    // Hàm tái sử dụng để cập nhật dòng
    function updateRow(orderId) {
        $.ajax({
            url: '/Admin/Order/GetUpdatedOrderRow',
            type: 'GET',
            data: { OrderId: orderId },
            success: function (res) {
                var mainRow = $('.tr_IndexBill_' + orderId);
                var nextRow = mainRow.next('tr'); // Xóa dòng phụ (nếu có)
                if (nextRow.length && !nextRow.hasClass('tr_IndexBill_' + orderId)) {
                    nextRow.remove();
                }
                mainRow.replaceWith(res); // Thay dòng chính
            },
            error: function (xhr, status, error) {
                console.error("Lỗi AJAX:", xhr.responseText);
                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Load dữ liệu mới');
                window.location.reload();
            }
        });
    }

    // Hàm xử lý trạng thái dropdown
    function updateSelectColor(selectElement) {
        var selectedOption = $(selectElement).find("option:selected");
        var selectedClass = selectedOption.attr("class");
        $(selectElement).removeClass("bg-warning bg-success bg-danger bg-dark")
            .addClass(selectedClass);
    }

    // Xử lý sự kiện cập nhật trạng thái
    $("select[name='Trangthai']").change(function () {
        var selectElement = $(this);
        var orderId = selectElement.attr("id").split("_")[1];
        var selectedStatus = selectElement.val();
        var btn = $('.btnIsConfirm[data-id="' + orderId + '"]');

        $.ajax({
            url: '/Admin/Order/UpdateOrderStatus',
            type: 'POST',
            data: {
                OrderId: orderId,
                status: selectedStatus
            },
            success: function (response) {
                if (response.success) {
                    updateRow(orderId);
                    btn.html(response.isConfirm
                        ? "<i class='fa fa-check text-success'></i>"
                        : "<i class='fas fa-times text-danger'></i>");
                } else {
                    if (user == 1) {
                        Swal.fire("Oppo !" + "\n" + "Phiên đăng nhập đã hết hạn" );
                    }
                    Swal.fire("Oppo !" + "\n" + response.message + "\n" + response.code);
                    selectElement.val("Chưa giao");
                }
            },
            error: function () {
                console.log("Lỗi khi cập nhật trạng thái");
                selectElement.val("Chưa giao");
            },
            complete: function () {
                updateSelectColor(selectElement);
            }
        });
    });

    // Hàm tái sử dụng để hiển thị popup và tải nội dung
    function loadPopupContent(url, id, target) {
        showPopup();
        $.ajax({
            url: url,
            type: 'GET',
            data: { id: id },
            success: function (response) {
                $(target).html(response);
            },
            error: function (xhr) {
                console.error(xhr.responseText);
            }
        });
    }

    // Xử lý sự kiện nút "Cập nhật Bill"
    $('.btnCapNhatBill').on('click', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        if (id) {
            loadPopupContent('/Admin/Order/Partial_EditOrder', id, '#loadDataBillEdit');
        }
    });

    // Xử lý sự kiện nút "Xem Bill"
    $('.btnXemBill').on('click', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        if (id) {
            loadPopupContent('/Admin/Order/Partial_DetailOrder', id, '#loadDataBillEdit');
        }
    });

    // Xử lý nút xác nhận
    $('body').on('click', '.btnIsConfirm', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data("id");

        $.ajax({
            url: '/admin/Order/IsConfirm',
            type: 'POST',
            data: { id: id },
            success: function (response) {
                if (response.success) {
                    btn.html(response.isConfirm
                        ? "<i class='fa fa-check text-success'></i>"
                        : "<i class='fas fa-times text-danger'></i>");
                    updateRow(id); // Cập nhật lại dòng nếu cần
                }
            }
        });
    });

    // Cập nhật màu sắc của tất cả dropdown khi load trang
    $("select[name='Trangthai']").each(function () {
        updateSelectColor(this);
    });
});












$(document).ready(function () {

    $('#clearInputBill').click(function () {
        $("#searchBill").val("");

        window.location.href = "/quan-ly-don-hang";
    });
    $('#DateOrder').on('change', function () {
        var ngayxuat = $('#DateOrder').val(); 
        if (ngayxuat) {
            $.ajax({
                url: '/Admin/Order/GetOrderByDate',  
                type: 'GET',
                data: { ngayxuat: ngayxuat },
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

    $('#searchBill').on('input', function () {
        var keyword = $(this).val().trim();
        if (keyword.length >= 3) {
            $('.loadding').show();
            $.ajax({
                url: '/Admin/Order/SuggestOrder',
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






