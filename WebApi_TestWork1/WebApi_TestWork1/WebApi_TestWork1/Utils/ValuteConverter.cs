namespace WebApi_TestWork1.Utils
{
    public static class ValuteConverter
    {
        //converts one currency to another relative to the current exchange rate
        public static decimal Convert(decimal CurrentValuteRate, decimal FromValuteAmount, decimal ToConvertedVluteRate )
        {
            return ((FromValuteAmount / CurrentValuteRate) * ToConvertedVluteRate); // calculation error +- ~0.002%
            // return (((FromValuteAmount / CurrentValuteRate) * ToConvertedVluteRate))*0.99998M; // As a temporary solution to the problem with the calculation error, the average percentage deviation of the values ​​from the initial values ​​was calculated, we will conditionally consider this % commission for the transaction by the bank.
        }
        
    }
}
