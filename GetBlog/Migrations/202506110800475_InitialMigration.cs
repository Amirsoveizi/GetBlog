using System;
using System.Data.Entity.Migrations;

public partial class InitialMigration : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "dbo.Articles",
            c => new
                {
                    ArticleId = c.Int(nullable: false, identity: true),
                    Title = c.String(nullable: false, maxLength: 200),
                    Content = c.String(nullable: false),
                    Excerpt = c.String(maxLength: 500),
                    FeaturedImageUrl = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    PublishedDate = c.DateTime(),
                    LastModifiedDate = c.DateTime(nullable: false),
                    IsPublished = c.Boolean(nullable: false),
                    Slug = c.String(maxLength: 250),
                    AuthorId = c.String(nullable: false, maxLength: 128),
                })
            .PrimaryKey(t => t.ArticleId)
            .ForeignKey("dbo.AspNetUsers", t => t.AuthorId)
            .Index(t => t.AuthorId);
        
        CreateTable(
            "dbo.AspNetUsers",
            c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    FullName = c.String(),
                    Email = c.String(maxLength: 256),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256),
                })
            .PrimaryKey(t => t.Id)
            .Index(t => t.UserName, unique: true, name: "UserNameIndex");
        
        CreateTable(
            "dbo.AspNetUserClaims",
            c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
            .PrimaryKey(t => t.Id)
            .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            .Index(t => t.UserId);
        
        CreateTable(
            "dbo.Comments",
            c => new
                {
                    CommentId = c.Int(nullable: false, identity: true),
                    Content = c.String(nullable: false, maxLength: 2000),
                    CreatedDate = c.DateTime(nullable: false),
                    IsApproved = c.Boolean(nullable: false),
                    ArticleId = c.Int(nullable: false),
                    AuthorId = c.String(nullable: false, maxLength: 128),
                    ParentCommentId = c.Int(),
                })
            .PrimaryKey(t => t.CommentId)
            .ForeignKey("dbo.Articles", t => t.ArticleId, cascadeDelete: true)
            .ForeignKey("dbo.AspNetUsers", t => t.AuthorId)
            .ForeignKey("dbo.Comments", t => t.ParentCommentId)
            .Index(t => t.ArticleId)
            .Index(t => t.AuthorId)
            .Index(t => t.ParentCommentId);
        
        CreateTable(
            "dbo.AspNetUserLogins",
            c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
            .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
            .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            .Index(t => t.UserId);
        
        CreateTable(
            "dbo.AuthorProfiles",
            c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    Bio = c.String(),
                    ProfilePictureUrl = c.String(),
                    TwitterHandle = c.String(maxLength: 100),
                    WebsiteUrl = c.String(maxLength: 150),
                })
            .PrimaryKey(t => t.UserId)
            .ForeignKey("dbo.AspNetUsers", t => t.UserId)
            .Index(t => t.UserId);
        
        CreateTable(
            "dbo.AspNetUserRoles",
            c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    RoleId = c.String(nullable: false, maxLength: 128),
                })
            .PrimaryKey(t => new { t.UserId, t.RoleId })
            .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
            .Index(t => t.UserId)
            .Index(t => t.RoleId);
        
        CreateTable(
            "dbo.Categories",
            c => new
                {
                    CategoryId = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100),
                    Slug = c.String(maxLength: 150),
                    ParentCategoryId = c.Int(),
                })
            .PrimaryKey(t => t.CategoryId)
            .ForeignKey("dbo.Categories", t => t.ParentCategoryId)
            .Index(t => t.ParentCategoryId);
        
        CreateTable(
            "dbo.Media",
            c => new
                {
                    MediaId = c.Int(nullable: false, identity: true),
                    FileName = c.String(nullable: false, maxLength: 255),
                    Url = c.String(nullable: false),
                    MimeType = c.String(maxLength: 50),
                    AltText = c.String(maxLength: 255),
                    UploadedDate = c.DateTime(nullable: false),
                    UploaderId = c.String(nullable: false, maxLength: 128),
                })
            .PrimaryKey(t => t.MediaId)
            .ForeignKey("dbo.AspNetUsers", t => t.UploaderId)
            .Index(t => t.UploaderId);
        
        CreateTable(
            "dbo.Tags",
            c => new
                {
                    TagId = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100),
                    Slug = c.String(maxLength: 150),
                })
            .PrimaryKey(t => t.TagId);
        
        CreateTable(
            "dbo.AuditLogs",
            c => new
                {
                    AuditId = c.Long(nullable: false, identity: true),
                    UserId = c.String(maxLength: 128),
                    Action = c.String(nullable: false, maxLength: 100),
                    EntityName = c.String(maxLength: 100),
                    EntityId = c.Int(),
                    Timestamp = c.DateTime(nullable: false),
                    Details = c.String(),
                })
            .PrimaryKey(t => t.AuditId)
            .ForeignKey("dbo.AspNetUsers", t => t.UserId)
            .Index(t => t.UserId);
        
        CreateTable(
            "dbo.AspNetRoles",
            c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
            .PrimaryKey(t => t.Id)
            .Index(t => t.Name, unique: true, name: "RoleNameIndex");
        
        CreateTable(
            "dbo.Settings",
            c => new
                {
                    SettingKey = c.String(nullable: false, maxLength: 100),
                    SettingValue = c.String(nullable: false),
                })
            .PrimaryKey(t => t.SettingKey);
        
        CreateTable(
            "dbo.CategoryArticles",
            c => new
                {
                    Category_CategoryId = c.Int(nullable: false),
                    Article_ArticleId = c.Int(nullable: false),
                })
            .PrimaryKey(t => new { t.Category_CategoryId, t.Article_ArticleId })
            .ForeignKey("dbo.Categories", t => t.Category_CategoryId, cascadeDelete: true)
            .ForeignKey("dbo.Articles", t => t.Article_ArticleId, cascadeDelete: true)
            .Index(t => t.Category_CategoryId)
            .Index(t => t.Article_ArticleId);
        
        CreateTable(
            "dbo.MediaArticles",
            c => new
                {
                    Media_MediaId = c.Int(nullable: false),
                    Article_ArticleId = c.Int(nullable: false),
                })
            .PrimaryKey(t => new { t.Media_MediaId, t.Article_ArticleId })
            .ForeignKey("dbo.Media", t => t.Media_MediaId, cascadeDelete: true)
            .ForeignKey("dbo.Articles", t => t.Article_ArticleId, cascadeDelete: true)
            .Index(t => t.Media_MediaId)
            .Index(t => t.Article_ArticleId);
        
        CreateTable(
            "dbo.TagArticles",
            c => new
                {
                    Tag_TagId = c.Int(nullable: false),
                    Article_ArticleId = c.Int(nullable: false),
                })
            .PrimaryKey(t => new { t.Tag_TagId, t.Article_ArticleId })
            .ForeignKey("dbo.Tags", t => t.Tag_TagId, cascadeDelete: true)
            .ForeignKey("dbo.Articles", t => t.Article_ArticleId, cascadeDelete: true)
            .Index(t => t.Tag_TagId)
            .Index(t => t.Article_ArticleId);
        
    }
    
    public override void Down()
    {
        DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
        DropForeignKey("dbo.AuditLogs", "UserId", "dbo.AspNetUsers");
        DropForeignKey("dbo.TagArticles", "Article_ArticleId", "dbo.Articles");
        DropForeignKey("dbo.TagArticles", "Tag_TagId", "dbo.Tags");
        DropForeignKey("dbo.Media", "UploaderId", "dbo.AspNetUsers");
        DropForeignKey("dbo.MediaArticles", "Article_ArticleId", "dbo.Articles");
        DropForeignKey("dbo.MediaArticles", "Media_MediaId", "dbo.Media");
        DropForeignKey("dbo.Categories", "ParentCategoryId", "dbo.Categories");
        DropForeignKey("dbo.CategoryArticles", "Article_ArticleId", "dbo.Articles");
        DropForeignKey("dbo.CategoryArticles", "Category_CategoryId", "dbo.Categories");
        DropForeignKey("dbo.Articles", "AuthorId", "dbo.AspNetUsers");
        DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
        DropForeignKey("dbo.AuthorProfiles", "UserId", "dbo.AspNetUsers");
        DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
        DropForeignKey("dbo.Comments", "ParentCommentId", "dbo.Comments");
        DropForeignKey("dbo.Comments", "AuthorId", "dbo.AspNetUsers");
        DropForeignKey("dbo.Comments", "ArticleId", "dbo.Articles");
        DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
        DropIndex("dbo.TagArticles", new[] { "Article_ArticleId" });
        DropIndex("dbo.TagArticles", new[] { "Tag_TagId" });
        DropIndex("dbo.MediaArticles", new[] { "Article_ArticleId" });
        DropIndex("dbo.MediaArticles", new[] { "Media_MediaId" });
        DropIndex("dbo.CategoryArticles", new[] { "Article_ArticleId" });
        DropIndex("dbo.CategoryArticles", new[] { "Category_CategoryId" });
        DropIndex("dbo.AspNetRoles", "RoleNameIndex");
        DropIndex("dbo.AuditLogs", new[] { "UserId" });
        DropIndex("dbo.Media", new[] { "UploaderId" });
        DropIndex("dbo.Categories", new[] { "ParentCategoryId" });
        DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
        DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
        DropIndex("dbo.AuthorProfiles", new[] { "UserId" });
        DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
        DropIndex("dbo.Comments", new[] { "ParentCommentId" });
        DropIndex("dbo.Comments", new[] { "AuthorId" });
        DropIndex("dbo.Comments", new[] { "ArticleId" });
        DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
        DropIndex("dbo.AspNetUsers", "UserNameIndex");
        DropIndex("dbo.Articles", new[] { "AuthorId" });
        DropTable("dbo.TagArticles");
        DropTable("dbo.MediaArticles");
        DropTable("dbo.CategoryArticles");
        DropTable("dbo.Settings");
        DropTable("dbo.AspNetRoles");
        DropTable("dbo.AuditLogs");
        DropTable("dbo.Tags");
        DropTable("dbo.Media");
        DropTable("dbo.Categories");
        DropTable("dbo.AspNetUserRoles");
        DropTable("dbo.AuthorProfiles");
        DropTable("dbo.AspNetUserLogins");
        DropTable("dbo.Comments");
        DropTable("dbo.AspNetUserClaims");
        DropTable("dbo.AspNetUsers");
        DropTable("dbo.Articles");
    }
}
