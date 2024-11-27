using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class ImportCart
    {
        public List<ImportCartItem> Items { get; set; }
        public ImportCart()
        {

            Items = new List<ImportCartItem>();
        }

        public void AddToCart(ImportCartItem item, int SoLuong)
        {
            var checkSanPham = Items.FirstOrDefault(x => x.ProductDetailId == item.ProductDetailId);

            if (checkSanPham != null)
            {
                checkSanPham.SoLuong = SoLuong;
                if (checkSanPham.ProductDetail.PriceSale > 0)
                {
                    checkSanPham.PriceTotal = (decimal)checkSanPham.ProductDetail.PriceSale * checkSanPham.SoLuong;
                }
                else
                {
                    checkSanPham.PriceTotal = (decimal)checkSanPham.ProductDetail.Price * checkSanPham.SoLuong;
                }

            }
            else
            {
                Items.Add(item);
            }
        }
        public void ClearCart()
        {
            Items.Clear();
        }
        public void Remove(int id)
        {
            var checkSanPham = Items.SingleOrDefault(x => x.ProductDetailId == id);
            if (checkSanPham != null)
            {
                Items.Remove(checkSanPham);
            }
        }

        //Cap Nhap San Pham Ra khoi gio hang
        public void UpSoLuong(int id, int SoLuong)
        {
            var checkSanPham = Items.SingleOrDefault(x => x.ProductDetailId == id);
            if (checkSanPham != null)
            {
                checkSanPham.SoLuong = SoLuong;
                if (checkSanPham.ProductDetail.PriceSale > 0)
                {

                    checkSanPham.PriceTotal = (decimal)checkSanPham.ProductDetail.PriceSale * checkSanPham.SoLuong;
                }
                else
                {
                    checkSanPham.PriceTotal = (decimal)checkSanPham.ProductDetail.Price * checkSanPham.SoLuong;
                }
            }
        }
    }
    public class ImportCartItem
    {

        public int ProductDetailId { get; set; }
        public int SoLuong { get; set; }
        public decimal PriceTotal { get; set; }

        public decimal Price { get; set; }

        public ProductDetail ProductDetail { get; set; }


    }
}