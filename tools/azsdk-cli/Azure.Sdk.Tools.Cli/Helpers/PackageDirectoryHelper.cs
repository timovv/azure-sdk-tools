namespace Azure.Sdk.Tools.Cli.Helpers;

public class PackageDirectoryInfo
{
    public string PackageRoot { get; set; }

    public string RepositoryRoot { get; set; }

    public string ServiceDirectory { get; set; }

    public string ServiceDirectoryName { get; set; }
}

public class PackageDirectoryHelper(IGitHelper gitHelper)
{
    public PackageDirectoryInfo GetPackageDirectoryInfo(string directory)
    {
        var fullPath = Path.GetFullPath(directory);
        var repositoryRoot = gitHelper.DiscoverRepoRoot(fullPath);

        // e.g. sdk/<service directory name>/<package directory name>
        var pathRelativeToRepositoryRoot = Path.GetRelativePath(repositoryRoot, fullPath);

        var parts = pathRelativeToRepositoryRoot.Split(Path.DirectorySeparatorChar);

        if (parts[0] != "sdk")
        {
            throw new ArgumentException("Directory does not represent a package directory. Packages are under the sdk/ folder in the repository.");
        }

        if(parts.Length < 3)
        {
            throw new ArgumentException("Directory is not deep enough to be a package directory. Packages are under the sdk/<service>/<package> folder structure.");
        }

        var serviceDirectory = Path.Combine(repositoryRoot, parts[0], parts[1]);
        var serviceDirectoryName = parts[1];

        return new PackageDirectoryInfo
        {
            PackageRoot = directory,
            RepositoryRoot = repositoryRoot,
            ServiceDirectory = serviceDirectory,
            ServiceDirectoryName = serviceDirectoryName
        };
    }
}