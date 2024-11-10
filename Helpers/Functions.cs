using System.Reflection;

namespace ZonefyDotnet.Helpers
{
    public static class Functions
    {
        // Helper function to update properties
        public static async Task UpdateProperties(object payload, object db_model)
        {
            await Task.Run(() =>
            {
                Type modelType = payload.GetType();
                PropertyInfo[] modelProperties = modelType.GetProperties();

                Type targetType = db_model.GetType();
                PropertyInfo[] targetProperties = targetType.GetProperties();

                foreach (PropertyInfo modelProperty in modelProperties)
                {
                    string modelName = modelProperty.Name;
                    object? modelValue = modelProperty.GetValue(payload);

                    foreach (PropertyInfo targetProperty in targetProperties)
                    {
                        string targetName = targetProperty.Name;
                        if (targetName == "Id") continue; // Skip Id property
                        object? targetValue = targetProperty.GetValue(db_model);

                        if (modelValue != null && modelName == targetName && !modelValue.Equals(targetValue))
                        {
                            if (targetProperty.CanWrite)
                            {
                                if (modelValue.GetType() == typeof(List<string>))
                                {
                                    var modelList = (List<string>)modelValue;
                                    var targetList = (List<string>)targetValue!;

                                    foreach (var item in modelList)
                                    {
                                        targetList.Add(item);
                                    }
                                    Console.WriteLine($"updated list {targetList}");
                                    targetProperty.SetValue(db_model, targetList);
                                }
                                else
                                {
                                    Console.WriteLine($"updated value {modelValue}");
                                    targetProperty.SetValue(db_model, modelValue);
                                }
                            }
                            break;
                        }
                    }
                }
            });
        }
    }
}
