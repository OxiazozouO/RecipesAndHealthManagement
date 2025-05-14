using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebServer.DatabaseModel;

public partial class RecipeAndHealthSystemContext : DbContext
{
    public RecipeAndHealthSystemContext()
    {
    }

    public RecipeAndHealthSystemContext(DbContextOptions<RecipeAndHealthSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryItem> CategoryItems { get; set; }

    public virtual DbSet<Collection> Collections { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<ConfigActivityLevel> ConfigActivityLevels { get; set; }

    public virtual DbSet<DietaryRecord> DietaryRecords { get; set; }

    public virtual DbSet<Dri> Dris { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<FavoriteItem> FavoriteItems { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<IngredientNutritional> IngredientNutritionals { get; set; }

    public virtual DbSet<Nutrient> Nutrients { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PhysicalSignsRecord> PhysicalSignsRecords { get; set; }

    public virtual DbSet<PreparationStep> PreparationSteps { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeItem> RecipeItems { get; set; }

    public virtual DbSet<Release> Releases { get; set; }

    public virtual DbSet<ReleaseFlowHistory> ReleaseFlowHistories { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SystemConfig> SystemConfigs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;user=root;password=123456;database=recipe_and_health_system");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("admin", tb => tb.HasComment("管理员"));

            entity.HasIndex(e => e.Name, "admin_pk").IsUnique();

            entity.HasIndex(e => e.RoleId, "admin_role_id_fk");

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasComment("头像")
                .HasColumnName("file_url");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasComment("名称")
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(44)
                .HasComment("密码")
                .HasColumnName("password");
            entity.Property(e => e.RoleId)
                .HasComment("角色ID")
                .HasColumnName("role_id");
            entity.Property(e => e.Salt)
                .HasMaxLength(49)
                .IsFixedLength()
                .HasComment("盐")
                .HasColumnName("salt");
            entity.Property(e => e.Status)
                .HasComment("账户状态")
                .HasColumnName("status");

            entity.HasOne(d => d.Role).WithMany(p => p.Admins)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("admin_role_id_fk");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("category", tb => tb.HasComment("分类"));

            entity.HasIndex(e => new { e.CName, e.TypeId }, "category_pk").IsUnique();

            entity.Property(e => e.CategoryId)
                .HasComment("ID")
                .HasColumnName("category_id");
            entity.Property(e => e.CName)
                .HasMaxLength(30)
                .HasComment("名称")
                .HasColumnName("c_name");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.TypeId)
                .HasComment("标注类型")
                .HasColumnName("type_id");
        });

        modelBuilder.Entity<CategoryItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("category_item", tb => tb.HasComment("分类项"));

            entity.HasIndex(e => e.CategoryId, "category_item_category_category_id_fk");

            entity.HasIndex(e => e.UserId, "category_item_user_user_id_fk");

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId)
                .HasComment("分类ID")
                .HasColumnName("category_id");
            entity.Property(e => e.IdCategory)
                .HasComment("目标类型")
                .HasColumnName("id_category");
            entity.Property(e => e.TId)
                .HasComment("目标ID")
                .HasColumnName("t_id");
            entity.Property(e => e.UserId)
                .HasComment("作者ID")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryItems)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("category_item_category_category_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.CategoryItems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("category_item_user_user_id_fk");
        });

        modelBuilder.Entity<Collection>(entity =>
        {
            entity.HasKey(e => e.CollectionId).HasName("PRIMARY");

            entity.ToTable("collection", tb => tb.HasComment("合集"));

            entity.HasIndex(e => e.UserId, "collection_user_user_id_fk");

            entity.Property(e => e.CollectionId)
                .HasComment("ID")
                .HasColumnName("collection_id");
            entity.Property(e => e.Content)
                .HasComment("内容")
                .HasColumnName("content");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasComment("封面图片")
                .HasColumnName("file_url");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改时间")
                .HasColumnType("datetime")
                .HasColumnName("modify_date");
            entity.Property(e => e.Status)
                .HasComment("合集状态")
                .HasColumnName("status");
            entity.Property(e => e.Summary)
                .HasMaxLength(200)
                .HasComment("简介")
                .HasColumnName("summary");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasComment("合集标题")
                .HasColumnName("title");
            entity.Property(e => e.UserId)
                .HasComment("作者ID")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Collections)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("collection_user_user_id_fk");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PRIMARY");

            entity.ToTable("comment", tb => tb.HasComment("评论"));

            entity.HasIndex(e => e.UserId, "comment_user_user_id_fk");

            entity.Property(e => e.CommentId)
                .HasComment("ID")
                .HasColumnName("comment_id");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .HasComment("评论内容")
                .HasColumnName("content");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Status)
                .HasComment("评论状态")
                .HasColumnName("status");
            entity.Property(e => e.TId)
                .HasComment("目标ID")
                .HasColumnName("t_id");
            entity.Property(e => e.TypeId)
                .HasComment("评论类型")
                .HasColumnName("type_id");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改时间")
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.UserId)
                .HasComment("用户ID")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comment_user_user_id_fk");
        });

        modelBuilder.Entity<ConfigActivityLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("config_activity_level", tb => tb.HasComment("活动水平"));

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.Key)
                .HasMaxLength(30)
                .HasComment("描述")
                .HasColumnName("key");
            entity.Property(e => e.Value)
                .HasComment("值")
                .HasColumnName("value");
        });

        modelBuilder.Entity<DietaryRecord>(entity =>
        {
            entity.HasKey(e => e.EdId).HasName("PRIMARY");

            entity.ToTable("dietary_record", tb => tb.HasComment("饮食记录"));

            entity.HasIndex(e => e.UserId, "eating_diary_user_user_id_fk");

            entity.Property(e => e.EdId)
                .HasComment("ID")
                .HasColumnName("ed_id");
            entity.Property(e => e.Dosages)
                .HasComment("用量列表")
                .HasColumnType("json")
                .HasColumnName("dosages");
            entity.Property(e => e.IdCategory)
                .HasComment("目标类型")
                .HasColumnName("id_category");
            entity.Property(e => e.NutrientContent)
                .HasComment("营养元素缓存")
                .HasColumnType("json")
                .HasColumnName("nutrient_content");
            entity.Property(e => e.Tid)
                .HasComment("目标ID")
                .HasColumnName("tid");
            entity.Property(e => e.TieUpDate)
                .HasComment("关联时间")
                .HasColumnType("datetime")
                .HasColumnName("tie_up_date");
            entity.Property(e => e.UserId)
                .HasComment("用户ID")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.DietaryRecords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dietary_record_user_user_id_fk");
        });

        modelBuilder.Entity<Dri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("dris", tb => tb.HasComment("DRIs"));

            entity.HasIndex(e => e.NutrientId, "nutrient_content_nutrients_id_fk");

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.AgeL)
                .HasComment("最小年龄")
                .HasColumnName("age_l");
            entity.Property(e => e.AgeR)
                .HasComment("最大年龄")
                .HasColumnName("age_r");
            entity.Property(e => e.Flag)
                .HasMaxLength(20)
                .HasComment("标识符")
                .HasColumnName("flag");
            entity.Property(e => e.Gender)
                .HasComment("性别")
                .HasColumnName("gender");
            entity.Property(e => e.NutrientId)
                .HasComment("营养元素ID")
                .HasColumnName("nutrient_id");
            entity.Property(e => e.Value)
                .HasComment("值")
                .HasColumnName("value");

            entity.HasOne(d => d.Nutrient).WithMany(p => p.Dris)
                .HasForeignKey(d => d.NutrientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DRIs_nutrients_id_fk");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PRIMARY");

            entity.ToTable("favorite", tb => tb.HasComment("收藏"));

            entity.HasIndex(e => e.UserId, "favorite_user_user_id_fk");

            entity.Property(e => e.FavoriteId)
                .HasComment("ID")
                .HasColumnName("favorite_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.FName)
                .HasMaxLength(30)
                .HasComment("名称")
                .HasColumnName("f_name");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasComment("封面图片")
                .HasColumnName("file_url");
            entity.Property(e => e.IdCategory)
                .HasComment("收藏类型")
                .HasColumnName("id_category");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改时间")
                .HasColumnType("datetime")
                .HasColumnName("modify_date");
            entity.Property(e => e.Refer)
                .HasMaxLength(200)
                .HasComment("简介")
                .HasColumnName("refer");
            entity.Property(e => e.UserId)
                .HasComment("用户ID")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorite_user_user_id_fk");
        });

        modelBuilder.Entity<FavoriteItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("favorite_item", tb => tb.HasComment("收藏项"));

            entity.HasIndex(e => new { e.FavoriteId, e.TId }, "favorite_item_pk").IsUnique();

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.FavoriteId)
                .HasComment("收藏ID")
                .HasColumnName("favorite_id");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改时间")
                .HasColumnType("datetime")
                .HasColumnName("modify_date");
            entity.Property(e => e.TId)
                .HasComment("目标ID")
                .HasColumnName("t_id");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PRIMARY");

            entity.ToTable("ingredient", tb => tb.HasComment("食材"));

            entity.HasIndex(e => e.UserId, "ingredient_user_user_id_fk");

            entity.Property(e => e.IngredientId)
                .HasComment("ID")
                .HasColumnName("ingredient_id");
            entity.Property(e => e.Allergy)
                .HasMaxLength(200)
                .HasComment("过敏信息")
                .HasColumnName("allergy");
            entity.Property(e => e.Content)
                .HasDefaultValueSql("'1'")
                .HasComment("净含量")
                .HasColumnName("content");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasComment("封面")
                .HasColumnName("file_url");
            entity.Property(e => e.IName)
                .HasMaxLength(30)
                .HasComment("名称")
                .HasColumnName("i_name");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改时间")
                .HasColumnType("datetime")
                .HasColumnName("modify_date");
            entity.Property(e => e.Quantity)
                .HasComment("转换计量")
                .HasColumnType("json")
                .HasColumnName("quantity");
            entity.Property(e => e.Refer)
                .HasMaxLength(200)
                .HasComment("描述")
                .HasColumnName("refer");
            entity.Property(e => e.Status)
                .HasComment("食材状态")
                .HasColumnName("status");
            entity.Property(e => e.Unit)
                .HasMaxLength(10)
                .HasComment("单位")
                .HasColumnName("unit");
            entity.Property(e => e.UserId)
                .HasComment("作者ID")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Ingredients)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingredient_user_user_id_fk");
        });

        modelBuilder.Entity<IngredientNutritional>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ingredient_nutritional", tb => tb.HasComment("食材营养元素"));

            entity.HasIndex(e => e.NutritionalId, "ingredient_nutritional_nutrients_id_fk");

            entity.HasIndex(e => new { e.IngredientId, e.NutritionalId }, "ingredient_nutritional_pk").IsUnique();

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.IngredientId)
                .HasComment("食材ID")
                .HasColumnName("ingredient_id");
            entity.Property(e => e.NutritionalId)
                .HasComment("营养元素ID")
                .HasColumnName("nutritional_id");
            entity.Property(e => e.Value)
                .HasComment("含量")
                .HasColumnName("value");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.IngredientNutritionals)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingredient_nutritional_ingredient_Ingredient_id_fk");

            entity.HasOne(d => d.Nutritional).WithMany(p => p.IngredientNutritionals)
                .HasForeignKey(d => d.NutritionalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingredient_nutritional_nutrients_id_fk");
        });

        modelBuilder.Entity<Nutrient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("nutrients", tb => tb.HasComment("营养元素"));

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(12)
                .HasComment("营养元素名称")
                .HasColumnName("name");
            entity.Property(e => e.Refer)
                .HasComment("介绍")
                .HasColumnType("text")
                .HasColumnName("refer");
            entity.Property(e => e.Unit)
                .HasMaxLength(4)
                .HasComment("计量单位")
                .HasColumnName("unit");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permission", tb => tb.HasComment("权限"));

            entity.HasIndex(e => e.Name, "permission_pk").IsUnique();

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(80)
                .HasComment("分组名")
                .HasColumnName("category");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasComment("请求路径")
                .HasColumnName("name");
            entity.Property(e => e.Title)
                .HasMaxLength(80)
                .HasComment("权限名")
                .HasColumnName("title");
        });

        modelBuilder.Entity<PhysicalSignsRecord>(entity =>
        {
            entity.HasKey(e => e.UpiId).HasName("PRIMARY");

            entity.ToTable("physical_signs_record", tb => tb.HasComment("体征记录"));

            entity.HasIndex(e => e.CalId, "user_physical_info_config_activity_level_id_fk");

            entity.HasIndex(e => e.UId, "user_physical_info_user_user_id_fk");

            entity.Property(e => e.UpiId)
                .HasComment("ID")
                .HasColumnName("upi_id");
            entity.Property(e => e.CalId)
                .HasComment("活动水平ID")
                .HasColumnName("cal_id");
            entity.Property(e => e.CreateDate)
                .HasComment("更新时间")
                .HasColumnType("date")
                .HasColumnName("create_date");
            entity.Property(e => e.FatPercentage)
                .HasComment("脂肪供能占比")
                .HasColumnName("fat_percentage");
            entity.Property(e => e.Height)
                .HasDefaultValueSql("'120'")
                .HasComment("身高")
                .HasColumnName("height");
            entity.Property(e => e.ProteinPercentage)
                .HasComment("蛋白质供能占比")
                .HasColumnName("protein_percentage");
            entity.Property(e => e.UId)
                .HasComment("用户ID")
                .HasColumnName("u_id");
            entity.Property(e => e.Weight)
                .HasDefaultValueSql("'30'")
                .HasComment("体重")
                .HasColumnName("weight");

            entity.HasOne(d => d.Cal).WithMany(p => p.PhysicalSignsRecords)
                .HasForeignKey(d => d.CalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicalSignsRecord_config_activity_level_id_fk");

            entity.HasOne(d => d.UIdNavigation).WithMany(p => p.PhysicalSignsRecords)
                .HasForeignKey(d => d.UId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicalSignsRecord_user_user_id_fk");
        });

        modelBuilder.Entity<PreparationStep>(entity =>
        {
            entity.HasKey(e => e.PreparationStepId).HasName("PRIMARY");

            entity.ToTable("preparation_step", tb => tb.HasComment("制作步骤"));

            entity.HasIndex(e => e.RecipeId, "preparation_step_recipe_recipe_id_fk");

            entity.Property(e => e.PreparationStepId)
                .HasComment("ID")
                .HasColumnName("preparation_step_id");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasComment("步骤图片")
                .HasColumnName("file_url");
            entity.Property(e => e.RecipeId)
                .HasComment("食谱ID")
                .HasColumnName("recipe_id");
            entity.Property(e => e.Refer)
                .HasMaxLength(200)
                .HasComment("描述")
                .HasColumnName("refer");
            entity.Property(e => e.RequiredIngredient)
                .HasMaxLength(600)
                .HasDefaultValueSql("'||'")
                .HasComment("耗时比例")
                .HasColumnName("required_ingredient");
            entity.Property(e => e.RequiredTime)
                .HasComment("所需时间")
                .HasColumnType("time")
                .HasColumnName("required_time");
            entity.Property(e => e.SequenceNumber)
                .HasComment("顺序编号")
                .HasColumnName("sequence_number");
            entity.Property(e => e.Summary)
                .HasMaxLength(200)
                .HasComment("小结")
                .HasColumnName("summary");
            entity.Property(e => e.Title)
                .HasMaxLength(30)
                .HasComment("标题")
                .HasColumnName("title");

            entity.HasOne(d => d.Recipe).WithMany(p => p.PreparationSteps)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("preparation_step_recipe_recipe_id_fk");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PRIMARY");

            entity.ToTable("recipe", tb => tb.HasComment("食谱"));

            entity.HasIndex(e => e.AuthorId, "recipe____fk");

            entity.Property(e => e.RecipeId)
                .HasComment("ID")
                .HasColumnName("recipe_id");
            entity.Property(e => e.AuthorId)
                .HasComment("作者ID")
                .HasColumnName("author_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasComment("封面")
                .HasColumnName("file_url");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改时间")
                .HasColumnType("datetime")
                .HasColumnName("modify_date");
            entity.Property(e => e.RName)
                .HasMaxLength(30)
                .HasComment("名称")
                .HasColumnName("r_name");
            entity.Property(e => e.Status)
                .HasComment("食谱状态")
                .HasColumnName("status");
            entity.Property(e => e.Summary)
                .HasMaxLength(200)
                .HasComment("总结")
                .HasColumnName("summary");
            entity.Property(e => e.Title)
                .HasMaxLength(30)
                .HasComment("标题")
                .HasColumnName("title");

            entity.HasOne(d => d.Author).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipe____fk");
        });

        modelBuilder.Entity<RecipeItem>(entity =>
        {
            entity.HasKey(e => e.RecipeItemId).HasName("PRIMARY");

            entity.ToTable("recipe_item", tb => tb.HasComment("食谱食材项"));

            entity.HasIndex(e => e.IngredientId, "recipe_item_ingredient_Ingredient_id_fk");

            entity.HasIndex(e => e.RecipeId, "recipe_item_recipe_recipe_id_fk");

            entity.Property(e => e.RecipeItemId)
                .HasComment("ID")
                .HasColumnName("recipe_item_id");
            entity.Property(e => e.Dosage)
                .HasPrecision(10)
                .HasComment("用量")
                .HasColumnName("dosage");
            entity.Property(e => e.IngredientId)
                .HasComment("食材ID")
                .HasColumnName("ingredient_id");
            entity.Property(e => e.RecipeId)
                .HasComment("食谱ID")
                .HasColumnName("recipe_id");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.RecipeItems)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipe_item_ingredient_Ingredient_id_fk");

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeItems)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipe_item_recipe_recipe_id_fk");
        });

        modelBuilder.Entity<Release>(entity =>
        {
            entity.HasKey(e => e.ReleaseId).HasName("PRIMARY");

            entity.ToTable("release", tb => tb.HasComment("发布"));

            entity.HasIndex(e => e.AuthorId, "release_user_user_id_fk");

            entity.Property(e => e.ReleaseId)
                .HasComment("ID")
                .HasColumnName("release_id");
            entity.Property(e => e.AuthorId)
                .HasComment("作者ID")
                .HasColumnName("author_id");
            entity.Property(e => e.Content)
                .HasComment("内容")
                .HasColumnName("content");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(60)
                .HasComment("封面缓存")
                .HasColumnName("file_url");
            entity.Property(e => e.IdCategory)
                .HasComment("目标类型")
                .HasColumnName("id_category");
            entity.Property(e => e.OpFlag)
                .HasComment("操作者类型")
                .HasColumnName("op_flag");
            entity.Property(e => e.ReleaseInfo)
                .HasComment("参考来源")
                .HasColumnType("text")
                .HasColumnName("release_info");
            entity.Property(e => e.TId)
                .HasDefaultValueSql("'-1'")
                .HasComment("目标ID")
                .HasColumnName("t_id");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasComment("标题缓存")
                .HasColumnName("title");

            entity.HasOne(d => d.Author).WithMany(p => p.Releases)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("release_user_user_id_fk");
        });

        modelBuilder.Entity<ReleaseFlowHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("release_flow_history", tb => tb.HasComment("审核状态流转表"));

            entity.HasIndex(e => e.ReleaseId, "release_flow_history_release_release_id_fk");

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Info)
                .HasMaxLength(100)
                .HasComment("补充信息")
                .HasColumnName("info");
            entity.Property(e => e.OpFlag)
                .HasComment("操作者类型")
                .HasColumnName("op_flag");
            entity.Property(e => e.OpId)
                .HasComment("操作者ID")
                .HasColumnName("op_id");
            entity.Property(e => e.ReleaseId)
                .HasComment("发布ID")
                .HasColumnName("release_id");
            entity.Property(e => e.Status)
                .HasComment("审核状态")
                .HasColumnName("status");

            entity.HasOne(d => d.Release).WithMany(p => p.ReleaseFlowHistories)
                .HasForeignKey(d => d.ReleaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("release_flow_history_release_release_id_fk");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PRIMARY");

            entity.ToTable("report", tb => tb.HasComment("举报"));

            entity.HasIndex(e => e.UserId, "report_user_user_id_fk");

            entity.Property(e => e.ReportId)
                .HasComment("ID")
                .HasColumnName("report_id");
            entity.Property(e => e.Content)
                .HasMaxLength(200)
                .HasComment("补充说明")
                .HasColumnName("content");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IdCategory)
                .HasComment("目标类型")
                .HasColumnName("id_category");
            entity.Property(e => e.ProcessingTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("处理时间")
                .HasColumnType("datetime")
                .HasColumnName("processing_time");
            entity.Property(e => e.RType)
                .HasComment("举报类型")
                .HasColumnName("r_type");
            entity.Property(e => e.Status)
                .HasComment("处理状态")
                .HasColumnName("status");
            entity.Property(e => e.StatusContent)
                .HasMaxLength(30)
                .HasComment("状态说明")
                .HasColumnName("status_content");
            entity.Property(e => e.TId)
                .HasComment("目标ID")
                .HasColumnName("t_id");
            entity.Property(e => e.UserId)
                .HasComment("举报者ID")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_user_user_id_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("role", tb => tb.HasComment("角色"));

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasComment("名称")
                .HasColumnName("name");
            entity.Property(e => e.Refer)
                .HasMaxLength(200)
                .HasComment("描述")
                .HasColumnName("refer");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("role_permission", tb => tb.HasComment("角色权限"));

            entity.HasIndex(e => new { e.PermissionId, e.RoleId }, "rp_pk_2").IsUnique();

            entity.HasIndex(e => e.RoleId, "rp_role_id_fk");

            entity.Property(e => e.Id)
                .HasComment("ID")
                .HasColumnName("id");
            entity.Property(e => e.PermissionId)
                .HasComment("权限ID")
                .HasColumnName("permission_id");
            entity.Property(e => e.RoleId)
                .HasComment("角色ID")
                .HasColumnName("role_id");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rp_permission_id_fk");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rp_role_id_fk");
        });

        modelBuilder.Entity<SystemConfig>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PRIMARY");

            entity.ToTable("system_config", tb => tb.HasComment("系统配置"));

            entity.HasIndex(e => e.ConfigName, "system_config_pk").IsUnique();

            entity.Property(e => e.ConfigId)
                .HasComment("ID")
                .HasColumnName("config_id");
            entity.Property(e => e.ConfigName)
                .HasMaxLength(30)
                .HasComment("键名")
                .HasColumnName("config_name");
            entity.Property(e => e.ConfigRefer)
                .HasMaxLength(200)
                .HasComment("描述")
                .HasColumnName("config_refer");
            entity.Property(e => e.ConfigValue)
                .HasComment("配置值")
                .HasColumnName("config_value");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改时间")
                .HasColumnType("datetime")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("user", tb => tb.HasComment("用户"));

            entity.HasIndex(e => e.UName, "user_pk").IsUnique();

            entity.Property(e => e.UserId)
                .HasComment("ID")
                .HasColumnName("user_id");
            entity.Property(e => e.BirthDate)
                .HasDefaultValueSql("'now()'")
                .HasComment("生日")
                .HasColumnType("date")
                .HasColumnName("birth_date");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsFixedLength()
                .HasComment("邮箱")
                .HasColumnName("email");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasComment("头像")
                .HasColumnName("file_url");
            entity.Property(e => e.Gender)
                .HasComment("性别")
                .HasColumnName("gender");
            entity.Property(e => e.Password)
                .HasMaxLength(44)
                .HasComment("密码")
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasComment("手机")
                .HasColumnName("phone");
            entity.Property(e => e.Salt)
                .HasMaxLength(49)
                .HasComment("盐")
                .HasColumnName("salt");
            entity.Property(e => e.Status)
                .HasComment("账户状态")
                .HasColumnName("status");
            entity.Property(e => e.UName)
                .HasMaxLength(20)
                .HasComment("名称")
                .HasColumnName("u_name");
            entity.Property(e => e.UnbanTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("解禁时间")
                .HasColumnType("datetime")
                .HasColumnName("unban_time");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
