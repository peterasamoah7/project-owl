namespace ProjectOwl.Models
{
    public static class AudioHelpers 
    {
        public static Priority GetPriority(decimal number)
        {
            if (number < 0)
                return Priority.High;
            else if (number > 0 && number < 5)
                return Priority.Medium;
            else
                return Priority.Low;
        }
    }
}
