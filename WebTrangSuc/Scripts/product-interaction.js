$(document).ready(function () {
    $('.icon-search i').click(function () {
        var productId = $(this).data('id');
        $.ajax({
            url: '/SanPham/GetProductDetail',
            type: 'GET',
            data: { id: productId },
            success: function (data) {
                $('#modalProductName').text(data.TenSanPham);
                $('#modalProductImage').attr('src', '/' + data.HinhAnh);
                $('#modalProductPrice').text(data.Gia + 'đ');
                if (data.GiamGia != null) {
                    $('#modalProductDiscountPrice').text(data.GiaGiam + 'đ');
                } else {
                    $('#modalProductDiscountPrice').text('Không giảm');
                }
                $('#modalProductQuantity').val(1);
                $('#stockWarning').hide();
                $('#modalProductQuantity').attr('max', data.SoLuong);
                $('#addToCart').data('id', data.SanPhamID);
                $('#productDetailModal').modal('show');
            }
        });
    });
    $(document).on('click', '.icon-search i', function () {
        var productId = $(this).data('id');
        window.location.href = '/SanPham/Info/' + productId;
    });
    // Kiểm tra tồn kho
    $('#modalProductQuantity').on('input', function () {
        var max = parseInt($(this).attr('max'));
        var val = parseInt($(this).val());
        if (val > max) {
            $('#stockWarning').show();
        } else {
            $('#stockWarning').hide();
        }
    });

    // Thêm giỏ hàng
    $('#addToCart').click(function () {
        var productId = $(this).data('id');
        var quantity = $('#modalProductQuantity').val();
        $.ajax({
            url: '/GioHang/AddToCart',
            type: 'POST',
            data: { id: productId, quantity: quantity },
            success: function () {
                alert('Đã thêm vào giỏ hàng!');
                $('#productDetailModal').modal('hide');
            }
        });
    });

    // Button Mua ngay
    $('.buy-now').click(function () {
        var productId = $(this).data('id');
        window.location.href = '/ThanhToan/Index?id=' + productId;
    });

    // Button Chi tiết
    $('.view-details').click(function () {
        var productId = $(this).data('id');
        window.location.href = '/SanPham/ChiTiet?id=' + productId;
    });
});
