using System.Collections.Generic;
using System.Security.AccessControl;

namespace Domain.DomainClasses
{
    public class User : DomainBase
    {
        public string Name { get; set; }

        public string RawSenderName { get; set; }

        public virtual List<UserEmail> UserEmails { get; set; }
    }
}
