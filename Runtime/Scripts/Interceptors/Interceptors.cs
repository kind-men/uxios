namespace KindMen.Uxios.Interceptors
{
    public delegate Config RequestInterception(Config request);
    public delegate Response ResponseInterception(Response response);
    public delegate Error ErrorInterception(Error error);

    public sealed class Interceptors
    {
        /// <summary>
        /// Ensure this interceptor is ran before the user-defined ones - the number is very low so that individual
        /// system interceptors might use subtraction or addition on top of this number to move around in relative
        /// priority.
        /// </summary>
        public const int DefaultSystemPriority = -1000;

        public const int DefaultUserPriority = 0;

        // Loggers are expected to be ran last so that all other interceptors have done their thing; as such
        // we give them a priority index of 10.000
        public const int DefaultLoggingPriority = 10000;

        
        public readonly PriorityList<RequestInterceptor> request = new();
        public readonly PriorityList<ResponseInterceptor> response = new();
    }
}