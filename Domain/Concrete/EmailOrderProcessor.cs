using System.Net;
using System.Net.Mail;
using System.Text;
using Domain.Abstract;
using Domain.Entities;

namespace Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "nurchik89@list.ru";
        public string MailFromAddress = "shantaevnursultan@gmail.com";
        public bool UseSsl = true;
        public string Username = "shantaevnursultan@gmail.com";
        public string Password = "jamilka6.10maccoffee";
        public string SeverName = "smtp.gmail.com";
        public int ServerPort = 587;
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor (EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.SeverName;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                var body = new StringBuilder().AppendLine("Новый заказ оформлен")
                    .AppendLine("---")
                    .AppendLine("Товары: ");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price*line.Quantity;
                    body.AppendFormat("{0} x {1} (Общая стоимость: {2:c})",
                                      line.Quantity, line.Product.Name, subtotal
                        );
                }

                body.AppendFormat("Cумма: {0:c}", cart.ComputeTotalValue())
                    .AppendLine("--")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingInfo.Name)
                    .AppendLine(shippingInfo.Line1)
                    .AppendLine(shippingInfo.City)
                    .AppendLine(shippingInfo.Country)
                    .AppendLine(shippingInfo.Zip)
                    .AppendLine("--");

                var mailMessage = new MailMessage
                    (
                    emailSettings.MailFromAddress,
                    emailSettings.MailToAddress,
                    "Новый заказ оформлен",
                    body.ToString()
                    );

                smtpClient.Send(mailMessage);
            }
        }
    }
}
