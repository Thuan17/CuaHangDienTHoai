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
                    url: '/Admin/Order/Partial_EditOrder',
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
                    url: '/Admin/Order/Partial_DetailOrder',
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
        var orderid = selectElement.attr("id").split("_")[1];
        var id = $(this).data('id');
        var selectedStatus = selectElement.val(); 
        var btn = $('.btnIsConfirm[data-id="' + orderid + '"]');
        var btnCapNhatBill = $('.btnCapNhatBill[data-id="' + id + '"]');
       
        $.ajax({
            url: '/Admin/Order/UpdateOrderStatus',
            type: 'POST',
            data: {
                OrderId: orderid,
                status: selectedStatus
            },
            success: function (response) {
                if (response.success) {
                    $.ajax({
                        url: '/Admin/Order/GetUpdatedOrderRow',
                        type: 'GET',
                        data: { OrderId: orderid },
                        success: function (res) {
                            $('.tr_IndexBill_' + orderid).replaceWith(res);
                        },
                        error: function (xhr, status, error) {
                            console.error("Lỗi AJAX:", xhr.responseText);
                            createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Load dữ liệu mới');
                            window.location.reload();
                        }
                    });


                    if (response.Confirm) {
                        

                        //if (selectedStatus.trim() !== "Chưa giao" ) {
                        //    btnCapNhatBill.addClass('hide');
                        //} else if (selectedStatus.trim() === "Chưa giao") {
                        //    btnCapNhatBill.addClass('show');
                        //} 
                        btn.html("<i class='fa fa-check text-success'></i>");
                    } else {
                        btn.html("<i class='fas fa-times text-danger'></i>");
                    }
                  
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

     
        updateSelectColor(this);
    });

    function updateRow(orderId) {
        $.ajax({
            url: '/Admin/Order/GetUpdatedOrderRow',
            type: 'GET',
            data: { OrderId: orderId },
            success: function (res) {
                $('.tr_IndexBill_' + orderId).replaceWith(res);



            },
            error: function (xhr, status, error) {
                console.error("Lỗi AJAX:", xhr.responseText);
                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Load dữ liệu mới');
                window.location.reload();
            }
        });
    }


    $("select[name='Trangthai']").each(function () {
        updateSelectColor(this);
    });


    $('body').on('click', '.btnIsConfirm', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data("id");
        var selectElement = $('#typeBill_' + id);

        $.ajax({
            url: '/admin/Order/IsConfirm',
            type: 'POST',
            data: { id: id },
            success: function (rs) {
                if (rs.success) {
                    if (rs.isConfirm) {
                        btn.html("<i class='fa fa-check text-success'></i>");
                      
                    } else {
                        btn.html("<i class='fas fa-times text-danger'></i>");
                    }
                }

            }
        });

        //$.ajax({
        //    url: '/admin/Order/IsConfirm',
        //    type: 'POST',
        //    data: { page: 1, id: id },
        //    success: function (rs) {
        //        if (rs.success) {
        //            if (rs.isConfirm) {
        //                btn.html("<i class='fa fa-check text-success'></i>");

        //            } else {
        //                btn.html("<i class='fas fa-times text-danger'></i>");
        //                selectElement.val("Chưa giao");
        //                updateSelectColor(selectElement);
        //            }
        //        }
        //        else {
        //            btn.html("<i class='fas fa-times text-danger'></i>");
        //            selectElement.val("Chưa giao");
        //            updateSelectColor(selectElement);
        //            Swal.fire("Oppo !" + "\n" + response.message);
        //        }

        //    },
        //    error: function () {
        //        console.log("Lỗi khi xác nhận đơn hàng: " + response.message);
        //        selectElement.val("Chưa giao");
        //        updateSelectColor(selectElement);
        //    },


        //});
    });
});












$(document).ready(function () {

    $('#clearInputBill').click(function () {
        $("#searchBill").val("");

        window.location.href = "/quan-ly-don-hang";
    });
    $('#DateOrder').on('change', function () {
        var ngayxuat = $('#DateOrder').val();  // Đảm bảo lấy giá trị từ input #DateOrder
        if (ngayxuat) {
            $.ajax({
                url: '/Admin/Order/GetOrderByDate',  // Kiểm tra đường dẫn URL của action này
                type: 'GET',
                data: { ngayxuat: ngayxuat },
                beforeSend: function () {
                    $('#loaddata').html('<div class="text-center"> <img src="/images/gif/loading.gif" /></div>');  // Đảm bảo đường dẫn ảnh là đúng
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






