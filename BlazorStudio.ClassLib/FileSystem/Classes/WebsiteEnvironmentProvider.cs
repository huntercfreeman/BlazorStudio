using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.AccountCase;
using Fluxor;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class WebsiteEnvironmentProvider : IEnvironmentProvider
{
    private readonly IState<AccountState> _accountStateWrap;

    public WebsiteEnvironmentProvider(IState<AccountState> accountStateWrap)
    {
        _accountStateWrap = accountStateWrap;
    }

    public IAbsoluteFilePath RootDirectoryAbsoluteFilePath => new AbsoluteFilePath(
        "/",
        true);

    public IAbsoluteFilePath HomeDirectoryAbsoluteFilePath => new AbsoluteFilePath(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        true);
}