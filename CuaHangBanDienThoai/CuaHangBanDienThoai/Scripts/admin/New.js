$(document).ready(function () {
    $('body').on('change', '.toggle__input', function () {
        var checkbox = $(this);
        var id = checkbox.attr('id').split('_')[1];
        var isActive = checkbox.is(':checked');
        var toggleType = checkbox.attr('class').split(' ')[1]; // Gets the second class (e.g., 'check_IsActive')

        console.log('Checkbox changed:', id, isActive, toggleType);

        var url = '';
        if (toggleType === 'check_IsActive') {
            url = '/Admin/News/IsActive';
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
                    console.log('Trạng thái đã được cập nhật thành công');
                } else {
                    console.error('Cập nhật trạng thái không thành công');
                    checkbox.prop('checked', !isActive);
                }
            },
            error: function (xhr, status, error) {
                console.error('Có lỗi xảy ra khi cập nhật trạng thái.');
                checkbox.prop('checked', !isActive);
            }
        });
    });
});

