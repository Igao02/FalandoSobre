using FalandoSobre.DomainCore.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FalandoSobre.Domain.Entities;

public class UserInfo : Entity
{
    public UserInfo()
    {
        //ORM Purpose
    }

    public string ProfilePhoto { get; set; } = string.Empty;

    public bool? Actived { get; set; } = true;

    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public string ApplicationUserId { get; set; }

    public byte[]? ProfilePhotoBytes { get; set; }

    public UserInfo(string profilePhoto, bool actived, DateTime createdAt, string applicationUserId, byte[]? profilePhotoBytes) : base()
    {
        ProfilePhoto = profilePhoto;
        Actived = actived;
        CreatedAt = createdAt;
        ApplicationUserId = applicationUserId;
        ProfilePhotoBytes = profilePhotoBytes;
    }
}
