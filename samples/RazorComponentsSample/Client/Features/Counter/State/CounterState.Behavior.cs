﻿namespace RazorComponentsSample.Client.Features.Counter
{
  using BlazorState;

  public partial class CounterState : State<CounterState>
  {
    public CounterState() { } // needed for serialization

    protected CounterState(CounterState aState) : this()
    {
      Count = aState.Count;
    }

    public override object Clone() => new CounterState(this);

    protected override void Initialize() => Count = 3;
  }
}
