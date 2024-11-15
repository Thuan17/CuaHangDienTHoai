$(document).ready(function () {
    var bg = $(".bg-sg");
    var popup = $("#popupBill");
    var btnEdit = $('.btnEdit');
    var btnDetail = $('.btnDetail');
    var btnResetPass = $('.btnResetPass');


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




    btnResetPass.each(function () {
        $(this).on('click', function (e) {
            e.preventDefault();
            var originalText = $(this).html();
            var $button = $(this);
            var employeeId = $(this).data('id');
            if (employeeId) {
                $button.prop('disabled', true);
                $button.find('.loading-image').show();
                $button.find('.button-text').hide();


                Swal.fire({
                    title: "Bạn muốn khôi phục mật khẩu ?",
                    text: "Vui lòng xác nhận !",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#3085d6",
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Xác nhận đổi!"
                }).then((result) => {
                    if (result.isConfirmed) {

                        $.ajax({
                            url: '/Admin/Employee/ResetPass',
                            type: 'POST',
                            data: {
                                employeeId: employeeId
                            },
                            success: function (rs) {
                                if (rs.Success) {
                                    Swal.fire({
                                        title: "Thành công !",
                                        text:rs.msg,
                                        icon: "success"
                                    });
                                    $button.prop('disabled', false);
                                    $button.find('.loading-image').hide();
                                    $button.find('.button-text').show();
                                } else {
                                    if (rs.Code == -99) {

                                        Swal.fire({
                                            title: "Chú ý!",
                                            text: rs.msg,
                                            icon: "Error"
                                        });
                                    } else {
                                        $button.prop('disabled', false);
                                        $button.find('.loading-image').hide();
                                        $button.find('.button-text').show();
                                        Swal.fire({
                                            title: "Chú ý!",
                                            text: rs.msg,
                                            icon: "warning"
                                        });
                                    }

                                }
                            },
                            error: function (xhr, status, error) {
                                $button.prop('disabled', false);
                                $button.find('.loading-image').hide();
                                $button.find('.button-text').show();
                                Swal.fire({
                                    icon: "error",
                                    title: "Lỗi",
                                    text: "Hệ thống lỗi",
                                });
                            }
                        });

                    } else {
                        $button.prop('disabled', false);
                        $button.find('.loading-image').hide();
                        $button.find('.button-text').show();
                    }
                });
            } 
        });
    });
});
$(document).ready(function () {
    $('#clearsearchEmp').click(function () {
        alert("thuận"); // Kiểm tra xem sự kiện click có hoạt động
        $("#searchEmp").val(""); // Đảm bảo đúng ID input
        window.location.reload(); // Tải lại trang
    });

    $('#searchEmp').on('input', function () {
        var keyword = $(this).val().trim();
        if (keyword.length > 4) {
            $('.loadding').show();
            $('.body').hide(); 
            $.ajax({
                url: '/Admin/Employee/Search',
                type: 'GET',
                data: { search: keyword },
              
                success: function (response) {
                    $('.loading').hide(); 
                    $('.body').show(); 
                    $('#loaddata').html(response);
                },
                error: function () {
                    $('.loading').hide(); 
                    console.log('Đã xảy ra lỗi khi tải dữ liệu.');
                }
            });
        }
    });


    $('#functionChose').change(function () {
     
        var functionChoseid = $(this).val();
        $('.loadding').show();
        $('.body').hide(); 
        if (functionChoseid) {
            $.ajax({
                url: '/Admin/Employee/EmployeeById',
                type: 'GET',
                data: { functionChoseid: functionChoseid },
                success: function (res) {
                    $('.loading').hide(); 
                    $("#loaddata").html(res);
                    $('.body').show(); 
                }, error: function (res) {
                    alert("Có lỗi load dữ liệu")
                }
            });
        } else {
            window.location.reload();
        }
    });
});
$(document).ready(function () {
    $('body').on('change', '.toggle__input', function () {
        var checkbox = $(this);
        var id = checkbox.attr('id').split('_')[1];
        var isActive = checkbox.is(':checked');
        var toggleType = checkbox.attr('class').split(' ')[1];
       
        if (isActive) {
            Swal.fire({
                title: "Bạn muốn khoá tài khoản nhân viên này ?",
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
                        url = '/admin/Employee/IsLock';
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
                title: "Bạn muốn mở khoá tai khoản nhân viên này  ?",
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
                        url = '/admin/Employee/IsLock';
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
