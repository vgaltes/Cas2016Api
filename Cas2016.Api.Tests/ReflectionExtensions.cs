namespace Cas2016.Api.Tests
{
    // http://stackoverflow.com/questions/10458714/how-to-mock-a-method-call-that-takes-a-dynamic-object
    public static class ReflectionExtensions
    {
        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            return (T) obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
    }
}