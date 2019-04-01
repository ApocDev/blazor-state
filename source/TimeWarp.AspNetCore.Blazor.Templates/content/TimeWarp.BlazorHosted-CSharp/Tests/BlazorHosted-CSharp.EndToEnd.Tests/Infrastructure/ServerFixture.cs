﻿namespace BlazorHosted_CSharp.EndToEnd.Tests.Infrastructure
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Hosting.Server.Features;
  using Microsoft.Extensions.Hosting;

  public class ServerFixture
  {
    private readonly Lazy<Uri> LazyUri;

    public ServerFixture()
    {
      LazyUri = new Lazy<Uri>(() =>
        new Uri(StartAndGetRootUri()));
    }

    public delegate IHostBuilder HostBuilderFactory(string[] args);

    public HostBuilderFactory BuildWebHostMethod { get; set; }
    public AspNetEnvironment Environment { get; set; } = AspNetEnvironment.Production;
    public Uri RootUri => LazyUri.Value;
    private IHostBuilder WebHost { get; set; }

    /// <summary>
    /// Find the path to the server that you are testing.
    /// </summary>
    /// <param name="aProjectName"></param>
    /// <returns>The Path to the project</returns>
    protected static string FindSitePath(string aProjectName)
    {
      string solutionDir = FindSolutionDir();
      string[] possibleLocations = new[]
      {
        Path.Combine(solutionDir, aProjectName),
        Path.Combine(solutionDir, "Source", aProjectName),
        Path.Combine(solutionDir, "src", aProjectName),
      };

      return possibleLocations.FirstOrDefault(Directory.Exists)
        ?? throw new ArgumentException($"Cannot find a sample or test site with name '{aProjectName}'.");
    }

    protected static string FindSolutionDir()
    {
      return FindClosestDirectoryContaining(
        aFilename: "BlazorHosted-CSharp.sln",
        aStartDirectory: Path.GetDirectoryName(typeof(ServerFixture).Assembly.Location));
    }

    protected static void RunInBackgroundThread(Action aAction)
    {
      var isDone = new ManualResetEvent(false);

      new Thread(() =>
      {
        aAction();
        isDone.Set();
      }).Start();

      isDone.WaitOne();
    }

    protected IHostBuilder CreateWebHost()
    {
      if (BuildWebHostMethod == null)
      {
        throw new InvalidOperationException(
            $"No value was provided for {nameof(BuildWebHostMethod)}");
      }

      string sitePath = FindSitePath(
                BuildWebHostMethod.Method.DeclaringType.Assembly.GetName().Name);

      IHostBuilder webHost = BuildWebHostMethod(new[]
      {
        "--urls", "http://127.0.0.1:0",
        "--contentroot", sitePath,
        "--environment", Environment.ToString(),
      });

      return webHost;
    }

    protected string StartAndGetRootUri()
    {
      WebHost = CreateWebHost();
      RunInBackgroundThread(WebHost.Start);
      return WebHost.ConfigureWebHost(a =>
          .Get<IServerAddressesFeature>()
          .Addresses.Single();
    }

    private static string FindClosestDirectoryContaining(
      string aFilename,
      string aStartDirectory)
    {
      string directory = aStartDirectory;
      while (true)
      {
        if (File.Exists(Path.Combine(directory, aFilename)))
          return directory;

        directory = Directory.GetParent(directory)?.FullName;
        if (string.IsNullOrEmpty(directory))
        {
          throw new FileNotFoundException(
              $"Could not locate a file called '{aFilename}' in " +
              $"directory '{aStartDirectory}' or any parent directory.");
        }
      }
    }
  }
}