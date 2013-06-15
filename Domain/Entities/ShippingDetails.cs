using System.ComponentModel.DataAnnotations;


namespace Domain.Entities
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Пожалуйста, введите имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите первую строку адреса")]
        public string Line1 { get; set; }

        public string Line2 { get; set; }
        public string Line3 { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название города")]
        public string City { get; set; }

        public string Zip { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название страны")]
        public string Country { get; set; }

        public bool GiftWrap { get; set; }
    }
}
