$(document).ready(function (e) {
    var bg = $(".bg-sg");
    var popup = $("#popupBanner");
    var btnCapNhatBanner = $('.btnCapNhatBanner');
    var btnDeleteBanner = $('.btnDeleteBanner');

    bg.on('click', function () {
        bg.hide();
        popup.hide();
    });

    $('.btnCloseEditBanner').on('click', function () {
        bg.hide();
        popup.hide();
    });

    btnCapNhatBanner.each(function () {
        $(this).on('click', function (e) {
            e.preventDefault();
            bg.show();
            popup.show();
            var id = $(this).data('id');
            if (id != null) {
                $.ajax({
                    url: '/admin/banner/Edit',
                    type: 'GET',
                    data: { id: id },
                    success: function (res) {
                        $('#loadDataBannerEdit').html(res);
                    },
                    error: function (xhr, status, error) {
                        console.error(xhr.responseText);
                    }
                });
            }
        });
    });
    btnDeleteBanner.each(function () {
        $(this).on('click', function (e) {

            e.preventDefault();
            var id = $(this).data('id');
            if (id != null) {
                Swal.fire({
                    title: "Bạn đang thao tác ?",
                    text: "Bạn muốn xoá banner này !",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#3085d6",
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Yes, delete it!"
                }).then((result) => {
                    if (result.isConfirmed) {

                        $.ajax({
                            url: '/admin/banner/delete',
                            type: 'POST',
                            data: { id: id },
                            success: function (res) {

                                if (res.success) {
                                    const Toast = Swal.mixin({
                                        toast: true,
                                        position: "top-end",
                                        showConfirmButton: false,
                                        timer: 1000,
                                        timerProgressBar: true,
                                        didOpen: (toast) => {
                                            toast.onmouseenter = Swal.stopTimer;
                                            toast.onmouseleave = Swal.resumeTimer;
                                        },
                                        willClose: () => {
                                            setTimeout(() => {
                                                location.reload();
                                            }, 100);
                                        }
                                    });

                                    Toast.fire({
                                        icon: "success",
                                        title: "Xoá thành công banner "
                                    });

                                }
                                else {
                                    Swal.fire({
                                        icon: "error",
                                        title: "Lỗi trong quá trình xoá...",
                                        text: res.msg,
                                        footer: 'Vui lòng liên hệ quản trị viên'
                                    });
                                }
                            },
                            error: function (xhr, status, error) {
                                console.error(xhr.responseText);
                            }
                        });



                    }
                });

            }

        });
    });



});