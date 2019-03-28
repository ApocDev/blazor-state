﻿namespace TestApp.Client.Layout
{
  using BlazorState.Services;
  using Microsoft.AspNetCore.Components;
  using Microsoft.AspNetCore.Components.Layouts;

  public class MainLayoutModel : LayoutComponentBase
  {
    [Inject] public JsRuntimeLocation JsRuntimeLocation { get; set; }
  }
}
