using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public static class Messages
    {
        public static string AyrımYeri = "---------------------------------------VERİTABANI";
        public static string ProductAdded = "Ürün eklendi";
        public static string ProductNameInvalid = "Ürün ismi geçersiz";
        public static string ProductNameMinTwoCharacter = "Ürün ismi en az iki karakter olabilir";
        public static string MaintenanceTime = "Bakım zamanı";
        public static string ProductsListed = "Ürünler listelendi";
        public static string ProductsListedWithCategory = "İstediğiniz kategorideki ürünler listelendi";
        public static string ProductsListedWithUnitPrice = "İstediğiniz fiyat aralığındaki ürünler listelendi";
        public static string ProductDetailListed = "Detayını istediğiniz ürün";
        public static string ProductUpdated = "Ürün güncellendi";
        public static string ProductDeleted = "Ürün silindi";
        public static string CategoriesListed = "Kategoriler listelendi";
        public static string CategoryDetailListed = "Detayını istediğiniz kategori";
        public static string CategoryAdded = "Kategori eklendi";
        public static string CategoryUpdated = "Kategori güncellendi";
        public static string CategoryDeleted = "Kategori silindi";
        public static string CustomerAdded = "Müşteri eklendi";
        public static string CustomerDeleted = "Müşteri silindi";
        public static string CustomersListed = "Müşteriler listelendi";
        public static string CustomerDetailListed = "Detayını istediğiniz müşteri";
        public static string CustomerUpdated = "Müşteri güncellendi";
        public static string OrderAdded = "Sipariş eklendi";
        public static string OrderDeleted = "Sipariş silindi";
        public static string OrdersListed = "Siparişler listelendi";
        public static string OrdersDetailListed = "Detayını istediğiniz sipariş";
        public static string OrderUpdated = "Sipariş güncellendi.";
        public static string AyrımYeri2 = "---------------------------------------VALIDATION";
        public static string ProductNotEmpty = "Ürün adı boş olamaz";
        public static string? AuthorizationDenied="Bu işlemi yapmak için yetkiniz yok";
        public static string UserRegistered="Kullanıcı kayıt edildi";
        public static string UserNotFound="Kullanıcı bulunamadı";
        public static string PasswordError="Girdiğiniz şifre geçersiz";
        public static string SuccessfulLogin="Giriş başarılı";
        public static string UserAlreadyExists="Bu kullanıcı zaten mevcut";
        public static string AccessTokenCreated="Token oluşturuldu";
    }
}
