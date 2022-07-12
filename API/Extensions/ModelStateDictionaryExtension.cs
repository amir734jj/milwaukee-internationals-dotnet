using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Extensions;

public static class ModelStateDictionaryExtension
{
    public static void ClearModelStateErrors(this ModelStateDictionary modelStateDictionary)
    {
        if (modelStateDictionary?.Values != null)
        {
            foreach (var modelValue in modelStateDictionary?.Values)
            {
                modelValue.Errors.Clear();
            }
        }
    }
}