// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using ModelContextProtocol.Server;
using Azure.Sdk.Tools.Cli.Commands;
using Azure.Sdk.Tools.Cli.Models;
using Azure.Sdk.Tools.Cli.Helpers;

namespace Azure.Sdk.Tools.Cli.Tools.Test;

[McpServerToolType, Description("Tool for deploying test resources")]
public class DeployTestResourcesTool(ILogger<DeployTestResourcesTool> logger, IPowershellHelper powershellHelper, PackageDirectoryHelper packageDirectoryHelper) : MCPTool
{
    public override CommandGroup[] CommandHierarchy { get; set; } = [SharedCommandGroups.Test];

    private readonly Option<string> resourceGroupOption = new(
        ["--resource-group", "-g"],
        "The resource group to deploy to"
    );

    private readonly Option<string> subscriptionOption = new(
        ["--subscription", "-s"],
        "The subscription ID to deploy to"
    );

    private readonly Option<bool> dryRunOption = new(
        ["--dry-run", "-d"],
        () => false,
        "Perform a dry run without actually deploying resources"
    );

    protected override Command GetCommand() =>
        new("deploy-test-resources", "Deploy test resources for testing purposes")
        {
            resourceGroupOption,
            subscriptionOption,
            dryRunOption
        };

    public override async Task<CommandResponse> HandleCommand(InvocationContext ctx, CancellationToken ct)
    {
        var packageDirectory = ctx.ParseResult.GetValueForOption(SharedOptions.PackagePath);
        var resourceGroup = ctx.ParseResult.GetValueForOption(resourceGroupOption);
        var subscription = ctx.ParseResult.GetValueForOption(subscriptionOption);
        var dryRun = ctx.ParseResult.GetValueForOption(dryRunOption);
        var pwshArgs = ctx.ParseResult.

        return await DeployTestResources(packageDirectory, resourceGroup, subscription, dryRun, ct);
    }

    [McpServerTool(Name = "azsdk_deploy_test_resources"), Description("Deploys test resources for Azure SDK testing")]
    public async Task<DefaultCommandResponse> DeployTestResources(
        string packagePath,
        string? resourceGroup = null,
        string? subscription = null,
        bool dryRun = false,
        CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("Starting deployment of test resources: {ResourceName}, ResourceGroup: {ResourceGroup}, Subscription: {Subscription}, DryRun: {DryRun}",
                packagePath, resourceGroup, subscription, dryRun);

            var serviceDirectoryName = packageDirectoryHelper.GetPackageDirectoryInfo(packagePath).ServiceDirectoryName;

            // run eng/common/New-TestResources.ps1 script
            // options: -ResourceGroupName; 

            // TODO: Add actual deployment implementation here
            await Task.Delay(100, ct); // Placeholder for async work

            return new DefaultCommandResponse
            {
                Result = $"Test resource '{resourceName}' deployment initiated successfully."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deploying test resources for: {ResourceName}", resourceName);
            return new DefaultCommandResponse
            {
                ResponseError = $"Failed to deploy test resources '{resourceName}': {ex.Message}"
            };
        }
    }
}