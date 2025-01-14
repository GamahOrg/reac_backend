namespace Core.Log.Domain.Shared.Extentions
{
    public static class FlageEnumExtention
    {
        public static IEnumerable<Enum> GetFlags(this Enum flags)
        {
            int flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                int bits = Convert.ToInt16(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
    }
}
