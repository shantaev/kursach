using System.ComponentModel;
using System.Web.Mvc;

namespace Domain.Entities
{
    public class Product
    {
        [DisplayName("Номер")]
        public int ProductID { get; set; }

        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("Категория")]
        public string Category { get; set; }

        [DisplayName("Цена")]
        public decimal Price { get; set; }

        public byte[] ImageData { get; set; }
        
        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }
    }
}
