using NSelene.Tests.Integration.SharedDriver.Harness.Constraints;

namespace NSelene.Tests.Integration.SharedDriver.Harness
{
    internal class Does : NUnit.Framework.Does
    {
        internal static NoTimeoutConstraint NotTimeout(TimeSpan? pollingPeriod = null)
        {
            return new NoTimeoutConstraint(pollingPeriod);
        }

        internal static TimeoutConstraint Timeout(string errorMessage)
        {
            return new TimeoutConstraint(errorMessage);
        }
    }
}
