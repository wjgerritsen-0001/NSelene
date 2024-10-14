using NUnit.Framework.Constraints;

namespace NSelene.Tests.Integration.SharedDriver.Harness.Constraints
{
    internal class NoTimeoutConstraint(TimeSpan? pollingPeriod) : Constraint
    {
        private readonly TimeSpan _pollingPeriod = pollingPeriod ?? TimeSpan.Zero;

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var AccuracyTimeout = 3.0;
            Configuration.Timeout = 2 * AccuracyTimeout;

            var beforeCall = DateTime.Now;
            try
            {
                if (actual is Action act)
                {
                    act.Invoke();
                }
                else
                {
                    return new ConstraintResult(this, "Not a System.Action object passed to the constraint", ConstraintStatus.Error);
                }
            }
            catch (TimeoutException ex)
            {
                return new ConstraintResult(this, ex.Message, ConstraintStatus.Failure);
            }
            var elapsedTime = DateTime.Now.Subtract(beforeCall);
            var elapsedTimeLimit = TimeSpan.FromSeconds(AccuracyTimeout);

            return new ConstraintResult(this, elapsedTime, elapsedTime < elapsedTimeLimit && elapsedTime >= _pollingPeriod);
        }

        public override string Description => $"Should not timeout" + (_pollingPeriod == TimeSpan.Zero ? "" : $" or finish in less then {_pollingPeriod}");
    }
}
