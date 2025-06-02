using System.ComponentModel.DataAnnotations;

namespace MasterServiceDemo.Models
{
    public class User
    {
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(6)]
        public string LoginOTP { get; set; }
    }

    public class UserModel
    {
        public string iduserData { get; set; }
        public string userDatacol { get; set; }
        public string userEmail { get; set; }
        public string userPhone { get; set; }

    }

    public class OrderModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
