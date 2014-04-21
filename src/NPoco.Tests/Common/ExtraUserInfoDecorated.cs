namespace NPoco.Tests.Common
{
    [TableName("ExtraUserInfos")]
    [TableAutoCreate(true)]
    [PrimaryKey("ExtraUserInfoId")]
    [ExplicitColumns]
    public class ExtraUserInfoDecorated
    {
        [IdentityColumn("ExtraUserInfoId")]
        public int ExtraUserInfoId { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Children")]
        public int Children { get; set; }
    }
}
