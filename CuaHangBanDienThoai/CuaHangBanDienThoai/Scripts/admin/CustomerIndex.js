$(document).ready(function () {
    $('.cbkItem').each(function () {
        const customerId = $(this).val();
        if (customerId) {
            $.getJSON('/Admin/Customer/GetOrderStatistics', { customerId: customerId }, function (data) {
                if (data.error) {
                    console.error("Lỗi từ server: ", data.error);
                    alert("Có lỗi xảy ra khi lấy dữ liệu: " + data.error);
                } else {
                    if (!isNaN(data.SuccessRate) && !isNaN(data.CancelRate) && !isNaN(data.ConFirmRate)) {

                        var nonConfirmProgressHtml = `
                                         <p class="mt-4 mb-1" style="font-size: .77rem;">Đơn chờ xác nhận: ${data.ConFirmCount}</p>
                                         <div class="progress" style="height: 5px;">
                                             <div class="progress-bar bg-warning" role="progressbar" style="width: ${data.ConFirmRate}%" aria-valuenow="${data.ConFirmRate}" aria-valuemin="0" aria-valuemax="100"></div>
                                         </div>
                                     `;
                        var successProgressHtml = `
                                         <p class="mt-4 mb-1" style="font-size: .77rem;">Đơn hàng thành công: ${data.SuccessCount}/${data.TotalOrder}</p>
                                         <div class="progress" style="height: 5px;">
                                             <div class="progress-bar bg-success" role="progressbar" style="width: ${data.SuccessRate}%" aria-valuenow="${data.SuccessRate}" aria-valuemin="0" aria-valuemax="100"></div>
                                         </div>
                                     `;
                        var cancelProgressHtml = `
                                         <p class="mt-4 mb-1" style="font-size: .77rem;">Đơn hàng hủy: ${data.CancelCount}/${data.TotalOrder}</p>
                                         <div class="progress" style="height: 5px;">
                                             <div class="progress-bar bg-danger" role="progressbar" style="width: ${data.CancelRate}%" aria-valuenow="${data.CancelRate}" aria-valuemin="0" aria-valuemax="100"></div>
                                         </div>
                                     `;


                        var billProgressHtml = `
                                         <p class="mt-4 mb-1" style="font-size: .77rem;">Mua tại cửa hàng: </p>
                                         <div class="progress" style="height: 5px;">
                                             <div class="progress-bar bg-danger" role="progressbar" style="width: ${data.Totalbill}%" aria-valuenow="${data.Totalbill}" aria-valuemin="0" aria-valuemax="100"></div>
                                         </div>
                                     `;
                    
                        $(`#tableOrder_${customerId}`).append(nonConfirmProgressHtml);
                        $(`#tableOrder_${customerId}`).append(successProgressHtml);
                        $(`#tableOrder_${customerId}`).append(cancelProgressHtml);
                        $(`#tableOrder_${customerId}`).append(billProgressHtml);
                    } else {
                        console.error("Dữ liệu tỷ lệ không hợp lệ!");
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                console.error("Lỗi khi gọi API: ", textStatus, errorThrown);
                alert("Có lỗi xảy ra khi kết nối với server. Vui lòng thử lại sau!");
            });
        }

    });
});

$(document).ready(function () {
    $('body').on('change', '.toggle__input', function () {
        var checkbox = $(this);
        var id = checkbox.attr('id').split('_')[1];
        var isActive = checkbox.is(':checked');
        var toggleType = checkbox.attr('class').split(' ')[1];
        console,
        console.log('Checkbox changed:', id, isActive, toggleType);
        if (isActive) {
            Swal.fire({
                title: "Bạn muốn khoá tai khoản ?",
                text: "Vui lòng xác nhận !",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Xác nhận khoá!"
            }).then((result) => {
                if (result.isConfirmed) {
                    var url = '';
                    if (toggleType === 'check_IsLock') {
                        url = '/admin/Customer/IsLock';
                    }
                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: {
                            id: id,
                            isActive: isActive
                        },
                        success: function (rs) {
                            if (rs.success) {
                                Swal.fire({
                                    title: "Thành công !",
                                    text: "Tài khoản đã được khoá",
                                    icon: "success"
                                });
                            } else {

                                checkbox.prop('checked', !isLock);
                                Swal.fire({
                                    title: "Chú ý!",
                                    text: "Khoá tài khoản thất bại",
                                    icon: "success"
                                });

                            }
                        },
                        error: function (xhr, status, error) {
                            console.error('Có lỗi xảy ra khi cập nhật trạng thái.');
                            checkbox.prop('checked', !isActive);
                            Swal.fire({
                                icon: "error",
                                title: "Lỗi",
                                text: "Hệ thống lỗi",
                            });
                        }
                    });

                }
            });
        }
        else {
            Swal.fire({
                title: "Bạn muốn mở khoá tai khoản ?",
                text: "Vui lòng xác nhận !",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Xác nhận mở khoá!"
            }).then((result) => {
                if (result.isConfirmed) {
                    var url = '';
                    if (toggleType === 'check_IsLock') {
                        url = '/admin/Customer/IsLock';
                    }
                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: {
                            id: id,
                            isActive: isActive
                        },
                        success: function (rs) {
                            if (rs.success) {
                                Swal.fire({
                                    title: "Thành công !",
                                    text: "Tài khoản đã được mở khoá",
                                    icon: "success"
                                });
                            } else {

                                checkbox.prop('checked', !isLock);
                                Swal.fire({
                                    title: "Chú ý!",
                                    text: " Mở khoá tài khoản thất bại",
                                    icon: "success"
                                });

                            }
                        },
                        error: function (xhr, status, error) {
                            console.error('Có lỗi xảy ra khi cập nhật trạng thái.');
                            checkbox.prop('checked', !isActive);
                            Swal.fire({
                                icon: "error",
                                title: "Lỗi",
                                text: "Hệ thống lỗi",
                            });
                        }
                    });

                }
            });
        }
       
    });
});


$(document).ready(function () {
    var bg = $(".bg-sg");
    var popup = $("#popupBill");
    var btnCapNhatBill = $('.btnCapNhatBill');
    var btnDetail = $('.btnDetail');
    bg.on('click', function () {
        bg.hide();
        popup.hide();
    });

    $('.btnCloseEditBill').on('click', function () {
        bg.hide();
        popup.hide();
    });
    btnDetail.each(function () {
        $(this).on('click', function () {
            bg.show();
            popup.show();
            var customerid = $(this).data('id');
            console.log("Admin/Customer", customerid);
            if (customerid) {
                $.ajax({
                    url: '/Admin/Customer/Partail_Detail',
                    data: { customerid: customerid },
                    type: 'GET',
                    success: function (response) {
                        $("#loadDataBillEdit").html(response);
                    }, error: function (response) {
                        alert("Lỗi load thông tin ");
                    }
                });
            }


        });
    });
});
  