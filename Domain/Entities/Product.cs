using System.ComponentModel;
using System.Web.Mvc;

namespace Domain.Entities
{
    public class Product
    {
        [DisplayName("�����")]
        public int ProductID { get; set; }

        [DisplayName("��������")]
        public string Name { get; set; }

        [DisplayName("��������")]
        public string Description { get; set; }

        [DisplayName("���������")]
        public string Category { get; set; }

        [DisplayName("����")]
        public decimal Price { get; set; }

        public byte[] ImageData { get; set; }
        
        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }
    }
}
