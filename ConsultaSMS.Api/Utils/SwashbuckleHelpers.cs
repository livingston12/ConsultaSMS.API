using System;
using System.Linq;

namespace WebConsultaSMS.Utils;

public static class SwashbuckleHelpers
{
    public static string DefaultSchemaIdSelector(Type modelType)
    {
        if (!modelType.IsConstructedGenericType)
            return modelType.Name.Replace("[]", "Array");

        var prefix = modelType
            .GetGenericArguments()
            .Select(genericArg => DefaultSchemaIdSelector(genericArg))
            .Aggregate((previous, current) => previous + current);

        return prefix + modelType.Name.Split('`').First();
    }
}
