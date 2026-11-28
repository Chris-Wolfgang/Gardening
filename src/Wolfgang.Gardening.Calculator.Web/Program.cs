using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Wolfgang.Gardening.Calculator.Web;

// An explicit Main rather than top-level statements: Stryker compiles the mutated project as a library,
// and a library cannot hold top-level statements (CS8805), so the mutation run could not build.
internal static class Program
{
    private static Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        return builder.Build().RunAsync();
    }
}
