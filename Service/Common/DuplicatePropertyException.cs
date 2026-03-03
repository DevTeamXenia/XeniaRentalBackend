namespace XeniaRentalBackend.Service.Common
{
    public class DuplicatePropertyException : Exception
    {
        public DuplicatePropertyException(string message)
            : base(message)
        {
        }
    }
}
