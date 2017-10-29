using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainClasses
{
    public class User : DomainBase
    {

        [MaxLength(254)]
        [Column(TypeName = "VARCHAR")]
        public string Email { get; set; }
    }
}
