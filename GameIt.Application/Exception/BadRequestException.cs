namespace GameIt.Application.Exeptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string massage) : base(massage)
        {

        }
    }
}
