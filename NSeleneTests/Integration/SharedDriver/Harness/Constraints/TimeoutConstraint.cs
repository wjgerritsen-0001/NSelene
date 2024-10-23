﻿using NUnit.Framework.Constraints;

namespace NSelene.Tests.Integration.SharedDriver.Harness.Constraints
{
    internal class TimeoutConstraint(string errorMessage) : Constraint
    {
        private readonly string errorMessage = $$"""
            Timed out after {{BaseTest.ShortTimeout}}s, while waiting for:
                {{errorMessage}}
            """;

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual is not Action act)
            {
                return new ConstraintResult(this, "Not a System.Action object passed to the constraint", ConstraintStatus.Error);
            }

            Configuration.Timeout = BaseTest.ShortTimeout;
            Configuration.PollDuringWaits = BaseTest.PollDuringWaits;

            var accuracyDelta = 2.0;
            var beforeCall = DateTime.Now;
            try
            {
                act.Invoke();
                return new ConstraintResult(this, "Did not timeout", ConstraintStatus.Failure);
            }
            catch (TimeoutException error)
            {
                if (!error.Message.Contains(errorMessage))
                {
                    return new ConstraintResult(this, error.Message, ConstraintStatus.Failure);
                }
                var elapsedTime = DateTime.Now.Subtract(beforeCall);
                return new ConstraintResult(this, elapsedTime, elapsedTime < TimeSpan.FromSeconds(BaseTest.ShortTimeout + BaseTest.PollDuringWaits + accuracyDelta) && elapsedTime >= TimeSpan.FromSeconds(BaseTest.ShortTimeout));
            }
        }

        public override string Description => $"Should timeout and throw TimeoutException with message: \r\n'{errorMessage}'";
    }
}
