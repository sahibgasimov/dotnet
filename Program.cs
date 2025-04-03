using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProfilerTestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            var app = builder.Build();

            // Basic endpoint to check if app is running
            app.MapGet("/", () => "Profiler Test App is running!");

            // CPU-intensive endpoint that performs calculations to generate load
            app.MapGet("/cpu-intensive", async (context) =>
            {
                await context.Response.WriteAsync("Starting CPU-intensive operation...\n");
                
                var stopwatch = Stopwatch.StartNew();
                
                // Run a CPU-intensive task
                PerformCpuIntensiveTask();
                
                stopwatch.Stop();
                
                await context.Response.WriteAsync($"CPU-intensive operation completed in {stopwatch.ElapsedMilliseconds}ms");
            });

            // Memory-intensive endpoint
            app.MapGet("/memory-intensive", async (context) =>
            {
                await context.Response.WriteAsync("Starting memory-intensive operation...\n");
                
                var stopwatch = Stopwatch.StartNew();
                
                // Allocate and work with a large amount of memory
                ConsumeMemory();
                
                stopwatch.Stop();
                
                await context.Response.WriteAsync($"Memory-intensive operation completed in {stopwatch.ElapsedMilliseconds}ms");
            });

            // Slow SQL-like query simulation
            app.MapGet("/slow-query", async (context) =>
            {
                await context.Response.WriteAsync("Starting slow query simulation...\n");
                
                var stopwatch = Stopwatch.StartNew();
                
                // Simulate a slow database query
                await SimulateSlowQueryAsync();
                
                stopwatch.Stop();
                
                await context.Response.WriteAsync($"Slow query completed in {stopwatch.ElapsedMilliseconds}ms");
            });

            // Load test that calls all endpoints
            app.MapGet("/load-test", async (context) =>
            {
                await context.Response.WriteAsync("Starting load test with multiple operations...\n");
                
                var stopwatch = Stopwatch.StartNew();
                
                // Run multiple operations in parallel
                var tasks = new List<Task>
                {
                    Task.Run(() => PerformCpuIntensiveTask()),
                    Task.Run(() => ConsumeMemory()),
                    Task.Run(() => SimulateSlowQueryAsync())
                };
                
                // Wait for all tasks to complete
                await Task.WhenAll(tasks);
                
                stopwatch.Stop();
                
                await context.Response.WriteAsync($"Load test completed in {stopwatch.ElapsedMilliseconds}ms");
            });

            app.Run();
        }

        // Simulate CPU-intensive calculation
        private static void PerformCpuIntensiveTask()
        {
            long result = 0;
            
            // Use prime number calculation to create CPU load
            for (int i = 2; i < 1000000; i++)
            {
                bool isPrime = true;
                for (int j = 2; j <= Math.Sqrt(i); j++)
                {
                    if (i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                
                if (isPrime)
                {
                    result += i;
                }
            }
            
            // Prevent optimization by using the result
            GC.KeepAlive(result);
        }

        // Simulate memory consumption
        private static void ConsumeMemory()
        {
            // Create a list that will consume memory
            var largeList = new List<string>();
            
            // Generate and store 1 million strings
            for (int i = 0; i < 1000000; i++)
            {
                largeList.Add($"Memory string {i} with some additional text to make it larger in memory");
            }
            
            // Process the data to prevent optimization
            var count = largeList.Count(s => s.Contains("999"));
            
            // Prevent optimization by using the result
            GC.KeepAlive(count);
            
            // Don't clear the list immediately to ensure memory pressure
            Thread.Sleep(1000);
        }

        // Simulate a slow database query
        private static async Task SimulateSlowQueryAsync()
        {
            // Simulate network latency and database processing time
            await Task.Delay(3000);
            
            // Simulate processing the results
            var results = new List<string>();
            for (int i = 0; i < 10000; i++)
            {
                results.Add($"Row {i}");
            }
            
            // Process the data to prevent optimization
            var count = results.Count(s => s.Contains("9"));
            
            // Prevent optimization by using the result
            GC.KeepAlive(count);
        }
    }
}
