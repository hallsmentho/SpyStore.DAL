using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace SpyStore.DAL.EF
{
    public class MyExecutionStrategy : ExecutionStrategy
    {
        public MyExecutionStrategy(ExecutionStrategyContext context) :
            base(context, ExecutionStrategy.DefaultMaxRetryCount, ExecutionStrategy.DefaultMaxDelay)
        {

        }

        public MyExecutionStrategy(ExecutionStrategyContext context, int maxRetryCount, TimeSpan maxRetryTime) :
            base(context, maxRetryCount, maxRetryTime)
        {

        }

        protected override bool ShouldRetryOn(Exception exception)
        {
            return true;
        }
    }
}
