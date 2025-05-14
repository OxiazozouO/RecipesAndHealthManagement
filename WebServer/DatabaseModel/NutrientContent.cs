namespace WebServer.DatabaseModel;

/// <summary>
/// DRIs
/// </summary>
public partial class NutrientContent
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 名字
    /// </summary>
    public string Flag { get; set; } = null!;

    /// <summary>
    /// 营养元素ID
    /// </summary>
    public int NutrientId { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 最小年龄
    /// </summary>
    public int AgeL { get; set; }

    /// <summary>
    /// 最大年龄
    /// </summary>
    public int AgeR { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public double Value { get; set; }

    public virtual Nutrient Nutrient { get; set; } = null!;
}
