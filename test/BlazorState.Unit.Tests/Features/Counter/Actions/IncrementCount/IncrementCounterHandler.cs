﻿namespace BlazorState.Tests.Features.Counter
{
  using System.Threading;
  using System.Threading.Tasks;
  using BlazorState;
  using BlazorState.Tests.Features.Base;

  public partial class CounterState
  {
    public class IncrementCounterHandler : BaseHandler<IncrementCounterAction, CounterState>
    {
      public IncrementCounterHandler(IStore aStore) : base(aStore) { }

      public override Task<CounterState> Handle(
        IncrementCounterAction aIncrementCounterRequest,
        CancellationToken aCancellationToken)
      {
        CounterState.Count += aIncrementCounterRequest.Amount;
        return Task.FromResult(CounterState);
      }
    }
  }
}