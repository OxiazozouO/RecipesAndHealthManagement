using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Android.Attribute;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class CollectionLengthAttribute(int minLength, int maxLength) : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;
        if (value is not ICollection collection)
            return new ValidationResult("集合不能为空");

        int length = collection.Count;
        if (length < minLength || length > maxLength)
            return new ValidationResult($"集合长度必须介于{minLength}和{maxLength}之间。");

        return ValidationResult.Success;
    }
}