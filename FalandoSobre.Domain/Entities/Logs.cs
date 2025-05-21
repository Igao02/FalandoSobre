using FalandoSobre.DomainCore.Entities;

namespace FalandoSobre.Domain.Entities;

public  class Logs : Entity
{
    public Logs ()
    {
        //ORM Purpose
    }

    public string UserName { get; set; }

    public string Action { get; set; }
    
    public string EntityType { get; set; }

    public DateTime Created_At { get; set; }

    public string ApplicationUserId { get; set; }

    public Logs(string userName, string action, string entityType, DateTime created_At, string applicationUserId)
    {
        UserName = userName;
        Action = action;
        EntityType = entityType;
        Created_At = created_At;
        ApplicationUserId = applicationUserId;
    }
}
