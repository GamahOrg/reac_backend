namespace Core.GenericRepository.Extensions
{
    public static class GeneralExtensions
    {
        public static Task<bool> IsInteger(this string value)
        {
            int number;

            var result = int.TryParse(value.ToCharArray(), out number);

            return Task.FromResult(result);
        }
    }
}
