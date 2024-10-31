using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class ShoppingCart
    {


        public List<ShoppingCartItem> Items { get; set; }
        public ShoppingCart()
        {

            Items = new List<ShoppingCartItem>();
        }

        public void AddToCart(ShoppingCartItem item, int SoLuong)
        {
            var checkSanPham = Items.FirstOrDefault(x => x.ProductDetailId == item.ProductDetailId);

            if (checkSanPham != null)
            {
                checkSanPham.Quantity = SoLuong;
                if (checkSanPham.PriceSale > 0)
                {
                    checkSanPham.PriceTotal = checkSanPham.PriceSale * checkSanPham.Quantity;
                }
                else
                {
                    checkSanPham.PriceTotal = checkSanPham.Price * checkSanPham.Quantity;
                }

            }
            else
            {
                Items.Add(item);
            }
        }
        public decimal GetTotalPrice()
        {
            return Items.Sum(x => x.PriceTotal);
        }
        public int GetTotalQuantity()
        {
            return Items.Sum(x => x.Quantity);
        }
        public void ClearCart()
        {
            Items.Clear();
        }

        public void UpdateQuantity(int id, int quantity)
        {
            var checkExits = Items.SingleOrDefault(x => x.ProductDetailId == id);
            if (checkExits != null)
            {
                checkExits.Quantity = quantity;
                checkExits.PriceTotal = checkExits.Price * checkExits.Quantity;
            }
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
                checkSanPham.Quantity = SoLuong;
                if (checkSanPham.PriceSale > 0)
                {

                    checkSanPham.PriceTotal = checkSanPham.PriceSale * checkSanPham.Quantity;
                }
                else
                {
                    checkSanPham.PriceTotal = checkSanPham.Price * checkSanPham.Quantity;
                }
            }
        }
    }

    public class ShoppingCartItem
    {
        public int ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string Alias { get; set; }
        public string CategoryName { get; set; }
        public string ProductImg { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceSale { get; set; }
        public decimal PriceTotal { get; set; }
        public decimal? OriginalPriceTotal { get; set; }

        public int? PercentPriceReduction { get; set; }
        public string CodeVoucher { get; set; }

        public string Color { get; set; }
        public int Capcity { get; set; }


        public ProductDetail ProductDetail { get; set; }
       





    }
}