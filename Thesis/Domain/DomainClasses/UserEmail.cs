namespace Domain.DomainClasses
{
    public class UserEmail : DomainBase
    {
        public string Email { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
